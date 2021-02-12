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

        private string userEmail = "";
        private List<SentMail> sentMail;
        private List<ReceivedMail> receivedMail;
        private List<CalendarEvent> calendarEvents;
        private CommunicationPercentage emailCommPercentage;
        private CommunicationPercentage meetingCommPercentage;

        private int TotalMinutesInMeetings
        {
            get
            {
                return GetMinutesFromSeconds(secondsInMeeting);
            }
        }
        private int TotalEmailCount
        {
            get
            {
                return EmailsSentCount + EmailsReceivedCount;
            }
        }
        private double ActualEmailPercentage
        {
            get
            {
                return numberOfMeetings == 0 ? 100 : emailCommPercentage.Weight;
            }
        }
        private double ActualMeetingPercentage
        {
            get
            {
                return TotalEmailCount == 0 ? 100 : meetingCommPercentage.Weight;
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
        protected readonly Dictionary<string, double> collaborators = new Dictionary<string, double>();


        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Dashboard");
            await FetchCommunicationPercentages();
            await UpdateDashboard();
        }
        private async Task FetchCommunicationPercentages()
        {
            var token = await ApiConn.GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var communicationPercentages = await Http.GetFromJsonAsync<List<CommunicationPercentage>>($"api/communication/weights");
                    emailCommPercentage = CommunicationPercentage.GetCommunicationPercentageForMedium(communicationPercentages, "email");
                    meetingCommPercentage = CommunicationPercentage.GetCommunicationPercentageForMedium(communicationPercentages, "meeting");
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
                    GetEmailCollaborators();
                    await GetAttendeesFromCalendarEvents();

                    await JsRuntime.InvokeVoidAsync("loadDashboardGraph", (object)GetSentAndReceivedEmailData(), (object)GetCalendarEventsData());
                }
                catch (AccessTokenNotAvailableException exception)                                          // TODO - Find out if this is still valid
                {
                    exception.Redirect();
                }
            }
        }

        private void GetEmailCollaborators()
        {
            double weightForSingleMail = ActualEmailPercentage / TotalEmailCount;

            GetSentEmailCollaborators(weightForSingleMail);
            GetReceivedEmailCollaborators(weightForSingleMail);
        }
        private void GetSentEmailCollaborators(double weight)
        {
            foreach (var email in sentMail)
            {
                double splitWeight = weight / email.Recipients.Count;
                foreach (var recipient in email.Recipients)
                {
                    string fullName = GetFullNameFromFormattedString(recipient);
                    AddToCollaborators(fullName, splitWeight);
                }
            }
        }
        private void GetReceivedEmailCollaborators(double weight)
        {
            foreach (var email in receivedMail)
            {
                string senderFullName = GetFullNameFromFormattedString(email.From);
                AddToCollaborators(senderFullName, weight);
            }
        }
        private async Task GetAttendeesFromCalendarEvents()
        {
            double weightForTenMinutes = ActualMeetingPercentage / TotalMinutesInMeetings;
            foreach (var meeting in calendarEvents)
            {
                List<string> attendees = meeting.GetAttendeesExcludingUser(await GetProcessingUserEmail());
                double meetingWeight = meeting.GetEventLenghtInMinutes() * weightForTenMinutes;
                double weightPerAttendee = meetingWeight / attendees.Count;
                foreach (var attendee in attendees)
                {
                    string fullName = GetFullNameFromFormattedString(attendee);
                    AddToCollaborators(fullName, weightPerAttendee);
                }
            }
        }

        private void AddToCollaborators(string userName, double singleUnitWeight)
        {
            if (collaborators.ContainsKey(userName) == false)
                collaborators[userName] = 0;
            collaborators[userName] += singleUnitWeight;
        }
    }
}
