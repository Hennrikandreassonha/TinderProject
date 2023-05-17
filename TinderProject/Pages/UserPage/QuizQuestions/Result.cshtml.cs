using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Dynamic;

namespace TinderProject.Pages.UserPage.QuizQuestions
{
    public class ResultModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _database;

        public ResultModel(IUserRepository userRepository, AppDbContext database)
        {
            _userRepository = userRepository;
            _database = database;
			Questions = new List<string>();
			
        }
		public User UserToUpdate { get; set; }
		public string PersonalityType { get; set; }
		public string PersonalityTypeMessage { get; set; }

        public List<string> Questions { get; set; }

        public void OnGet(List<string> questions) 
		{ 
			Questions.AddRange(questions);
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			UserToUpdate = _userRepository.GetLoggedInUser();

			if (UserToUpdate == null)
			{
				return NotFound();
			}

			PersonalityType = DeterminePersonalityType(UserToUpdate.UserQuiz);
			UserToUpdate.PersonalityType = PersonalityType;

			PersonalityTypeMessage = $"Your personality type is: {PersonalityType}";

			_database.Users.Update(UserToUpdate);
			_database.SaveChanges();

			return Page();
		}

		public string DeterminePersonalityType(Quiz UserQuiz)
		{
			if (UserQuiz.Question1 == "I" && UserQuiz.Question2 == "S"
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
