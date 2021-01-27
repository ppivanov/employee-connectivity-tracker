using EctBlazorApp.Server;
using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;

namespace EctBlazorApp.ServerTests
{
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public sealed class AuthorizationAttributesTests : IDisposable
    {
        private readonly EctDbContext _dbContext;

        public AuthorizationAttributesTests()
        {
            _dbContext = InMemoryDb.InitInMemoryDbContext();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod()]
        public void AuthorizeAdmin_IsNotAdmin_Unauthorized()
        {
            TestAuthorizeAdmin("alice@ect.ie", Assert.IsNotNull);
        }

        [TestMethod()]
        public void AuthorizeAdmin_IsAdmin_Authorized()
        {
            TestAuthorizeAdmin("admin@ect.ie", Assert.IsNull);
        }

        [TestMethod()]
        public void AuthorizeLeader_IsNotLeader_Unauthorized()
        {
            TestAuthorizeLeader("admin@ect.ie", Assert.IsNotNull);                                                                                                // The Result property is null only if the user has access 
        }
        
        [TestMethod()]
        public void AuthorizeLeader_IsLeader_Authorized()
        {
            TestAuthorizeLeader("alice@ect.ie", Assert.IsNull);                                                                                                // The Result property is null only if the user has access 
        }

        private delegate void AuthorizeAttributeTestDelegate(IActionResult authContextResult);

        private void TestAuthorizeAdmin(string preferredUsername, AuthorizeAttributeTestDelegate method)
        {
            AuthorizationFilterContext filterContext = MockObjects.GetAuthorizationFilterContext();

            Mock<AuthorizeAdminAttribute> mockAttribute = new Mock<AuthorizeAdminAttribute>()                                                       //Creating an instance of the attribute and mocking the result of GetDbContextFromAuthorizationFilterContext()
            {
                CallBase = true
            };
            mockAttribute.Setup(a => a.GetDbContextFromAuthorizationFilterContext(
                It.IsAny<AuthorizationFilterContext>())).Returns(_dbContext);

            Mock<IMockableMisc> mockHttpContext = new Mock<IMockableMisc>()                                                                         //Mocking the static method GetPreferredUsername
            {
                CallBase = true
            };
            // make testable via the interface IMockable
            mockHttpContext.Setup(hc => hc.GetPreferredUsername(It.IsAny<HttpContext>())).ReturnsAsync(preferredUsername);
            HttpContextExtensions.Implementation = mockHttpContext.Object;

            mockAttribute.Object.OnAuthorization(filterContext);

            method.Invoke(filterContext.Result);
        }
        private void TestAuthorizeLeader(string preferredUsername, AuthorizeAttributeTestDelegate method)
        {
            AuthorizationFilterContext filterContext = MockObjects.GetAuthorizationFilterContext();

            Mock<AuthorizeLeaderAttribute> mockAttribute = new Mock<AuthorizeLeaderAttribute>()                                                       //Creating an instance of the attribute and mocking the result of GetDbContextFromAuthorizationFilterContext()
            {
                CallBase = true
            };
            mockAttribute.Setup(a => a.GetDbContextFromAuthorizationFilterContext(
                It.IsAny<AuthorizationFilterContext>())).Returns(_dbContext);

            Mock<IMockableMisc> mockHttpContext = new Mock<IMockableMisc>()                                                                         //Mocking the static method GetPreferredUsername
            {
                CallBase = true
            };
            // make testable via the interface IMockable
            mockHttpContext.Setup(hc => hc.GetPreferredUsername(It.IsAny<HttpContext>())).ReturnsAsync(preferredUsername);
            HttpContextExtensions.Implementation = mockHttpContext.Object;

            mockAttribute.Object.OnAuthorization(filterContext);

            method.Invoke(filterContext.Result);
        }

    }
}
