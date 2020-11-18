using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPut]
        public async Task<ActionResult> UpdateDatabaseRecordsForUser(GraphUserRequestDetails userDetails)
        {
            using var client = new HttpClient();
            if (userDetails == null || userDetails.GraphToken == null || userDetails.UserId == null)
                return BadRequest("No inputs");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userDetails.GraphToken);
            EctUser userForParms = await GetExistingEctUserOrNewWrapperAsync(userDetails.UserId, client, _dbContext);
            bool eventsSaved = await userForParms.UpdateCalendarEventRecordsWrapperAsync(client, _dbContext);
            if (!eventsSaved)
                return BadRequest("Failure trying to update records");

            // update receivedMail
            // update sentMail
            return Ok("Records up to date");
        }
    }
}
