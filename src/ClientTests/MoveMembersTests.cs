using Bunit;
using Bunit.TestDoubles.Authorization;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Pages;
using EctBlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

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
            authState.SetupGet(x => x.IsAdmin).Returns(false);
            authState.SetupGet(x => x.IsLeader).Returns(false);

            AddScopedServices();

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<MoveMembers>());
            var expectedMarkup = "<p>You do not have access to this page. Click <a href=\"/\">here</a> to go back.</p>";

            component.MarkupMatches(expectedMarkup);
        }

        [TestMethod]
        public void TeamLead_NoAccess()
        {
            authState.SetupGet(x => x.IsAdmin).Returns(false);
            authState.SetupGet(x => x.IsLeader).Returns(true);

            AddScopedServices();

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<MoveMembers>());
            var expectedMarkup = "<p>You do not have access to this page. Click <a href=\"/\">here</a> to go back.</p>";

            component.MarkupMatches(expectedMarkup);
        }

        [TestMethod]
        public void Admin_HasAccess()
        {
            authState.SetupGet(x => x.IsAdmin).Returns(true);
            authState.SetupGet(x => x.IsLeader).Returns(false);

            AddScopedServices();

            var component = testContext.RenderComponent<CascadingAuthenticationState>(x => x.AddChildContent<MoveMembers>());
            var expectedNumberOfInputFields = 2;

            var actualNumberOfInputFields = component.FindAll("input").Count;

            Assert.AreEqual(expectedNumberOfInputFields, actualNumberOfInputFields);

        }
        // todo: select teams, move member, delete member, submit + error messages

        private void AddScopedServices()
        {
            testContext.Services.AddScoped(tc => new DashboardState());
            testContext.Services.AddScoped(tc => authState.Object);
            testContext.Services.AddScoped(tc => mockApi.Object);
            testContext.Services.AddScoped(tc => jsRuntime.Object);

            var authContext = testContext.Services.AddTestAuthorization();
            authContext.SetAuthorized("Test User");
        }
    }
}
