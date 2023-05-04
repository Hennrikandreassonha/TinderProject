using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinderProject.Models;

namespace TinderProject.Repositories.Repositories_Interfaces
{
    public interface IInteractionRepository
    {
        public ICollection<User> GetLikedUsers(int userId);
    }
}