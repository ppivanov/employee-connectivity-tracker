using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Behaviour
{
    public static class UserBehaviour
    {
        public static Task<EctUser> GetExistingEctUserOrNewWrapperAsync(string userId, HttpClient client, EctDbContext dbContext)
        {
            return GetExistingEctUserOrNewAsync(userId, client, dbContext);
        }
        private static async Task<EctUser> GetExistingEctUserOrNewAsync(string userId, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                EctUser userForUserIdParm = dbContext.Users.First(user => user.Email.Equals(userId));
                return userForUserIdParm;
            }
            catch (Exception)
            {
                EctUser addUserResult = await dbContext.AddUser(userId, client);
                return addUserResult;
            }
        }

        public static Task<bool> UpdateCalendarEventRecordsWrapperAsync(this EctUser user, HttpClient client, EctDbContext dbContext)
        {
            return UpdateCalendarEventRecordsAsync(user, client, dbContext);
        }
        private static async Task<bool> UpdateCalendarEventRecordsAsync(EctUser user, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                GraphEventsResponse graphEvents = await client.GetMissingCalendarEvents(user);
                var calendarEvents = CalendarEvent.CastGraphEventsToCalendarEvents(graphEvents.Value);
                if (calendarEvents.Count < 1)
                    return false;

                if (user.CalendarEvents == null) user.CalendarEvents = new List<CalendarEvent>();
                foreach (var calendarEvent in calendarEvents)
                {
                    user.CalendarEvents.Add(calendarEvent);
                }
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
