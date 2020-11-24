using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Server.Behaviour.UserBehaviour;
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
            if (userDetails == null || userDetails.GraphToken == null || userDetails.UserId == null)
                return BadRequest("No inputs");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userDetails.GraphToken);
            EctUser userForParms = await GetExistingEctUserOrNewWrapperAsync(userDetails.UserId, client, _dbContext);

            bool eventsSaved = await RetryUpdateMethodIfFails(client, _dbContext, userForParms.UpdateCalendarEventRecordsWrapperAsync);
            if (!eventsSaved)
                errorString.Append("Failed: Could not update calendar event records\n");

            bool receivedEmails = await RetryUpdateMethodIfFails(client, _dbContext, userForParms.UpdateReceivedMailRecordsWrapperAsync);
            if (!receivedEmails)
                errorString.Append("Failed: Could not update received email records\n");

            bool sentEmails = await RetryUpdateMethodIfFails(client, _dbContext, userForParms.UpdateSentMailRecordsWrapperAsync);
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
        public async Task<ActionResult<DashboardResponse>> StatsForDashboard([FromQuery] string userId,[FromQuery] string fromDate, [FromQuery] string toDate)               // TODO - Secure this so that only the person logged in can view the data
        {

            DateTime formattedFromDate = NewDateTimeFromString(fromDate);
            DateTime formattedToDate = NewDateTimeFromString(toDate);
            List<ReceivedMail> receivedMail = await GetReceivedMailInDateRange(formattedFromDate, formattedToDate);
            List<SentMail> sentMail = await GetSentMailInDateRange(formattedFromDate, formattedToDate);
            List<CalendarEvent> calendarEvents = await GetCalendarEventsInDateRange(formattedFromDate, formattedToDate);

            double secondsInMeeting = GetTotalSecondsForEvents(calendarEvents);

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

        private async Task<List<ReceivedMail>> GetReceivedMailInDateRange(DateTime fromDate, DateTime toDate)
        {
            var receivedMail =
                await _dbContext.ReceivedEmails.Where(c =>
                    c.ReceivedAt >= fromDate
                    && c.ReceivedAt < toDate).ToListAsync();
            return receivedMail;
        }
        private async Task<List<SentMail>> GetSentMailInDateRange(DateTime fromDate, DateTime toDate)
        {
            var sentMail =
                await _dbContext.SentEmails.Where(c =>
                    c.SentAt >= fromDate
                    && c.SentAt < toDate).ToListAsync();
            return sentMail;
        }

        private async Task<List<CalendarEvent>> GetCalendarEventsInDateRange(DateTime fromDate, DateTime toDate)
        {
            var calendarEvents =
                    await _dbContext.CalendarEvents.Where(c =>
                        c.Start >= fromDate
                        && c.End < toDate).ToListAsync();
            return calendarEvents;
        }

        private double GetTotalSecondsForEvents(List<CalendarEvent> calendarEvents)
        {
            double seconds = 0;
            foreach (var singleEvent in calendarEvents)
                seconds += (singleEvent.End - singleEvent.Start).TotalSeconds;

            return seconds;
        }
    }
}
