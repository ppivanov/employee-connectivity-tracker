using EctBlazorApp.Server.GraphModels;
using EctBlazorApp.Shared;
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
        public async static Task<bool> UpdateCalendarEventRecordsForUser(EctUser user, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                string eventsUrl = ConstructGraphUrlForEvents(user);
                var response = await client.GetAsync(eventsUrl);

                string contentAsString = await response.Content.ReadAsStringAsync();
                var graphEvents = JsonConvert.DeserializeObject<GraphEventsResponse>(contentAsString);

                int attendeesLimit = 20;

                foreach (var graphEvent in graphEvents.Value)
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
                    //for (int i = 0; i < Math.Min(attendeesLimit, graphEvent.Attendees.Length); i++)         // if the graphEvent is a large group meeting,
                    //    calendarEvent.Attendees.Add(graphEvent.Attendees[i].ToString());                    // then perhaps we don't want to waste the resourses 
                    //                                                                                        // transferring them over to the database
                    calendarEvent.Attendees.AddRange(
                        graphEvent.Attendees.Take(attendeesLimit)
                            .Select(a => a.ToString()));
                    ; dbContext.CalendarEvents.Add(calendarEvent);
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            // Save to DB
            return false;
        }


        // TODO REFACTOR
        public static async Task<EctUser> AddUser(string userId, HttpClient client, EctDbContext dbContext)
        {
            string userInfoUrl = ConstructGraphUrlForUser(userId);

            var graphResponse = await client.GetAsync(userInfoUrl);
            if (!graphResponse.IsSuccessStatusCode)
                return null;

            string contentAsString = await graphResponse.Content.ReadAsStringAsync();
            var graphUser = JsonConvert.DeserializeObject<GraphUserResponse>(contentAsString);
            try
            {
                EctUser newUser = new EctUser(graphUser);
                dbContext.Users.Add(newUser);
                await dbContext.SaveChangesAsync();
                return newUser;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string ConstructGraphUrlForUser(string userId)
        {
            string userEndpoint = $"{baseGraphUrl}/users/{userId}?$select=displayName,id,userPrincipalName";

            return userEndpoint;
        }

        private static string ConstructGraphUrlForEvents(EctUser user)
        {
            string formattedFromDate = user.LastSignIn.ToString("yyyy-MM-dd");
            string formattedToDate = DateTime.Now.ToString("yyyy-MM-dd");
            string eventsEndpoint = $"{baseGraphUrl}/users/{user.Email}/events$filter=start/datetime ge '{formattedFromDate}' " +
                $"and end/datetime lt '{formattedToDate}'&$select=subject,organizer,attendees,start,end";

            return eventsEndpoint;
        }
    }
}
