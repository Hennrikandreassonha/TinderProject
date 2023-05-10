using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinderProject.Pages.Premium
{
    public class ShowLikerInfoModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _database;

        public User? LikerUser { get; set; }
        public User CurrentUser { get; set; }

        public ShowLikerInfoModel(IUserRepository userRepository, AppDbContext database)
        {
            _userRepository = userRepository;
            _database = database;
            LikerUser = new User();
            CurrentUser = new User();
        }

        public void OnGet(int userId)
        {
            CurrentUser = _userRepository.GetLoggedInUser();
            LikerUser = _database.Users
                        .Include(u => u.Interests).ToList()
                        .FirstOrDefault(u => u.Id == userId);
        }
    }
}
