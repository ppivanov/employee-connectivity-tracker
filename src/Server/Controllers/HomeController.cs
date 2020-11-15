using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static EctBlazorApp.Server.CommonMethods.GraphMethods;

namespace EctBlazorApp.Server.Controllers
{
    [Authorize]
    [Route("api/main")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly EctDbContext _dbContext;

        public HomeController(EctDbContext context)
        {
            _dbContext = context;
        }

        [Route("update-records")]
        [HttpGet]
        public async Task<ActionResult> UpdateDatabaseRecordsForUser([FromQuery] string graphToken, [FromQuery] string userId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", graphToken);
            EctUser userForParms = await GetExistingEctUserOrNew(userId, client, _dbContext);
            // update calendarEvents
            // update receivedMail
            // update sentMail
            return BadRequest();
        }

        private static async Task<EctUser> GetExistingEctUserOrNew(string userId, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                EctUser userForUserIdParm = dbContext.Users.First(user => user.Email.Equals(userId));
                return userForUserIdParm;
            }
            catch (Exception)
            {
                EctUser addUserResult = await dbContext.AddUser(userId, client);
                return addUserResult;
            }
        }

        public static Task<EctUser> GetExistingEctUserOrNewWrapper(string userId, HttpClient client, EctDbContext dbContext)
        {
            return GetExistingEctUserOrNew(userId, client, dbContext);
        }

    }
}
