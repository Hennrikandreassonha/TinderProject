using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinderProject.Pages.UserPage.QuizQuestions
{
    public class Question4Model : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _database;

        public Question4Model(IUserRepository userRepository, AppDbContext database)
        {
            _userRepository = userRepository;
            _database = database;
        }
        [BindProperty]
        public Quiz UserQuiz { get; set; }
        public User UserToUpdate { get; set; }

        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (UserQuiz == null)
            {
                return Page();
            }

            UserToUpdate = _userRepository.GetLoggedInUser();

            if (UserToUpdate == null)
            {
                return NotFound();
            }

            UserQuiz.Question1 = Request.Form["Answer"];

			UserToUpdate.UserQuiz = UserQuiz;

			_database.Users.Update(UserToUpdate);
            _database.SaveChanges();

            return RedirectToPage("Result");
        }
    }
}
