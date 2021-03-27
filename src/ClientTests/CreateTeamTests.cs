using Bunit;
using Bunit.TestDoubles.Authorization;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Pages;
using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            AddScopedServices(isAdmin: false, isLeader: false, isAuthorized: true);

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<ManageTeam>());

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void TeamLead_NoAccess()
        {
            AddScopedServices(isAdmin: false, isLeader: true, isAuthorized: true);

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<ManageTeam>());

            var actualNestedComponent = component.FindComponent<NoAccess>();

            Assert.AreNotEqual(null, actualNestedComponent);
        }

        [TestMethod]
        public void Admin_HasAccess()
        {
            AddScopedServices(isAdmin: true, isLeader: false, isAuthorized: true);

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<ManageTeam>());
            var expectedNumberOfInputFields = 3;                                                                                // Team name / Team lead / Filter available users

            var actualNumberOfInputFields = component.FindAll("input").Count;

            component.Find(".create-team");
            component.Find(".selected-members");
            component.Find(".available-users");

            Assert.AreEqual(expectedNumberOfInputFields, actualNumberOfInputFields);
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
    }
}
