using BlazorDateRangePicker;
using EctBlazorApp.Client.Graph;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public abstract class DashboardClass : ComponentBase
    {
        [Inject]
        protected IControllerConnection ApiConn { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        protected DateTimeOffset? FromDate { get; set; } = DateTimeOffset.Now;
        protected DateTimeOffset? ToDate { get; set; } = DateTimeOffset.Now.AddDays(1);

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

        protected List<DateTime> SplitDateRangeToChunks()                                                 // These methods can be moved into their own API to allow for scaling
        {
            DateTime startDate = FromDate.Value.Date;
            DateTime endDate = ToDate.Value.Date;
            List<DateTime> dateTimeChunks = new List<DateTime>();

            while (startDate <= endDate)
            {
                dateTimeChunks.Add(startDate);
                startDate = startDate.AddDays(1);
            }

            return dateTimeChunks;
        }

        protected abstract object[][] GetCalendarEventsData();
        protected abstract object[][] GetSentAndReceivedEmailData();
        protected abstract Task UpdateDashboard();
    }
}
