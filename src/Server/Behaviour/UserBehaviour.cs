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
        public static Task<bool> UpdateCalendarEventRecordsWrapperAsync(this EctUser user, HttpClient client, EctDbContext dbContext)
        {
            return UpdateCalendarEventRecordsAsync(user, client, dbContext);
        }
        public static Task<bool> UpdateReceivedMailRecordsWrapperAsync(this EctUser user, HttpClient client, EctDbContext dbContext)
        {
            return UpdateReceivedMailRecordsAsync(user, client, dbContext);
        }
        public static Task<bool> UpdateSentMailRecordsWrapperAsync(this EctUser user, HttpClient client, EctDbContext dbContext)
        {
            return UpdateSentMailRecordsAsync(user, client, dbContext);
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
        
        private static async Task<bool> UpdateCalendarEventRecordsAsync(EctUser user, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                GraphEventsResponse graphEvents = await client.GetMissingCalendarEvents(user);
                if (graphEvents.Value.Length < 1)
                    return true;
                var calendarEvents = CalendarEvent.CastGraphEventsToCalendarEvents(graphEvents.Value);
                if (calendarEvents.Count < 1)
                    return false;

                if (user.CalendarEvents == null) user.CalendarEvents = new List<CalendarEvent>();
                foreach (var calendarEvent in calendarEvents)
                    user.CalendarEvents.Add(calendarEvent);
                
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
       
        private static async Task<bool> UpdateReceivedMailRecordsAsync(EctUser user, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                GraphReceivedMailResponse graphReceivedMail = await client.GetMissingReceivedMail(user);
                if (graphReceivedMail.Value.Length < 1)
                    return true;
                var receivedMailList = ReceivedMail.CastGraphReceivedMailToReceivedMail(graphReceivedMail.Value);
                if (receivedMailList.Count < 1)
                    return false;

                if (user.ReceivedEmails == null) user.ReceivedEmails = new List<ReceivedMail>();
                foreach (var receivedMail in receivedMailList)
                    user.ReceivedEmails.Add(receivedMail);

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static async Task<bool> UpdateSentMailRecordsAsync(EctUser user, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                GraphSentMailResponse graphReceivedMail = await client.GetMissingSentMail(user);
                if (graphReceivedMail.Value.Length < 1)
                    return true;
                var sentMailList = SentMail.CastGraphSentMailToSentMail(graphReceivedMail.Value);
                if (sentMailList.Count < 1)
                    return false;

                if (user.SentEmails == null) user.SentEmails = new List<SentMail>();
                foreach (var sentMail in sentMailList)
                    user.SentEmails.Add(sentMail);

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
