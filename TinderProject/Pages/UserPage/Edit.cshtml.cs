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
		public User UserInterest { get; set; }

		public void OnGet()
		{
			LoggedInUser = _userRepository.GetLoggedInUser();
			UserInterest = _database.Users.Include(i => i.Interests).FirstOrDefault(u => u.Id == LoggedInUser.Id);
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var loggedInUser = _userRepository.GetLoggedInUser();
			var userToUpdate = _database.Users.Find(loggedInUser.Id);

			if (userToUpdate == null)
			{
				return NotFound();
			}

			userToUpdate.FirstName = LoggedInUser.FirstName;
			userToUpdate.LastName = LoggedInUser.LastName;
			userToUpdate.DateOfBirth = LoggedInUser.DateOfBirth;
			userToUpdate.Gender = LoggedInUser.Gender;
			userToUpdate.Preference = LoggedInUser.Preference;
			userToUpdate.ProfilePictureUrl = LoggedInUser.ProfilePictureUrl;
			userToUpdate.Description = LoggedInUser.Description;
			userToUpdate.PremiumUser = LoggedInUser.PremiumUser;

			userToUpdate.Interests = LoggedInUser.Interests;

			_database.Users.Update(userToUpdate);
			_database.SaveChanges();

			return RedirectToPage("/UserPage/Index");
		}
	}
}