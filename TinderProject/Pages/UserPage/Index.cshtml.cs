using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TinderProject.Repositories.Repositories_Interfaces;

namespace TinderProject.Pages.UserPage
{
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public IndexModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void OnGet()
        {
            var loggedInUser = _userRepository.GetLoggedInUser();
        }
    }
}