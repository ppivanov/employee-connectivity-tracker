using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Graph
{
    public class ControllerConnection : IControllerConnection
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpClient _httpClient;

        public ControllerConnection(IAccessTokenProvider accessTokenProvider, HttpClient httpClient)
        {
            _accessTokenProvider = accessTokenProvider;
            _httpClient = httpClient;
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
            using var client = new HttpClient();

            var accessToken = await GetAccessTokenAsync();
            if (accessToken == null)
                return "Token missing";

            var userDetails = new GraphUserRequestDetails
            {
                UserId = userEmail,
                GraphToken = accessToken
            };

            var json = JsonConvert.SerializeObject(userDetails);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetAPITokenAsync());
            var response = await _httpClient.PutAsync($"api/main/update-tracking-records", data);

            // Perhaps loop a few times if unsuccessful??
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
    }
}
