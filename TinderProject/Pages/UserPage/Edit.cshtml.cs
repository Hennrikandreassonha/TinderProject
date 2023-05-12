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
using TinderProject.Repositories;

namespace TinderProject.Pages.UserPage
{
    public class EditModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _database;
        private readonly FileRepository _fileRepo;

        public EditModel(IUserRepository userRepository, AppDbContext database, FileRepository fileRepo)
        {
            _userRepository = userRepository;
            _database = database;
            _fileRepo = fileRepo;
        }

        [BindProperty]
        public User LoggedInUser { get; set; }
        public List<string> AllInterests { get; set; }
        public List<string> AllCuisines { get; set; }
        public User UserToUpdate { get; set; }
        public List<string> PhotoURLs { get; set; } = new List<string>();
        public void OnGet()
        {
            AllInterests = System.IO.File.ReadAllLines("./Data/DataToUsers/Interests.txt").ToList();
            AllCuisines = System.IO.File.ReadAllLines("./Data/DataToUsers/Cuisines.txt").ToList();

            LoggedInUser = _userRepository.GetLoggedInUser();

            string userFolderPath = Path.Combine(
                _fileRepo.FolderPath,
                LoggedInUser.Id.ToString()
                );

            Directory.CreateDirectory(userFolderPath);
            string[] files = Directory.GetFiles(userFolderPath);
            foreach (string file in files)
            {
                string url = _fileRepo.GetFileURL(file);
                PhotoURLs.Add(url);
            }
        }

        //Kommenterar ut IFormFile eftersom den g�r att ModelState blir invalid, om man inte l�gger till en bild. Den �r required allts�
        public async Task<IActionResult> OnPost(int loggedInId, List<string> interestsToAdd, List<string> cuisinesToAdd) //IFormFile photo)
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

            /*string path = Path.Combine(
                UserToUpdate.Id.ToString(),
                Guid.NewGuid().ToString() + "-" + photo.FileName
                );
            await _fileRepo.SaveFileAsync(photo, path);*/

            UserToUpdate.FirstName = LoggedInUser.FirstName;
            UserToUpdate.LastName = LoggedInUser.LastName;
            UserToUpdate.DateOfBirth = LoggedInUser.DateOfBirth;
            UserToUpdate.Gender = LoggedInUser.Gender;
            UserToUpdate.Preference = LoggedInUser.Preference;
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

            //Kommenterar ut detta d� det inte funkar f�r tillf�llet
			/*if (UserToUpdate.Cuisines != null)
			{
				UserToUpdate.Cuisines.Clear();
			}

			List<Cuisines> newCuisines = cuisinesToAdd
				.Where(cuisine => cuisine != null)
				.Select(cuisine => new Cuisines
				{
					Cuisine = cuisine,
					UserId = UserToUpdate.Id
				}).ToList();

			UserToUpdate.Cuisines.AddRange(newCuisines);*/

			_database.Users.Update(UserToUpdate);
            _database.SaveChanges();

            return RedirectToPage("/UserPage/Index");
        }
    }
}