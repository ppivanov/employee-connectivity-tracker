﻿using EctBlazorApp.Server.Behaviour;
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
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
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
        public async Task UpdateCalendarEventRecordsWrapperAsync_TwoMissingEvents_EventsSavedSuccessfully()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] orgraniserDetails = { GetTestUser("Roger RogerS"), GetTestUser("Jessica JessicaS") };

            GraphEventsResponse mockEvent = GetMockGraphEventResponseOneDayAfterLastLogin(contextUser, orgraniserDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingCalendarEvents(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockEvent);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateCalendarEventRecordsWrapperAsync(new HttpClient(), _dbContext);
            bool eventsAddedToDb = true;
            foreach (var orgraniser in orgraniserDetails)
            {
                if (contextUser.CalendarEvents.Any(e => e.Organizer.Contains(orgraniser.ToString())) == false)
                {
                    eventsAddedToDb = false;
                    break;                                                                                                      // test has failed at this stage - no point looping through rest of the organisers
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

            bool actualValue = await contextUser.UpdateCalendarEventRecordsWrapperAsync(new HttpClient(), _dbContext);
            
            Assert.IsTrue(actualValue);
        }

        [TestMethod()]
        public async Task UpdateReivedMailRecordsWrapperAsync_TwoMissingEmails_EmailsSavedSuccessfully()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] senderDetails = { GetTestUser("Roger RogerS"), GetTestUser("Jessica JessicaS") };

            GraphReceivedMailResponse mockGraphResponse = GetMockGraphMailResponseOneDayAfterLastLogin(contextUser, senderDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingReceivedMail(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateReceivedMailRecordsWrapperAsync(new HttpClient(), _dbContext);
            bool mailAddedToDb = true;
            foreach (var sender in senderDetails)
            {
                if (contextUser.ReceivedEmails.Any(m => m.From.Equals(sender.ToString())) == false)
                {
                    mailAddedToDb = false;
                    break;                                                                                                      // test has failed at this stage - no point looping through rest of the organisers
                }
            }

            Assert.IsTrue(actualValue);
            Assert.IsTrue(mailAddedToDb);
        }

        [TestMethod()]
        public async Task UpdateReivedMailRecordsWrapperAsync_NoEmails_RecordsUpToDate()
        {
            EctUser contextUser = _dbContext.Users.First(user => user.Email.Equals("alice@ect.ie"));
            MicrosoftGraphEmailAddress[] senderDetails = new MicrosoftGraphEmailAddress[0];

            GraphReceivedMailResponse mockGraphResponse = GetMockGraphMailResponseOneDayAfterLastLogin(contextUser, senderDetails);
            Mock<IMockableMethods> mock = new Mock<IMockableMethods>
            {
                CallBase = true
            };
            mock.Setup(x => x.GetMissingReceivedMail(It.IsAny<HttpClient>(), It.IsAny<EctUser>())).ReturnsAsync(mockGraphResponse);

            HttpClientExtensions.Implementation = mock.Object;

            bool actualValue = await contextUser.UpdateReceivedMailRecordsWrapperAsync(new HttpClient(), _dbContext);

            Assert.IsTrue(actualValue);
        }
    }
}