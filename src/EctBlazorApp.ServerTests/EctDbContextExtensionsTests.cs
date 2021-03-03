using EctBlazorApp.Server;
using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Server.MailKit;
using EctBlazorApp.Shared.Entities;
using EctBlazorApp.Shared.GraphModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static EctBlazorApp.ServerTests.MockObjects;
using static EctBlazorApp.Shared.SharedMethods;


namespace EctBlazorApp.ServerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class EctDbContextExtensionsTests : IDisposable
    {
        private readonly EctDbContext _dbContext;
        private readonly EctMailKit _mailKit;
        private HttpClient _httpClient;

        public EctDbContextExtensionsTests()
        {
            _dbContext = InMemoryDb.InitInMemoryDbContext();
            _httpClient = new();
            _mailKit = GetMailKit();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _httpClient.Dispose();
            _httpClient = null;
        }

        [TestMethod]
        public async Task GetExistingEctUserOrNew_ExistingUserEmail_AliceIsReturned()
        {
            string userEmail = "alice@ect.ie";
            EctUser expectedUser = _dbContext.Users.FirstOrDefault(user => user.Email.Equals(userEmail));
            EctUser actualUser = await _dbContext.GetExistingEctUserOrNewAsync(userEmail, _httpClient, _mailKit);

            Assert.AreSame(expectedUser, actualUser);
        }

        [TestMethod]
        public async Task GetExistingEctUserOrNew_NonExistentUserEmail_BobIsReturned()
        {
            DateTime expectedLastSignIn = new(2020, 10, 1);
            GraphUserResponse mockGraphResponse = GetMockGraphUserResponse("Bob BobS");
            Mock<IMockableGraphMethods> mock = new() { CallBase = true };
            mock.Setup(x => x.GetGraphUser(It.IsAny<HttpClient>(), It.IsAny<string>())).ReturnsAsync(mockGraphResponse);
            HttpClientExtensions.Implementation = mock.Object;

            EctUser actualUser = await _dbContext.GetExistingEctUserOrNewAsync(mockGraphResponse.UserPrincipalName, _httpClient, _mailKit);

            Assert.AreEqual(mockGraphResponse.UserPrincipalName, actualUser.Email);
            Assert.AreEqual(mockGraphResponse.DisplayName, actualUser.FullName);
            Assert.AreEqual(expectedLastSignIn, actualUser.LastSignIn);
        }

        [TestMethod]
        public void GetReceivedMailInDateRangeForUserId_NonexistentUserId_ReturnsEmptyList()
        {
            List<ReceivedMail> expectedList = new();
            List<ReceivedMail> actualList;
            int userId = 999;
            DateTime startOfOctober = new(2020, 10, 1);
            DateTime endOfNovember = new(2020, 11, 30);

            actualList = _dbContext.GetReceivedMailInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod]
        public void GetReceivedMailInDateRangeForUserId_UserIdForAlice_ReturnsSingleReceivedMail()
        {
            List<ReceivedMail> expectedList;
            List<ReceivedMail> actualList;
            int userId = 1;
            DateTime startOfOctober = new(2020, 10, 1);
            DateTime endOfNovember = new(2020, 11, 30);

            expectedList = _dbContext.ReceivedEmails.Where(re =>
                re.EctUserId == userId
                && re.ReceivedAt >= startOfOctober
                && re.ReceivedAt < endOfNovember).ToList();

            actualList = _dbContext.GetReceivedMailInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod]
        public void GetCalendarEventsInDateRangeForUserId_NonexistentUserId_ReturnsEmptyList()
        {
            List<CalendarEvent> expectedList = new List<CalendarEvent>();
            List<CalendarEvent> actualList;
            int userId = 999;
            DateTime startOfOctober = new(2020, 10, 1);
            DateTime endOfNovember = new(2020, 11, 30);

            actualList = _dbContext.GetCalendarEventsInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod]
        public void GetCalendarEventsInDateRangeForUserId_UserIdForAlice_ReturnsSingleCalendarEvent()
        {
            List<CalendarEvent> expectedList;
            List<CalendarEvent> actualList;
            int userId = 1;
            DateTime startOfOctober = new(2020, 10, 1);
            DateTime endOfNovember = new(2020, 11, 30);

            expectedList = _dbContext.CalendarEvents.Where(ce =>
                ce.EctUserId == userId
                && ce.End < endOfNovember
                && ce.Start >= startOfOctober).ToList();

            actualList = _dbContext.GetCalendarEventsInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod]
        public void GetSentMailInDateRangeForUserId_NonexistentUserId_ReturnsEmptyList()
        {
            List<SentMail> expectedList = new();
            List<SentMail> actualList;
            int userId = 999;
            DateTime startOfOctober = new(2020, 10, 1);
            DateTime endOfNovember = new(2020, 11, 30);

            actualList = _dbContext.GetSentMailInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestMethod]
        public void GetSentMailInDateRangeForUserId_UserIdForAlice_ReturnsSingleSentMail()
        {
            List<SentMail> expectedList;
            List<SentMail> actualList;
            int userId = 1;
            DateTime startOfOctober = new(2020, 10, 1);
            DateTime endOfNovember = new(2020, 11, 30);

            expectedList = _dbContext.SentEmails.Where(se =>
                se.EctUserId == userId
                && se.SentAt >= startOfOctober
                && se.SentAt < endOfNovember).ToList();

            actualList = _dbContext.GetSentMailInDateRangeForUserId(userId, startOfOctober, endOfNovember);

            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [DataTestMethod]
        [DataRow("admin@ect.ie", true)]
        [DataRow("doesnt@exist.ie", false)]
        [DataRow("alice@ect.ie", false)]
        [TestMethod]
        public void IsEmailForAdmin_Parametarized(string userEmail, bool expectedResult)
        {
            bool actualResult = _dbContext.IsEmailForAdmin(userEmail);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow("alice@ect.ie", true)]
        [DataRow("admin@ect.ie", false)]
        [DataRow("homer@ect.ie", false)]
        [DataRow("doesnt@exist.ie", false)]
        [TestMethod]
        public void IsEmailForLeader_Parametarized(string userEmail, bool expectedResult)
        {
            bool actualResult = _dbContext.IsEmailForLeader(userEmail);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public async Task GetUserFromHashOrProcessingUser_HashIsEmpty_ReturnsHomer()
        {
            string hashedUserId = "";
            EctUser expectedUser = _dbContext.Users.FirstOrDefault(u => u.Email.Equals("homer@ect.ie"));
            
            EctUser actualUser = await _dbContext.GetUserFromHashOrProcessingUser(hashedUserId, MockPreferredUsername_Homer);

            Assert.AreEqual(expectedUser, actualUser);
        }

        [TestMethod]
        public async Task GetUserFromHashOrProcessingUser_AliceRequestsHomerWithHash_ReturnsHomer()
        {
            EctUser expectedUser = _dbContext.Users.FirstOrDefault(u => u.Email.Equals("homer@ect.ie"));
            string hashedUserId = ComputeSha256Hash(expectedUser.Id.ToString());

            EctUser actualUser = await _dbContext.GetUserFromHashOrProcessingUser(hashedUserId, MockPreferredUsername_Alice);

            Assert.AreEqual(expectedUser, actualUser);
        }

        [TestMethod]
        public async Task GetUserFromHashOrProcessingUser_AliceRequestsAdminWithHash_ReturnsNull()
        {
            EctUser admin = _dbContext.Users.FirstOrDefault(u => u.Email.Equals("admin@ect.ie"));
            string hashedUserId = ComputeSha256Hash(admin.Id.ToString());

            EctUser actualUser = await _dbContext.GetUserFromHashOrProcessingUser(hashedUserId, MockPreferredUsername_Alice);

            Assert.AreEqual(null, actualUser);
        }

        private Task<string> MockPreferredUsername(string email)
        {
            return Task.Run(() => { return email; });
        }
        private Task<string> MockPreferredUsername_Alice()
        {
            return MockPreferredUsername("alice@ect.ie");
        }
        private Task<string> MockPreferredUsername_Homer()
        {
            return MockPreferredUsername("homer@ect.ie");
        }
    }
}
