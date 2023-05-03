using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinderProject.Data;
using TinderProject.Models;
using TinderProject.Repositories.Repositories_Interfaces;

namespace TinderProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _httpContextAccessor = contextAccessor;
        }

        public ICollection<User> GetAllFemale()
        {
            return _context.Users.Where(x => x.Gender == "Female").ToList();
        }

        public ICollection<User> GetAllMale()
        {
            return _context.Users.Where(x => x.Gender == "Male").ToList();
        }

        public ICollection<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetLoggedInUser()
        {
            var user = _httpContextAccessor.HttpContext.User;
            string subject = user.FindFirst(ClaimTypes.NameIdentifier).Value;
            string issuer = user.FindFirst(ClaimTypes.NameIdentifier).Issuer;

            return _context.Users.Single(p => p.OpenIDIssuer == issuer && p.OpenIDSubject == subject);
        }
        public ICollection<User> GetPreferedUsers(User loggedInUser)
        {
            //Gets the users prefered matches.
            //Depending on prefered type and age.
            if (loggedInUser.Preference == SwipePreference.Male)
            {
                return GetAllMale();
            }
            if (loggedInUser.Preference == SwipePreference.Female)
            {
                return GetAllFemale();
            }
            else
            {
                return GetAllUsers().Where(x => x.Id != loggedInUser.Id).ToArray();
            }

        }
        public User? GetUser(int id)
        {
            return _context.Users.Find(id);
        }
    }
}