﻿using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
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

namespace EctBlazorApp.Client.Pages.Dashboards
{
    public class PersonalDashboardClass : DashboardClass
    {
        [Parameter]
        public string HashedUserId { get; set; }
        [Inject]
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected DashboardState DashboardState { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private List<SentMail> sentMail;
        private List<ReceivedMail> receivedMail;
        private List<CalendarEvent> calendarEvents;
        private string userEmailAddress = string.Empty;

        protected int EmailsReceivedCount => receivedMail != null ? receivedMail.Count : 0;
        protected int EmailsSentCount => sentMail != null ? sentMail.Count : 0;
        protected override int TotalEmailsCount => EmailsSentCount + EmailsReceivedCount;
        protected string ServerMessage { get; set; }

        protected override async Task FindCollaborators()
        {
            CollaboratorsDict.Clear();
            FindEmailCollaborators();
            await FindAttendeesFromCalendarEvents();
        }

        protected override object[][] GetCalendarEventsData()
        {
            Dictionary<string, double> subjectAndCount = new Dictionary<string, double>();

            foreach (var calendarEvent in calendarEvents)                                                               // loop and count using the dict
            {
                double eventDurationSeconds = CalendarEvent.GetTotalSecondsForSingleEvent(calendarEvent);
                double eventDurationMinutes = GetMinutesFromSeconds(eventDurationSeconds);
                SecondsInMeeting += eventDurationSeconds;

                if (subjectAndCount.ContainsKey(calendarEvent.Subject))
                    subjectAndCount[calendarEvent.Subject] += eventDurationMinutes;
                else
                    subjectAndCount.Add(calendarEvent.Subject, eventDurationMinutes);
            }

            object[][] newList = new object[subjectAndCount.Count][];
            int i = 0;
            foreach (KeyValuePair<string, double> dictionaryEntry in subjectAndCount)
                newList[i++] = new object[] { dictionaryEntry.Key, dictionaryEntry.Value };

            return newList;
        }

        protected override object[][] GetEmailData()
        {
            var dates = SplitDateRangeToChunks(FromDate.Value, ToDate.Value);
            object[][] newList = new object[dates.Count][];

            for (int i = 0; i < dates.Count; i++)                                                                       // for every day in the range - count the sent and received emails
            {
                DateTime date = dates[i];
                int countOfSentMail = sentMail.Count(sm => sm.SentAt.Date == date);
                int countOfReceivedMail = receivedMail.Count(rm => rm.ReceivedAt.Date == date);

                newList[i] = new object[] { date.ToString("dd MMM"), countOfSentMail, countOfReceivedMail };
            }

            return newList;
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Dashboard");
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);                                               // method fetches the user permissions only if needed
            await FetchCommunicationPoints();
        }

        protected override async Task OnParametersSetAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Dashboard");
            await UpdateDashboard();
        }

        protected override async Task UpdateDashboard()
        {
            SecondsInMeeting = 0;
            string queryString = GetDateRangeQueryString(FromDate.Value, ToDate.Value);
            string userIdQueryString = string.Empty;
            if(string.IsNullOrEmpty(HashedUserId) == false)                                                             // if a team lead is requesting to view the data for a member
            {
                DashboardState.SetIsDrillDown(true);
                userIdQueryString = $"&UID={HashedUserId}";
            }
            else DashboardState.SetIsDrillDown(false);

            var response = await ApiConn.FetchDashboardResponse($"{queryString}{userIdQueryString}");
            if (response == null) 
            {
                ServerMessage = "User not found";
                return;
            }
            await ExtractDataFromResponse(response);
            await FindCollaborators();                                                                                  // Populate the dictionary for top collaborators 

            await JsRuntime.InvokeVoidAsync("loadDashboardGraph", (object)GetEmailData(), (object)GetCalendarEventsData());
        }
        

        private async Task ExtractDataFromResponse(DashboardResponse response)
        {
            sentMail = response.SentMail;
            receivedMail = response.ReceivedMail;
            calendarEvents = response.CalendarEvents;
            NumberOfMeetings = calendarEvents != null ? calendarEvents.Count : 0;
            userEmailAddress = response.UserEmail;

            if (string.IsNullOrEmpty(response.UserFullName) == false)                                                   // if the API has populated this property then display it
                await JsRuntime.InvokeVoidAsync("setPageTitle", response.UserFullName);
        }
        private void FindEmailCollaborators()
        {
            FindSentEmailCollaborators();
            FindReceivedEmailCollaborators();
        }
        private void FindReceivedEmailCollaborators()
        {
            foreach (var email in receivedMail)
            {
                string senderFullName = GetFullNameFromFormattedString(email.From);
                AddPointsToCollaborators(senderFullName, EmailCommPoints.Points);
            }
        }
        private async Task FindAttendeesFromCalendarEvents()
        {
            string emailToFilterOut = string.IsNullOrEmpty(userEmailAddress) ? await GetEmailForProcessingUser() : userEmailAddress;        // if the view is a drilldown then the attribute will be populated
            foreach (var meeting in calendarEvents)
            {
                List<string> attendees = meeting.GetAttendeesExcludingUser(emailToFilterOut);                           // get all the attendees from the meeting except the user passed in as a parm
                foreach (var attendee in attendees)
                {
                    string fullName = GetFullNameFromFormattedString(attendee);
                    AddPointsToCollaborators(fullName, MeetingCommPoints.Points);
                }
            }
        }
        private void FindSentEmailCollaborators()
        {
            foreach (var email in sentMail)
            {
                foreach (var recipient in email.Recipients)
                {
                    string fullName = GetFullNameFromFormattedString(recipient);
                    AddPointsToCollaborators(fullName, EmailCommPoints.Points);
                }
            }
        }
        private async Task<string> GetEmailForProcessingUser()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.GetUserEmail();                                                                                 // custom extension method in GraphClaimsPrincipalExtensions
        }
    }
}
