using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinderProject.Pages.UserPage.QuizQuestions
{
    public class Question3Model : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _database;

        public Question3Model(IUserRepository userRepository, AppDbContext database)
        {
            _userRepository = userRepository;
            _database = database;
            Questions = new List<string>();
        }
        [BindProperty]
        public Quiz UserQuiz { get; set; }
        public User UserToUpdate { get; set; }
        public List<string> Questions { get; set; }

        public void OnGet()
        {
           
        }
        public IActionResult OnPost(string Answer, List<string> questions)
        {
            UserToUpdate = _userRepository.GetLoggedInUser();

            if (UserToUpdate == null)
            {
                return NotFound();
            }

            
            if (Answer == null)
            {
                return Page();
            }

            questions.Add(Answer);
            Questions.AddRange(questions);


            return RedirectToPage("Question4", new { questions = Questions });
        }
    }
}
