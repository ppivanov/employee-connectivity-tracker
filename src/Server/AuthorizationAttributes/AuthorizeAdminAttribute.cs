using EctBlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EctBlazorApp.Server.AuthorizationAttributes
{

    public class AuthorizeAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var dbContext = GetDbContextFromAuthorizationFilterContext(context);

            var emailFromContext = await context.HttpContext.GetPreferredUsername();
            bool userIsAdmin = dbContext.IsEmailForAdmin(emailFromContext);

            if (!userIsAdmin)
                context.Result = new UnauthorizedResult();
        }

        public virtual EctDbContext GetDbContextFromAuthorizationFilterContext(AuthorizationFilterContext context)                                          //This method is public only because it had to be mocked in the unit tests
        {
            var dbContext = context.HttpContext
                                .RequestServices
                                .GetService(typeof(EctDbContext)) as EctDbContext;

            return dbContext;
        }
    }
}
