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
		public User LoggedInUser { get; set; }
		public List<string> AllPersonalityTypes { get; set; }
		public User UserToUpdate { get; set; }

		[BindProperty]
		public Quiz UserQuiz { get; set; }

		public void OnGet()
        {
            AllPersonalityTypes = System.IO.File.ReadAllLines("./Data/DataToUsers/Personalitytypes.txt").ToList();

			LoggedInUser = _userRepository.GetLoggedInUser();
		}

        public IActionResult OnPost(int loggedInId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

			UserToUpdate = _database.Users.FirstOrDefault(u => u.Id == loggedInId);

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
			if(UserQuiz.Question1 == "alone" && UserQuiz.Question2 == "senses-information"
				&& UserQuiz.Question3 == "logic-analysis" && UserQuiz.Question4 == "detailed-plan-deadlines")
			{
				return "Inspector(ISTJ)";
			}
			else if (UserQuiz.Question1 == "alone" && UserQuiz.Question2 == "senses-information" &&
			 UserQuiz.Question3 == "logic-analysis" && UserQuiz.Question4 == "general-idea-adjust")
			{
				return "Commander (ENTJ)";
			}
			else if (UserQuiz.Question1 == "alone" && UserQuiz.Question2 == "feeling-intuition"
			&& UserQuiz.Question3 == "feeling-intuition" && UserQuiz.Question4 == "general-idea-adjust")
			{
				return "Composer (ISFP)";
			}
			else if (UserQuiz.Question1 == "around-people" && UserQuiz.Question2 == "senses-information"
			&& UserQuiz.Question3 == "logic-analysis" && UserQuiz.Question4 == "detailed-plan-deadlines")
			{
				return "Dynamo (ESTP)";
			}
			else if (UserQuiz.Question1 == "around-people" && UserQuiz.Question2 == "feeling-intuition"
			&& UserQuiz.Question3 == "feeling-intuition" && UserQuiz.Question4 == "general-idea-adjust")
			{
				return "Performer (ESFP)";
			}
			else if (UserQuiz.Question1 == "alone" && UserQuiz.Question2 == "senses-information"
			&& UserQuiz.Question3 == "logic-analysis" && UserQuiz.Question4 == "general-idea-adjust")
			{
				return "Architect (INTP)";
			}
			else if (UserQuiz.Question1 == "alone" && UserQuiz.Question2 == "feeling-intuition"
			&& UserQuiz.Question3 == "feelings-affect-others" && UserQuiz.Question4 == "detailed-plan-deadlines")
			{
				return "Counselor (INFJ)";
			}
			else if (UserQuiz.Question1 == "around-people" && UserQuiz.Question2 == "senses-information"
			&& UserQuiz.Question3 == "logic-analysis" && UserQuiz.Question4 == "general-idea-adjust")
			{
				return "Visionary (ENTP)";
			}
			else if (UserQuiz.Question1 == "around-people" && UserQuiz.Question2 == "feeling-intuition"
			&& UserQuiz.Question3 == "feelings-affect-others" && UserQuiz.Question4 == "general-idea-adjust")
			{
				return "Champion (ENFP)";
			}
			else if (UserQuiz.Question1 == "around-people" && UserQuiz.Question2 == "senses-information"
			&& UserQuiz.Question3 == "logic-analysis" && UserQuiz.Question4 == "detailed-plan-deadlines")
			{
				return "Supervisor (ESTJ)";
			}
			else if (UserQuiz.Question1 == "around-people" && UserQuiz.Question2 == "senses-information"
			&& UserQuiz.Question3 == "senses-information" && UserQuiz.Question4 == "detailed-plan-deadlines")
			{
				return "Craftsman (ISTP)";
			}
			else if (UserQuiz.Question1 == "around-people" && UserQuiz.Question2 == "feeling-intuition"
			&& UserQuiz.Question3 == "feelings-affect-others" && UserQuiz.Question4 == "detailed-plan-deadlines")
			{
				return "Teacher (ENFJ)";
			}
			else if (UserQuiz.Question1 == "alone" && UserQuiz.Question2 == "senses-information"
			&& UserQuiz.Question3 == "senses-information" && UserQuiz.Question4 == "general-idea-adjust")
			{
				return "Craftsman (ISTP)";
			}

			return "Unknown";
		}
    }
}
