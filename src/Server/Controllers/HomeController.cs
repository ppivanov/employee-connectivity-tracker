using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static EctBlazorApp.Server.Behaviour.UserBehaviour;

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
            EctUser userForParms = await GetExistingEctUserOrNewWrapper(userId, client, _dbContext);
            // update calendarEvents
            // update receivedMail
            // update sentMail
            return BadRequest();
        }
    }
}
