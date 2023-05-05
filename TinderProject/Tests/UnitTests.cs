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
            var mockUserRepo = new Mock<IUserRepository>();
            var mockDbContext = new Mock<AppDbContext>();

            var page = new Pages.IndexModel(mockUserRepo.Object, mockDbContext.Object);

            int user1Id = 1;
            int user2Id = 11;

            page.CheckIfMatch();
        }
    }
}