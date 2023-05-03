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
    public class Index : PageModel
    {
        private readonly AppDbContext _database;
        private readonly IUserRepository _userRepository;
        public Message Message { get; set; }
        public User User { get; set; }
        public List<Message> ReceivedMessages { get; set; }

        public Index(AppDbContext database, IUserRepository userRepository)
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
                .Where(m => m.SentToId == currentUser.Id)
                .GroupBy(u => u.User.Id)
                .Select(m => m.FirstOrDefault())
                .ToList();

            ReceivedMessages.AddRange(findReceivedMessages);


        }



        public IActionResult OnPost(string message)
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var messagesToAdd = new Message
            {
                SentMessage = message,
                SentTime = DateTime.Now,
                SentToId = 2,
                User = currentUser
            };


            _database.Messages.Add(messagesToAdd);
            _database.SaveChanges();

            return RedirectToPage("/Index");
        }
    }
}
