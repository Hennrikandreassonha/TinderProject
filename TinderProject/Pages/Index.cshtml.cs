﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinderProject.Utilities;
namespace TinderProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly IAppDbContext _context;
        public IndexModel(IUserRepository repo, IAppDbContext context, IMatchRepository matchRepo)
        {
            _userRepo = repo;
            _context = context;
            _matchRepo = matchRepo;
        }
        public User LoggedInUser { get; set; }
        public List<User> UsersToSwipe { get; set; }
        public User CurrentUserShown { get; set; }
        [BindProperty]
        public bool Match { get; set; }
        [BindProperty]
        public bool SuperLike { get; set; }
        [BindProperty]
        public bool NoUsersToSwipe { get; set; }
        [BindProperty]
        public bool SmartMatching { get; set; }
        public void OnGet(string? options)
        {
            //Fixa så att den nya användaren visas Efter popupen tagits bort och inte innan.
            if (options == "true")
            {
                Match = true;
            }
            if (options == "super")
            {
                SuperLike = true;
            }

            LoggedInUser = _userRepo.GetLoggedInUser();

            if (LoggedInUser != null && ProfileChecker.ProfileIsComplete(LoggedInUser))
            {
                SmartMatching = HttpContext.Session.GetString("smartMatching") == "true" || HttpContext.Session.GetString("smartMatching") == null;

                UsersToSwipe = _userRepo.GetUsersToSwipe(LoggedInUser).ToList();

                if (SmartMatching)
                {
                    UsersToSwipe = _matchRepo.OrderByMatchingTypes(UsersToSwipe, LoggedInUser);
                }
                else
                {
                    UsersToSwipe = _matchRepo.OrderByLeastMatchingTypes(UsersToSwipe, LoggedInUser);
                }

                if (UsersToSwipe.Count == 0)
                {
                    NoUsersToSwipe = true;
                    return;
                }
            }
            else
            {
                return;
            }

            var currentUserIndex = HttpContext.Session.GetInt32("currentUserIndex");

            //If we have reached end of users or we are missing indexvalue the index will be set to zero.
            if (currentUserIndex == null || currentUserIndex >= UsersToSwipe.Count)
            {
                HttpContext.Session.SetInt32("currentUserIndex", 0);
                currentUserIndex = HttpContext.Session.GetInt32("currentUserIndex");
            }

            CurrentUserShown = UsersToSwipe[(int)currentUserIndex!];

            //Getting the personalityoptions
            //These are the 4 Letters that makes the personality type.
            //We are getting these since if its a match we make them another color.
            HttpContext.Session.SetString("userPLetters", _matchRepo.GetPersonalityLetters(LoggedInUser));
            HttpContext.Session.SetString("currentSwipeUserPLetters", _matchRepo.GetPersonalityLetters(CurrentUserShown));

            HttpContext.Session.SetString("currentSwipeUserPStyle", CurrentUserShown.PersonalityType.Substring(0, CurrentUserShown.PersonalityType.Length - 6));
        }
        public IActionResult OnPost(string options, string smartMatching, int userId)
        {
            if (smartMatching == "true")
            {
                HttpContext.Session.SetString("smartMatching", "true");
                return RedirectToPage("/Index");
            }
            else if (smartMatching == "false")
            {
                HttpContext.Session.SetString("smartMatching", "false");
                return RedirectToPage("/Index");
            }

            LoggedInUser = _userRepo.GetLoggedInUser();
            UsersToSwipe = _userRepo.GetUsersToSwipe(LoggedInUser).ToList();
            var likedUser = _context.Users.Find(userId);

            if (options == "like" && CheckIfMatch(LoggedInUser.Id, userId))
            {
                ViewData["Match"] = "true";
                IncrementUserIndex();
                return RedirectToPage("/Index", new { options = "true" });
            }

            if (options == "like")
            {
                NewInteraction(LoggedInUser, likedUser);
            }
            if (options == "super")
            {
                return RedirectToPage("/Index", new { options = "super" });
            }

            IncrementUserIndex();
            return RedirectToPage("/Index");
        }
        public IActionResult OnPostSendMsgSuper(string messageToSend, int userIdToSend)
        {
            LoggedInUser = _userRepo.GetLoggedInUser();

            //Adding the message directly to the Database.
            Message msg = new()
            {
                SentMessage = messageToSend,
                SentTime = DateTime.Now,
                SentToId = userIdToSend,
                SentFromId = LoggedInUser.Id
            };

            _context.Messages.Add(msg);
            NewInteraction(LoggedInUser, _userRepo.GetUser(userIdToSend));

            IncrementUserIndex();
            return RedirectToPage("/Index");
        }
        public void IncrementUserIndex()
        {
            //Increments the index which is used for showing users.

            var loggedInUser = _userRepo.GetLoggedInUser();
            UsersToSwipe = _userRepo.GetUsersToSwipe(loggedInUser).ToList();

            if (GetCurrentUserIndex() == UsersToSwipe.Count)
            {
                HttpContext.Session.SetInt32("currentUserIndex", 0);

            }
            var currentUserIndex = GetCurrentUserIndex();
            currentUserIndex++;

            HttpContext.Session.SetInt32("currentUserIndex", currentUserIndex);
        }
        public int GetCurrentUserIndex()
        {
            return HttpContext.Session.GetInt32("currentUserIndex").GetValueOrDefault();
        }
        public void NewInteraction(User loggedInUser, User likedUser)
        {
            Interaction newLike = new()
            {
                LikerId = loggedInUser.Id,
                LikedId = likedUser.Id,
                DateLiked = DateTime.Now
            };

            _context.Interactions.Add(newLike);
            _context.SaveChanges();

        }
        public bool CheckIfMatch(int loggedInUserId, int likedUserId)
        {
            //Gets the Currently Liked User and checks if it likes loggedInUser.
            //In that case it´s a match.
            var likedUserLikes = _userRepo.GetUserLikes(likedUserId);

            if (likedUserLikes != null)
            {
                foreach (var item in likedUserLikes)
                {
                    if (item.LikedId == loggedInUserId)
                    {
                        CreateNewMatch(loggedInUserId, likedUserId);
                        return true;
                    }
                }
            }
            return false;
        }
        public void CreateNewMatch(int user1Id, int user2Id)
        {
            Match newMatch = new()
            {
                User1Id = user1Id,
                User2Id = user2Id,
                MatchDate = DateTime.Now
            };

            _context.Matches.Add(newMatch);
            _context.SaveChanges();
        }
    }
}