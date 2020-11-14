using EctBlazorApp.Server.Controllers;
using EctBlazorApp.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public static class GraphMethods
    {
        private const string baseGraphUrl = "https://graph.microsoft.com/v1.0";
        public static bool UpdateCalendarEventRecordsForUser(string accessToken, string userId, ILogger<HomeController> logger)
        {
            // construct graph URL for events
            // HTTP request
            // Parse results
            // Save to DB
            return false;
        }


        // TODO REFACTOR
        public static async Task<bool> AddUser(string accessToken, string userId, ILogger<HomeController> logger, EctDbContext dbContext)
        {
            string userInfoUrl = ConstructGraphUrlForUser(userId);
            using (var client = new HttpClient())
            {
                var graphResponse = await client.GetAsync(userInfoUrl);
                if (!graphResponse.IsSuccessStatusCode)
                {
                    logger.LogError($"Unsuccessfull request to: {userInfoUrl}");
                    return false;
                }
                string contentAsString = await graphResponse.Content.ReadAsStringAsync();
                var graphUser = JsonConvert.DeserializeObject<GraphUserResponse>(contentAsString);

                EctUser newUser = new EctUser(graphUser);
                dbContext.Users.Add(newUser);
                await dbContext.SaveChangesAsync();
            }
            return false;
        }

        private static string ConstructGraphUrlForUser(string userId)
        {
            StringBuilder url = new StringBuilder(baseGraphUrl);
            string userEndpoint = $"/users/{userId}?$select=displayName,id,userPrincipalName";

            url.Append(userEndpoint);
            return url.ToString();
        }

        private static string ConstructGraphUrlForEvents(string userId)
        {
            StringBuilder url = new StringBuilder(baseGraphUrl);
            string eventsEndpoint = $"/users/{userId}/events";

            url.Append(eventsEndpoint);
            return url.ToString();
        }
    }
}
