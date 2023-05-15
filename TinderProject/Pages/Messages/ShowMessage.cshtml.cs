using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinderProject.Data;
using TinderProject.Models;
using TinderProject.Repositories.Repositories_Interfaces;

namespace TinderProject.Pages.Messages
{
    public class MessegesModel : PageModel
    {
        private readonly AppDbContext _database;
        private readonly IUserRepository _userRepository;
        private static Random random = new();

        public Message Message { get; set; }
        public User OtherUser { get; set; }
        public User CurrentUser { get; set; }
        public List<User> User { get; set; }
        public List<Message> Messages { get; set; }
        public List<User> NoConversation { get; set; }

        public MessegesModel(AppDbContext database, IUserRepository userRepository)
        {
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

            //Look if a match has a conversation 
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
            AddMessage(message, userId);
            return RedirectToPage();
        }
        public void AddMessage(string message, int userId)
        {
            var user = _database.Users.FirstOrDefault(u => u.Id == userId);
            var currentUser = _userRepository.GetLoggedInUser();

            var messagesToAdd = new Message
            {
                SentMessage = message,
                SentTime = DateTime.Now,
                SentToId = userId,
                SentFromId = currentUser.Id,
                isRead = false,

            };

            _database.Messages.Add(messagesToAdd);
            _database.SaveChanges();
        }
        public IActionResult OnPostCuisine(int userId)
        {
            //If there is a common cuisine it will be used.
            //Otherwise we will use the matchedusers cuisine.
            var matchedUser = _userRepository.GetUser(userId);
            CurrentUser = _userRepository.GetLoggedInUser();

            string? cuisine;
            if (CommonCuisine(CurrentUser, matchedUser))
            {
                cuisine = GetCommonCuisine(CurrentUser, matchedUser);
            }
            else
            {
                cuisine = GetCuisine(matchedUser);
            }

            //Skicka denna till API.
            var answer = "return from API";

            var message = "";
            if (CommonCuisine(CurrentUser, matchedUser))
            {
                message += $"I see that we both love {cuisine}, how about we cook some {answer}?";
            }
            else
            {
                message += $"I see that you like {cuisine}, how about we cook some {answer}?";
            }


            AddMessage(message, matchedUser.Id);
            return RedirectToPage();
        }
        public bool CommonCuisine(User loggedInUser, User user2)
        {
            //If users have a common cuisine it will return true.
            foreach (var item in loggedInUser.Cuisines)
            {
                return user2.Cuisines.Contains(item);
            }
            return false;
        }
        public string GetCommonCuisine(User loggedInUser, User matchedUser)
        {
            var commonCuisines = loggedInUser.Cuisines.Intersect(matchedUser.Cuisines).ToArray();

            int randomIndex = random.Next(0, commonCuisines.Length);

            return commonCuisines[randomIndex].Cuisine;
        }
        public string GetCuisine(User matchedUser)
        {
            int randomIndex = random.Next(0, matchedUser.Cuisines.Count);

            return matchedUser.Cuisines[randomIndex].Cuisine;
        }
    }
}