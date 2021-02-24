using EctBlazorApp.Shared.Entities;
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
        // should be triggered every Sunday at noon
        public static void ProcessNotifications(this EctTeam team, EctDbContext dbContext)
        {                                                                                       //                              Examples:
            DateTime currentWeekEnd = DateTime.Now.AddDays(1);                                  // Following Monday             March 1st
            DateTime currentWeekStart = currentWeekEnd.AddDays(-7);                             // Last week's Monday           Feb  22th

            DateTime previousWeekEnd = currentWeekStart;                                        // Last week's Monday           Feb  22th
            DateTime previousWeekStart = currentWeekStart.AddDays(-7);                          // The Monday before that       Feb  15th

            var emailMessage = new StringBuilder("");

            foreach (var member in team.Members)
            {
                int currentWeekPoints = GetCommunicationPointsForUserId(member.Id, currentWeekStart, currentWeekEnd, dbContext);
                int previousWeekPoints = GetCommunicationPointsForUserId(member.Id, previousWeekStart, previousWeekEnd, dbContext);

                string message = team.ProcessPoints(member, currentWeekPoints, previousWeekPoints);
            }

            // send email to leader && additional recipients if specified
        }

        private static int GetCommunicationPointsForUserId(int userId, DateTime fromDate, DateTime toDate, EctDbContext dbContext)
        {
            List<ReceivedMail> receivedMail = dbContext.GetReceivedMailInDateRangeForUserId(userId, fromDate, toDate);
            List<SentMail> sentMail = dbContext.GetSentMailInDateRangeForUserId(userId, fromDate, toDate);
            List<CalendarEvent> calendarEvents = dbContext.GetCalendarEventsInDateRangeForUserId(userId, fromDate, toDate);

            int mailCount = receivedMail.Count + sentMail.Count;
            int minutesInMeetings = GetMinutesFromEvents(calendarEvents);

            int totalCommunicationPoints = GetCommunicationPoints(mailCount, minutesInMeetings, dbContext);
            return totalCommunicationPoints;
        }

        private static int GetMinutesFromEvents(List<CalendarEvent> calendarEvents)
        {
            double totalSeconds = CalendarEvent.GetTotalSecondsForEvents(calendarEvents);
            int totalMinutes = GetMinutesFromSeconds(totalSeconds);
            return totalMinutes;
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
            {
                emailMessage.Append($"{user.FullName} is below the threshold that is" +
                    " set for their team. ${currentWeekPoints}/{team.PointsThreshold}\n");
            }

            // Check the % difference in the users current and past data

            return emailMessage.ToString();
        }
    }
}
