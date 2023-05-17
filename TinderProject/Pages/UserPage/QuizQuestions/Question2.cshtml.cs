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
			Questions = new List<string>();
		}

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

			return RedirectToPage("Question3", new { questions = Questions });
		}
	}
}
