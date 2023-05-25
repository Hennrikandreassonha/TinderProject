using Microsoft.AspNetCore.Mvc;
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

            //If we have reached the end of users or we are missing an index value, the index will be set to zero.
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

            //Options for match and superlike popups.
            if (options == "true")
            {
                Match = true;

                int userId = (int)HttpContext.Session.GetInt32("userToShow")!;

                CurrentUserShown = _userRepo.GetUser(userId)!;
            }
            if (options == "super")
            {
                SuperLike = true;

                int userId = (int)HttpContext.Session.GetInt32("userToShow")!;
                CurrentUserShown = _userRepo.GetUser(userId)!;
            }
        }

        public IActionResult OnPost(string options, string smartMatching, int userId)
        {
            //Options for changing the smart matchmaking.
            //These will trigger a page reloao.
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
            var likedUser = _context.Users.Find(userId);

            if (options == "like" && CheckIfMatch(LoggedInUser.Id, userId))
            {
                IncrementUserIndex();
                HttpContext.Session.SetInt32("userToShow", likedUser.Id);

                return RedirectToPage("/Index", new { options = "true" });
            }

            if (options == "like")
            {
                NewInteraction(LoggedInUser, likedUser);
            }
            if (options == "super")
            {
                if (CheckIfMatch(LoggedInUser.Id, userId))
                {
                    HttpContext.Session.SetInt32("userToShow", likedUser.Id);

                    return RedirectToPage("/Index", new { options = "true" });
                }

                NewInteraction(LoggedInUser, likedUser);
				HttpContext.Session.SetInt32("userToShow", likedUser.Id);

                return RedirectToPage("/Index", new { options = "super" });
            }

            IncrementUserIndex();
            return RedirectToPage("/Index");
        }

        public IActionResult OnPostSendMsgSuper(string messageToSend, int userIdToSend)
        {
            //Kolla så att den lägger till en interaction även om man inte skickar ett meddelande.
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