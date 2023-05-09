using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinderProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly AppDbContext _context;
        public IndexModel(IUserRepository repo, AppDbContext context, IMatchRepository matchRepo)
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
        public bool NoUsersToSwipe { get; set; }
        [BindProperty]
        public bool SmartMatching { get; set; }
        public void OnGet(string? match)
        {
            //Fixa så att den nya användaren visas Efter popupen tagits bort och inte innan.
            if (match == "true")
            {
                Match = true;
            }

            SmartMatching = HttpContext.Session.GetString("smartMatching") == "true" || HttpContext.Session.GetString("smartMatching") == null;

            LoggedInUser = _userRepo.GetLoggedInUser();
            if (LoggedInUser != null)
            {
                UsersToSwipe = _userRepo.GetUsersToSwipe(LoggedInUser).ToList();
                if (SmartMatching)
                {
                    UsersToSwipe = _matchRepo.OrderByMatchingTypes(UsersToSwipe, LoggedInUser);
                }
                else
                {
                    UsersToSwipe = _matchRepo.OrderByLeastMatchingTypes(UsersToSwipe, LoggedInUser);
                }
            }

            if (UsersToSwipe.Count == 0)
            {
                NoUsersToSwipe = true;
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
        }
        public IActionResult OnPost(string like, string smartMatching, int userId)
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

            var loggedInUser = _userRepo.GetLoggedInUser();
            UsersToSwipe = _userRepo.GetUsersToSwipe(loggedInUser).ToList();

            if (like == "true" && CheckIfMatch(loggedInUser.Id, userId))
            {
                ViewData["Match"] = "true";
                IncrementUserIndex();
                return RedirectToPage("/Index", new { match = "true" });
            }
            var likedUser = _context.Users.Find(userId);

            if (like == "true")
            {
                NewInteraction(loggedInUser, likedUser);
            }

            IncrementUserIndex();
            return RedirectToPage("/Index");
        }
        public void IncrementUserIndex()
        {
            //Increments the index which is used for showing users.
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
            return HttpContext.Session.GetInt32("currentUserIndex").GetValueOrDefault(); ;
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

            _context.Add(newMatch);
            _context.SaveChanges();
        }
    }
}