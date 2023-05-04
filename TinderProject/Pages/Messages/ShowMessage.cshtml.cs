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

        public Message Message { get; set; }
        public User User { get; set; }
        public List<Message> Messages { get; set; }

        public MessegesModel(AppDbContext database, IUserRepository userRepository)
        {
            _database = database;
            _userRepository = userRepository;
            Messages = new List<Message>();
        }

        //fixa så man kan se alla meddelanden
        public void OnGet()
        {
            var currentUser = _userRepository.GetLoggedInUser();
            User = currentUser;
            var messages = _database.Messages
            .Include(m => m.User)
            .Where(m => m.User.Id == User.Id || m.SentToId == User.Id)
            .OrderBy(m => m.SentTime)
            .ToList();

            Messages.AddRange(messages);
        }



        public IActionResult OnPost(string message)
        {
            var sender = 0;
            var currentUser = _userRepository.GetLoggedInUser();
            var findReceivedMessages = _database.Messages
                .Include(m => m.User)
                .Where(m => m.SentToId == currentUser.Id);
            foreach (var c in findReceivedMessages)
            {
                sender = c.User.Id;

            }

            var messagesToAdd = new Message
            {
                SentMessage = message,
                SentTime = DateTime.Now,
                SentToId = sender,
                User = currentUser
            };


            _database.Messages.Add(messagesToAdd);
            _database.SaveChanges();

            return RedirectToPage();
        }
    }
}
