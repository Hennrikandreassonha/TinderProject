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

        public Match Match { get; set; }
        public Message Message { get; set; }
        public User User { get; set; }
        public List<Message> Messages { get; set; }
        public List<User> NoConversation { get; set; }
        public IndexModel(AppDbContext database, IUserRepository userRepository)
        {
            _database = database;
            _userRepository = userRepository;
            Messages = new List<Message>();
            NoConversation = new List<User>();
        }


        public void OnGet()
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var findMessages = _database.Messages
                .Include(m => m.User)
                .Where(m => m.SentToId == currentUser.Id)
                .GroupBy(u => u.User.Id)
                .Select(m => m.FirstOrDefault())
                .ToList();

            Messages.AddRange(findMessages);


            //Visa matcher som inte har p�b�rjat en konversation f�r att kunna klicka. 

            var matches = _database.Matches.ToList();
            var messages = _database.Messages.ToList();

            foreach (var match in matches)
            {
                bool haveConversation = messages.Any(m =>
                (m.SentToId == match.User2Id && m.User.Id == match.User2Id)
                || (m.SentToId == match.User1Id && m.User.Id == match.User1Id));

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

            //var id1 = _database.Users.Where(id=> id.Id == 11).SingleOrDefault();
            //var id2 = _database.Users.Where(id => id.Id == 5).SingleOrDefault();


            //Match testmatch = new()
            //{
            //    User1Id = id1.Id,
            //    User2Id = id2.Id,
            //    MatchDate = DateTime.Now
            //};

            //_database.Add(testmatch);
            //_database.SaveChanges();

        }
    }
}
