// Code adapted from https://github.com/microsoftgraph/msgraph-training-aspnet-core/blob/master/demo/GraphTutorial/Controllers/CalendarController.cs

using EctWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EctWebApp.CommonMethods.CommonStaticMethods;

namespace EctWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly GraphServiceClient _graphClient;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            GraphServiceClient graphClient,
            ILogger<DashboardController> logger)
        {
            _graphClient = graphClient;
            _logger = logger;
        }

        [AuthorizeForScopes(Scopes = new[] { "Calendars.Read", "Mail.Read" })]                      // Minimum set of permissions required for this view
        public async Task<IActionResult> Index(int start, int end)                                  // start - 0 == 12am today; end - 0 == 11:59pm today
        {
            DateTime fromDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.Today.AddDays(start));
            DateTime toDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.Today.AddDays(end + 1).AddMinutes(-1));

            string userTimeZone = User.GetUserGraphTimeZone();
            string mailQueryString = GetReceivedMailDateRangeQueryString(fromDate, toDate);
            List<QueryOption> calendarEventsQueryOptions = GetCalendarEventViewOptions(fromDate, toDate);

            try
            {                                                                                                                                                                   // These may be refactored outside of the controller method.
                List<CalendarEvent> calendarEvents = await CalendarEvent.GetCalendarEventsForQueryOptionsParmAsync(calendarEventsQueryOptions, userTimeZone, _graphClient);
                List<ReceivedMail> receivedMail = await ReceivedMail.GetReceivedMailForQueryStringParmAsync(mailQueryString, _graphClient); // does preferred user timezone have effect on results?
                List<SentMail> sentMail = await SentMail.GetSentMailForQueryStringParmAsync(mailQueryString, _graphClient); // does preferred user timezone have effect on results?

                DashboardViewModel model = new DashboardViewModel(fromDate, toDate, calendarEvents, receivedMail, sentMail);
                return View(model);
            }
            catch (ServiceException ex)
            {
                if (ex.InnerException is MicrosoftIdentityWebChallengeUserException)
                    throw;

                return View(new DashboardViewModel())
                    .WithError($"{mailQueryString}\nError", ex.Message);
            }
        }
    }
}
