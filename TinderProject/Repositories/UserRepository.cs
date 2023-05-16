using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinderProject.Data.Dtos;
using TinderProject.Models;

namespace TinderProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static Random random = new();
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _httpContextAccessor = contextAccessor;
        }
        public ICollection<User> GetAllFemale()
        {
            return _context.Users
                .Where(x => x.Gender == GenderType.Female)
                .Include(x => x.Interests)
                //.Include(x => x.PersonalityType)
                .ToList();
        }

        public ICollection<User> GetAllMale()
        {
            return _context.Users
            .Where(x => x.Gender == GenderType.Male)
            .Include(x => x.Interests)
            //.Include(x => x.PersonalityType)
            .ToList();
        }

        public ICollection<User> GetAllOther()
        {
            return _context.Users
            .Where(x => x.Gender == GenderType.Other)
            .Include(x => x.Interests)
            //.Include(x => x.PersonalityType)
            .ToList();
        }

        public ICollection<User> GetAllUsers()
        {
            return _context.Users
            .Include(x => x.Interests)
            //.Include(x => x.PersonalityType)
            .ToList();
        }

        public User? GetLoggedInUser()
        {
            var user = _httpContextAccessor.HttpContext.User;
            string subject = user.FindFirst(ClaimTypes.NameIdentifier).Value;
            string issuer = user.FindFirst(ClaimTypes.NameIdentifier).Issuer;


            return _context.Users.Include(x => x.Interests).Include(y => y.Cuisines)
                .Single(p => p.OpenIDIssuer == issuer && p.OpenIDSubject == subject);
        }
        public ICollection<User> GetUsersToSwipe(User user)
        {
            //Gets the Id´s of all the Users that the user Already likes.
            //Removes these since you cant like somone who is already liked.
            //Also removes those who are already matched.
            List<User> userList = new();

            userList = (user.Preference == SwipePreference.All) ? GetAllUsers().ToList() : userList;
            userList = (user.Preference == SwipePreference.Male) ? GetAllMale().ToList() : userList;
            userList = (user.Preference == SwipePreference.Female) ? GetAllFemale().ToList() : userList;

            userList = FilterAge(userList, user);

            var userLikesIds = GetUserLikes(user).Select(x => x.LikedId);
            var userMatches = GetMatches(user);

            //This statement filters out the logged-in user, users who have already matched, users who have already been liked, those without interests, and those without personality types.
            return userList
                 .Where(u => u.Id != user.Id &&
                 !userLikesIds.Contains(u.Id) &&
                 !userMatches.Select(m => m.User1Id).Contains(u.Id) &&
                 !userMatches.Select(m => m.User2Id).Contains(u.Id) &&
                 u.Interests != null && u.PersonalityType != null)
                 .ToList();
        }
        public List<User> FilterAge(List<User> userList, User loggedinUser)
        {
            //Getting the users that are atleast half users age + 7;

            var minAge = loggedinUser.Age / 2 + 7;
            return userList.Where(x => x.Age >= minAge).ToList();
        }
        public User? GetUser(int id)
        {
            return _context.Users.Include(x => x.Interests).Include(x => x.Cuisines).FirstOrDefault(x => x.Id == id);
        }
        /*public ICollection<PersonalType> GetPersonalTypes(User user)
        {
            return _context.PersonalTypes.Where(x => x.UserId == user.Id).ToArray();
        }*/
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

        public ApiModel? GetUserApi(string interest)
        {
            //This will return 1 random user that has the given interest.
            List<User> userList = new();

            foreach (var user in GetAllUsers())
            {
                if (user.Interests != null)
                {
                    foreach (var item in user.Interests)
                    {
                        if (item.Interest == interest)
                        {
                            userList.Add(user);
                        }
                    }
                }
            }

            if (!userList.Any())
            {
                return null;
            }
            int randomIndex = random.Next(0, userList.Count);
            var selectedUser = userList[randomIndex];

            ApiModel apiModel = new()
            {
                FirstName = selectedUser.FirstName,
                LastName = selectedUser.LastName,
                Age = selectedUser.Age.ToString(),
                PersonalityType = selectedUser.PersonalityType,
                Description = selectedUser.Description,
            };

            foreach (var item in selectedUser.Interests)
            {
                apiModel.UserInterests.Add(item.Interest);
            }

            return apiModel;
        }
    }
}