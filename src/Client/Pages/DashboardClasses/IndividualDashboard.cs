using EctBlazorApp.Client.Graph;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public class PersonalDashboardClass : DashboardClass
    {
        [Parameter]
        public string HashedUserId { get; set; }
        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private List<SentMail> sentMail;
        private List<ReceivedMail> receivedMail;
        private List<CalendarEvent> calendarEvents;
        private string userEmailAddress = "";

        protected override int TotalEmailsCount
        {
            get => EmailsSentCount + EmailsReceivedCount;
        }

        protected int EmailsSentCount
        {
            get => sentMail != null ? sentMail.Count : 0;
        }
        protected int EmailsReceivedCount
        {
            get => receivedMail != null ? receivedMail.Count : 0;
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
            string userIdQueryString = string.IsNullOrEmpty(HashedUserId) ? "" : $"&UID={HashedUserId}";

            var response = await ApiConn.FetchDashboardResponse($"{queryString}{userIdQueryString}");
            await ExtractDataFromResponse(response);
            await FindCollaborators();

            await JsRuntime.InvokeVoidAsync("loadDashboardGraph", (object)GetEmailData(), (object)GetCalendarEventsData());
        }

        private async Task ExtractDataFromResponse(DashboardResponse response)
        {
            sentMail = response.SentMail;
            receivedMail = response.ReceivedMail;
            calendarEvents = response.CalendarEvents;
            secondsInMeeting = response.SecondsInMeeting;
            numberOfMeetings = calendarEvents != null ? calendarEvents.Count : 0;
            userEmailAddress = response.UserEmail;

            if (string.IsNullOrEmpty(response.UserFullName) == false)
                await JsRuntime.InvokeVoidAsync("setPageTitle", response.UserFullName);
        }

        private async Task<string> GetEmailForProcessingUser()
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
            string emailToFilterOut =
                string.IsNullOrEmpty(userEmailAddress) ? await GetEmailForProcessingUser() : userEmailAddress;
            foreach (var meeting in calendarEvents)
            {
                List<string> attendees = meeting.GetAttendeesExcludingUser(emailToFilterOut);
                foreach (var attendee in attendees)
                {
                    string fullName = GetFullNameFromFormattedString(attendee);
                    AddPointsToCollaborators(fullName, meetingCommPoints.Points);
                }
            }
        }
    }
}
