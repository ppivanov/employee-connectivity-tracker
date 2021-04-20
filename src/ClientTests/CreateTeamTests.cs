using Bunit;
using EctBlazorApp.Client.Pages;
using EctBlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace EctBlazorApp.ClientTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class CreateTeamTests : ComponentTests
    {
        public CreateTeamTests() : base() { }

        [TestMethod]
        public void NormalUser_NoAccess()
        {
            AddScopedServices(isAdmin: false, isLeader: false);
            var component = RenderComponent();

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void TeamLead_NoAccess()
        {
            AddScopedServices(isAdmin: false, isLeader: true);
            var component = RenderComponent();

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void Admin_HasAccess()
        {
            AddScopedServices(isAdmin: true, isLeader: false);
            var component = RenderComponent();
            const int expectedNumberOfInputFields = 3;                                                                                // Team name / Team lead / Filter available users

            var actualNumberOfInputFields = component.FindAll("input").Count;

            component.Find(".create-team");
            component.Find(".selected-members");
            component.Find(".available-users");
            component.Find(".btn-submit-form");

            Assert.AreEqual(expectedNumberOfInputFields, actualNumberOfInputFields);
        }

        [TestMethod]
        public void AllAvailableMembersDisplayed()
        {
            AddScopedServices(isAdmin: true, isLeader: false);
            mockApi.Setup(ma => ma.GetUsersEligibleForMembers()).Returns(GetFiveAvailableMembers(htmlEncoded: false));
            var component = RenderComponent();
            const int expectedNumberOfUsers = 5;

            var actualNumberOfUsers = component.FindAll(".available-user").Count;
            var actualNumberOfLeaderOptions = component.FindAll(".leader-option").Count;

            Assert.AreEqual(expectedNumberOfUsers, actualNumberOfUsers);
            Assert.AreEqual(expectedNumberOfUsers, actualNumberOfLeaderOptions);
        }

        [TestMethod]
        public void MoveUserToSelected()
        {
            AddScopedServices(isAdmin: true, isLeader: false);
            mockApi.Setup(ma => ma.GetUsersEligibleForMembers()).Returns(GetFiveAvailableMembers(htmlEncoded: false));
            var component = RenderComponent();

            var firstMemberNameEmail = component.Find(".available-user-name-email").InnerHtml;                          // get the name-email of the first user
            component.Find(".btn-select").Click();                                                                      // move the first user to selected

            const int expectedNumberOfSelectedUsers = 1;
            var actualNumberOfSelectedUsers = component.FindAll(".selected-member").Count;
            Assert.AreEqual(expectedNumberOfSelectedUsers, actualNumberOfSelectedUsers);                                // verify there is only 1 member on the list

            var selectedMemberNameEmail = component.Find(".selected-member-name-email").InnerHtml;
            Assert.AreEqual(firstMemberNameEmail, selectedMemberNameEmail);                                             // verify the member is the one it was clicked on

            const int expectedNumberOfLeaderOptions = 4;
            var actualNumberOfLeaderOptions = component.FindAll(".leader-option").Count;
            Assert.AreEqual(expectedNumberOfLeaderOptions, actualNumberOfLeaderOptions);                                // verify that the rest of the members can still eligible for leaders
        }

        [TestMethod]
        public void RemoveMemberFromSelected()
        {
            AddScopedServices(isAdmin: true, isLeader: false);
            mockApi.Setup(ma => ma.GetUsersEligibleForMembers()).Returns(GetFiveAvailableMembers(htmlEncoded: false));
            var component = RenderComponent();

            var firstMemberBeforeMove = component.Find(".available-user-name-email").InnerHtml;                         // get the name-email of the first user
            component.Find(".btn-select").Click();                                                                      // move the first user to selected

            component.Find(".btn-deselect").Click();                                                                    // deselect the member

            const int expectedNumberOfSelectedUsers = 0;
            var actualNumberOfSelectedUsers = component.FindAll(".selected-member").Count;
            Assert.AreEqual(expectedNumberOfSelectedUsers, actualNumberOfSelectedUsers);                                // verify there are no members

            var allAvailableUsers = component.FindAll(".available-user-name-email");
            var lastAvailableUser = allAvailableUsers[allAvailableUsers.Count - 1].InnerHtml;
            Assert.AreEqual(firstMemberBeforeMove, lastAvailableUser);                                                  // verify the user is put at the bottom of the available users list

            const int expectedNumberOfLeaderOptions = 5;
            var actualNumberOfLeaderOptions = component.FindAll(".leader-option").Count;
            Assert.AreEqual(expectedNumberOfLeaderOptions, actualNumberOfLeaderOptions);                                // verify there are 5 available users
        }

        [TestMethod]
        public void FilterAvailableUsersByName()
        {
            AddScopedServices(isAdmin: true, isLeader: false);
            mockApi.Setup(ma => ma.GetUsersEligibleForMembers()).Returns(GetFiveAvailableMembers(htmlEncoded: false));
            var component = RenderComponent();

            var filterUsersInput = component.Find("#filter-users");
            filterUsersInput.Input("member 1");

            const int expectedNumberOfUsers = 1;
            var actualNumberOfUsers = component.FindAll(".available-user").Count;

            Assert.AreEqual(expectedNumberOfUsers, actualNumberOfUsers);
        }

        [TestMethod]
        public void FilterAvailableUsersByEmail()
        {
            AddScopedServices(isAdmin: true, isLeader: false);
            mockApi.Setup(ma => ma.GetUsersEligibleForMembers()).Returns(GetFiveAvailableMembers(htmlEncoded: false));
            var component = RenderComponent();

            var filterUsersInput = component.Find("#filter-users");
            filterUsersInput.Input("member.one");

            const int expectedNumberOfUsers = 1;
            var actualNumberOfUsers = component.FindAll(".available-user").Count;

            Assert.AreEqual(expectedNumberOfUsers, actualNumberOfUsers);
        }


        private IRenderedComponent<CascadingAuthenticationState> RenderComponent()
        {
            return testContext.RenderComponent<CascadingAuthenticationState>(parameters => parameters.AddChildContent<ManageTeam>()); ;
        }

        private static Task<IEnumerable<string>> GetFiveAvailableMembers(bool htmlEncoded)
        {
            string leftBracket = htmlEncoded ? "&lt;" : "<";
            string rightBracket = htmlEncoded ? "&gt;" : ">";
            IEnumerable<string> members = new List<string>()
            {
                $"Member 1 {leftBracket}member.one@ect.ie{rightBracket}",
                $"Member 2 {leftBracket}member.two@ect.ie{rightBracket}",
                $"Member 3 {leftBracket}member.three@ect.ie{rightBracket}",
                $"Member 4 {leftBracket}member.four@ect.ie{rightBracket}",
                $"Member 5 {leftBracket}member.five@ect.ie{rightBracket}",
            };

            return Task.FromResult(members);
        }
    }
}
