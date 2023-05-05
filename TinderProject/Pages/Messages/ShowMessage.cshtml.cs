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

        
        public void OnGet(int? userId)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            User = currentUser;
            var messages = _database.Messages
            .Include(m => m.User)
            .Where(m =>
            (m.User.Id == currentUser.Id && m.SentToId == userId) ||
            (m.SentFromId == userId && m.SentToId == currentUser.Id))
            .OrderBy(m => m.SentTime)
            .ToList();

            Messages.AddRange(messages);
        }



        public IActionResult OnPost(string message,int userId)
        {
          
            var currentUser = _userRepository.GetLoggedInUser();
            

            var messagesToAdd = new Message
            {
                SentMessage = message,
                SentTime = DateTime.Now,
                SentToId = userId,
                SentFromId = currentUser.Id
            };


            _database.Messages.Add(messagesToAdd);
            _database.SaveChanges();

            return RedirectToPage();
        }
    }
}
