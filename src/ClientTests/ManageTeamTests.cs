using Bunit;
using EctBlazorApp.Client.Pages;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.ClientTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class ManageTeamTests : ComponentTests
    {
        private const string LEADER_NAME_EMAIL = "Leader Lead <leader@ect.ie>";

        public ManageTeamTests() : base() { }

        [TestMethod]
        public void NormalUser_NoAccess()
        {
            AddScopedServices(isAdmin: false, isLeader: false);
            var component = RenderComponent();

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void RandomTeamLead_NoAccess()
        {
            AddScopedServices(isAdmin: false, isLeader: true);
            var component = RenderComponent();

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void Admin_NoAccess()
        {
            AddScopedServices(isAdmin: true, isLeader: false);
            var component = RenderComponent();

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void TeamLead_HasAccess()
        {
            AddScopedServices(isAdmin: false, isLeader: true);
            mockApi.Setup(ma => ma.IsProcessingUserLeaderForTeam(It.IsAny<string>())).Returns(GetMockTeamRequestDetails());
            var component = RenderComponent();
            const int expectedNumberOfInputFields = 7;                                                                                // Team name / Team lead / Filter available users
                                                                                                                                      // Pts for notification / margin / full name to add / email to add
            var actualNumberOfInputFields = component.FindAll("input").Count;
            Assert.AreEqual(expectedNumberOfInputFields, actualNumberOfInputFields);

            component.Find(".create-team");
            component.Find(".selected-members");
            component.Find(".available-users");
            component.Find(".notification-options");
            component.Find(".btn-reset-form");
            component.Find(".btn-submit-form");
        }

        [TestMethod]
        public void VerifyTeamNameAndLeaderAreDisabled()
        {
            AddScopedServices(isAdmin: false, isLeader: true);
            mockApi.Setup(ma => ma.IsProcessingUserLeaderForTeam(It.IsAny<string>())).Returns(GetMockTeamRequestDetails());
            var component = RenderComponent();

            var nameInputField = component.Find("#name");
            var leaderInputField = component.Find("#createTeamLeader");

            Assert.IsTrue(nameInputField.ToMarkup().Contains("disabled"));
            Assert.IsTrue(leaderInputField.ToMarkup().Contains("disabled"));
        }

        [TestMethod]
        public void VerifyLeaderCannotBeRemovedFromNotificationOptions()
        {
            AddScopedServices(isAdmin: false, isLeader: true);
            mockApi.Setup(ma => ma.IsProcessingUserLeaderForTeam(It.IsAny<string>())).Returns(GetMockTeamRequestDetails());
            var component = RenderComponent();
            var expectedUser = EncodeFormattedString(LEADER_NAME_EMAIL);
            var expectedUserCount = 1;

            var usersToNotify_beforeClick = component.FindAll(".user-to-notify-row");
            Assert.AreEqual(expectedUserCount, usersToNotify_beforeClick.Count);

            var userToNotify = usersToNotify_beforeClick[0];
            Assert.IsTrue(userToNotify.InnerHtml.Contains(expectedUser));

            component.Find(".edit-notification-options-leader");

            var usersToNotify_afterClick = component.FindAll(".user-to-notify-row");
            Assert.AreEqual(expectedUserCount, usersToNotify_afterClick.Count);
        }

        [TestMethod]
        public void AddUserToNotify_RandomUser_Added()
        {
            AddScopedServices(isAdmin: false, isLeader: true);
            mockApi.Setup(ma => ma.IsProcessingUserLeaderForTeam(It.IsAny<string>())).Returns(GetMockTeamRequestDetails());
            var component = RenderComponent();
            var expectedUserCount = 2;
            var fullNameInput = "Random User";
            var emailInput = "random.user@ect.ie";
            var expectedNameEmail = FormatAndEncodeNameAndEmail(fullNameInput, emailInput);

            var nameInputField = component.Find("#userToNotify_name");
            var emailInputField = component.Find("#userToNotify_email");
            nameInputField.Change(fullNameInput);
            emailInputField.Change(emailInput);

            component.Find("#add-user-to-notify").Click();

            var usersToNotify_afterClick = component.FindAll(".user-to-notify-row");
            Assert.AreEqual(expectedUserCount, usersToNotify_afterClick.Count);

            var userToNotify = usersToNotify_afterClick[1];
            Assert.IsTrue(userToNotify.InnerHtml.Contains(expectedNameEmail));
        }
        
        [TestMethod]
        public void RemoveMember_FirstMember_RemovedAndHighlighted()
        {
            AddScopedServices(isAdmin: false, isLeader: true);
            mockApi.Setup(ma => ma.IsProcessingUserLeaderForTeam(It.IsAny<string>())).Returns(GetMockTeamRequestDetails());
            var component = RenderComponent();
            var expectedUserCount = 4;

            component.Find(".btn-deselect-member").Click();                                                // deselect the first mem

            var remainingMembers = component.FindAll(".selected-member");
            Assert.AreEqual(expectedUserCount, remainingMembers.Count);

            var availableUsers = component.FindAll(".available-user");
            var lastAvailableUser = availableUsers[availableUsers.Count - 1];
            Assert.IsTrue(lastAvailableUser.ClassList.Any(cl => cl.Equals("removed-member")));
        }

        [TestMethod]
        public void SelectMember_FirstAvaiableUser()
        {
            AddScopedServices(isAdmin: false, isLeader: true);
            mockApi.Setup(ma => ma.IsProcessingUserLeaderForTeam(It.IsAny<string>())).Returns(GetMockTeamRequestDetails());
            mockApi.Setup(ma => ma.GetUsersEligibleForMembers())
                .Returns(Task.FromResult(new List<string> { "R R <r@ect.ie>" }.AsEnumerable()));            // mock API response
            var component = RenderComponent();

            component.Find(".btn-select").Click();                                                          // Move the first user to selected

            const int expectedNumberOfSelectedUsers = 6;
            var actualNumberOfSelectedUsers = component.FindAll(".selected-member").Count;
            Assert.AreEqual(expectedNumberOfSelectedUsers, actualNumberOfSelectedUsers);                    // assert the number of members

            const int expectedNumberOfLeaderOptions = 0;
            var actualNumberOfLeaderOptions = component.FindAll(".leader-option").Count;
            Assert.AreEqual(expectedNumberOfLeaderOptions, actualNumberOfLeaderOptions);                    // assert the number of avaiable users
        }


        private IRenderedComponent<CascadingAuthenticationState> RenderComponent()
        {
            return testContext.RenderComponent<CascadingAuthenticationState>(parameters =>
                parameters.AddChildContent<ManageTeam>(childContent => childContent.Add(p => p.HashedTeamId, "12345")));
        }

        private static Task<EctTeamRequestDetails> GetMockTeamRequestDetails()
        {
            EctTeamRequestDetails team = new()
            {
                TeamId = "12345",
                Name = "bUnit Test Team",
                LeaderNameAndEmail = LEADER_NAME_EMAIL,
                MemberNamesAndEmails = GetFiveAvailableMembers(htmlEncoded: false),
                CurrentNotificationOptions = GetNotificationOptions(LEADER_NAME_EMAIL),
                NewNotificationOptions = GetNotificationOptions(LEADER_NAME_EMAIL),
            };

            return Task.FromResult(team);
        }

        private static NotificationOptionsResponse GetNotificationOptions(string leaderNameEmail)
        {
            NotificationOptionsResponse options = new()
            {
                MarginForNotification = 100,
                PointsThreshold = 0,
                UsersToNotify = new() { leaderNameEmail }
            };

            return options;
        }

        private static List<string> GetFiveAvailableMembers(bool htmlEncoded)
        {
            string leftBracket = htmlEncoded ? "&lt;" : "<";
            string rightBracket = htmlEncoded ? "&gt;" : ">";
            List<string> members = new()
            {
                $"Member 1 {leftBracket}member.one@ect.ie{rightBracket}",
                $"Member 2 {leftBracket}member.two@ect.ie{rightBracket}",
                $"Member 3 {leftBracket}member.three@ect.ie{rightBracket}",
                $"Member 4 {leftBracket}member.four@ect.ie{rightBracket}",
                $"Member 5 {leftBracket}member.five@ect.ie{rightBracket}",
            };

            return members;
        }

        private static string FormatAndEncodeNameAndEmail(string name, string email)
        {
            var formattedString = FormatFullNameAndEmail(name, email);
            return EncodeFormattedString(formattedString);
        }

        private static string EncodeFormattedString(string toFormat)
        {
            var formatted = toFormat.Replace("<", "&lt;");
            formatted = formatted.Replace(">", "&gt;");

            return formatted;
        }
    }
}
