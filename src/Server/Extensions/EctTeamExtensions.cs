using EctBlazorApp.Server.MailKit;
using EctBlazorApp.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Server.Extensions
{
    public static class EctTeamExtensions
    {
        // this should be triggered every Sunday at noon for most accurate results
        public static void ProcessNotifications(EctMailKit mailKit, EctDbContext dbContext)
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
                    int currentWeekPoints = GetCommunicationPointsForUserId(member.Id, currentWeekStart, currentWeekEnd, dbContext);
                    int previousWeekPoints = GetCommunicationPointsForUserId(member.Id, previousWeekStart, previousWeekEnd, dbContext);
                    currentWeekPoints = currentWeekPoints > 0 ? currentWeekPoints : 1;
                    previousWeekPoints = previousWeekPoints > 0 ? previousWeekPoints : 1;

                    string message = team.ProcessPoints(member, currentWeekPoints, previousWeekPoints);
                    emailMessage.Append(message);
                }
                if (emailMessage.ToString().Equals(heading)) emailMessage.Append("None\n");

                team.SendNotificationEmail(emailMessage.ToString(), mailKit, dbContext);
            }
        }

        private static int GetCommunicationPointsForUserId(int userId, DateTime fromDate, DateTime toDate, EctDbContext dbContext)
        {
            List<ReceivedMail> receivedMail = dbContext.GetReceivedMailInDateRangeForUserId(userId, fromDate, toDate);
            List<SentMail> sentMail = dbContext.GetSentMailInDateRangeForUserId(userId, fromDate, toDate);
            List<CalendarEvent> calendarEvents = dbContext.GetCalendarEventsInDateRangeForUserId(userId, fromDate, toDate);

            int mailCount = receivedMail.Count + sentMail.Count;
            int minutesInMeetings = CalendarEvent.GetTotalMinutesFromEvents(calendarEvents);

            int totalCommunicationPoints = GetCommunicationPoints(mailCount, minutesInMeetings, dbContext);
            return totalCommunicationPoints;
        }

        private static int GetCommunicationPoints(int mailCount, int minutesInMeetings, EctDbContext dbContext)
        {
            IEnumerable<CommunicationPoint> commPoints = dbContext.CommunicationPoints;
            int emailPoints = CommunicationPoint.GetCommunicationPointForMedium(commPoints, "email").Points;
            int meetingPoints = CommunicationPoint.GetCommunicationPointForMedium(commPoints, "meeting").Points;

            int totalPoints = (int)(mailCount * emailPoints + minutesInMeetings / 10.0 * meetingPoints);

            return totalPoints;
        }
        
        private static string ProcessPoints(this EctTeam team, EctUser user, int currentWeekPoints, int previousWeekPoints)
        {
            var emailMessage = new StringBuilder("");
            if(currentWeekPoints < team.PointsThreshold)
                emailMessage.Append($"{user.FullName} is below the threshold" +
                    $" set for their team. {currentWeekPoints}/{team.PointsThreshold}\n");

            float percentDifferenceInPoints = currentWeekPoints / previousWeekPoints * 100 - 100;
            if (percentDifferenceInPoints < 0 && percentDifferenceInPoints >= team.MarginForNotification)
                emailMessage.Append($"{user.FullName} has been communicating {percentDifferenceInPoints}% less this week compared to last week.\n");

            if (emailMessage.Length > 0) emailMessage.Append("\n");

            return emailMessage.ToString();
        }

        private static void SendNotificationEmail(this EctTeam team, string messageContent, EctMailKit mailKit, EctDbContext dbContext)
        {
            var teamLead = dbContext.Users.First(u => u.Id.Equals(team.LeaderId));
            mailKit.SendNotificationEmail(teamLead, messageContent);
            // send email to additional recipients if specified
        }
    }
}
