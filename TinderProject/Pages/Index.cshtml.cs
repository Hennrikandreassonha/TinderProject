using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinderProject.Data;
using TinderProject.Repositories.Repositories_Interfaces;

namespace TinderProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepo;
        private readonly AppDbContext _conext;
        public IndexModel(IUserRepository repo, AppDbContext conext)
        {
            _userRepo = repo;
            _conext = conext;
        }
        public List<User> UsersToSwipe { get; set; }
        public User CurrentUserShown { get; set; }
        [BindProperty]
        public bool Match { get; set; }
        [BindProperty]
        public bool NoUsersToSwipe { get; set; }
        public void OnGet(string? match)
        {

            if (match == "true")
            {
                Match = true;
            }

            var loggedInUser = _userRepo.GetLoggedInUser();

            //Måste exkludera de som redan är matchade.
            if (loggedInUser != null)
            {
                UsersToSwipe = _userRepo.GetUsersToSwipe(loggedInUser).ToList();
            }

            if (UsersToSwipe.Count == 0)
            {
                NoUsersToSwipe = true;
                return;
            }

            var currentUserIndex = HttpContext.Session.GetInt32("currentUserIndex");

            //If we have reached end of users or we dont have a value the index will be set to zero.
            if (currentUserIndex == null || currentUserIndex >= UsersToSwipe.Count())
            {
                HttpContext.Session.SetInt32("currentUserIndex", 0);

                currentUserIndex = HttpContext.Session.GetInt32("currentUserIndex");
            }

            CurrentUserShown = UsersToSwipe[(int)currentUserIndex!];
        }
        public IActionResult OnPost(string like)
        {
            // Insert like/dislike logic here, such as updating the database with the like.
            var userIndex = GetCurrentUserIndex();

            var loggedInUser = _userRepo.GetLoggedInUser();

            UsersToSwipe = _userRepo.GetUsersToSwipe(loggedInUser).ToList();
            var likedUser = UsersToSwipe[userIndex];

            if (like == "true" && CheckIfMatch(loggedInUser.Id, likedUser.Id))
            {
                ViewData["Match"] = "true";
                IncrementUserIndex();
                return RedirectToPage("/Index", new { match = "true" });
            }

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

            _conext.Interactions.Add(newLike);
            _conext.SaveChanges();

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

            _conext.Add(newMatch);
            _conext.SaveChanges();
        }
    }
}