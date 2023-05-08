using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinderProject.Pages.Premium
{
    public class IndexModel : PageModel
    {
		private readonly IUserRepository _userRepository;
		private readonly AppDbContext _database;
		public List<Interaction> Likers { get; set; }	
		public List<User> Users { get; set; }

		public IndexModel(IUserRepository userRepository, AppDbContext database)
		{
			_userRepository = userRepository;
			_database = database;
			Likers = new List<Interaction>();
			Users = new List<User>();
		}


		public void OnGet()
        {
			var currentUser = _userRepository.GetLoggedInUser();

			Likers = _database.Interactions
				.Where(l=> l.LikedId == currentUser.Id)
				.ToList();

            foreach (var liker in Likers)
            {
                var user = _database.Users.Find(liker.LikerId);
                if (user != null)
                {
                    Users.Add(user);
                }
            }
        }
    }
}
