using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EctBlazorApp.Client.Models;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json;

namespace EctBlazorApp.Client.Graph
{
    public class MicrosoftCalendarEventsProvider : ICalendarEventsProvider
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpClient _httpClient;

        public MicrosoftCalendarEventsProvider(IAccessTokenProvider accessTokenProvider, HttpClient httpClient)
        {
            _accessTokenProvider = accessTokenProvider;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CalendarEvent>> GetEventsInDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var accessToken = await GetAccessTokenAsync();
            if (accessToken == null)
                return null;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            var response = await _httpClient.GetAsync(ConstructGraphUrlForEvents(fromDate, toDate));

            if (!response.IsSuccessStatusCode)
                return null;

            var contentAsString = await response.Content.ReadAsStringAsync();
            var microsoftEvents = JsonConvert.DeserializeObject<GraphEventsResponse>(contentAsString);

            var events = CastMicrosoftGraphEventsToCalendarEvents(microsoftEvents.Value);

            return events;
        }

        public async Task<string> GetCalendarEventsForEmail(string userEmail)
        {
            var accessToken = await GetAccessTokenAsync();
            if (accessToken == null)
                return "Token missing";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetAPITokenAsync());
            var response = await _httpClient.GetAsync($"api/calendar/get-calendar-events-api?graphToken={accessToken}&userId={userEmail}");

            if (!response.IsSuccessStatusCode)
                return "Request unsuccessful";

            var contentAsString = await response.Content.ReadAsStringAsync();
            return contentAsString;
        }

        public async Task<string> UpdateDatabaseRecords(string userEmail)
        {
            var accessToken = await GetAccessTokenAsync();
            if (accessToken == null)
                return "Token missing";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetAPITokenAsync());
            var response = await _httpClient.GetAsync($"api/main/update-records?graphToken={accessToken}&userId={userEmail}");


            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var tokenRequest = await _accessTokenProvider.RequestAccessToken(new AccessTokenRequestOptions
            {
                Scopes = new[]
                {
                    "https://graph.microsoft.com/User.Read",
                    "https://graph.microsoft.com/Calendars.Read",
                    // "https://graph.microsoft.com/CallRecords.Read.All",
                    "https://graph.microsoft.com/Chat.Read.All",
                    "https://graph.microsoft.com/Mail.Read"
                }
            });

            if (tokenRequest.TryGetToken(out var token))
            {
                if (token != null)
                    return token.Value;
            }
            return null;
        }
        private async Task<string> GetAPITokenAsync()
        {
            var tokenRequest = await _accessTokenProvider.RequestAccessToken(new AccessTokenRequestOptions
            {
                Scopes = new[]
                {
                    "api://5f468f03-5a1f-4571-9e1e-9606014e5728/API.Access"
                }
            });

            if (tokenRequest.TryGetToken(out var token))
            {
                if (token != null)
                    return token.Value;
            }
            return null;
        }

        private string ConstructGraphUrlForEvents(DateTime fromDate, DateTime toDate)
        {
            string formattedFromDate = fromDate.ToString("yyyy-MM-dd");
            string formattedToDate = toDate.ToString("yyyy-MM-dd");
            string graphUrl = $"https://graph.microsoft.com/v1.0/me/events?$filter=start/datetime ge '{formattedFromDate}' " +
                $"and end/datetime lt '{formattedToDate}'&$select=subject,organizer,attendees,start,end";
            return graphUrl;
        }

        private List<CalendarEvent> CastMicrosoftGraphEventsToCalendarEvents(MicrosoftGraphEvent[] graphEvents)
        {
            const int attendeesLimit = 20;
            var events = new List<CalendarEvent>();

            foreach (var graphEvent in graphEvents)
            {
                CalendarEvent calendarEvent = new CalendarEvent
                {
                    Subject = graphEvent.Subject,
                    Start = graphEvent.Start.ConvertToLocalDateTime(),
                    End = graphEvent.End.ConvertToLocalDateTime(),
                    Organizer = graphEvent.Organizer.ToString(),
                    Attendees = new List<string>()
                };
                // TODO -> find a solution or remove the limit
                for (int i = 0; i < Math.Min(attendeesLimit, graphEvent.Attendees.Length); i++)         // if the graphEvent is a large group meeting,
                    calendarEvent.Attendees.Add(graphEvent.Attendees[i].ToString());                    // then perhaps we don't want to waste the resourses 
                                                                                                        // transferring them over to the database
                events.Add(calendarEvent);
            }
            return events;
        }
    }
}
