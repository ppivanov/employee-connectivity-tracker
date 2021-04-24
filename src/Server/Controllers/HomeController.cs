using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Server.MailKit;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/main")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly EctDbContext _dbContext;
        private readonly EctMailKit _mailKit;

        public HomeController(EctDbContext context, EctMailKit mailKit)
        {
            _dbContext = context;
            _mailKit = mailKit;
        }

        [Route("tracking-records")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateTrackingRecordsForUser(GraphUserRequestDetails userDetails)
        {
            using HttpClient client = new();
            if (userDetails == null || userDetails.GraphToken == null)
                return BadRequest("No inputs");

            string userId = await HttpContext.GetPreferredUsername();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userDetails.GraphToken);

            EctUser userForParms = await _dbContext.GetExistingEctUserOrNewAsync(userId, client, _mailKit);
            string errorString = await UpdateCommunicationRecordsForUser(client, userForParms);

            if (string.IsNullOrEmpty(errorString) == false)
                return BadRequest(errorString.ToString());

            try
            {
                userForParms.LastSignIn = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return Ok("User records up to date");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Please, try again later.");
            }
        }

        [Route("dashboard-stats")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DashboardResponse>> StatsForDashboard([FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] string UID = "")
        {
            EctUser user = await _dbContext.GetUserFromHashOrProcessingUser(UID, HttpContext.GetPreferredUsername);
            if (user == null)
                return BadRequest(new DashboardResponse
                { 
                    CalendarEvents = new List<CalendarEvent>(),
                    ReceivedMail = new List<ReceivedMail>(),
                    SentMail = new List<SentMail>(),
                    UserFullName = string.Empty,
                    UserEmail = string.Empty,
                });

            user.OutputCommunicationRecordsInRange(fromDate, toDate, 
                out List<ReceivedMail> receivedMail, out List<SentMail> sentMail, out List<CalendarEvent> calendarEvents);          // output variables

            string userFullName = string.IsNullOrEmpty(UID) ? string.Empty : user.FullName;
            string userEmailAddress = string.IsNullOrEmpty(UID) ? string.Empty : user.Email;
            return Ok(new DashboardResponse
                {
                    CalendarEvents = calendarEvents,
                    ReceivedMail = receivedMail,
                    SentMail = sentMail,
                    UserFullName = userFullName,
                    UserEmail = userEmailAddress
                });
        }

        private async Task<string> UpdateCommunicationRecordsForUser(HttpClient client, EctUser user)
        {
            StringBuilder errorString = new(string.Empty);
            bool eventsSaved = await RetryUpdateMethodIfFails(client, _dbContext, user.UpdateCalendarEventRecordsAsync);
            if (!eventsSaved)
                errorString.Append("Failed: Could not update calendar event records\n");

            bool receivedEmails = await RetryUpdateMethodIfFails(client, _dbContext, user.UpdateReceivedMailRecordsAsync);
            if (!receivedEmails)
                errorString.Append("Failed: Could not update received email records\n");

            bool sentEmails = await RetryUpdateMethodIfFails(client, _dbContext, user.UpdateSentMailRecordsAsync);
            if (!sentEmails)
                errorString.Append("Failed: Could not update sent email records\n");

            return errorString.ToString();
        }

        private delegate Task<bool> UpdateMetgodDelegate(HttpClient client, EctDbContext dbContext);
        private static async Task<bool> RetryUpdateMethodIfFails(HttpClient client, EctDbContext dbContext, UpdateMetgodDelegate method)
        {
            const int defaultRetryCount = 3;
            int retryCount = 0;
            while (retryCount <= defaultRetryCount)
            {
                bool operationSuccessful = await method.Invoke(client, dbContext);
                if (operationSuccessful)
                    return true;

                retryCount++;
            }
            return false;
        }
    }
}
