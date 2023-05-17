using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinderProject.Pages.UserPage.QuizQuestions
{
    public class Question1Model : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _database;

        public Question1Model(IUserRepository userRepository, AppDbContext database)
        {
            _userRepository = userRepository;
            _database = database;
        }

        public User UserToUpdate { get; set; }

        public List<string> Questions { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string Answer)
        {
            UserToUpdate = _userRepository.GetLoggedInUser();

            if (UserToUpdate == null)
            {
                return NotFound();
            }
            Questions = new List<string>();
            {
                Questions.Add(Answer);

            }

            if (Answer == null)
            {
                return Page();
            }

            return RedirectToPage("Question2", new { questions = Questions });
        }
    }
}
