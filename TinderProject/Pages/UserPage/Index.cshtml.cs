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
	public class IndexModel : PageModel
	{
		private readonly IUserRepository _userRepository;
		private readonly AppDbContext _database;

		public IndexModel(IUserRepository userRepository, AppDbContext database)
		{
			_userRepository = userRepository;
			_database = database;
		}

		public User LoggedInUser { get; set; }

		public void OnGet()
		{
			LoggedInUser = _userRepository.GetLoggedInUser();

			//loggedInUser = _database.Users.Find(loggedInUser.Id);
		}
	}
}