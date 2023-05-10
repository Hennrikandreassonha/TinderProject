using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinderProject.Models;

namespace TinderProject.Repositories.Repositories_Interfaces
{
    public interface IUserRepository
    {
        User? GetLoggedInUser();
        User? GetUser(int id);
        ICollection<User> GetAllUsers();
        ICollection<User> GetAllFemale();
        ICollection<User> GetAllMale();
        ICollection<User> GetUsersToSwipe(User user);
        ICollection<Interaction> GetUserLikes(User user);
        ICollection<Interaction> GetUserLikes(int userId);
        //ICollection<PersonalType> GetPersonalTypes(User user);
    }
}