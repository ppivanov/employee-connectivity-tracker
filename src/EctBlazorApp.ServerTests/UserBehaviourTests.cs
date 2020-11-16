using EctBlazorApp.Server.Behaviour;
using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.ServerTests;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Tests
{
    [TestClass()]
    public class UserBehaviourTests : IDisposable
    {
        private readonly EctDbContext _dbContext;

        public UserBehaviourTests()
        {
            _dbContext = InMemoryDb.InitInMemoryDbContext();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod()]
        public async Task GetExistingEctUserOrNewWrapper_ExistingUserEmail_AliceIsReturned()
        {
            string userEmail = "alice@ect.ie";
            EctUser expectedUser = _dbContext.Users.First(user => user.Email == userEmail);
            EctUser actualUser = await UserBehaviour.GetExistingEctUserOrNewWrapper(userEmail, new HttpClient(), _dbContext);
            
            Assert.AreSame(expectedUser, actualUser);
        }

        [TestMethod()]
        public async Task GetExistingEctUserOrNewWrapper_NonExistentUserEmail_BobIsReturned()
        {
            DateTime expectedLastSignIn = new DateTime(2020, 10, 1);
            GraphUserResponse mockGraphResponse = new GraphUserResponse() { DisplayName = "Bob BobS", Id = "7f3030345244a", UserPrincipalName = "bob@ect.ie" };
            Mock<ITestableExtensionMethods> mock = new Mock<ITestableExtensionMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetGraphUser(It.IsAny<HttpClient>(), It.IsAny<string>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            EctUser actualUser = await UserBehaviour.GetExistingEctUserOrNewWrapper(mockGraphResponse.UserPrincipalName, new HttpClient(), _dbContext);

            Assert.AreEqual(mockGraphResponse.UserPrincipalName, actualUser.Email);
            Assert.AreEqual(mockGraphResponse.DisplayName, actualUser.FullName);
            Assert.AreEqual(expectedLastSignIn, actualUser.LastSignIn);
        }
    }
}