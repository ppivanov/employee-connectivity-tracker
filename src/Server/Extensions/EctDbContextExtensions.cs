using EctBlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Extensions
{
    public static class EctDbContextExtensions
    {
        public static List<CalendarEvent> GetCalendarEventsInDateRangeForUserId(this EctDbContext dbContext, int userId, DateTime fromDate, DateTime toDate)
        {
            var calendarEvents =
                     dbContext.CalendarEvents.Where(c =>
                        c.EctUserId == userId
                        && c.Start >= fromDate
                        && c.End < toDate).ToList();
            return calendarEvents;
        }
        public static async Task<EctUser> GetExistingEctUserOrNewAsync(this EctDbContext dbContext, string userId, HttpClient client)
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
        public static List<ReceivedMail> GetReceivedMailInDateRangeForUserId(this EctDbContext dbContext, int userId, DateTime fromDate, DateTime toDate)
        {
            var receivedMail =
                dbContext.ReceivedEmails.Where(mail =>
                    mail.EctUserId == userId
                    && mail.ReceivedAt >= fromDate
                    && mail.ReceivedAt < toDate).ToList();
            return receivedMail;
        }
        public static List<SentMail> GetSentMailInDateRangeForUserId(this EctDbContext dbContext, int userId, DateTime fromDate, DateTime toDate)
        {
            var sentMail =
                dbContext.SentEmails.Where(mail =>
                    mail.EctUserId == userId
                    && mail.SentAt >= fromDate
                    && mail.SentAt < toDate).ToList();
            return sentMail;
        }

        public static bool IsEmailForAdmin(this EctDbContext dbContext, string email)
        {
            return dbContext.Administrators.Any(admin => admin.User.Email.Equals(email));
        }
    }
}
