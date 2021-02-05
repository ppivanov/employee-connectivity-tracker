using EctBlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.AuthorizationAttributes
{
    public abstract class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public virtual EctDbContext GetDbContextFromAuthorizationFilterContext(AuthorizationFilterContext context)                                          //This method is public only because it had to be mocked in the unit tests
        {
            var dbContext = context.HttpContext
                                .RequestServices
                                .GetService(typeof(EctDbContext)) as EctDbContext;

            return dbContext;
        }

        protected delegate bool AuthorizationDelegate(EctDbContext dbContext, string email);
        protected async Task DoesUserHaveAccess(AuthorizationFilterContext context, AuthorizationDelegate method)
        {
            var dbContext = GetDbContextFromAuthorizationFilterContext(context);

            var emailFromContext = await context.HttpContext.GetPreferredUsername();
            bool userHasAccess = method.Invoke(dbContext, emailFromContext);

            if (!userHasAccess)
                context.Result = new UnauthorizedResult();
        }
        public abstract void OnAuthorization(AuthorizationFilterContext context);
    }

}
