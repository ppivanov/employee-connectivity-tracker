using EctBlazorApp.Server.Behaviour;
using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.ServerTests;
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

namespace EctBlazorApp.Server.Tests
{
    [TestClass()]
    public class UserBehaviourTests : IDisposable
    {
        private readonly EctDbContext _dbContext;
        private HttpClient _httpClient;

        public UserBehaviourTests()
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
        public async Task GetExistingEctUserOrNewWrapper_ExistingUserEmail_AliceIsReturned()
        {
            string userEmail = "alice@ect.ie";
            EctUser expectedUser = _dbContext.Users.First(user => user.Email.Equals(userEmail));
            EctUser actualUser = await _dbContext.GetExistingEctUserOrNewAsync(userEmail, new HttpClient());

            Assert.AreSame(expectedUser, actualUser);
        }

        [TestMethod()]
        public async Task GetExistingEctUserOrNewWrapper_NonExistentUserEmail_BobIsReturned()
        {
            DateTime expectedLastSignIn = new DateTime(2020, 10, 1);
            GraphUserResponse mockGraphResponse = GetMockGraphUserResponse("Bob BobS");
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetGraphUser(It.IsAny<HttpClient>(), It.IsAny<string>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            EctUser actualUser = await _dbContext.GetExistingEctUserOrNewAsync(mockGraphResponse.UserPrincipalName, new HttpClient());

            Assert.AreEqual(mockGraphResponse.UserPrincipalName, actualUser.Email);
            Assert.AreEqual(mockGraphResponse.DisplayName, actualUser.FullName);
            Assert.AreEqual(expectedLastSignIn, actualUser.LastSignIn);
        }

        [TestMethod()]
        public async Task UpdateCalendarEventRecordsWrapperAsync_TwoMissingEvents_EventsSavedSuccessfully()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] orgraniserDetails = { GetTestUser("John JohnS"), GetTestUser("Jessica JessicaS") };

            GraphEventsResponse mockEvent = GetMockGraphEventResponseOneDayAfterLastLogin(contextUser, orgraniserDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingCalendarEvents(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockEvent);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateCalendarEventRecordsAsync(new HttpClient(), _dbContext);
            bool eventsAddedToDb = true;
            foreach (var orgraniser in orgraniserDetails)
            {
                if (contextUser.CalendarEvents.Any(e => e.Organizer.Contains(orgraniser.ToString())) == false)
                {
                    eventsAddedToDb = false;
                    break;
                }
            }

            Assert.IsTrue(actualValue);
            Assert.IsTrue(eventsAddedToDb);
        }

        [TestMethod()]
        public async Task UpdateCalendarEventRecordsWrapperAsync_NoEvents_RecordsUpToDate()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] orgraniserDetails = new MicrosoftGraphEmailAddress[0];

            GraphEventsResponse mockEvent = GetMockGraphEventResponseOneDayAfterLastLogin(contextUser, orgraniserDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingCalendarEvents(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockEvent);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateCalendarEventRecordsAsync(new HttpClient(), _dbContext);

            Assert.IsTrue(actualValue);
        }

        [TestMethod()]
        public async Task UpdateReceivedMailRecordsWrapperAsync_TwoMissingEmails_EmailsSavedSuccessfully()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] senderDetails = { GetTestUser("John JohnS"), GetTestUser("Jessica JessicaS") };

            GraphReceivedMailResponse mockGraphResponse = GetMockGraphReceivedMailResponseOneDayAfterLastLogin(contextUser, senderDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingReceivedMail(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateReceivedMailRecordsAsync(new HttpClient(), _dbContext);
            bool mailAddedToDb = true;
            foreach (var sender in senderDetails)
            {
                if (contextUser.ReceivedEmails.Any(m => m.From.Equals(sender.ToString())) == false)
                {
                    mailAddedToDb = false;
                    break;
                }
            }

            Assert.IsTrue(actualValue);
            Assert.IsTrue(mailAddedToDb);
        }

        [TestMethod()]
        public async Task UpdateReceivedMailRecordsWrapperAsync_NoEmails_RecordsUpToDate()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] senderDetails = new MicrosoftGraphEmailAddress[0];

            GraphReceivedMailResponse mockGraphResponse = GetMockGraphReceivedMailResponseOneDayAfterLastLogin(contextUser, senderDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingReceivedMail(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateReceivedMailRecordsAsync(new HttpClient(), _dbContext);

            Assert.IsTrue(actualValue);
        }

        [TestMethod()]
        public async Task UpdateSentMailRecordsWrapperAsync_TwoMissingEmails_EmailsSavedSuccessfully()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] senderDetails = { GetTestUser("John JohnS"), GetTestUser("Jessica JessicaS") };

            GraphSentMailResponse mockGraphResponse = GetMockGraphSentMailResponseOneDayAfterLastLogin(contextUser, senderDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingSentMail(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateSentMailRecordsAsync(new HttpClient(), _dbContext);
            bool mailAddedToDb = true;
            foreach (var sender in senderDetails)
            {
                if (contextUser.SentEmails.Any(m => m.RecipientsAsString.Contains(sender.ToString())) == false)
                {
                    mailAddedToDb = false;
                    break;
                }
            }

            Assert.IsTrue(actualValue);
            Assert.IsTrue(mailAddedToDb);
        }

        [TestMethod()]
        public async Task UpdateSentMailRecordsWrapperAsync_NoEmails_RecordsUpToDate()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] recipientDetails = new MicrosoftGraphEmailAddress[0];

            GraphSentMailResponse mockGraphResponse = GetMockGraphSentMailResponseOneDayAfterLastLogin(contextUser, recipientDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingSentMail(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateSentMailRecordsAsync(new HttpClient(), _dbContext);

            Assert.IsTrue(actualValue);
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
    }
}