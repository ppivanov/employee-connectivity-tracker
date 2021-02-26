using EctBlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;

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

    public class AuthorizeThirdPartyAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string headerValue = context.HttpContext.Request.Headers["Authorization"];
            string hashValue = headerValue.Split(" ")[1];
            string expectedHash = GetHashedAuthorizationHeader();

            if (hashValue.Equals(expectedHash) == false)
                context.Result = new UnauthorizedResult();
        }

        // Source code: https://www.c-sharpcorner.com/article/compute-sha256-hash-in-c-sharp/
        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string GetHashedAuthorizationHeader()
        {
            const string authorizationKeyword = "7irrcUoDw#ig&DLT";
            string authorizationHash = ComputeSha256Hash(authorizationKeyword);

            return authorizationHash;
        }
    }
}
