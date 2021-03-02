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
    [TestClass]
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
            _dbContext.Dispose();
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void AuthorizeAdmin_IsNotAdmin_Unauthorized()
        {
            TestAuthorizationAttribute<AuthorizeAdminAttribute>("alice@ect.ie", Assert.IsNotNull);
        }

        [TestMethod]
        public void AuthorizeAdmin_IsAdmin_Authorized()
        {
            TestAuthorizationAttribute<AuthorizeAdminAttribute>("admin@ect.ie", Assert.IsNull);
        }

        [TestMethod]
        public void AuthorizeLeader_IsNotLeader_Unauthorized()
        {
            TestAuthorizationAttribute<AuthorizeLeaderAttribute>("admin@ect.ie", Assert.IsNotNull);                                                                                                // The Result property is null only if the user has access 
        }
        
        [TestMethod]
        public void AuthorizeLeader_IsLeader_Authorized()
        {
            TestAuthorizationAttribute<AuthorizeLeaderAttribute>("alice@ect.ie", Assert.IsNull);                                                                                                // The Result property is null only if the user has access 
        }

        private delegate void AuthorizeAttributeTestDelegate(IActionResult authContextResult);

        private void TestAuthorizationAttribute<T> (string preferredUsername, AuthorizeAttributeTestDelegate method) where T : CustomAuthorizeAttribute
        {
            AuthorizationFilterContext filterContext = MockObjects.GetAuthorizationFilterContext();
            
            Mock<T> mockAttribute = new Mock<T>()                                                       //Creating an instance of the attribute and mocking the result of GetDbContextFromAuthorizationFilterContext()
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
