using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EctBlazorApp.Shared.GraphModels;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

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

            var events = CalendarEvent.CastGraphEventsToCalendarEvents(microsoftEvents.Value);

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

            var userDetails = new GraphUserRequestDetails
            {
                UserId = userEmail,
                GraphToken = accessToken
            };

            using var client = new HttpClient();

            var json = JsonConvert.SerializeObject(userDetails);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetAPITokenAsync());
            var response = await _httpClient.PutAsync($"api/main/update-records", data);

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
    }
}
