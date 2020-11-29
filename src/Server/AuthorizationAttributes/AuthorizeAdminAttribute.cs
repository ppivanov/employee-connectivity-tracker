using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using EctBlazorApp.Server.CommonMethods;

namespace EctBlazorApp.Server.AuthorizationAttributes
{

    public class AuthorizeAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var dbContext = GetDbContextFromAuthorizationFilterContext(context);

            var emailFromContext = await context.HttpContext.GetPrefferredUsername();
            bool userIsAdmin = dbContext.IsEmailForAdmin(emailFromContext);

            if (userIsAdmin == false)
                context.Result = new UnauthorizedResult();
        }

        private EctDbContext GetDbContextFromAuthorizationFilterContext(AuthorizationFilterContext context)
        {
            var dbContext = context.HttpContext
                                .RequestServices
                                .GetService(typeof(EctDbContext)) as EctDbContext;

            return dbContext;
        }
    }
}
