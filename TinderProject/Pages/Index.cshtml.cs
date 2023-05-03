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

        public void OnGet(int id)
        {
            var selectedUser = _userRepo.GetUser(id);

            var females = _userRepo.GetAllFemale();

            var allUsers = _userRepo.GetAllUsers();

            var loggedIn = _userRepo.GetLoggedInUser();
        }
    }
}