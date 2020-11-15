using EctBlazorApp.Shared.GraphModels;
using EctBlazorApp.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public class GraphMethods : ITestableExtensionMethods
    {
        private const string baseGraphUrl = "https://graph.microsoft.com/v1.0";
        private static string ConstructGraphUrlForEvents(EctUser user)
        {
            string formattedFromDate = user.LastSignIn.ToString("yyyy-MM-dd");          // TODO - this must include the time so we don't pull in duplicate events
            string formattedToDate = DateTime.Now.ToString("yyyy-MM-dd");
            string eventsEndpoint = $"{baseGraphUrl}/users/{user.Email}/events$filter=start/datetime ge '{formattedFromDate}' " +
                $"and end/datetime lt '{formattedToDate}'&$select=subject,organizer,attendees,start,end";

            return eventsEndpoint;
        }
        private static string ConstructGraphUrlForUser(string userId)
        {
            string userEndpoint = $"{baseGraphUrl}/users/{userId}?$select=displayName,id,userPrincipalName";

            return userEndpoint;
        }
        public async Task<GraphUserResponse> GetGraphUser(HttpClient client, string userId)
        {
            string userInfoUrl = ConstructGraphUrlForUser(userId);

            var graphResponse = await client.GetAsync(userInfoUrl);
            if (!graphResponse.IsSuccessStatusCode)
                return null;

            string contentAsString = await graphResponse.Content.ReadAsStringAsync();
            GraphUserResponse graphUser = JsonConvert.DeserializeObject<GraphUserResponse>(contentAsString);

            return graphUser;
        }
        public async Task<GraphEventsResponse> GetMissingCalendarEvents(EctUser user, HttpClient client)
        {
            string eventsUrl = ConstructGraphUrlForEvents(user);
            var response = await client.GetAsync(eventsUrl);

            string contentAsString = await response.Content.ReadAsStringAsync();
            GraphEventsResponse graphEvents = JsonConvert.DeserializeObject<GraphEventsResponse>(contentAsString);

            return graphEvents;
        } // TODO - unit tests
    }
}
