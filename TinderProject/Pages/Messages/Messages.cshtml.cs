using System.Runtime.Serialization.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinderProject.Data;
using TinderProject.Models;
using TinderProject.Repositories.Repositories_Interfaces;
using Newtonsoft.Json;
using TinderProject.Controllers;
using System.Collections.Specialized;
using TinderProject.Repositories;

namespace TinderProject.Pages.Messages
{
    public class MessagesModel : PageModel
    {
        private readonly AppDbContext _database;
        private readonly IUserRepository _userRepository;
        private readonly BlobRepo _blobRepo;

        private static Random random = new();

        public Message Message { get; set; }
        public User OtherUser { get; set; }
        public User CurrentUser { get; set; }
        public List<User> User { get; set; }
        public List<Message> Messages { get; set; }
        public List<User> NoConversation { get; set; }

        public MessagesModel(AppDbContext database, IUserRepository userRepository, BlobRepo blobRepo)
        {
            _blobRepo = blobRepo;
            _database = database;
            _userRepository = userRepository;
            Messages = new List<Message>();
            User = new List<User>();
            NoConversation = new List<User> { };
        }

        public void OnGet(int? userId)
        {
            CurrentUser = _userRepository.GetLoggedInUser();
            OtherUser = _database.Users.SingleOrDefault(u => u.Id == userId);

            Messages = _database.Messages
                .Where(m => (m.SentToId == CurrentUser.Id && m.SentFromId == userId) ||
                      (m.SentToId == userId && m.SentFromId == CurrentUser.Id))
                .OrderBy(m => m.SentTime)
                .ToList();

            foreach (var message in Messages)
            {
                if (CurrentUser.Id == message.SentToId)
                {
                    message.isRead = true;
                    _database.Messages.Update(message);

                }
                _database.SaveChanges();
            }

            var otherUsersIds = _database.Messages
                .Where(m => m.SentToId == CurrentUser.Id || m.SentFromId == CurrentUser.Id)
                .Select(m => m.SentToId == CurrentUser.Id ? m.SentFromId : m.SentToId)
                .Distinct()
                .ToList();

            User.AddRange(_database.Users.Where(u => otherUsersIds.Contains(u.Id)).ToList());

            //Check if a match has a conversation 
            var matches = _database.Matches.ToList();
            var messages = _database.Messages.ToList();

            foreach (var match in matches)
            {
                bool haveConversation = messages.Any(m =>
                (m.SentToId == match.User2Id && m.SentFromId == match.User1Id)
                || (m.SentToId == match.User1Id && m.SentFromId == match.User2Id));

                if (!haveConversation)
                {
                    if (match.User1Id == CurrentUser.Id)
                    {
                        NoConversation.Add(_database.Users.Single(u => u.Id == match.User2Id));
                    }
                    else if (match.User2Id == CurrentUser.Id)
                    {
                        NoConversation.Add(_database.Users.Single(u => u.Id == match.User1Id));
                    }
                }
            }
        }

        public IActionResult OnPost(string message, int userId)
        {
            if (_userRepository.GetUser(userId) == null)
            {
                return NotFound();
            }
            if (message != null)
            {
                AddMessage(message, userId);
            }

            Console.WriteLine($"String: {_blobRepo.GetBlobUrlAsync("")}");
            var sstring = _blobRepo.GetBlobUrlAsync("");
            return RedirectToPage();
        }
        public void AddMessage(string message, int userId)
        {
            var user = _database.Users.FirstOrDefault(u => u.Id == userId);
            var currentUser = _userRepository.GetLoggedInUser();

            var messagesToAdd = new Message
            {
                SentMessage = message,
                SentTime = GetSweTime(DateTime.Now),
                SentToId = userId,
                SentFromId = currentUser.Id,
                isRead = false,

            };

            _database.Messages.Add(messagesToAdd);
            _database.SaveChanges();

        }
        public DateTime GetSweTime(DateTime time)
        {
            TimeZoneInfo swedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            return TimeZoneInfo.ConvertTime(time, swedishTimeZone);
        }
        public async Task<IActionResult> OnPostCuisine(int userId)
        {
            //If there is a common cuisine it will be used.
            //Otherwise we will use the matchedusers cuisine.
            var matchedUser = _userRepository.GetUser(userId);
            CurrentUser = _userRepository.GetLoggedInUser();

            string url = "https://getfoodfunction.azurewebsites.net/api/FoodFunction?code=8HQjzgXDSLqW2HPRUHgVapILdvLzdn-cshHG_0YNS-tXAzFuGQtT3A==&Cuisine=";

            string cuisine;
            var message = "";

            //If users have common cuisine add it to url, otherwise add the matched users cuisine.
            if (CommonCuisine(CurrentUser, matchedUser))
            {
                cuisine = GetCommonCuisine(CurrentUser, matchedUser);
                url += cuisine;
            }
            else
            {
                cuisine = GetCuisine(matchedUser);
                url += cuisine;
            }

            //Create client to send get request.
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            string content = await response.Content.ReadAsStringAsync();
            //Convert get request to Dish
            Dish jsonDish = JsonConvert.DeserializeObject<Dish>(content)!;

            //Getting the picture.


            if (CommonCuisine(CurrentUser, matchedUser))
            {
                message += $"Aaah! I see that we both love {jsonDish.category}, have you been to {jsonDish.country}? Did you know that {jsonDish.description} The ingredients are {string.Join(", ", jsonDish.ingredient)} and the primary ingredient is {jsonDish.primaryIngredient}. Would you like to to go on a date and cook this with me?";
            }
            else
            {
                message += $"Aaah! I see that you like {jsonDish.category}, have you been to {jsonDish.country}? Did you know that {jsonDish.description}The ingredients are {string.Join(", ", jsonDish.ingredient)} and the primary ingredient is {jsonDish.primaryIngredient}. Would you like to to go on a date and cook this with me?";
            }

            var sstring = _blobRepo.GetBlobUrlAsync("");
            AddMessage(message, matchedUser.Id);

            return RedirectToPage();

        }

