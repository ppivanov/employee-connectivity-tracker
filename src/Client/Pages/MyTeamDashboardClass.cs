using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public class MyTeamDashboardClass : DashboardClass
    {
        protected List<EctUser> teamMembers;

        protected override async Task OnInitializedAsync()
        {
            await jsRuntime.InvokeVoidAsync("setPageTitle", "My Team");
            await UpdateDashboard();
        }

        protected override object[][] GetCalendarEventsData()
        {
            //Dictionary<string, int> subjectAndCount = new Dictionary<string, int>();

            //    // loop and count using the dict
            //    foreach (var calendarEvent in calendarEvents)
            //    {
            //        if (subjectAndCount.ContainsKey(calendarEvent.Subject))
            //        {
            //            subjectAndCount[calendarEvent.Subject]++;
            //        }
            //        else
            //        {
            //            subjectAndCount.Add(calendarEvent.Subject, 1);
            //        }
            //    }

            //    object[][] newList = new object[subjectAndCount.Count + 1][];
            //    newList[0] = new object[] { "Event subject", "Number of events" };
            //    int i = 1;
            //    foreach (KeyValuePair<string, int> dictionaryEnty in subjectAndCount)
            //    {
            //        newList[i] = new object[] { dictionaryEnty.Key, dictionaryEnty.Value.ToString() };
            //        i++;
            //    }


            object[][] newList = new object[0][];
            return newList;
        }
        protected override object[][] GetSentAndReceivedEmailData()
        {
            var dates = SplitDateRangeToChunks();
            object[][] newList = new object[dates.Count + 1][];

            for (int i = 0; i < dates.Count; i++)
            {
                DateTime date = dates[i];
                int totalReceivedOnDate = 0;
                int totalSentOnDate = 0;

                StringBuilder sentMailTooltipText = new StringBuilder("");
                StringBuilder receivedMailTooltipText = new StringBuilder("");

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

                newList[i] = new object[] { date.ToString("dd MMM"), totalSentOnDate,
                sentMailTooltipText.ToString(), totalReceivedOnDate, receivedMailTooltipText.ToString() };
            }
            return newList;
        }


        protected override async Task UpdateDashboard()
        {
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

                    await jsRuntime.InvokeVoidAsync("loadMyTeamDashboardGraph", (object)GetSentAndReceivedEmailData());
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
        }
    }
}
