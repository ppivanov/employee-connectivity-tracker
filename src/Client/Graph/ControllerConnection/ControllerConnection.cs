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
                catch (AccessTokenNotAvailableException exception)                                          // TODO - Find out if this is still valid
                {
                    exception.Redirect();
                }
            }
            return new DashboardResponse();
        }

        public async Task<Boolean> IsProcessingUserAnAdmin(HttpClient Http)
        {
            var adminResponse = await IsProcessingUserAuthorizedForRole(Http, UserRoles.admin);
            return adminResponse;
        }
        public async Task<Boolean> IsProcessingUserALeader(HttpClient Http)
        {
            var leaderResponse = await IsProcessingUserAuthorizedForRole(Http, UserRoles.leader);
            return leaderResponse;
        }

        private async Task<Boolean> IsProcessingUserAuthorizedForRole(HttpClient Http, UserRoles role)
        {
            var token = await GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    bool authResponse = await Http.GetFromJsonAsync<Boolean>($"api/auth/is-{role}");
                    return authResponse;
                }
                catch (AccessTokenNotAvailableException)                                          // TODO - Find out if this is still valid
                {
                    // user not logged in
                }
            }
            return false;
        }
    }
}
