using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinderProject.Data;
using TinderProject.Repositories.Repositories_Interfaces;

namespace TinderProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepo;

        public IndexModel(IUserRepository repo)
        {
            _userRepo = repo;
        }
        public List<User> UsersToSwipe { get; set; }

        //The user is waiting to be liked/disliked right now.
        public User CurrentUserShown { get; set; }
        public void OnGet()
        {
            var loggedInUser = _userRepo.GetLoggedInUser();

            if (loggedInUser != null)
            {
                UsersToSwipe = _userRepo.GetPreferedUsers(loggedInUser).ToList();
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
            if (like == "true")
            {
                CheckIfMatch();
            }

            IncrementUserIndex();
            return RedirectToPage("/Index");
        }
        public void IncrementUserIndex()
        {
            //Increments the index which is used for showing users.

            var currentUserIndex = HttpContext.Session.GetInt32("currentUserIndex").GetValueOrDefault();
            currentUserIndex++;

            HttpContext.Session.SetInt32("currentUserIndex", currentUserIndex);
        }
        public void CheckIfMatch(){

        }
    }
}