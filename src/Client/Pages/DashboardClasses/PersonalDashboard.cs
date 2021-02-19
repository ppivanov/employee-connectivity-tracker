using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public class PersonalDashboardClass : DashboardClass
    {
        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private List<SentMail> sentMail;
        private List<ReceivedMail> receivedMail;
        private List<CalendarEvent> calendarEvents;

        protected override int TotalEmailsCount
        {
            get
            {
                return EmailsSentCount + EmailsReceivedCount;
            }
        }

        protected int EmailsSentCount
        {
            get
            {
                return sentMail != null ? sentMail.Count : 0;
            }
        }
        protected int EmailsReceivedCount
        {
            get
            {
                return receivedMail != null ? receivedMail.Count : 0;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Dashboard");
            await FetchCommunicationPoints();
            await UpdateDashboard();
        }

        protected override object[][] GetCalendarEventsData()
        {
            Dictionary<string, int> subjectAndCount = new Dictionary<string, int>();

            // loop and count using the dict
            foreach (var calendarEvent in calendarEvents)
            {
                if (subjectAndCount.ContainsKey(calendarEvent.Subject))
                {
                    subjectAndCount[calendarEvent.Subject]++;
                }
                else
                {
                    subjectAndCount.Add(calendarEvent.Subject, 1);
                }
            }

            object[][] newList = new object[subjectAndCount.Count][];
            int i = 0;
            foreach (KeyValuePair<string, int> dictionaryEntry in subjectAndCount)
            {
                newList[i++] = new object[] { dictionaryEntry.Key, dictionaryEntry.Value };
            }

            return newList;
        }
        protected override object[][] GetEmailData()
        {
            var dates = SplitDateRangeToChunks(FromDate.Value, ToDate.Value);
            object[][] newList = new object[dates.Count][];

            for (int i = 0; i < dates.Count; i++)
            {
                DateTime date = dates[i];
                int countOfSentMail = sentMail.Count(sm => sm.SentAt.Date == date);
                int countOfReceivedMail = receivedMail.Count(rm => rm.ReceivedAt.Date == date);

                newList[i] = new object[] { date.ToString("dd MMM"), countOfSentMail, countOfReceivedMail };
            }

            return newList;
        }

        protected override async Task UpdateDashboard()
        {
            string queryString = GetDateRangeQueryString(FromDate.Value, ToDate.Value);
            var response = await ApiConn.FetchDashboardResponse(queryString);
            ExtractDataFromResponse(response);
            await FindCollaborators();

            await JsRuntime.InvokeVoidAsync("loadDashboardGraph", (object)GetEmailData(), (object)GetCalendarEventsData());
        }

        private void ExtractDataFromResponse(DashboardResponse response)
        {
            sentMail = response.SentMail;
            receivedMail = response.ReceivedMail;
            calendarEvents = response.CalendarEvents;
            secondsInMeeting = response.SecondsInMeeting;
            numberOfMeetings = calendarEvents != null ? calendarEvents.Count : 0;
        }

        private async Task<string> GetProcessingUserEmail()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.GetUserEmail();
        }

        protected override async Task FindCollaborators()
        {
            collaboratorsDict.Clear();
            FindEmailCollaborators();
            await FindAttendeesFromCalendarEvents();
        }
        private void FindEmailCollaborators()
        {
            FindSentEmailCollaborators();
            FindReceivedEmailCollaborators();
        }
        private void FindSentEmailCollaborators()
        {
            foreach (var email in sentMail)
            {
                foreach (var recipient in email.Recipients)
                {
                    string fullName = GetFullNameFromFormattedString(recipient);
                    AddPointsToCollaborators(fullName, emailCommPoints.Points);
                }
            }
        }
        private void FindReceivedEmailCollaborators()
        {
            foreach (var email in receivedMail)
            {
                string senderFullName = GetFullNameFromFormattedString(email.From);
                AddPointsToCollaborators(senderFullName, emailCommPoints.Points);
            }
        }
        private async Task FindAttendeesFromCalendarEvents()
        {
            foreach (var meeting in calendarEvents)
            {
                List<string> attendees = meeting.GetAttendeesExcludingUser(await GetProcessingUserEmail());
                foreach (var attendee in attendees)
                {
                    string fullName = GetFullNameFromFormattedString(attendee);
                    AddPointsToCollaborators(fullName, meetingCommPoints.Points);
                }
            }
        }
    }
}
