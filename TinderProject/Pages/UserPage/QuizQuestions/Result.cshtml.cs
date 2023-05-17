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

			PersonalityType = DeterminePersonalityType();
			UserToUpdate.PersonalityType = PersonalityType;

			PersonalityTypeMessage = $"Your personality type is: {PersonalityType}";

			_database.Users.Update(UserToUpdate);
			_database.SaveChanges();

			return Page();
		}

		public string DeterminePersonalityType()
		{
			if (Questions[0] == "I" && Questions[1] == "S" && Questions[2] == "T" && Questions[3] == "J")
			{
				return "Inspector (ISTJ)";
			}
			else if (Questions[0] == "I" && Questions[1] == "S" && Questions[2] == "T" && Questions[3] == "P")
			{
				return "Craftsman (ISTP)";
			}
			else if (Questions[0] == "I" && Questions[1] == "S" && Questions[2] == "F" && Questions[3] == "J")
			{
				return "Protector (ISFJ)";
			}
			else if (Questions[0] == "I" && Questions[1] == "S" && Questions[2] == "F" && Questions[3] == "P")
			{
				return "Composer (ISFP)";
			}
			else if (Questions[0] == "I" && Questions[1] == "N"	&& Questions[2] == "T" && Questions[3] == "J")
			{
				return "Mastermind (INTJ)";
			}
			else if (Questions[0] == "I" && Questions[1] == "N" && Questions[2] == "T" && Questions[3] == "P")
			{
				return "Architect (INTP)";
			}
			else if (Questions[0] == "I" && Questions[1] == "N" && Questions[2] == "F" && Questions[3] == "J")
			{
				return "Counselor (INFJ)";
			}
			else if (Questions[0] == "I" && Questions[1] == "N" && Questions[2] == "F" && Questions[3] == "P")
			{
				return "Healer (INFP)";
			}
			else if (Questions[0] == "E" && Questions[1] == "S"	&& Questions[2] == "T" && Questions[3] == "J")
			{
				return "Supervisor (ESTJ)";
			}
			else if (Questions[0] == "E" && Questions[1] == "S" && Questions[2] == "T" && Questions[3] == "P")
			{
				return "Dynamo (ESTP)";
			}
			else if (Questions[0] == "E" && Questions[1] == "S" && Questions[2] == "F" && Questions[3] == "J")
			{
				return "Provider (ESFJ)";
			}
			else if (Questions[0] == "E" && Questions[1] == "S" && Questions[2] == "F" && Questions[3] == "P")
			{
				return "Performer (ESFP)";
			}
			else if (Questions[0] == "E" && Questions[1] == "N" && Questions[2] == "T" && Questions[3] == "J")
			{
				return "Commander (ENTJ)";
			}
			else if (Questions[0] == "E" && Questions[1] == "N" && Questions[2] == "T" && Questions[3] == "P")
			{
				return "Visionary (ENTP)";
			}
			else if (Questions[0] == "E" && Questions[1] == "N" && Questions[2] == "F" && Questions[3] == "J")
			{
				return "Teacher (ENFJ)";
			}
			else if (Questions[0] == "E" && Questions[1] == "N" && Questions[2] == "F" && Questions[3] == "P")
			{
				return "Champion (ENFP)";
			}

			return "Unknown";
		}
	}
}
