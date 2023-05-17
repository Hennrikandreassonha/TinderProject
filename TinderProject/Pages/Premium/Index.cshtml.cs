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
		public bool IsPremium { get; set; }

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
				.Where(l => l.LikedId == currentUser.Id)
				.ToList();

			IsPremium = currentUser.PremiumUser;

			foreach (var liker in Likers)
			{
				var user = _database.Users.Find(liker.LikerId);
				var match = _database.Matches.FirstOrDefault(m => (m.User1Id == currentUser.Id && m.User2Id == liker.LikerId) ||
				(m.User1Id == liker.LikerId && m.User2Id == currentUser.Id));

				if (user != null && match == null)
				{
					Users.Add(user);
				}
			}
		}
	}
}
