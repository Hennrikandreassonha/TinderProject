using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TinderProject.Models;

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
            return _context.Users.Where(x => x.Gender == GenderType.Female).Include(x => x.Interests).ToList();
        }

        public ICollection<User> GetAllMale()
        {
            return _context.Users.Where(x => x.Gender == GenderType.Male).Include(x => x.Interests).ToList();
        }

        public ICollection<User> GetAllOther()
        {
            return _context.Users.Where(x => x.Gender == GenderType.Other).Include(x => x.Interests).ToList();
        }

        public ICollection<User> GetAllUsers()
        {
            return _context.Users.Include(x => x.Interests).ToList();
        }

        public User? GetLoggedInUser()
        {
            var user = _httpContextAccessor.HttpContext.User;
            string subject = user.FindFirst(ClaimTypes.NameIdentifier).Value;
            string issuer = user.FindFirst(ClaimTypes.NameIdentifier).Issuer;

            return _context.Users.Single(p => p.OpenIDIssuer == issuer && p.OpenIDSubject == subject);
        }
        public ICollection<User> GetUsersToSwipe(User user)
        {
            //Gets the Id´s of all the Users that the user Already likes.
            //Removes these since you cant like somone who is already liked.
            //Also removes those who are already matched.
            var userLikesIds = GetUserLikes(user).Select(x => x.LikedId);
            var userMatches = GetMatches(user);

            return GetAllUsers()
             .Where(u => u.Id != user.Id &&
           !userLikesIds.Contains(u.Id) &&
           !userMatches.Select(m => m.User1Id).Contains(u.Id) &&
           !userMatches.Select(m => m.User2Id).Contains(u.Id))
            .ToList();
        }
        public User? GetUser(int id)
        {
            return _context.Users.Find(id);
        }

        public ICollection<Interaction> GetUserLikes(User user)
        {
            return _context.Interactions.Where(x => x.LikerId == user.Id).ToArray();
        }

        public ICollection<Interaction> GetUserLikes(int userId)
        {
            return _context.Interactions.Where(x => x.LikerId == userId).ToArray();
        }
        public ICollection<Match> GetMatches(User user)
        {
            return _context.Matches.Where(x => x.User2Id == user.Id || x.User1Id == user.Id).ToArray();
        }
    }
}