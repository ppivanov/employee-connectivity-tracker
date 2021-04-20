using EctBlazorApp.Client.Shared;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Client.Pages.Dashboards
{
    public class MyTeamDashboardClass : DashboardClass
    {
        [Inject]
        protected CustomAuthState AuthState { get; set; }
        [Inject]
        protected NavigationManager NavManager { get; set; }
        protected List<EctUser> TeamMembers { get; set; }
        protected NotificationOptionsResponse CurrentNotificationOptions { get; set; } = null;
        
        protected int EmailsSent { get; set; } = 0;
        protected int EmailsReceived { get; set; } = 0;
        protected bool IsLeader => AuthState.IsLeader;
        protected bool IsSubmitting { get; set; } = false;
        protected string LeaderNameAndEmail { get; set; } = string.Empty;
        protected override int TotalEmailsCount => EmailsSent + EmailsReceived;

        public virtual async Task JsInterop(string function, string parameter = "")                                     // Used to mock JavaScript function calls in unit tests
        {
            await JsRuntime.InvokeVoidAsync(function, parameter);
        }               


        protected override Task FindCollaborators()
        {
            foreach (var member in TeamMembers)                                                                         // Meeting collaborators added to dictionary in GetCalendarEventData() to avoid looping over the user list again
            {
                int totalEmails = member.SentEmails.Count + member.ReceivedEmails.Count;
                double pointsToAdd = totalEmails * EmailCommPoints.Points;
                AddPointsToCollaborators(member.FullName, pointsToAdd);
            }

            return Task.CompletedTask;
        }

        protected override object[][] GetCalendarEventsData()
        {
            Dictionary<string, HashSet<(DateTime, DateTime)>> eventsBySubject = new Dictionary<string, HashSet<(DateTime, DateTime)>>();        // using a the meeting subject as key and a hashset to get all unique start-end dates
            
            foreach (var member in TeamMembers)                                                                         // looping on all the members and all of their meetings
            {
                int numberOfMeetingsBefore = NumberOfMeetings;
                foreach (var calendarEvent in member.CalendarEvents)
                {
                    (DateTime, DateTime) eventDateTimeRange = (calendarEvent.Start, calendarEvent.End);
                    if (eventsBySubject.ContainsKey(calendarEvent.Subject))
                    {
                        if (eventsBySubject[calendarEvent.Subject].Contains(eventDateTimeRange) == false)               // only add if the specific meeting at the specific time has not been added
                        {
                            eventsBySubject[calendarEvent.Subject].Add(eventDateTimeRange);
                            NumberOfMeetings++;
                        }
                    }
                    else
                    {
                        eventsBySubject.Add(calendarEvent.Subject, new HashSet<(DateTime, DateTime)>() { { eventDateTimeRange } });             // if none of the meetings so far have had the subject, add a new one and initialize a set for the times
                        NumberOfMeetings++;
                    }
                }
                double pointsToAdd = (NumberOfMeetings - numberOfMeetingsBefore) * MeetingCommPoints.Points;
                AddPointsToCollaborators(member.FullName, pointsToAdd);
            }
            
            object[][] newList = new object[eventsBySubject.Count][];
            int i = 0;
            foreach (KeyValuePair<string, HashSet<(DateTime, DateTime)>> dictionaryEntry in eventsBySubject)            // loop over the dictionary and count the number of elements in the set
            {
                int totalDurationSeconds = 0;
                int totalDurationMinutes = 0;
                foreach (var dateRange in dictionaryEntry.Value)
                {
                    int singleEventDurationSeconds = GetSecondsFromDateTimeRange(dateRange.Item1, dateRange.Item2);
                    totalDurationSeconds += singleEventDurationSeconds;
                }
                SecondsInMeeting += totalDurationSeconds;
                totalDurationMinutes = GetMinutesFromSeconds(totalDurationSeconds);
                newList[i++] = new object[] { dictionaryEntry.Key, totalDurationMinutes };
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
                int totalReceivedOnDate = 0;
                int totalSentOnDate = 0;
                string tooltipDate = date.ToString("dd MMM");
                StringBuilder sentMailTooltipText = new StringBuilder($"{tooltipDate}\n");
                StringBuilder receivedMailTooltipText = new StringBuilder($"{tooltipDate}\n");

                foreach (var member in TeamMembers)
                {
                    string memberFirstName = member.FullName.Split(" ")[0];
                    int countOfSentMail = member.SentEmails.Count(sm => sm.SentAt.Date == date);                        // the number of emails for that member on that specific date
                    int countOfReceivedMail = member.ReceivedEmails.Count(sm => sm.ReceivedAt.Date == date);

                    sentMailTooltipText.Append($"{memberFirstName}: {countOfSentMail}\n");
                    receivedMailTooltipText.Append($"{memberFirstName}: {countOfReceivedMail}\n");

                    totalSentOnDate += countOfSentMail;
                    totalReceivedOnDate += countOfReceivedMail;
                }

                newList[i] = new object[] { tooltipDate, totalSentOnDate, sentMailTooltipText.ToString(), totalReceivedOnDate, receivedMailTooltipText.ToString() };
                EmailsReceived += totalReceivedOnDate;
                EmailsSent += totalSentOnDate;
            }
            return newList;
        }

        protected override async Task OnInitializedAsync()
        {
            await JsInterop("setPageTitle", "My Team");
            await CustomAuthState.GetUserPermissions(AuthState, ApiConn);                                               // only executed when needed
            
            if (IsLeader)                                                                                               // if the user is a leader only then attempt to load in the data
            {
                CurrentNotificationOptions = await ApiConn.FetchCurrentNotificationOptions();
                await FetchCommunicationPoints();
                await UpdateDashboard();
            }
            Initialized = true;
            await InvokeAsync(StateHasChanged);
        }

        protected async Task RedirectToDasboard(string userFullName)
        {
            int userId = TeamMembers.FirstOrDefault(u => u.FullName.Equals(userFullName)).Id;
            string hasedUserId = ComputeSha256Hash(userId.ToString());
            await JsRuntime.InvokeVoidAsync("open", $"/dashboard/{hasedUserId}", "_blank");                             // open a new browser tab for viewing that member
        }

        protected override async Task UpdateDashboard()
        {
            ResetAttributeValues();
            string queryString = GetDateRangeQueryString(FromDate.Value, ToDate.Value);

            var response = await ApiConn.FetchTeamDashboardResponse(queryString);
            TeamMembers = response.TeamMembers;
            LeaderNameAndEmail = response.LeaderNameAndEmail;

            await FindCollaborators();
            Initialized = true;
            await InvokeAsync(StateHasChanged);                                                                                                 // Force a refresh of the component before trying to load the js graphs
            await JsInterop("setPageTitle", response.TeamName);
            await JsRuntime.InvokeVoidAsync("loadMyTeamDashboardGraph", (object)GetEmailData(), (object)GetCalendarEventsData());               // GetCalendarEventsData is adding only some of the collaborators to the dictionary
        }


        private void ResetAttributeValues()
        {
            NumberOfMeetings = 0;
            SecondsInMeeting = 0;
            CollaboratorsDict.Clear();

            TeamMembers = null;
            EmailsSent = 0;
            EmailsReceived = 0;
            LeaderNameAndEmail = string.Empty;
        }
    }
}
