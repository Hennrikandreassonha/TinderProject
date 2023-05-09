using Xunit;
using Moq;
using TinderProject.Pages;

namespace TinderProject.Tests
{
    public class UnitTests
    {
        private static readonly Mock<IAppDbContext> FakeDb = new();
        private static readonly Mock<IUserRepository> FakeUserRepo = new();
        private static readonly Mock<IMatchRepository> FakeMatchRepo = new();

        public UnitTests()
        {
            // Initialize the options builder and database context
            // IndexModel page = new(fakeRepo.Object, fakeDb.Object);
        }
        [Fact]
        public void GetUserTest()
        {
            IndexModel model = new(FakeUserRepo.Object, FakeDb.Object, FakeMatchRepo.Object);

            int userId = 1;
            var user = new User
            {
                Id = userId,
                FirstName = "Test",
                LastName = " "
            };

            FakeUserRepo.Setup(x => x.GetUser(userId)).Returns(user);
            var userFromRepo = FakeUserRepo.Object.GetUser(userId);

            Assert.Equal(userId, userFromRepo.Id);
        }

        [Fact]
        public void CheckIfMatch_ReturnsTrueWhenMatchFound()
        {
            var user1 = new User { Id = 1 };
            var user2 = new User { Id = 2 };

            //Skapa en fake interaction
            var interaction1 = new Interaction { Id = 1, LikerId = 1, LikedId = 2, Liker = user1, Liked = user2 };

            user1.LikedUsers = new List<Interaction> { interaction1 };

            //Skapar en fake lista med en fejkad interaction mellan de 2 användarna.
            var likedUserInteractions = new List<Interaction> { interaction1 };
            //Vi måste ange att denna ska returnera listan med den fejkade interactionen eftersom metoden används i CheckIfMatch().
            FakeUserRepo.Setup(repo => repo.GetUserLikes(It.IsAny<int>())).Returns(likedUserInteractions);

            //Skapar en modell av Sidan som har vår Match-metod.
            IndexModel model = new(FakeUserRepo.Object, FakeDb.Object, FakeMatchRepo.Object);

            //Skapar en nytt fake object för vår Models klass.
            //Denna använder vi för att ändra beteendet.
            var fakeMatches = new Mock<DbSet<Models.Match>>();

            //Anger att db.Matches ska returnera vårt fakeobjekt o inte riktigt.
            FakeDb.Setup(db => db.Matches).Returns(fakeMatches.Object);

            //Anger att när vi använder .Add så ska den inte kasta exception.
            fakeMatches.Setup(matches => matches.Add(It.IsAny<Models.Match>())).Verifiable();

            int loggedInUserId = 2;
            int likedUserId = 1;
            bool result = model.CheckIfMatch(loggedInUserId, likedUserId);

            // Assert: Check if the result matches your expected outcome
            Assert.True(result);
        }

        [Fact]
        public void CheckIfMatch_ReturnsFalseWhenNoMatch()
        {
            //Arrange
            var loggedInUser = new User { Id = 1 };
            var likedUser = new User { Id = 2 };

            var interaction = new Interaction { Id = 1, LikerId = 1, LikedId = 2, Liker = loggedInUser, Liked = likedUser };
            loggedInUser.LikedUsers = new List<Interaction> { interaction };

            var likedUserInteractions = new List<Interaction> { interaction };
            FakeUserRepo.Setup(repo => repo.GetUserLikes(It.IsAny<int>())).Returns(likedUserInteractions);

            IndexModel model = new(FakeUserRepo.Object, FakeDb.Object, FakeMatchRepo.Object);

            var fakeMatches = new Mock<DbSet<Models.Match>>();
            FakeDb.Setup(db => db.Matches).Returns(fakeMatches.Object);
            fakeMatches.Setup(matches => matches.Add(It.IsAny<Models.Match>())).Verifiable();

            int loggedInUserId = 2;
            int likedUserId = 1;
            
            //Act
            bool result = model.CheckIfMatch(loggedInUserId, likedUserId);

            //Assert
            Assert.False(result);
        }
    }
}

