using EctBlazorApp.Server;
using EctBlazorApp.Server.Behaviour;
using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static EctBlazorApp.ServerTests.MockObjects;


namespace EctBlazorApp.ServerTests
{
    [TestClass()]
    public class EctDbContextExtensionsTests : IDisposable
    {
        private readonly EctDbContext _dbContext;
        private HttpClient _httpClient;

        public EctDbContextExtensionsTests()
        {
            _dbContext = InMemoryDb.InitInMemoryDbContext();
            _httpClient = new HttpClient();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _httpClient = null;
        }

        [TestMethod()]
        public async Task GetExistingEctUserOrNew_ExistingUserEmail_AliceIsReturned()
        {
            string userEmail = "alice@ect.ie";
            EctUser expectedUser = _dbContext.Users.First(user => user.Email.Equals(userEmail));
            EctUser actualUser = await _dbContext.GetExistingEctUserOrNewAsync(userEmail, _httpClient);

            Assert.AreSame(expectedUser, actualUser);
        }

        [TestMethod()]
        public async Task GetExistingEctUserOrNew_NonExistentUserEmail_BobIsReturned()
        {
            DateTime expectedLastSignIn = new DateTime(2020, 10, 1);
            GraphUserResponse mockGraphResponse = GetMockGraphUserResponse("Bob BobS");
            Mock<IMockableGraphMethods> mock = new Mock<IMockableGraphMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetGraphUser(It.IsAny<HttpClient>(), It.IsAny<string>())).ReturnsAsync(mockGraphResponse);
            HttpClientExtensions.Implementation = mock.Object;

            EctUser actualUser = await _dbContext.GetExistingEctUserOrNewAsync(mockGraphResponse.UserPrincipalName, _httpClient);

            Assert.AreEqual(mockGraphResponse.UserPrincipalName, actualUser.Email);
            Assert.AreEqual(mockGraphResponse.DisplayName, actualUser.FullName);
            Assert.AreEqual(expectedLastSignIn, actualUser.LastSignIn);
        }

        [TestMethod()]
        public void GetReceivedMailInDateRangeForUserId_NonexistentUserId_ReturnsEmptyList()
        {
            List<ReceivedMail> expectedList = new List<ReceivedMail>();
            List<ReceivedMail> actualList;
            int userId = 999;
            DateTime startOfOctober = new DateTime(2020, 10, 1);
            DateTime endOfNovember = new DateTime(2020, 11, 30);

            actualList = _dbContext.GetReceivedMailInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod()]
        public void GetReceivedMailInDateRangeForUserId_UserIdForAlice_ReturnsSingleReceivedMail()
        {
            List<ReceivedMail> expectedList;
            List<ReceivedMail> actualList;
            int userId = 1;
            DateTime startOfOctober = new DateTime(2020, 10, 1);
            DateTime endOfNovember = new DateTime(2020, 11, 30);

            expectedList = _dbContext.ReceivedEmails.Where(re =>
                re.EctUserId == userId
                && re.ReceivedAt >= startOfOctober
                && re.ReceivedAt < endOfNovember).ToList();

            actualList = _dbContext.GetReceivedMailInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod()]
        public void GetCalendarEventsInDateRangeForUserId_NonexistentUserId_ReturnsEmptyList()
        {
            List<CalendarEvent> expectedList = new List<CalendarEvent>();
            List<CalendarEvent> actualList;
            int userId = 999;
            DateTime startOfOctober = new DateTime(2020, 10, 1);
            DateTime endOfNovember = new DateTime(2020, 11, 30);

            actualList = _dbContext.GetCalendarEventsInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod()]
        public void GetCalendarEventsInDateRangeForUserId_UserIdForAlice_ReturnsSingleCalendarEvent()
        {
            List<CalendarEvent> expectedList;
            List<CalendarEvent> actualList;
            int userId = 1;
            DateTime startOfOctober = new DateTime(2020, 10, 1);
            DateTime endOfNovember = new DateTime(2020, 11, 30);

            expectedList = _dbContext.CalendarEvents.Where(ce =>
                ce.EctUserId == userId
                && ce.End < endOfNovember
                && ce.Start >= startOfOctober).ToList();

            actualList = _dbContext.GetCalendarEventsInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod()]
        public void GetSentMailInDateRangeForUserId_NonexistentUserId_ReturnsEmptyList()
        {
            List<SentMail> expectedList = new List<SentMail>();
            List<SentMail> actualList;
            int userId = 999;
            DateTime startOfOctober = new DateTime(2020, 10, 1);
            DateTime endOfNovember = new DateTime(2020, 11, 30);

            actualList = _dbContext.GetSentMailInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod()]
        public void GetSentMailInDateRangeForUserId_UserIdForAlice_ReturnsSingleSentMail()
        {
            List<SentMail> expectedList;
            List<SentMail> actualList;
            int userId = 1;
            DateTime startOfOctober = new DateTime(2020, 10, 1);
            DateTime endOfNovember = new DateTime(2020, 11, 30);

            expectedList = _dbContext.SentEmails.Where(se =>
                se.EctUserId == userId
                && se.SentAt >= startOfOctober
                && se.SentAt < endOfNovember).ToList();

            actualList = _dbContext.GetSentMailInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }
    }
}
