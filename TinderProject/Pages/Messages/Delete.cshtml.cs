using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinderProject.Models;

namespace TinderProject.Pages.Messages
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _database;
        private readonly IUserRepository _userRepository;
        public User User { get; set; }
        

        public DeleteModel(AppDbContext database, IUserRepository userRepository)
        {
            _database = database;
            _userRepository = userRepository;
           User= new User();
        }

        public void OnGet(int userId)
        {
            User = _database.Users.Find(userId);
            
        }
     
        public IActionResult OnPost(int userId)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var matches = _database.Matches
                .Where(m => (m.User1Id == userId && m.User2Id == currentUser.Id) ||
                            (m.User1Id == currentUser.Id && m.User2Id == userId))
                            .ToList();

            var messages = _database.Messages.
                Where(m => (m.SentFromId == userId && m.SentToId == currentUser.Id) ||
                           (m.SentToId == userId && m.SentFromId == currentUser.Id)).
                ToList();

            _database.Matches.RemoveRange(matches);
            _database.Messages.RemoveRange(messages);

            _database.SaveChanges();

            return RedirectToPage("/Messages/Index");
        }
    }
}
