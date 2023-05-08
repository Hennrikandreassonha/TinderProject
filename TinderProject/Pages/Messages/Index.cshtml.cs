using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TinderProject.Data;
using TinderProject.Models;
using TinderProject.Models.ModelEnums;
using TinderProject.Repositories.Repositories_Interfaces;

namespace TinderProject.Pages.Messages
{
    public class IndexModel
        : PageModel
    {
        private readonly AppDbContext _database;
        private readonly IUserRepository _userRepository;

        public List<User> User { get; set; }

        public List<Message> Messages { get; set; }
        public List<User> NoConversation { get; set; }
        public IndexModel(AppDbContext database, IUserRepository userRepository)
        {
            _database = database;
            _userRepository = userRepository;
            Messages = new List<Message>();
            NoConversation = new List<User>();

            User = new List<User>();
        }

        public void OnGet()
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var otherUsersIds = _database.Messages
                .Where(m => m.SentToId == currentUser.Id || m.SentFromId == currentUser.Id)
                .Select(m => m.SentToId == currentUser.Id ? m.SentFromId : m.SentToId)
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
                    if (match.User1Id == currentUser.Id)
                    {
                        NoConversation.Add(_database.Users.Single(u => u.Id == match.User2Id));
                    }
                    else if (match.User2Id == currentUser.Id)
                    {
                        NoConversation.Add(_database.Users.Single(u => u.Id == match.User1Id));
                    }
                }
            }

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

            return RedirectToPage();
        }
    }
}
