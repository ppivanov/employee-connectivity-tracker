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
using static EctBlazorApp.Shared.SharedMethods;

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
            isLeader = await ApiConn.IsProcessingUserALeader();
            if (isLeader)
                await UpdateDashboard();
        }

        protected override object[][] GetCalendarEventsData()
        {
            Dictionary<string, int> subjectAndCount = new Dictionary<string, int>();

            foreach (var member in teamMembers)
            {
                foreach (var calendarEvent in member.CalendarEvents)
                {
                    if (subjectAndCount.ContainsKey(calendarEvent.Subject))
                        subjectAndCount[calendarEvent.Subject]++;
                    else
                        subjectAndCount.Add(calendarEvent.Subject, 1);
                }
            }
            // loop and count using the dict

            object[][] newList = new object[subjectAndCount.Count][];
            int i = 0;
            foreach (KeyValuePair<string, int> dictionaryEntry in subjectAndCount)
            {
                newList[i] = new object[] { dictionaryEntry.Key, dictionaryEntry.Value };
                i++;
            }
            return newList;
        }
        protected override object[][] GetEmailData()
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
            string queryString = GetDateRangeQueryString(FromDate.Value, ToDate.Value);

            var token = await ApiConn.GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    teamMembers = await Http.GetFromJsonAsync<List<EctUser>>($"api/team/get-team-stats{queryString}");

                    initialized = true;
                    await InvokeAsync(StateHasChanged);                                                                     // Force a refresh of the component before trying to load the js graphs

                    await JsRuntime.InvokeVoidAsync("loadMyTeamDashboardGraph", (object)GetEmailData(), (object)GetCalendarEventsData());
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
        }
    }
}
