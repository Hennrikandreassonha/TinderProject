using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TinderProject.Data;
using TinderProject.Models;
using TinderProject.Repositories.Repositories_Interfaces;
using System.IO;


namespace TinderProject.Pages.UserPage
{
	public class EditModel : PageModel
	{
		private readonly IUserRepository _userRepository;
		private readonly AppDbContext _database;

		public EditModel(IUserRepository userRepository, AppDbContext database)
		{
			_userRepository = userRepository;
			_database = database;
		}

		[BindProperty]
		public User LoggedInUser { get; set; }
		public List<string> AllInterests { get; set; }
		public User UserToUpdate { get; set; }
		public void OnGet()
		{
			AllInterests = System.IO.File.ReadAllLines("./Data/DataToUsers/Interests.txt").ToList();

			LoggedInUser = _userRepository.GetLoggedInUser();
		}

		public IActionResult OnPost(int loggedInId, List<string> interestsToAdd)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			//LoggedInUser = _userRepository.GetLoggedInUser();
			//UserInterest = _database.Users.Include(i => i.Interests).FirstOrDefault(u => u.Id == LoggedInUser.Id);
			UserToUpdate = _database.Users.Include(u => u.Interests).FirstOrDefault(u => u.Id == loggedInId);

			if (UserToUpdate == null)
			{
				return NotFound();
			}

			UserToUpdate.FirstName = LoggedInUser.FirstName;
			UserToUpdate.LastName = LoggedInUser.LastName;
			UserToUpdate.DateOfBirth = LoggedInUser.DateOfBirth;
			UserToUpdate.Gender = LoggedInUser.Gender;
			UserToUpdate.Preference = LoggedInUser.Preference;
			UserToUpdate.ProfilePictureUrl = LoggedInUser.ProfilePictureUrl;
			UserToUpdate.Description = LoggedInUser.Description;
			UserToUpdate.PremiumUser = LoggedInUser.PremiumUser;

			if (UserToUpdate.Interests.Clear != null)
			{
				UserToUpdate.Interests.Clear();
			}

			List<Interests> newInterests = interestsToAdd
				.Where(interest => interest != null)
				.Select(interest => new Interests
				{
					Interest = interest,
					UserId = UserToUpdate.Id
				}).ToList();

			UserToUpdate.Interests.AddRange(newInterests);

			_database.Users.Update(UserToUpdate);
			_database.SaveChanges();

			return RedirectToPage("/UserPage/Index");
		}
	}
}