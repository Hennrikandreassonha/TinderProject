using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace TinderProject.Tests
{
    public class UnitTests
    {
        [Fact]
        public void IsMatchTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder()
            var mockUserRepo = new Mock<IUserRepository>();
            var mockDbContext = new Mock<AppDbContext>();

            var page = new Pages.IndexModel(mockUserRepo.Object, mockDbContext.Object);

            int loggedInUserId = 1;
            int likedUserId = 2;

            Interaction testInteraction = new()
            {
                LikerId = loggedInUserId,
                LikedId = likedUserId,
                DateLiked = DateTime.Now
            };

            // mockDbContext.SetupAdd(testInteraction);

            Assert.True(page.CheckIfMatch(loggedInUserId, likedUserId));
        }
    }
}