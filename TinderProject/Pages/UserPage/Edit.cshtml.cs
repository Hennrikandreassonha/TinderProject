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
        public string UserPhoto { get; set; }

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
            LoggedInUser = _userRepository.GetLoggedInUser();
            //Removing photo from modelstate since its not required.

            if (photo == null)
            {
                ModelState.Remove("photo");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (LoggedInUser == null)
            {
                return NotFound();
            }

            //Clear directory because users can only have one pic.
            if (photo != null)
            {
                _fileRepo.ClearDirectory(LoggedInUser);

                string path = Path.Combine(
                    LoggedInUser.Id.ToString(),
                    Guid.NewGuid().ToString() + "-" + photo.FileName
                    );

                //Saving pic to user directory.
                await _fileRepo.SaveFileAsync(photo, path);

                LoggedInUser.ProfilePictureUrl = _fileRepo.GetProfilePic(LoggedInUser);
            }

            LoggedInUser.FirstName = LoggedInUser.FirstName;
            LoggedInUser.LastName = LoggedInUser.LastName;
            LoggedInUser.DateOfBirth = LoggedInUser.DateOfBirth;
            LoggedInUser.Gender = LoggedInUser.Gender;
            LoggedInUser.Preference = LoggedInUser.Preference;
            LoggedInUser.Description = LoggedInUser.Description;
            LoggedInUser.PremiumUser = LoggedInUser.PremiumUser;
            LoggedInUser.PersonalityType = LoggedInUser.PersonalityType;

            LoggedInUser.Interests?.Clear();

            List<Interests> newInterests = interestsToAdd
                .Where(interest => interest != null)
                .Select(interest => new Interests
                {
                    Interest = interest,
                    UserId = LoggedInUser.Id
                }).ToList();

            LoggedInUser.Interests.AddRange(newInterests);

            LoggedInUser.Cuisines?.Clear();

            List<Cuisines> newCuisines = cuisinesToAdd
                .Where(cuisine => cuisine != null)
                .Select(cuisine => new Cuisines
                {
                    Cuisine = cuisine,
                    UserId = LoggedInUser.Id
                }).ToList();

            LoggedInUser.Cuisines.AddRange(newCuisines);

            _database.Users.Update(LoggedInUser);
            _database.SaveChanges();

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