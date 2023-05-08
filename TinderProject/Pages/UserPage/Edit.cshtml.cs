using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TinderProject.Data;
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
		public User User { get; set; }

		public void OnGet()
		{
			var loggedInUser = _userRepository.GetLoggedInUser();

			User = _database.Users.Find(loggedInUser.Id);
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

			userToUpdate.FirstName = User.FirstName;
			userToUpdate.LastName = User.LastName;
			userToUpdate.DateOfBirth = User.DateOfBirth;
			userToUpdate.Gender = User.Gender;
			userToUpdate.Preference = User.Preference;
			userToUpdate.ProfilePictureUrl = User.ProfilePictureUrl;
			userToUpdate.Description = User.Description;
			userToUpdate.PremiumUser = User.PremiumUser;

			_database.Users.Update(userToUpdate);
			_database.SaveChanges();

			return RedirectToPage("/UserPage/Index");
		}
	}
}