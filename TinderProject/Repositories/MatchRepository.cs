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
            //This function orders the SwipeList based on amount of PersonalTypes matching. 
            //The person with most matching PersonalTypes will be at index 0.

            var userPersonalTypes = loggedInUser.PersonalTypes;

            List<KeyValuePair<int, User>> matchingUsers = new List<KeyValuePair<int, User>>();

            foreach (var user in userList)
            {
                var personalTypes = _userRepo.GetPersonalTypes(user);
                int matchCount = 0;

                foreach (var type in personalTypes)
                {
                    if (userPersonalTypes.Contains(type))
                    {
                        matchCount++;
                    }
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
    }
}