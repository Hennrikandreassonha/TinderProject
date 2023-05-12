using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinderProject.Models;

namespace TinderProject.Repositories.Repositories_Interfaces
{
    public interface IMatchRepository
    {
        List<User> OrderByMatchingTypes(ICollection<User> userList, User loggedInUser);
        List<User> OrderByLeastMatchingTypes(ICollection<User> userList, User loggedInUser);
        public string GetPersonalityLetters(User user);

    }
}