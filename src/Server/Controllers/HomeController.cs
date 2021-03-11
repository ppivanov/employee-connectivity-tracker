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
using static EctBlazorApp.Shared.SharedMethods;

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
            using var client = new HttpClient();
            StringBuilder errorString = new StringBuilder("");
            if (userDetails == null || userDetails.GraphToken == null)
                return BadRequest("No inputs");

            string userId = await HttpContext.GetPreferredUsername();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userDetails.GraphToken);
            EctUser userForParms = await _dbContext.GetExistingEctUserOrNewAsync(userId, client, _mailKit);

            bool eventsSaved = await RetryUpdateMethodIfFails(client, _dbContext, userForParms.UpdateCalendarEventRecordsAsync);
            if (!eventsSaved)
                errorString.Append("Failed: Could not update calendar event records\n");

            bool receivedEmails = await RetryUpdateMethodIfFails(client, _dbContext, userForParms.UpdateReceivedMailRecordsAsync);
            if (!receivedEmails)
                errorString.Append("Failed: Could not update received email records\n");

            bool sentEmails = await RetryUpdateMethodIfFails(client, _dbContext, userForParms.UpdateSentMailRecordsAsync);
            if (!sentEmails)
                errorString.Append("Failed: Could not update sent email records\n");

            if (errorString.Length > 1)
                return BadRequest(errorString.ToString());

            userForParms.LastSignIn = DateTime.Now;
            try
            {
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
                    SecondsInMeeting = 0,
                    UserFullName = "",
                    UserEmail = "",
                });

            DateTime fromDateTime = NewDateTimeFromString(fromDate);
            DateTime toDateTime = NewDateTimeFromString(toDate);
            List<ReceivedMail> receivedMail = _dbContext.GetReceivedMailInDateRangeForUserId(user.Id, fromDateTime, toDateTime);
            List<SentMail> sentMail = _dbContext.GetSentMailInDateRangeForUserId(user.Id, fromDateTime, toDateTime);
            List<CalendarEvent> calendarEvents = _dbContext.GetCalendarEventsInDateRangeForUserId(user.Id, fromDateTime, toDateTime);

            double secondsInMeeting = CalendarEvent.GetTotalSecondsForEvents(calendarEvents);
            string userFullName = string.IsNullOrEmpty(UID) ? "" : user.FullName;
            string userEmailAddress = string.IsNullOrEmpty(UID) ? "" : user.Email;
            return Ok(new DashboardResponse
                {
                    CalendarEvents = calendarEvents,
                    ReceivedMail = receivedMail,
                    SentMail = sentMail,
                    SecondsInMeeting = secondsInMeeting,
                    UserFullName = userFullName,
                    UserEmail = userEmailAddress
                });
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
