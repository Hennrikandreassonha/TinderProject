using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinderProject.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly AppDbContext _context;
        private IUserRepository _userRepo;
        public MatchRepository(AppDbContext context, IUserRepository userRepo)
        {
            _context = context;
            _userRepo = userRepo;
        }
        public List<User> OrderByMatchingTypes(ICollection<User> userList, User loggedInUser)
        {
            List<KeyValuePair<int, User>> matchingUsers = new List<KeyValuePair<int, User>>();

            foreach (var user in userList)
            {
                int matchCount = 0;

                for (int i = 0; i < user.PersonalityType.Length; i++)
                {
                    matchCount += (user.PersonalityType[i] == loggedInUser.PersonalityType[i]) ? 1 : 0;
                }

                matchingUsers.Add(new KeyValuePair<int, User>(matchCount, user));
            }

            return matchingUsers.OrderByDescending(x => x.Key).Select(x => x.Value).ToList();
        }

        public List<User> OrderByLeastMatchingTypes(ICollection<User> userList, User loggedInUser)
        {
            var sortedUsers = OrderByMatchingTypes(userList, loggedInUser);
            sortedUsers.Reverse();
            return sortedUsers;
        }
        public string GetPersonalityLetters(User user)
        {
            string mystring = user.PersonalityType.Reverse().ToString();

             return mystring.Substring(1,4);
        }
    }
}