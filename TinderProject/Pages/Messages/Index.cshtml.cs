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

        public Index(AppDbContext database, IUserRepository userRepository)
        {
            _database = database;
            _userRepository = userRepository;
        }

        public void OnGet()
        {
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
