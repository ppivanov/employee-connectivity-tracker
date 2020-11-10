using System;
using System.IO;
using System.Security.Claims;
using Microsoft.Graph;

namespace EctBlazorApp.Client.Graph
{

    public static class MicrosoftIdentityClaimTypes
    {
        public const string DisplayName = "name";
        public const string TenantId = "tid";
        public const string PreferredUsername = "preferred_username";
    }

    // Helper methods to access Graph user data stored in
    // the claims principal
    public static class GraphClaimsPrincipalExtensions
    {
        public static string GetUserDisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(c => c.Type == MicrosoftIdentityClaimTypes.DisplayName).Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(c => c.Type == MicrosoftIdentityClaimTypes.PreferredUsername).Value;
        }

        public static string GetUserTenantId(this ClaimsPrincipal claimsPrincipal)                                                          // can be used for another layer of authentication?
        {
            return claimsPrincipal.FindFirst(c => c.Type == MicrosoftIdentityClaimTypes.TenantId).Value;
        }
    }

}
