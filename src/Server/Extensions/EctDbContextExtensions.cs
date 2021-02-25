using EctBlazorApp.Server.MailKit;
using EctBlazorApp.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        public static async Task<EctUser> GetExistingEctUserOrNewAsync(this EctDbContext dbContext, string userId, HttpClient client, EctMailKit mailKit)
        {
            try
            {
                EctUser userForUserIdParm = dbContext.Users.First(user => user.Email.Equals(userId));
                return userForUserIdParm;
            }
            catch (Exception)
            {
                EctUser addUserResult = await dbContext.AddUser(userId, client);
                mailKit.SendWelcome(addUserResult);

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
            bool isAdmin = dbContext.Administrators.Any(admin => admin.User.Email.Equals(email));

            return isAdmin;
        }

        public static bool IsEmailForLeader(this EctDbContext dbContext, string email)
        {
            try
            {
                EctUser user = dbContext.Users.First(u => u.Email.Equals(email));
                bool isLeader = dbContext.Teams.Any(t => t.LeaderId == user.Id);
                
                return isLeader;
            }
            catch (Exception)
            {
                return false;
            }

        }

        // this should be triggered every Sunday at noon for most accurate results
        public static void ProcessNotifications(this EctDbContext dbContext, EctMailKit mailKit)
        {                                                                                       //                              Examples:
            DateTime currentWeekEnd = DateTime.Now.AddDays(1);                                  // Following Monday             March 1st
            DateTime currentWeekStart = currentWeekEnd.AddDays(-7);                             // Last week's Monday           Feb  22th

            DateTime previousWeekEnd = currentWeekStart;                                        // Last week's Monday           Feb  22th
            DateTime previousWeekStart = currentWeekStart.AddDays(-7);                          // The Monday before that       Feb  15th

            var heading = $"Stats for week starting: {currentWeekStart.ToString("dd dddd, MMM yyyy")},\n" +
                $"compared to week starting: {previousWeekStart.ToString("dd dddd, MMM yyyy")}\n\nPotentially isolated members:\n\n";
            var emailMessage = new StringBuilder(heading);

            var teams = dbContext.Teams.Include(t => t.Members).ToList();
            foreach (var team in teams)
            {
                var members = team.Members.ToList();
                foreach (var member in members)
                {
                    int currentWeekPoints = dbContext.GetCommunicationPointsForUserId(member.Id, currentWeekStart, currentWeekEnd);
                    int previousWeekPoints = dbContext.GetCommunicationPointsForUserId(member.Id, previousWeekStart, previousWeekEnd);
                    currentWeekPoints = currentWeekPoints > 0 ? currentWeekPoints : 1;
                    previousWeekPoints = previousWeekPoints > 0 ? previousWeekPoints : 1;

                    string message = GenerateNotificationMessage(team, member, currentWeekPoints, previousWeekPoints);
                    emailMessage.Append(message);
                }
                if (emailMessage.ToString().Equals(heading)) emailMessage.Append("None\n");

                dbContext.SendNotificationEmail(emailMessage.ToString(), team, mailKit);
            }
        }

        private static int GetCommunicationPointsForUserId(this EctDbContext dbContext, int userId, DateTime fromDate, DateTime toDate)
        {
            List<ReceivedMail> receivedMail = dbContext.GetReceivedMailInDateRangeForUserId(userId, fromDate, toDate);
            List<SentMail> sentMail = dbContext.GetSentMailInDateRangeForUserId(userId, fromDate, toDate);
            List<CalendarEvent> calendarEvents = dbContext.GetCalendarEventsInDateRangeForUserId(userId, fromDate, toDate);

            int mailCount = receivedMail.Count + sentMail.Count;
            int minutesInMeetings = CalendarEvent.GetTotalMinutesFromEvents(calendarEvents);

            int totalCommunicationPoints = dbContext.CalculateTotalCommunicationPoints(mailCount, minutesInMeetings);
            return totalCommunicationPoints;
        }
        private static int CalculateTotalCommunicationPoints(this EctDbContext dbContext, int mailCount, int minutesInMeetings)
        {
            IEnumerable<CommunicationPoint> commPoints = dbContext.CommunicationPoints;
            int emailPoints = CommunicationPoint.GetCommunicationPointForMedium(commPoints, "email").Points;
            int meetingPoints = CommunicationPoint.GetCommunicationPointForMedium(commPoints, "meeting").Points;

            int totalPoints = (int)(mailCount * emailPoints + minutesInMeetings / 10.0 * meetingPoints);

            return totalPoints;
        }
        private static void SendNotificationEmail(this EctDbContext dbContext, string messageContent, EctTeam team, EctMailKit mailKit)
        {
            var teamLead = dbContext.Users.First(u => u.Id.Equals(team.LeaderId));
            mailKit.SendNotificationEmail(teamLead, messageContent);
            // send email to additional recipients if specified
        }
        private static string GenerateNotificationMessage(EctTeam team, EctUser user, int currentWeekPoints, int previousWeekPoints)
        {
            var emailMessage = new StringBuilder("");
            if (currentWeekPoints < team.PointsThreshold)
                emailMessage.Append($"{user.FullName} is below the threshold" +
                    $" set for their team. {currentWeekPoints}/{team.PointsThreshold}\n");

            float percentDifferenceInPoints = currentWeekPoints / previousWeekPoints * 100 - 100;
            float realPercentDifference = percentDifferenceInPoints * -1;
            if (percentDifferenceInPoints < 0 && realPercentDifference >= team.MarginForNotification)
                emailMessage.Append($"{user.FullName} has been communicating {realPercentDifference}% less this week compared to last week.\n");

            if (emailMessage.Length > 0) emailMessage.Append("\n");

            return emailMessage.ToString();
        }
    }
}
