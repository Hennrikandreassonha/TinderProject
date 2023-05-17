using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TinderProject.Pages
{
	public class TinderUser : PageModel
	{
		private readonly IUserRepository _userRepository;
		private readonly AppDbContext _database;

		public User? LikerUser { get; set; }
		public User CurrentUser { get; set; }

		[BindProperty]
		public bool Match { get; set; }

		public TinderUser(IUserRepository userRepository, AppDbContext database)
		{
			_userRepository = userRepository;
			_database = database;
			LikerUser = new User();
			CurrentUser = new User();
		}

		public void OnGet(int id, string? match)
		{
			if (match == "true")
			{
				Match = true;
			}

			CurrentUser = _userRepository.GetLoggedInUser();
			LikerUser = _database.Users.Include(i => i.Interests).Where(u => u.Id == id).FirstOrDefault();
		}

		public IActionResult OnPost(int userId, bool like)
		{
			if (like)
			{
				CurrentUser = _userRepository.GetLoggedInUser();
				Match newMatch = new()
				{
					User1Id = userId,
					User2Id = CurrentUser.Id,
					MatchDate = DateTime.Now
				};

				_database.Matches.Add(newMatch);
				_database.SaveChanges();

				return RedirectToPage("/Premium/ShowLikerInfo", new { userId, match = "true" });
			}
			else
			{
				return RedirectToPage("/Premium/Index");
			}

		}

	}
}
