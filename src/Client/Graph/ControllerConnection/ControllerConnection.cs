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
using System.Text.Json;
using System.Threading;
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

        public Task<IEnumerable<EctUser>> FetchAdminstrators()
        {
            return HttpGet<IEnumerable<EctUser>>("api/auth/administrators", new List<EctUser>());
        }

        public Task<NotificationOptionsResponse> FetchCurrentNotificationOptions()
        {
            var errorResponse = new NotificationOptionsResponse
            {
                PointsThreshold = -1,
                MarginForNotification = -1
            };
            return HttpGet<NotificationOptionsResponse>("api/team/notification-options", errorResponse);
        }

        public async Task<(CommunicationPoint, CommunicationPoint)> FetchCommunicationPoints()
        {
            var defaultResponse = new List<CommunicationPoint>
            {
                new CommunicationPoint { Medium = "email", Points = 0 },
                new CommunicationPoint { Medium = "meeting", Points = 0 }
            };
            var response = await HttpGet<IEnumerable<CommunicationPoint>>("api/communication/points", defaultResponse);
            var emailCommPoints = CommunicationPoint.GetCommunicationPointForMedium(response, "email");
            var meetingCommPoints = CommunicationPoint.GetCommunicationPointForMedium(response, "meeting");

            return (emailCommPoints, meetingCommPoints);
        }

        public Task<DashboardResponse> FetchDashboardResponse(string queryString)
        {
            return HttpGet<DashboardResponse>($"api/main/dashboard-stats{queryString}", null);
        }

        public Task<TeamDashboardResponse> FetchTeamDashboardResponse(string queryString)
        {
            var defaultResponse = new TeamDashboardResponse { TeamMembers = new List<EctUser>(), TeamName = string.Empty };
            return HttpGet<TeamDashboardResponse>($"api/team/team-stats{queryString}", defaultResponse);
        }

        public Task<IEnumerable<EctTeamRequestDetails>> FetchAllTeams()
        {
            return HttpGet<IEnumerable<EctTeamRequestDetails>>("api/team", new List<EctTeamRequestDetails>());
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

            if (tokenRequest.TryGetToken(out var token) && token != null)
                    return token.Value;
            
            return null;
        }

        public Task<string> GetHashedTeamId()
        {
            return HttpGet<string>("api/team/team-id", string.Empty);
        }

        public Task<IEnumerable<string>> GetUsersEligibleForMembers()
        {
            return HttpGet<IEnumerable<string>>("api/auth/app-users", new List<string>());
        }

        public Task<bool> IsProcessingUserAnAdmin()
        {
            return IsProcessingUserAuthorizedForRole(UserRoles.admin);
        }

        public Task<bool> IsProcessingUserALeader()
        {
            return IsProcessingUserAuthorizedForRole(UserRoles.leader);
        }

        public Task<EctTeamRequestDetails> IsProcessingUserLeaderForTeam(string hashedTeamId)
        {
            return HttpGet<EctTeamRequestDetails>($"api/auth/is-leader-for-team?TID={hashedTeamId}", null);
        }

        public Task<(bool, string)> SubmitMoveMemberTeams(IEnumerable<EctTeamRequestDetails> teams)
        {
            return HttpPut("api/team/move-members", teams);
        }

        public Task<(bool, string)> SubmitNotificationOptions(NotificationOptionsResponse notificationOptions)
        {
            return HttpPut("api/team/notification-options", notificationOptions);
        }

        public Task<(bool, string)> SubmitPoints(IEnumerable<CommunicationPoint> communicationPoints)
        {
            return HttpPut("api/communication/points", communicationPoints);
        }

        public Task<(bool, string)> SubmitTeamData(bool isNewTeam, EctTeamRequestDetails teamDetails)
        {
            if (isNewTeam)
                return HttpPost("api/team", teamDetails);
            else
                return HttpPut("api/team", teamDetails);
        }

        public async Task<string> UpdateDatabaseRecords()
        {
            var accessToken = await GetAccessTokenAsync();
            var userDetails = new GraphUserRequestDetails
            {
                GraphToken = accessToken
            };
            var apiResponse = await HttpPut("api/main/tracking-records", userDetails, accessToken);
            
            return apiResponse.Item2;
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

            if (tokenRequest.TryGetToken(out var token) && token != null)
                    return token.Value;
            
            return null;
        }
        private async Task<T> HttpGet<T>(string endpoint, T defaultResponse)
        {
            var token = await GetAPITokenAsync();
            if (token != null)
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    T response = await _httpClient.GetFromJsonAsync<T>(endpoint);

                    return response;
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
                catch(Exception){
                    return defaultResponse;
                }
            }
            return defaultResponse;
        }
        private async Task<(bool, string)> HttpPost<T>(string endpoint, T data)
        {
            var token = await GetAPITokenAsync();
            return await HttpSendData(endpoint, data, token, _httpClient.PostAsJsonAsync);
        }
        private async Task<(bool, string)> HttpPut<T>(string endpoint, T data)
        {
            var token = await GetAPITokenAsync();
            return await HttpPut(endpoint, data, token);
        }
        private async Task<(bool, string)> HttpPut<T>(string endpoint, T data, string accessToken)
        {
            return await HttpSendData(endpoint, data, accessToken, _httpClient.PutAsJsonAsync);
        }
        private delegate Task<HttpResponseMessage> HttpSendMethod<T>(string endpoint, T data, JsonSerializerOptions options = null, CancellationToken cancellationToken = default);
        private async Task<(bool, string)> HttpSendData<T>(string endpoint, T data, string accessToken, HttpSendMethod<T> method)
        {
            const string tokenErrorMessage = "Error retrieving access token. Please, try again later.";
            if (accessToken != null)
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await method.Invoke(endpoint, data);
                    var serverMessage = await response.Content.ReadAsStringAsync();

                    return (response.IsSuccessStatusCode, serverMessage);
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    exception.Redirect();
                }
            }
            return (false, tokenErrorMessage);
        }
        private Task<bool> IsProcessingUserAuthorizedForRole(UserRoles role)
        {
            return HttpGet<bool>($"api/auth/is-{role}", false);
        }
    }
}
