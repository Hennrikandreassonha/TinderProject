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
        public User UserToUpdate { get; set; }
        public string UserPhoto { get; set; }
		public IActionResult RedidirectToPage { get; private set; }

		public void OnGet()
        {
            LoggedInUser = _userRepository.GetLoggedInUser();

            string userFolderPath = Path.Combine(
                _fileRepo.FolderPath,
                LoggedInUser.Id.ToString()
                );

            //Getting all the files from the user directory.
            Directory.CreateDirectory(userFolderPath);
            string[] files = Directory.GetFiles(userFolderPath);

            UserPhoto = _fileRepo.GetProfilePic(LoggedInUser);
        }
        public async Task<IActionResult> OnPost(List<string> interestsToAdd, List<string> cuisinesToAdd, IFormFile photo)
        {
            UserToUpdate = _userRepository.GetLoggedInUser();
            //Removing photo from modelstate since its not required.
            ModelState.Remove("photo");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (UserToUpdate == null)
            {
                return NotFound();
            }

            //Clear directory because users can only have one pic.
            if (photo != null)
            {
                _fileRepo.ClearDirectory(UserToUpdate);

                string path = Path.Combine(
                    UserToUpdate.Id.ToString(),
                    Guid.NewGuid().ToString() + "-" + photo.FileName
                    );

                //Saving pic to user directory.
                await _fileRepo.SaveFileAsync(photo, path);
                UserToUpdate.ProfilePictureUrl = _fileRepo.GetProfilePic(UserToUpdate);
            }

			int wordCount = LoggedInUser.Description.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
			if (wordCount > 30)
			{
				string errorMessage = $"Your description is {wordCount} words. The description can only be 30 words.";
				ModelState.AddModelError("LoggedInUser.Description", errorMessage);
				return Page();
			}

			UserToUpdate.FirstName = LoggedInUser.FirstName;
            UserToUpdate.LastName = LoggedInUser.LastName;
            UserToUpdate.DateOfBirth = LoggedInUser.DateOfBirth;
            UserToUpdate.Gender = LoggedInUser.Gender;
            UserToUpdate.Preference = LoggedInUser.Preference;
            UserToUpdate.Description = LoggedInUser.Description;
            UserToUpdate.PremiumUser = LoggedInUser.PremiumUser;
            UserToUpdate.PersonalityType = UserToUpdate.PersonalityType;

            UserToUpdate.Interests?.Clear();

            List<Interests> newInterests = interestsToAdd
                .Where(interest => interest != null)
                .Select(interest => new Interests
                {
                    Interest = interest,
                    UserId = UserToUpdate.Id
                }).ToList();

            UserToUpdate.Interests.AddRange(newInterests);
            UserToUpdate.Cuisines?.Clear();

            List<Cuisines> newCuisines = cuisinesToAdd
                .Where(cuisine => cuisine != null)
                .Select(cuisine => new Cuisines
                {
                    Cuisine = cuisine,
                    UserId = UserToUpdate.Id
                }).ToList();

            UserToUpdate.Cuisines.AddRange(newCuisines);

            _database.Users.Update(UserToUpdate);
            _database.SaveChanges();

            if(UserToUpdate.PersonalityType == null)
            {
                return RedirectToPage("/UserPage/QuizQuestions/Index");
            }

            return RedirectToPage("/UserPage/Index");
        }
        public List<string> GetUserInterests()
        {
            var user = _userRepository.GetLoggedInUser();

            return user.Interests.Select(x => x.Interest).ToList();
        }
        public List<string> GetUserCuisines()
        {
            var user = _userRepository.GetLoggedInUser();

            return user.Cuisines.Select(x => x.Cuisine).ToList();
        }
    }
}