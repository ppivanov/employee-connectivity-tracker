using Bunit;
using Bunit.TestDoubles.Authorization;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Pages;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EctBlazorApp.ClientTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class MoveMembersTests
    {
        private Bunit.TestContext testContext;
        private Mock<IControllerConnection> mockApi;
        private Mock<IJSRuntime> jsRuntime;
        private Mock<CustomAuthState> authState;

        [TestInitialize]
        public void Setup()
        {
            testContext = new();
            mockApi = new();
            jsRuntime = new();
            authState = new();
        }

        [TestCleanup]
        public void Teardown()
        {
            testContext.Dispose();
        }

        [TestMethod]
        public void NormalUser_NoAccess()
        {
            AddScopedServices(isAdmin: false, isLeader: false);

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<MoveMembers>());

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void TeamLead_NoAccess()
        {
            AddScopedServices(isAdmin: false, isLeader: true);

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<MoveMembers>());

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void Admin_HasAccess()
        {
            AddScopedServices(isAdmin: true, isLeader: false);

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<MoveMembers>());
            var expectedNumberOfInputFields = 2;

            var actualNumberOfInputFields = component.FindAll("input").Count;

            Assert.IsTrue(component.Markup.Contains("class=\"move-members\""));
            Assert.IsTrue(component.Markup.Contains("class=\"team float-left\""));
            Assert.IsTrue(component.Markup.Contains("class=\"team float-right\""));

            Assert.AreEqual(expectedNumberOfInputFields, actualNumberOfInputFields);
        }

        [TestMethod]
        public void SelectTeam()
        {
            AddScopedServices(isAdmin: true, isLeader: false);
            mockApi.Setup(ma => ma.FetchAllTeams()).Returns(MockAllTeamsResponseFromApi());

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<MoveMembers>());

            var leftTeamInputField = component.Find("#lt-selection");                                                       // get the left team selection input field

            var testTeam = "test team";
            leftTeamInputField.Input(testTeam);                                                                             // enter 'test team'

            var testTeamLeader = "TT Leader &lt;tt.leader@ect.ie&gt;";
            var leader = component.Find("#lt-leader-name-email");                                                       // find the table cell with the leader's name and email

            var expectedListOfMembers = GetFiveMembersForTeam(testTeam, htmlEncoded: true);
            var actualListOfMembers = component.FindAll(".lt-member-name-email");

            foreach (var expectedMember in expectedListOfMembers)
            {
                var isPresent = actualListOfMembers.Any(am => am.InnerHtml.Contains(expectedMember));
                Assert.IsTrue(isPresent);
            }

            Assert.IsTrue(leader.InnerHtml.Contains(testTeamLeader));
        }

        // todo: select teams, move member, delete member, submit + error messages

        private void AddScopedServices(bool isAdmin, bool isLeader)
        {
            authState.SetupGet(x => x.IsAdmin).Returns(isAdmin);
            authState.SetupGet(x => x.IsLeader).Returns(isLeader);

            testContext.Services.AddScoped(tc => new DashboardState());
            testContext.Services.AddScoped(tc => authState.Object);
            testContext.Services.AddScoped(tc => mockApi.Object);
            testContext.Services.AddScoped(tc => jsRuntime.Object);

            var authContext = testContext.Services.AddTestAuthorization();
            authContext.SetAuthorized("Test User");
        }

        private static Task<IEnumerable<EctTeamRequestDetails>> MockAllTeamsResponseFromApi()
        {
            IEnumerable<EctTeamRequestDetails> teams = new List<EctTeamRequestDetails>()
            {
                new()
                {
                    Name = "Test team",
                    LeaderNameAndEmail = "TT Leader <tt.leader@ect.ie>",
                    MemberNamesAndEmails = GetFiveMembersForTeam(teamName: "Test team", htmlEncoded: false),
                },
                new()
                {
                    Name = "Legends",
                    LeaderNameAndEmail = "Legends Leader <legends.leader@ect.ie>",
                    MemberNamesAndEmails = GetFiveMembersForTeam(teamName: "Legends", htmlEncoded: false),
                }
            };

            return Task.FromResult(teams);
        }

        private static List<string> GetFiveMembersForTeam(string teamName, bool htmlEncoded)
        {
            string memberFirstName = GetMemberFirstNameFromTeamName(teamName).ToUpper();
            string emailAddressFirstName = memberFirstName.ToLower();
            string leftBracket = htmlEncoded ? "&lt;" : "<";
            string rightBracket = htmlEncoded ? "&gt;" : ">";
            List<string> members = new()
            {
                $"{memberFirstName} Member 1 {leftBracket}{emailAddressFirstName}.member.one@ect.ie{rightBracket}",
                $"{memberFirstName} Member 2 {leftBracket}{emailAddressFirstName}.member.two@ect.ie{rightBracket}",
                $"{memberFirstName} Member 3 {leftBracket}{emailAddressFirstName}.member.three@ect.ie{rightBracket}",
                $"{memberFirstName} Member 4 {leftBracket}{emailAddressFirstName}.member.four@ect.ie{rightBracket}",
                $"{memberFirstName} Member 5 {leftBracket}{emailAddressFirstName}.member.five@ect.ie{rightBracket}",
            };

            return members;
        }

        private static string GetMemberFirstNameFromTeamName(string teamName)
        {
            string memberFirstName = string.Empty;
            var splitTeamName = teamName.Split(" ");
            if (splitTeamName.Length < 2)
                memberFirstName = teamName;
            else
            {
                foreach (var word in splitTeamName)
                    memberFirstName += word.ElementAt(0);
            }

            return memberFirstName;
        }
    }
}
