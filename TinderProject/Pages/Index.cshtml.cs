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
        public void OnGet()
        {

            if (ViewData["match"] == "true")
            {
                Match = true;
                ViewData["Match"] = "false";
            }
            var loggedInUser = _userRepo.GetLoggedInUser();

            //Måste exkludera de som redan är matchade.
            if (loggedInUser != null)
            {
                UsersToSwipe = GetUsersToSwipe(loggedInUser);
            }

            var currentUserIndex = HttpContext.Session.GetInt32("currentUserIndex");

            //If we have reached end of users or we dont have a value the index will be set to zero.
            if (currentUserIndex == null || currentUserIndex == UsersToSwipe.Count())
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

            UsersToSwipe = _userRepo.GetPreferedUsers(loggedInUser).ToList();
            var likedUser = UsersToSwipe[userIndex];

            if (like == "true" && CheckIfMatch(loggedInUser, likedUser))
            {
                ViewData["Match"] = "true";
                return RedirectToPage("Index");
            }

            NewInteraction(loggedInUser, likedUser);

            IncrementUserIndex();
            return RedirectToPage("/Index");
        }
        public List<User> GetUsersToSwipe(User loggedInUser)
        {
            //Gets the Id´s of all the Users that the user Already likes.
            //Removes these since you cant like somone who is already liked.
            var userLikesIds = _userRepo.GetUserLikes(loggedInUser).Select(x => x.LikedId);

            return _userRepo.GetAllUsers()
    .Where(u => u.Id != loggedInUser.Id && !userLikesIds.Contains(u.Id)).ToList();
        }
        public void IncrementUserIndex()
        {
            //Increments the index which is used for showing users.
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
        public bool CheckIfMatch(User loggedInUserId, User likedUserId)
        {
            //Gets the Currently Liked User and checks if it likes loggedInUser.
            //In that case it´s a match.
            foreach (var item in _userRepo.GetUserLikes(likedUserId))
            {
                if (item.LikedId == loggedInUserId.Id)
                {
                    CreateNewMatch(loggedInUserId, likedUserId);
                    return true;
                }
            }
            return false;
        }
        public void CreateNewMatch(User user1, User user2)
        {
            Match newMatch = new()
            {
                User1Id = user1.Id,
                User2Id = user2.Id,
                MatchDate = DateTime.Now
            };

            _conext.Add(newMatch);
            _conext.SaveChanges();
        }
    }
}