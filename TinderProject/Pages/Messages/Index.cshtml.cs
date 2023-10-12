using Microsoft.AspNetCore.Mvc.RazorPages;
using TinderProject.Repositories;
namespace TinderProject.Pages.Messages
{
    public class IndexModel
        : PageModel
    {
        private readonly AppDbContext _database;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public List<User> User { get; set; }
        public List<Message> Messages { get; set; }
        public List<User> NoConversation { get; set; }

        public IndexModel(AppDbContext database, IUserRepository userRepository, IConfiguration config)
        {
            _database = database;
            _userRepository = userRepository;
            _configuration = config;
            Messages = new List<Message>();
            NoConversation = new List<User>();
            User = new List<User>();
        }

        public void OnGet()
        {
            var currentUser = _userRepository.GetLoggedInUser();
            var otherUsersIds = _database.Messages
                .Where(m => m.SentToId == currentUser.Id || m.SentFromId == currentUser.Id)
                .Select(m => m.SentToId == currentUser.Id ? m.SentFromId : m.SentToId)
                .Distinct()
                .ToList();

            User.AddRange(_database.Users.Where(u => otherUsersIds.Contains(u.Id)).ToList());

            //Check if a match has a conversation 
            var matches = _database.Matches.ToList();
            var messages = _database.Messages.ToList();

            foreach (var match in matches)
            {
                bool haveConversation = messages.Any(m =>
                (m.SentToId == match.User2Id && m.SentFromId == match.User1Id)
                || (m.SentToId == match.User1Id && m.SentFromId == match.User2Id));

                if (!haveConversation)
                {
                    if (match.User1Id == currentUser.Id)
                    {
                        NoConversation.Add(_database.Users.Single(u => u.Id == match.User2Id));

                        if (match.User1Id > 20)
                        {
                            var user = _userRepository.GetUser(match.User1Id);

                            BlobRepo _blobRepo = new BlobRepo(_configuration);
                            var PictureSasURI = _blobRepo.GenerateSASLink(user);
                            _database.Update(user);
                            user.ProfilePictureUrl = PictureSasURI;
                            _database.SaveChanges();
                        }
                        else if(match.User2Id > 20){
                        {
                            var user = _userRepository.GetUser(match.User2Id);

                            BlobRepo _blobRepo = new BlobRepo(_configuration);
                            var PictureSasURI = _blobRepo.GenerateSASLink(user);
                            _database.Update(user);
                            user.ProfilePictureUrl = PictureSasURI;
                            _database.SaveChanges();
                        }
                        }
                    }
                    else if (match.User2Id == currentUser.Id)
                    {
                        NoConversation.Add(_database.Users.Single(u => u.Id == match.User1Id));
                        
                    }
                }
            }
        }
    }
}
