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
			if (!ModelState.IsValid)
			{
				return;
			}

			UserToUpdate = _userRepository.GetLoggedInUser();

			if (UserToUpdate == null)
			{
				return;
			}

			PersonalityType = DeterminePersonalityType(questions);
			UserToUpdate.PersonalityType = PersonalityType;

			PersonalityTypeMessage = $"Your personality type is: {PersonalityType}";

			_database.Users.Update(UserToUpdate);
			_database.SaveChanges();
		}

		public string DeterminePersonalityType(List<string> questions)
		{
			if (questions[0] == "I" && questions[1] == "S" && questions[2] == "T" && questions[3] == "J")
			{
				return "Inspector (ISTJ)";
			}
			else if (questions[0] == "I" && questions[1] == "S" && questions[2] == "T" && questions[3] == "P")
			{
				return "Craftsman (ISTP)";
			}
			else if (questions[0] == "I" && questions[1] == "S" && questions[2] == "F" && questions[3] == "J")
			{
				return "Protector (ISFJ)";
			}
			else if (questions[0] == "I" && questions[1] == "S" && questions[2] == "F" && questions[3] == "P")
			{
				return "Composer (ISFP)";
			}
			else if (questions[0] == "I" && questions[1] == "N" && questions[2] == "T" && questions[3] == "J")
			{
				return "Mastermind (INTJ)";
			}
			else if (questions[0] == "I" && questions[1] == "N" && questions[2] == "T" && questions[3] == "P")
			{
				return "Architect (INTP)";
			}
			else if (questions[0] == "I" && questions[1] == "N" && questions[2] == "F" && questions[3] == "J")
			{
				return "Counselor (INFJ)";
			}
			else if (questions[0] == "I" && questions[1] == "N" && questions[2] == "F" && questions[3] == "P")
			{
				return "Healer (INFP)";
			}
			else if (questions[0] == "E" && questions[1] == "S" && questions[2] == "T" && questions[3] == "J")
			{
				return "Supervisor (ESTJ)";
			}
			else if (questions[0] == "E" && questions[1] == "S" && questions[2] == "T" && questions[3] == "P")
			{
				return "Dynamo (ESTP)";
			}
			else if (questions[0] == "E" && questions[1] == "S" && questions[2] == "F" && questions[3] == "J")
			{
				return "Provider (ESFJ)";
			}
			else if (questions[0] == "E" && questions[1] == "S" && questions[2] == "F" && questions[3] == "P")
			{
				return "Performer (ESFP)";
			}
			else if (questions[0] == "E" && questions[1] == "N" && questions[2] == "T" && questions[3] == "J")
			{
				return "Commander (ENTJ)";
			}
			else if (questions[0] == "E" && questions[1] == "N" && questions[2] == "T" && questions[3] == "P")
			{
				return "Visionary (ENTP)";
			}
			else if (questions[0] == "E" && questions[1] == "N" && questions[2] == "F" && questions[3] == "J")
			{
				return "Teacher (ENFJ)";
			}
			else if (questions[0] == "E" && questions[1] == "N" && questions[2] == "F" && questions[3] == "P")
			{
				return "Champion (ENFP)";
			}

			return "Unknown";
		}
	}
}

