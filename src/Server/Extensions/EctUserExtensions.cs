using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Extensions
{
    public static class EctUserExtensions
    {
        public static async Task<bool> UpdateCalendarEventRecordsAsync(this EctUser user, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                GraphEventsResponse graphEvents = await client.GetMissingCalendarEvents(user);
                if (graphEvents.Value == null || graphEvents.Value.Length < 1)
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

        public static async Task<bool> UpdateReceivedMailRecordsAsync(this EctUser user, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                GraphReceivedMailResponse graphReceivedMail = await client.GetMissingReceivedMail(user);
                if (graphReceivedMail.Value == null || graphReceivedMail.Value.Length < 1)
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

        public static async Task<bool> UpdateSentMailRecordsAsync(this EctUser user, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                GraphSentMailResponse graphSentMail = await client.GetMissingSentMail(user);
                if (graphSentMail.Value == null || graphSentMail.Value.Length < 1)
                    return true;
                var sentMailList = SentMail.CastGraphSentMailToSentMail(graphSentMail.Value);
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

        public static void MakeLeader(this EctUser user, EctTeam team)
        {
            if (user.LeaderOf == null)
                user.LeaderOf = new List<EctTeam>();

            user.LeaderOf.Add(team);
        }
    }
}
