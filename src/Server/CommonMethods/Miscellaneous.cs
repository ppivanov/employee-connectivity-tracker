using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
namespace EctBlazorApp.Server.CommonMethods
{
    public class Miscellaneous : IMockableMisc
    {
        public async Task<string> GetPreferredUsername(HttpContext controllerContext)                                                                           // in almost every case the claim 'preferred_username' is the email address of the user
        {
            var token = await controllerContext.GetTokenAsync("access_token");
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;

            var tokenClaims = tokenS.Claims;
            var prefferredUsername = tokenClaims.FirstOrDefault(c => c.Type == "preferred_username").Value;

            return prefferredUsername;
        }
    }
}
