using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinderProject.Data.Dtos;
using TinderProject.Models;

namespace TinderProject.Repositories.Repositories_Interfaces
{
	public interface IUserRepository
	{
		User? GetLoggedInUser();
		User? GetUser(int id);
		ApiModel? GetUserApi(string interest);
		ICollection<User> GetAllUsers();
		ICollection<User> GetAllFemale();
		ICollection<User> GetAllMale();
		ICollection<User> GetUsersToSwipe(User user);
		ICollection<Interaction> GetUserLikes(User user);
		ICollection<Interaction> GetUserLikes(int userId);
        bool SetProfilePic(User user, string picUrl);

    }
}