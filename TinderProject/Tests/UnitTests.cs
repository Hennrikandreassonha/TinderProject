using Xunit;
using Moq;
using TinderProject.Pages;

namespace TinderProject.Tests
{
    public class UnitTests
    {
        private readonly Mock<AppDbContext> fakeDb = new();
        private readonly Mock<IUserRepository> fakeRepo = new();
        private readonly Mock<IMatchRepository> fakeRepo2 = new();

        public UnitTests()
        {
            // Initialize the options builder and database context
            // IndexModel page = new(fakeRepo.Object, fakeDb.Object);
        }
        [Fact]
        public void GetUserTest()
        {
            IndexModel model = new(fakeRepo.Object, fakeDb.Object, fakeRepo2.Object);

            int userId = 1;
            var user = new User
            {
                Id = userId,
                FirstName = "Test",
                LastName = " "
            };

            fakeRepo.Setup(x => x.GetUser(userId)).Returns(user);

            var userFromRepo = fakeRepo.Object.GetUser(userId);

            Assert.Equal(userId, userFromRepo.Id);
        }

        // public User GetUser()
        // {

        // }
    }
}