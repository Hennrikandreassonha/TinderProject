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
                .ToList();
        }

        public ICollection<User> GetAllMale()
        {
            return _context.Users
            .Where(x => x.Gender == GenderType.Male)
            .Include(x => x.Interests)
            .ToList();
        }

        public ICollection<User> GetAllOther()
        {
            return _context.Users
            .Where(x => x.Gender == GenderType.Other)
            .Include(x => x.Interests)
            .ToList();
        }

        public ICollection<User> GetAllUsers()
        {
            return _context.Users
            .Include(x => x.Interests)
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
            //Gets the Id´s of all the users that the user already likes.
            //Removes these since you can´t like someone who is already liked.
            //Also removes those who are already matched.
            List<User> userList = new();

            userList = (user.Preference == SwipePreference.All) ? GetAllUsers().ToList() : userList;
            userList = (user.Preference == SwipePreference.Male) ? GetAllMale().ToList() : userList;
            userList = (user.Preference == SwipePreference.Female) ? GetAllFemale().ToList() : userList;

            userList = FilterAge(userList, user);

            //Filtering out those who havnt got any profilepicture.
            userList = userList.Where(x => x.ProfilePictureUrl != null && !string.IsNullOrEmpty(x.ProfilePictureUrl.Trim())).ToList();

            var userLikesIds = GetUserLikes(user).Select(x => x.LikedId);
            var userMatches = GetMatches(user);

            //This statement filters out the logged in user, users who have already matched, users who have already been liked,
            //those without interests, and those without personality types.
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
            var minAge = 0;
            var maxAge = 0;
            //Getting the users that are at least half the user´s age plus seven years;
            if (loggedinUser.AgeFormula)
            {
                minAge = loggedinUser.Age / 2 + 7;
                maxAge = (loggedinUser.Age - 7) * 2;
                return userList.Where(x => x.Age >= minAge && x.Age <= maxAge).ToList();
            }

            return userList.Where(x => x.Age >= loggedinUser.MinAge && x.Age <= loggedinUser.MaxAge).ToList();
        }
        public User? GetUser(int id)
        {
            return _context.Users.Include(x => x.Interests).Include(x => x.Cuisines).FirstOrDefault(x => x.Id == id);
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

        public ApiModel? GetUserApi(string interest)
        {
            //This will return one random user that has the given interest.
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
                ProfileUrl = $"https://tinderapp.azurewebsites.net/TinderUser/{selectedUser.Id}"
            };

            foreach (var item in selectedUser.Interests)
            {
                apiModel.UserInterests.Add(item.Interest);
            }

            return apiModel;
        }
        public bool SetProfilePic(User user, string picUrl)
        {
            _context.Users.Update(user);
            user.ProfilePictureUrl = picUrl;
            _context.SaveChanges();

            return true;
        }
    }
}