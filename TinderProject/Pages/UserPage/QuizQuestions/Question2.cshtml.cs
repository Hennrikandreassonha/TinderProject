using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinderProject.Pages.UserPage.QuizQuestions
{
    public class Question2Model : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _database;

        public Question2Model(IUserRepository userRepository, AppDbContext database)
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

            UserQuiz.Question2 = Request.Form["Answer"];

            if (UserQuiz.Question2 == null)
            {
                return Page();
            }

            UserToUpdate.UserQuiz = UserQuiz;

			_database.Users.Update(UserToUpdate);
            _database.SaveChanges();

            return RedirectToPage("Question3");
        }
    }
}
