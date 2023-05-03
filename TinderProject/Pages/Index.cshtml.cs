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
        public ICollection<User>? UsersToSwipe { get; set; }

        //The user is waiting to be liked/disliked right now.
        public User CurrentUserShown { get; set; }
        public void OnGet()
        {
            var loggedInUser = _userRepo.GetLoggedInUser();

            if (loggedInUser != null)
            {
                UsersToSwipe = _userRepo.GetPreferedUsers(loggedInUser);
            }

            CurrentUserShown = UsersToSwipe[]
        }
    }
}