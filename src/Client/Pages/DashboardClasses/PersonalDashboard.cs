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
using static EctBlazorApp.Shared.SharedCommonMethods;

namespace EctBlazorApp.Client.Pages.DashboardClasses
{
    public class PersonalDashboardClass : DashboardClass
    {
        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private List<SentMail> sentMail;
        private List<ReceivedMail> receivedMail;
        private List<CalendarEvent> calendarEvents;
        private CommunicationPoint emailCommPoints;
        private CommunicationPoint meetingCommPoints;

        private int TotalMinutesInMeetings
        {
            get
            {
                return GetMinutesFromSeconds(secondsInMeeting);
            }
        }
        private int TotalPoints
        {
            get
            {
                double totalMeetingPoints = TotalMinutesInMeetings / 10.0 * meetingCommPoints.Points;
                int totalEmailPoints = (EmailsSentCount + EmailsReceivedCount) * emailCommPoints.Points;
                return (int)(totalEmailPoints + totalMeetingPoints);
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
        protected string GetFormattedTimeInMeeting
        {
            get
            {
                return FormatSecondsToHoursAndMinutes(secondsInMeeting);
            }
        }
        protected int numberOfMeetings = 0;
        protected double secondsInMeeting = 0;
        protected double totalWeight = 0;
        protected readonly Dictionary<string, double> collaboratorsDict = new Dictionary<string, double>();
        protected IEnumerable<KeyValuePair<string, double>> collaborators
        {
            get
            {
                /* Adapted from: https://stackoverflow.com/a/298 */
                List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
                foreach (var key in collaboratorsDict.Keys)
                    list.Add(new KeyValuePair<string, double>(key, collaboratorsDict[key] / TotalPoints * 100));

                list.Sort(
                    (KeyValuePair<string, double> pair1, KeyValuePair<string, double> pair2) => 
                    {
                        return pair2.Value.CompareTo(pair1.Value);                              // descending order
                    }
                );
                return list.Take(10);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Dashboard");
            await FetchCommunicationPoints();
            await UpdateDashboard();
        }

        private async Task FetchCommunicationPoints()
        {
            var token = await ApiConn.GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var CommunicationPoints = await Http.GetFromJsonAsync<List<CommunicationPoint>>($"api/communication/points");
                    emailCommPoints = CommunicationPoint.GetCommunicationPointForMedium(CommunicationPoints, "email");
                    meetingCommPoints = CommunicationPoint.GetCommunicationPointForMedium(CommunicationPoints, "meeting");

                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
        }
        private async Task<string> GetProcessingUserEmail()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.GetUserEmail();
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

            object[][] newList = new object[subjectAndCount.Count + 1][];
            newList[0] = new object[] { "Event subject", "Number of events" };
            int i = 1;
            foreach (KeyValuePair<string, int> dictionaryEnty in subjectAndCount)
            {
                newList[i] = new object[] { dictionaryEnty.Key, dictionaryEnty.Value.ToString() };
                i++;
            }

            return newList;
        }
        protected override object[][] GetSentAndReceivedEmailData()
        {
            var dates = SplitDateRangeToChunks();
            object[][] newList = new object[dates.Count + 1][];

            newList[0] = new object[] { "Date", "Sent Emails", "Received Emails" };
            for (int i = 1; i <= dates.Count; i++)
            {
                int index = i - 1;                                                                      // We add a row to the array that contains the value descriptions, but we still need the first date at position 0
                DateTime date = dates[index];
                int countOfSentMail = sentMail.Count(sm => sm.SentAt.Date == date);
                int countOfReceivedMail = receivedMail.Count(rm => rm.ReceivedAt.Date == date);

                newList[i] = new object[] { date.ToString("dd MMM"), countOfSentMail, countOfReceivedMail };
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
                    var response = await Http.GetFromJsonAsync<DashboardResponse>($"api/main/get-dashboard-stats{queryString}");
                    sentMail = response.SentMail;
                    receivedMail = response.ReceivedMail;
                    calendarEvents = response.CalendarEvents;
                    secondsInMeeting = response.SecondsInMeeting;
                    numberOfMeetings = calendarEvents.Count;
                    await GetCollaborators();

                    await JsRuntime.InvokeVoidAsync("loadDashboardGraph", (object)GetSentAndReceivedEmailData(), (object)GetCalendarEventsData());
                }
                catch (AccessTokenNotAvailableException exception)                                          // TODO - Find out if this is still valid
                {
                    exception.Redirect();
                }
            }
        }

        private async Task GetCollaborators()
        {
            collaboratorsDict.Clear();
            GetEmailCollaborators();
            await GetAttendeesFromCalendarEvents();
        }
        private void GetEmailCollaborators()
        {
            GetSentEmailCollaborators();
            GetReceivedEmailCollaborators();
        }
        private void GetSentEmailCollaborators()
        {
            foreach (var email in sentMail)
            {
                foreach (var recipient in email.Recipients)
                {
                    string fullName = GetFullNameFromFormattedString(recipient);
                    AddPointsToCollaborator(fullName, emailCommPoints.Points);
                }
            }
        }
        private void GetReceivedEmailCollaborators()
        {
            foreach (var email in receivedMail)
            {
                string senderFullName = GetFullNameFromFormattedString(email.From);
                AddPointsToCollaborator(senderFullName, emailCommPoints.Points);
            }
        }
        private async Task GetAttendeesFromCalendarEvents()
        {
            foreach (var meeting in calendarEvents)
            {
                List<string> attendees = meeting.GetAttendeesExcludingUser(await GetProcessingUserEmail());
                foreach (var attendee in attendees)
                {
                    string fullName = GetFullNameFromFormattedString(attendee);
                    AddPointsToCollaborator(fullName, meetingCommPoints.Points);
                }
            }
        }

        private void AddPointsToCollaborator(string fullName, double points)
        {
            if (collaboratorsDict.ContainsKey(fullName))
                collaboratorsDict[fullName] += points;
            else
                collaboratorsDict.Add(fullName, points);
        }
    }
}
