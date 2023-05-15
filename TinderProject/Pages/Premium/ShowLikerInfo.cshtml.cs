using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TinderProject.Pages.Premium
{
    public class ShowLikerInfoModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _database;

        public User? LikerUser { get; set; }
        public User CurrentUser { get; set; }

        [BindProperty]
        public bool Match { get; set; }

        public ShowLikerInfoModel(IUserRepository userRepository, AppDbContext database)
        {
            _userRepository = userRepository;
            _database = database;
            LikerUser = new User();
            CurrentUser = new User();
        }

        public void OnGet(int userId, string? match)
        {
            if (match == "true")
            {
                Match = true;
            }

            CurrentUser = _userRepository.GetLoggedInUser();
            LikerUser = _database.Users.Include(i => i.Interests).Where(u => u.Id == userId).FirstOrDefault();
        }

        public IActionResult OnPost(int userId, bool like)
        {
            if (like)
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
                
                return RedirectToPage("/Premium/ShowLikerInfo", new { userId, match = "true" } );
            }
            else
            {

                return RedirectToPage("/Premium/Index");
            }

        }

    }
}
