using BlazorDateRangePicker;
using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public abstract class DashboardClass : ComponentBase
    {
        [Inject]
        protected IControllerConnection ApiConn { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        protected bool initialized = false;
        protected int numberOfMeetings = 0;
        protected double secondsInMeeting = 0;
        protected CommunicationPoint emailCommPoints;
        protected CommunicationPoint meetingCommPoints;

        protected DateTimeOffset? FromDate { get; set; } = DateTimeOffset.Now; 
        protected DateTimeOffset? ToDate { get; set; } = DateTimeOffset.Now.AddDays(1);
        protected string FormattedTimeInMeeting
        {
            get
            {
                return FormatSecondsToHoursAndMinutes(secondsInMeeting);
            }
        }
        protected async Task CustomApply(MouseEventArgs e, DateRangePicker picker)
        {
            await picker.ClickApply(e);

            await UpdateDashboard();
        }
        protected async Task ResetClick(MouseEventArgs e, DateRangePicker picker)
        {
            FromDate = DateTimeOffset.Now;
            ToDate = DateTimeOffset.Now.AddDays(1);
            // Close the picker
            await picker.Close();
            // Fire OnRangeSelectEvent
            await picker.OnRangeSelect.InvokeAsync(new DateRange());

            await UpdateDashboard();
        }

        protected async Task FetchCommunicationPoints()
        {
            var result = await ApiConn.FetchCommunicationPoints();

            emailCommPoints = result.Item1;
            meetingCommPoints = result.Item2;
        }

        protected abstract object[][] GetCalendarEventsData();
        protected abstract object[][] GetEmailData();
        protected abstract Task UpdateDashboard();
    }
}
