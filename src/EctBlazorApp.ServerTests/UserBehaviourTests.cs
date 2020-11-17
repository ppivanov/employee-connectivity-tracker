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
using static EctBlazorApp.ServerTests.MockObjects;
using static EctBlazorApp.Shared.SharedCommonMethods;

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
            EctUser expectedUser = _dbContext.Users.First(user => user.Email.Equals(userEmail));
            EctUser actualUser = await UserBehaviour.GetExistingEctUserOrNewWrapperAsync(userEmail, new HttpClient(), _dbContext);

            Assert.AreSame(expectedUser, actualUser);
        }

        [TestMethod()]
        public async Task GetExistingEctUserOrNewWrapper_NonExistentUserEmail_BobIsReturned()
        {
            DateTime expectedLastSignIn = new DateTime(2020, 10, 1);
            GraphUserResponse mockGraphResponse = GetMockGraphUserResponse("Bob BobS");
            Mock<ITestableExtensionMethods> mock = new Mock<ITestableExtensionMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetGraphUser(It.IsAny<HttpClient>(), It.IsAny<string>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            EctUser actualUser = await UserBehaviour.GetExistingEctUserOrNewWrapperAsync(mockGraphResponse.UserPrincipalName, new HttpClient(), _dbContext);

            Assert.AreEqual(mockGraphResponse.UserPrincipalName, actualUser.Email);
            Assert.AreEqual(mockGraphResponse.DisplayName, actualUser.FullName);
            Assert.AreEqual(expectedLastSignIn, actualUser.LastSignIn);
        }

        [TestMethod()]
        public async Task UpdateCalendarEventRecordsWrapperAsyncTest()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            GraphEventsResponse mockEvent = GetMockGraphEventResponseOneDayAfterLastLogin(contextUser, "Roger RogerS");
            Mock<ITestableExtensionMethods> mock = new Mock<ITestableExtensionMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingCalendarEvents(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockEvent);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateCalendarEventRecordsWrapperAsync(new HttpClient(), _dbContext);
            MicrosoftGraphEvent graphEvent = mockEvent.Value[0];
            var eventAddedToDb = 
                //_dbContext.CalendarEvents.Where(
                //e => e.Organizer.Equals(graphEvent.Organizer));
                _dbContext.CalendarEvents.Where(e => e.Attendees.Contains(
                    FormatFullNameAndEmail(contextUser.FullName, contextUser.Email)
                )).Any();                                                              // TODO - Fix this part -- Calendar events not connected to a user???

            Assert.IsTrue(actualValue);
            Assert.IsTrue(eventAddedToDb);
        }
    }
}