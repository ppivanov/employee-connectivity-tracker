using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedCommonMethods;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public class MyTeamDashboardClass : DashboardClass
    {
        protected bool isLeader = false;
        protected List<EctUser> teamMembers;
        protected int emailsSent = 0;
        protected int emailsReceived = 0;

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "My Team");
            isLeader = await ApiConn.IsProcessingUserALeader(Http);
            if (isLeader)
                await UpdateDashboard();
        }

        protected override object[][] GetCalendarEventsData()
        {
            object[][] newList = new object[0][];               // TODO
            return newList;
        }
        protected override object[][] GetSentAndReceivedEmailData()
        {
            var dates = SplitDateRangeToChunks(FromDate.Value, ToDate.Value);
            object[][] newList = new object[dates.Count + 1][];

            for (int i = 0; i < dates.Count; i++)
            {
                DateTime date = dates[i];
                int totalReceivedOnDate = 0;
                int totalSentOnDate = 0;
                string tooltipDate = date.ToString("dd MMM");
                StringBuilder sentMailTooltipText = new StringBuilder($"{tooltipDate}\n");
                StringBuilder receivedMailTooltipText = new StringBuilder($"{tooltipDate}\n");

                foreach (var member in teamMembers)
                {
                    string memberFirstName = member.FullName.Split(" ")[0];
                    int countOfSentMail = member.SentEmails.Count(sm => sm.SentAt.Date == date);
                    sentMailTooltipText.Append($"{memberFirstName}: {countOfSentMail}\n");

                    int countOfReceivedMail = member.ReceivedEmails.Count(sm => sm.ReceivedAt.Date == date);
                    receivedMailTooltipText.Append($"{memberFirstName}: {countOfReceivedMail}\n");

                    totalSentOnDate += countOfSentMail;
                    totalReceivedOnDate += countOfReceivedMail;
                }

                newList[i] = new object[] { tooltipDate, totalSentOnDate,
                    sentMailTooltipText.ToString(), totalReceivedOnDate, receivedMailTooltipText.ToString() };
                emailsReceived += totalReceivedOnDate;
                emailsSent += totalSentOnDate;
            }
            return newList;
        }

        protected override async Task UpdateDashboard()
        {
            emailsSent = 0;
            emailsReceived = 0;
            DateTime fromDate = FromDate.Value.Date;
            DateTime toDate = ToDate.Value.Date;

            string queryString = $"?fromDate={fromDate.ToString("yyyy-MM-dd")}&toDate={toDate.ToString("yyyy-MM-dd")}";

            var token = await ApiConn.GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    teamMembers = await Http.GetFromJsonAsync<List<EctUser>>($"api/team/get-team-stats{queryString}");

                    initialized = true;
                    await InvokeAsync(StateHasChanged);                                                                     // Force a refresh of the component before trying to load the js graphs

                    await JsRuntime.InvokeVoidAsync("loadMyTeamDashboardGraph", (object)GetSentAndReceivedEmailData());
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
        }
    }
}