        public async Task<IActionResult> OnPostCuisineAnswer(string answerData, int userId)
        {
            CurrentUser = _userRepository.GetLoggedInUser();
            OtherUser = _userRepository.GetUser(userId);

            // var matchedUser = _userRepository.GetUser(userId);
            Dish jsonDish = JsonConvert.DeserializeObject<Dish>(answerData);

            var message = "";

            if (jsonDish != null)
            {
                if (CommonCuisine(CurrentUser, OtherUser))
                {
                    message += $"Aaah! I see that we both love {jsonDish.category}, have you been to {jsonDish.country}? Did you know that {jsonDish.description} The ingredients are {string.Join(", ", jsonDish.ingredient)} and the primary ingredient is {jsonDish.primaryIngredient}. Would you like to to go on a date and cook this with me?";
                }
                else
                {
                    message += $"Aaah! I see that you like {jsonDish.category}, have you been to {jsonDish.country}? Did you know that {jsonDish.description}The ingredients are {string.Join(", ", jsonDish.ingredient)} and the primary ingredient is {jsonDish.primaryIngredient}. Would you like to to go on a date and cook this with me?";
                }

                AddMessage(message, OtherUser.Id);
            }
            else
            {
                message += "I didnt find any suitable dish for us, what type of food do you like?";
            }

            //Reseting cuisine
            HttpContext.Session.SetString("cuisine", "");
            HttpContext.Session.SetString("commonCuisine", "");

            return RedirectToPage();
        }

        public bool CommonCuisine(User loggedInUser, User user2)
        {
            //If users have a common cuisine it will return true.

            var loggedInUserCuisines = loggedInUser.Cuisines.Select(x => x.Cuisine).ToArray();
            var user2Cuisines = user2.Cuisines.Select(x => x.Cuisine).ToArray();

            return loggedInUserCuisines.Intersect(user2Cuisines).Any();
        }

        public string GetCommonCuisine(User loggedInUser, User matchedUser)
        {
            var loggedInUserCuisines = loggedInUser.Cuisines.Select(x => x.Cuisine).ToArray();
            var user2Cuisines = matchedUser.Cuisines.Select(x => x.Cuisine).ToArray();

            var matchedCuisines = loggedInUserCuisines.Intersect(user2Cuisines).ToArray();

            int randomIndex = random.Next(0, matchedCuisines.Length);

            return matchedCuisines[randomIndex];
        }

        public string GetCuisine(User matchedUser)
        {
            int randomIndex = random.Next(0, matchedUser.Cuisines.Count);

            return matchedUser.Cuisines[randomIndex].Cuisine;
        }

        // public async Task<IActionResult> OnPostCuisine(int userId)
        // {
        //     //If there is a common cuisine it will be used.
        //     //Otherwise we will use the matchedusers cuisine.
        //     var matchedUser = _userRepository.GetUser(userId);
        //     CurrentUser = _userRepository.GetLoggedInUser();



        //     if (CommonCuisine(CurrentUser, matchedUser))
        //     {
        //         HttpContext.Session.SetString("commonCuisine", GetCommonCuisine(CurrentUser, matchedUser));
        //     }
        //     else
        //     {
        //         HttpContext.Session.SetString("cuisine", GetCuisine(matchedUser));
        //     }



        //     return RedirectToPage();
        // }
        // public async Task<IActionResult> OnPostCuisineAnswer(string answerData, int userId)
        // {
        //     CurrentUser = _userRepository.GetLoggedInUser();
        //     OtherUser = _userRepository.GetUser(userId);



        //     // var matchedUser = _userRepository.GetUser(userId);
        //     Dish jsonDish = JsonConvert.DeserializeObject<Dish>(answerData);



        //     var message = "";



        //     if (jsonDish != null)
        //     {
        //         if (CommonCuisine(CurrentUser, OtherUser))
        //         {
        //             message += $"Aaah! I see that we both love {jsonDish.category}, have you been to {jsonDish.country}? Did you know that {jsonDish.description} The ingredients are {string.Join(", ", jsonDish.ingredient)} and the primary ingredient is {jsonDish.primaryIngredient}. Would you like to to go on a date and cook this with me?";
        //         }
        //         else
        //         {
        //             message += $"Aaah! I see that you like {jsonDish.category}, have you been to {jsonDish.country}? Did you know that {jsonDish.description}The ingredients are {string.Join(", ", jsonDish.ingredient)} and the primary ingredient is {jsonDish.primaryIngredient}. Would you like to to go on a date and cook this with me?";
        //         }



        //         AddMessage(message, OtherUser.Id);
        //     }
        //     else
        //     {
        //         message += "I didnt find any suitable dish for us, what type of food do you like?";
        //     }



        //     //Reseting cuisine
        //     HttpContext.Session.SetString("cuisine", "");
        //     HttpContext.Session.SetString("commonCuisine", "");



        //     return RedirectToPage();
        // }
    }
}