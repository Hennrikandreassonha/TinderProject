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

        [BindProperty]
        public Quiz UserQuiz { get; set; }
        public User UserToUpdate { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            UserToUpdate = _userRepository.GetLoggedInUser();

            if (UserToUpdate == null)
            {
                return NotFound();
            }

            UserQuiz.Question1 = Request.Form["Answer"];
            
            if (UserQuiz.Question1 == null)
            {
                return Page();
            }

            UserQuiz.Question2 = ""; // Set the value for Question2
			UserQuiz.Question3 = ""; // Set the value for Question3
			UserQuiz.Question4 = "";

			UserToUpdate.UserQuiz = UserQuiz;

			_database.Users.Update(UserToUpdate);
            _database.SaveChanges();

            return RedirectToPage("Question2");
        }
    }
}
