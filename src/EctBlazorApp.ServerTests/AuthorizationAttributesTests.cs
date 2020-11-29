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
using System.Collections.Generic;

namespace EctBlazorApp.ServerTests
{
    [TestClass()]
    public class AuthorizationAttributesTests : IDisposable
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
            ControllerContext controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };
            AuthorizationFilterContext filterContext = new AuthorizationFilterContext(controllerContext, new List<IFilterMetadata>());

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
            mockHttpContext.Setup(hc => hc.GetPreferredUsername(It.IsAny<HttpContext>())).ReturnsAsync("alice@ect.ie");
            HttpContextExtensions.Implementation = mockHttpContext.Object;

            mockAttribute.Object.OnAuthorization(filterContext);

            Assert.IsNotNull(filterContext.Result);                                                                                                 // The Result property is null only if the user has access 
        }

        [TestMethod()]
        public void AuthorizeAdmin_IsAdmin_Authorized()
        {
            ControllerContext controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };
            AuthorizationFilterContext filterContext = new AuthorizationFilterContext(controllerContext, new List<IFilterMetadata>());

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
            mockHttpContext.Setup(hc => hc.GetPreferredUsername(It.IsAny<HttpContext>())).ReturnsAsync("admin@ect.ie");
            HttpContextExtensions.Implementation = mockHttpContext.Object;

            mockAttribute.Object.OnAuthorization(filterContext);

            Assert.IsNull(filterContext.Result);
        }
    }
}
