using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Graph
{
    public enum UserRoles
    {
        admin,
        leader
    }

    public class ControllerConnection : IControllerConnection
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpClient _httpClient;

        public ControllerConnection() { }

        public ControllerConnection(IAccessTokenProvider accessTokenProvider, HttpClient httpClient)
        {
            _accessTokenProvider = accessTokenProvider;
            _httpClient = httpClient;
        }

        public async Task TestNotifications()
        {
            var token = await GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    await _httpClient.GetFromJsonAsync<List<CommunicationPoint>>($"api/team/test-notifications");
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
        }
        public async Task<string> UpdateDatabaseRecords()
        {
            var accessToken = await GetAccessTokenAsync();
            if (accessToken == null)
                return "Token missing";

            var userDetails = new GraphUserRequestDetails
            {
                GraphToken = accessToken
            };

            var json = JsonConvert.SerializeObject(userDetails);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetAPITokenAsync());
            var response = await _httpClient.PutAsync($"api/main/update-tracking-records", data);

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
        public async Task<string> GetAPITokenAsync()
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

        public async Task<(CommunicationPoint, CommunicationPoint)> FetchCommunicationPoints()
        {
            var token = await GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var CommunicationPoints = await _httpClient.GetFromJsonAsync<List<CommunicationPoint>>($"api/communication/points");
                    var emailCommPoints = CommunicationPoint.GetCommunicationPointForMedium(CommunicationPoints, "email");
                    var meetingCommPoints = CommunicationPoint.GetCommunicationPointForMedium(CommunicationPoints, "meeting");

                    return (emailCommPoints, meetingCommPoints);
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
            return (new CommunicationPoint(), new CommunicationPoint());
        }

        public async Task<DashboardResponse> FetchDashboardResponse(string queryString)
        {
            var token = await GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await _httpClient.GetFromJsonAsync<DashboardResponse>($"api/main/get-dashboard-stats{queryString}");
                    return response;
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
            return new DashboardResponse();
        }

        public async Task<Boolean> IsProcessingUserAnAdmin()
        {
            var adminResponse = await IsProcessingUserAuthorizedForRole(UserRoles.admin);
            return adminResponse;
        }
        public async Task<Boolean> IsProcessingUserALeader()
        {
            var leaderResponse = await IsProcessingUserAuthorizedForRole(UserRoles.leader);
            return leaderResponse;
        }

        private async Task<Boolean> IsProcessingUserAuthorizedForRole(UserRoles role)
        {
            var token = await GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    bool authResponse = await _httpClient.GetFromJsonAsync<Boolean>($"api/auth/is-{role}");
                    return authResponse;
                }
                catch (AccessTokenNotAvailableException)                                          // TODO - Find out if this is still valid
                {
                    // user not logged in
                }
            }
            return false;
        }

        public async Task<(bool, string)> SubmitPoints(List<CommunicationPoint> communicationPoints)
        {
            var token = await GetAPITokenAsync();
            const string tokenErrorMessage = "Error retrieving access token. Please, try again later.";
            if (token == null)
                return (true, tokenErrorMessage);

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PutAsJsonAsync<List<CommunicationPoint>>($"api/communication/points/update", communicationPoints);
                var serverMessage = await response.Content.ReadAsStringAsync();
                var isError = false;
                if (response.IsSuccessStatusCode == false)
                    isError = true;

                return (isError, serverMessage);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            return (true, tokenErrorMessage);
        }

        public async Task<(bool, string)> SubmitPointsThreshold(NotificationOptionsResponse notificationOptions)
        {
            var token = await GetAPITokenAsync();
            const string tokenErrorMessage = "Error retrieving access token. Please, try again later.";
            if (token == null)
                return (true, tokenErrorMessage);

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PutAsJsonAsync<NotificationOptionsResponse>($"api/team/set-notification-options", notificationOptions);
                var serverMessage = await response.Content.ReadAsStringAsync();
                var isError = false;
                if (response.IsSuccessStatusCode == false)
                    isError = true;

                return (isError, serverMessage);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            return (true, tokenErrorMessage);
        }
        
        public async Task<NotificationOptionsResponse> FetchCurrentNotificationOptions()
        {
            var errorResponse = new NotificationOptionsResponse
            {
                PointsThreshold = -1,
                MarginForNotification = -1
            };
            var token = await GetAPITokenAsync();
            if (token == null)
                return errorResponse;

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetFromJsonAsync<NotificationOptionsResponse>($"api/team/get-notification-options");
                return response;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            return errorResponse;                                                                                                          // This return should never be executed
        }
    }
}
