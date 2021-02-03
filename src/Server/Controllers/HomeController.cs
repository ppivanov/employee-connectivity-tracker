using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Server.MailKit;
using EctBlazorApp.Shared;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedCommonMethods;

namespace EctBlazorApp.Server.Controllers
{
    [Authorize]
    [Route("api/main")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly EctDbContext _dbContext;
        private readonly MailKitMetadata _mailKitMetadata;

        public HomeController(EctDbContext context, MailKitMetadata mailKit)
        {
            _dbContext = context;
            _mailKitMetadata = mailKit;
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
        public async Task<ActionResult<DashboardResponse>> StatsForDashboard([FromQuery] string fromDate, [FromQuery] string toDate)    
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            int userId = _dbContext.Users.First(u => u.Email == userEmail).Id;                          // TODO - refactor this so that all necessary records are pulled out in a single dbContext linq query

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

        [Route("test-mail/{address}")]
        [HttpGet]
        public ActionResult TestMail(string address)
        {
            EmailMessage message = new EmailMessage();
            message.Sender = new MailboxAddress("Self", _mailKitMetadata.Sender);
            message.Reciever = new MailboxAddress("Receiver name", address);
            message.Subject = "Welcome";
            message.Content = "Hello World!";
            var mimeMessage = message.CreateMimeMessage();
            try
            {
                using SmtpClient smtpClient = new SmtpClient();
                smtpClient.Connect(_mailKitMetadata.SmtpServer, _mailKitMetadata.Port, true);
                smtpClient.Authenticate(_mailKitMetadata.UserName, _mailKitMetadata.Password);
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);
            }
            catch (Exception)
            {
                return BadRequest("Server error");
            }
            
            return Ok("Sent");
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
