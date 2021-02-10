using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
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
        private List<SentMail> sentMail;
        private List<ReceivedMail> receivedMail;
        private List<CalendarEvent> calendarEvents;
        private List<CommunicationPercentage> communicationPercentages;
        protected Dictionary<string, int> emailCollaborators = new Dictionary<string, int>();

        protected int emailsSent
        {
            get
            {
                return sentMail != null ? sentMail.Count : 0;
            }
        }
        protected int emailsReceived
        {
            get
            {
                return receivedMail != null ? receivedMail.Count : 0;
            }
        }
        protected int numberOfMeetings = 0;
        protected double secondsInMeeting = 0;

        protected string GetFormattedTimeInMeeting
        {
            get
            {
                return FormatSecondsToHoursAndMinutes(secondsInMeeting);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("setPageTitle", "Dashboard");
            await UpdateDashboard();
            //await GetCommunicationPercentages();
        }

        private void AddToDictionary(Dictionary<string, int> dictionary, string userName)
        {
            if (dictionary.ContainsKey(userName) == false) 
                dictionary[userName] = 0;
            dictionary[userName]++;
        }

        private async Task GetCommunicationPercentages()
        {
            var token = await ApiConn.GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    communicationPercentages = await Http.GetFromJsonAsync<List<CommunicationPercentage>>($"api/communication/weights");
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
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
            GetSentEmailCollaborators();
            GetReceivedEmailCollaborators();
        }
        private void GetSentEmailCollaborators()
        {
            foreach (var email in sentMail)
            {
                string[] recipients = email.RecipientsAsString.Split(" | ");
                foreach (var recipient in recipients)
                {
                    string fullName = GetFullNameFromFormattedString(recipient);
                    AddToDictionary(emailCollaborators, fullName);
                }
            }
        }
        private void GetReceivedEmailCollaborators()
        {
            foreach (var email in receivedMail)
            {
                string senderFullName = GetFullNameFromFormattedString(email.From);
                AddToDictionary(emailCollaborators, senderFullName);
            }
        }
    }
}
