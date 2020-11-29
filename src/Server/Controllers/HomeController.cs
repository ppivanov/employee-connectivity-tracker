using EctBlazorApp.Server.Behaviour;
using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Server.Behaviour.EctDbContextExtensions;
using static EctBlazorApp.Shared.SharedCommonMethods;

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

        [Route("update-tracking-records")]
        [HttpPut]
        public async Task<ActionResult> UpdateTrackingRecordsForUser(GraphUserRequestDetails userDetails)
        {
            using var client = new HttpClient();
            StringBuilder errorString = new StringBuilder("");
            if (userDetails == null || userDetails.GraphToken == null)
                return BadRequest("No inputs");

            string userId = await HttpContext.GetPreferredUsername();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userDetails.GraphToken);
            EctUser userForParms = await _dbContext.GetExistingEctUserOrNewAsync(userId, client);

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
            else
            {
                userForParms.LastSignIn = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }

            return Ok("User records up to date");
        }

        [Route("get-dashboard-stats")]
        [HttpGet]
        public async Task<ActionResult<DashboardResponse>> StatsForDashboard([FromQuery] string fromDate, [FromQuery] string toDate)               // TODO - Secure this so that only the person logged in can view the data
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            int userId = _dbContext.Users.First(u => u.Email == userEmail).Id;

            DateTime formattedFromDate = NewDateTimeFromString(fromDate);
            DateTime formattedToDate = NewDateTimeFromString(toDate);
            List<ReceivedMail> receivedMail = _dbContext.GetReceivedMailInDateRangeForUserId(userId, formattedFromDate, formattedToDate);
            List<SentMail> sentMail = _dbContext.GetSentMailInDateRangeForUserId(userId, formattedFromDate, formattedToDate);
            List<CalendarEvent> calendarEvents = _dbContext.GetCalendarEventsInDateRangeForUserId(userId, formattedFromDate, formattedToDate);

            double secondsInMeeting = CalendarEvent.GetTotalSecondsForEvents(calendarEvents);

            return new DashboardResponse
            {
                CalendarEvents = calendarEvents,
                ReceivedMail = receivedMail,
                SentMail = sentMail,
                SecondsInMeeting = secondsInMeeting
            };
        }

        private delegate Task<bool> UpdateMetgodDelegate(HttpClient client, EctDbContext dbContext);

        private async Task<bool> RetryUpdateMethodIfFails(HttpClient client, EctDbContext dbContext, UpdateMetgodDelegate method)
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
