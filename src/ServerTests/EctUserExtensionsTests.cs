using EctBlazorApp.Server;
using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared.Entities;
using EctBlazorApp.Shared.GraphModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static EctBlazorApp.ServerTests.MockObjects;

namespace EctBlazorApp.ServerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class EctUserExtensionsTests : IDisposable
    {
        private readonly EctDbContext _dbContext;

        public EctUserExtensionsTests()
        {
            _dbContext = InMemoryDb.InitInMemoryDbContext();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task UpdateCalendarEventRecordsAsync_TwoMissingEvents_EventsSavedSuccessfully()
        {
            EctUser contextUser = _dbContext.Users.FirstOrDefault(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] orgraniserDetails = { GetTestUser("John JohnS"), GetTestUser("Jessica JessicaS") };

            GraphEventsResponse mockEvent = GetMockGraphEventResponseOneDayAfterLastLogin(contextUser, orgraniserDetails);
            Mock<IMockableGraphMethods> mock = new Mock<IMockableGraphMethods>
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

        [TestMethod]
        public async Task UpdateCalendarEventRecordsAsync_NoEvents_RecordsUpToDate()
        {
            EctUser contextUser = _dbContext.Users.FirstOrDefault(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] orgraniserDetails = Array.Empty<MicrosoftGraphEmailAddress>();

            GraphEventsResponse mockEvent = GetMockGraphEventResponseOneDayAfterLastLogin(contextUser, orgraniserDetails);
            Mock<IMockableGraphMethods> mock = new() { CallBase = true };
            mock.Setup(x => x.GetMissingCalendarEvents(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockEvent);
            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateCalendarEventRecordsAsync(new HttpClient(), _dbContext);

            Assert.IsTrue(actualValue);
        }

        [TestMethod]
        public async Task UpdateReceivedMailRecordsAsync_TwoMissingEmails_EmailsSavedSuccessfully()
        {
            EctUser contextUser = _dbContext.Users.FirstOrDefault(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] senderDetails = { GetTestUser("John JohnS"), GetTestUser("Jessica JessicaS") };

            GraphReceivedMailResponse mockGraphResponse = GetMockGraphReceivedMailResponseOneDayAfterLastLogin(contextUser, senderDetails);
            Mock<IMockableGraphMethods> mock = new() { CallBase = true };
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

        [TestMethod]
        public async Task UpdateReceivedMailRecordsAsync_NoEmails_RecordsUpToDate()
        {
            EctUser contextUser = _dbContext.Users.FirstOrDefault(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] senderDetails = Array.Empty<MicrosoftGraphEmailAddress>();

            GraphReceivedMailResponse mockGraphResponse = GetMockGraphReceivedMailResponseOneDayAfterLastLogin(contextUser, senderDetails);
            Mock<IMockableGraphMethods> mock = new() { CallBase = true };
            mock.Setup(x => x.GetMissingReceivedMail(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockGraphResponse);
            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateReceivedMailRecordsAsync(new HttpClient(), _dbContext);

            Assert.IsTrue(actualValue);
        }

        [TestMethod]
        public async Task UpdateSentMailRecordsAsync_TwoMissingEmails_EmailsSavedSuccessfully()
        {
            EctUser contextUser = _dbContext.Users.FirstOrDefault(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] senderDetails = { GetTestUser("John JohnS"), GetTestUser("Jessica JessicaS") };

            GraphSentMailResponse mockGraphResponse = GetMockGraphSentMailResponseOneDayAfterLastLogin(contextUser, senderDetails);
            Mock<IMockableGraphMethods> mock = new() { CallBase = true };
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

        [TestMethod]
        public async Task UpdateSentMailRecordsAsync_NoEmails_RecordsUpToDate()
        {
            EctUser contextUser = _dbContext.Users.FirstOrDefault(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] recipientDetails = Array.Empty<MicrosoftGraphEmailAddress>();

            GraphSentMailResponse mockGraphResponse = GetMockGraphSentMailResponseOneDayAfterLastLogin(contextUser, recipientDetails);
            Mock<IMockableGraphMethods> mock = new() { CallBase = true };
            mock.Setup(x => x.GetMissingSentMail(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockGraphResponse);
            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateSentMailRecordsAsync(new HttpClient(), _dbContext);

            Assert.IsTrue(actualValue);
        }
    }
}
