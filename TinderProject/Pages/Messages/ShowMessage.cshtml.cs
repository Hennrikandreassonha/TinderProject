using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinderProject.Data;
using TinderProject.Repositories.Repositories_Interfaces;

namespace TinderProject.Pages.Messages
{
    public class MessegesModel : PageModel
    {
        private readonly AppDbContext _database;
        private readonly IUserRepository _userRepository;

        public Message Message { get; set; }
        public User User { get; set; }
        public List<Message> ReceivedMessages { get; set; }

        public MessegesModel(AppDbContext database, IUserRepository userRepository)
        {
            _database = database;
            _userRepository = userRepository;
            ReceivedMessages = new List<Message>();
        }

        public void OnGet()
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var findReceivedMessages = _database.Messages
                .Include(m => m.User)
                .Where(m => m.SentToId == currentUser.Id);

            ReceivedMessages.AddRange(findReceivedMessages);
        }


        //Fixa detta
        public IActionResult OnPost(string message)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var messagesToAdd = new Message
            {
                SentMessage = message,
                SentTime = DateTime.Now,
                SentToId = 11,
                User = currentUser
            };


            _database.Messages.Add(messagesToAdd);
            _database.SaveChanges();

            return Page();
        }
    }
}
