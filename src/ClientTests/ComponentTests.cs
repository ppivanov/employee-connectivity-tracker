using Bunit.TestDoubles.Authorization;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Linq;

namespace EctBlazorApp.ClientTests
{
    public abstract class ComponentTests : IDisposable
    {
        protected readonly Bunit.TestContext testContext;
        protected readonly Mock<IControllerConnection> mockApi;
        protected readonly Mock<IJSRuntime> jsRuntime;
        protected readonly Mock<CustomAuthState> authState;

        protected ComponentTests()
        {
            testContext = new();
            mockApi = new();
            jsRuntime = new();
            authState = new();
        }

        public void Dispose()
        {
            testContext.Dispose();
        }

        protected void AddScopedServices(bool isAdmin, bool isLeader, bool isAuthorized)
        {
            authState.SetupGet(x => x.IsAdmin).Returns(isAdmin);
            authState.SetupGet(x => x.IsLeader).Returns(isLeader);

            testContext.Services.AddScoped(tc => new DashboardState());
            testContext.Services.AddScoped(tc => authState.Object);
            testContext.Services.AddScoped(tc => mockApi.Object);
            testContext.Services.AddScoped(tc => jsRuntime.Object);

            var authContext = testContext.Services.AddTestAuthorization();
            if (isAuthorized)
                authContext.SetAuthorized("Test User");
        }

        protected static string GetMemberFirstNameFromTeamName(string teamName)
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
