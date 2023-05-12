using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
            LikerUser = _database.Users.Include(i => i.Interests).Where(u => u.Id == userId).FirstOrDefault();

        }

        public IActionResult OnPost(int userId)
        {
            CurrentUser = _userRepository.GetLoggedInUser();
            Match newMatch = new()
            {
                User1Id = userId,
                User2Id = CurrentUser.Id,
                MatchDate = DateTime.Now
            };

            _database.Matches.Add(newMatch);
            _database.SaveChanges();

            return RedirectToPage("/Premium/Index");
        }

    }
}
