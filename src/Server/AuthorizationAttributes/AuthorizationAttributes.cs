using EctBlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EctBlazorApp.Server.AuthorizationAttributes
{
    public class AuthorizeAdminAttribute : CustomAuthorizeAttribute, IAuthorizationFilter
    {
        public override async void OnAuthorization(AuthorizationFilterContext context)
        {
            await DoesUserHaveAccess(context, EctDbContextExtensions.IsEmailForAdmin);
        }
    }

    public class AuthorizeLeaderAttribute : CustomAuthorizeAttribute, IAuthorizationFilter
    {
        public override async void OnAuthorization(AuthorizationFilterContext context)
        {
            await DoesUserHaveAccess(context, EctDbContextExtensions.IsEmailForLeader);
        }
    }
}
