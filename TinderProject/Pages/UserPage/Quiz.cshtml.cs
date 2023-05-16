using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinderProject.Pages.UserPage
{
    public class QuizModel : PageModel
    {
		private readonly IUserRepository _userRepository;
		private readonly AppDbContext _database;

		public QuizModel(IUserRepository userRepository, AppDbContext database)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

			var UserToUpdate = _userRepository.GetLoggedInUser();

			if (UserToUpdate == null)
			{
				return NotFound();
			}

			UserToUpdate.PersonalityType = DeterminePersonalityType(UserQuiz);

			//UserToUpdate.PersonalityType = LoggedInUser.PersonalityType;

			_database.Users.Update(UserToUpdate);
			_database.SaveChanges();

			return RedirectToPage("/UserPage/Index");
		}

		public string DeterminePersonalityType(Quiz UserQuiz)
		{
			if(UserQuiz.Question1 == "I" && UserQuiz.Question2 == "S"
				&& UserQuiz.Question3 == "T" && UserQuiz.Question4 == "J")
			{
				return "Inspector (ISTJ)";
			}
			else if (UserQuiz.Question1 == "I" && UserQuiz.Question2 == "S" &&
			 UserQuiz.Question3 == "T" && UserQuiz.Question4 == "P")
			{
				return "Craftsman (ISTP)";
			}
			else if (UserQuiz.Question1 == "I" && UserQuiz.Question2 == "S"
			&& UserQuiz.Question3 == "F" && UserQuiz.Question4 == "J")
			{
				return "Protector (ISFJ)";
			}
			else if (UserQuiz.Question1 == "I" && UserQuiz.Question2 == "S"
			&& UserQuiz.Question3 == "F" && UserQuiz.Question4 == "P")
			{
				return "Composer (ISFP)";
			}
			else if (UserQuiz.Question1 == "I" && UserQuiz.Question2 == "N"
			&& UserQuiz.Question3 == "T" && UserQuiz.Question4 == "J")
			{
				return "Mastermind (INTJ)";
			}
			else if (UserQuiz.Question1 == "I" && UserQuiz.Question2 == "N"
			&& UserQuiz.Question3 == "T" && UserQuiz.Question4 == "P")
			{
				return "Architect (INTP)";
			}
			else if (UserQuiz.Question1 == "I" && UserQuiz.Question2 == "N"
			&& UserQuiz.Question3 == "F" && UserQuiz.Question4 == "J")
			{
				return "Counselor (INFJ)";
			}
			else if (UserQuiz.Question1 == "I" && UserQuiz.Question2 == "N"
			&& UserQuiz.Question3 == "F" && UserQuiz.Question4 == "P")
			{
				return "Healer (INFP)";
			}
			else if (UserQuiz.Question1 == "E" && UserQuiz.Question2 == "S"
			&& UserQuiz.Question3 == "T" && UserQuiz.Question4 == "J")
			{
				return "Supervisor (ESTJ)";
			}
			else if (UserQuiz.Question1 == "E" && UserQuiz.Question2 == "S"
			&& UserQuiz.Question3 == "T" && UserQuiz.Question4 == "P")
			{
				return "Dynamo (ESTP)";
			}
			else if (UserQuiz.Question1 == "E" && UserQuiz.Question2 == "S"
			&& UserQuiz.Question3 == "F" && UserQuiz.Question4 == "J")
			{
				return "Provider (ESFJ)";
			}
			else if (UserQuiz.Question1 == "E" && UserQuiz.Question2 == "S"
			&& UserQuiz.Question3 == "F" && UserQuiz.Question4 == "P")
			{
				return "Performer (ESFP)";
			}
			else if (UserQuiz.Question1 == "E" && UserQuiz.Question2 == "N"
			&& UserQuiz.Question3 == "T" && UserQuiz.Question4 == "J")
			{
				return "Commander (ENTJ)";
			}
			else if (UserQuiz.Question1 == "E" && UserQuiz.Question2 == "N"
			&& UserQuiz.Question3 == "T" && UserQuiz.Question4 == "P")
			{
				return "Visionary (ENTP)";
			}
			else if (UserQuiz.Question1 == "E" && UserQuiz.Question2 == "N"
			&& UserQuiz.Question3 == "F" && UserQuiz.Question4 == "J")
			{
				return "Teacher (ENFJ)";
			}
			else if (UserQuiz.Question1 == "E" && UserQuiz.Question2 == "N"
			&& UserQuiz.Question3 == "F" && UserQuiz.Question4 == "P")
			{
				return "Champion (ENFP)";
			}

			return "Unknown";
		}
    }
}
