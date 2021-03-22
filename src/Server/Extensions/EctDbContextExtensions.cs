using EctBlazorApp.Server.MailKit;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

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
            EctUser user = dbContext.Users.FirstOrDefault(u => u.Email.Equals(email));
            if (user == null) 
                return false;

            bool isLeader = dbContext.Teams.Any(t => t.LeaderId == user.Id);

            return isLeader;

        }

        public static EctTeamRequestDetails IsLeaderForTeamId(this EctDbContext dbContext, string email, string hashedTeamId)
        {
            EctUser user = dbContext.Users.FirstOrDefault(u => u.Email.Equals(email));
            if (user == null)
                return null;

            EctTeam team = dbContext.Teams.AsEnumerable().FirstOrDefault(t => ComputeSha256Hash(t.Id.ToString()).Equals(hashedTeamId));
            if (team == null || team.LeaderId != user.Id)
                return null;

            team.Members = dbContext.Users.Where(u => u.MemberOfId == team.Id).ToList();
            team.Leader = user;
            EctTeamRequestDetails teamDetails = new(team);

            return teamDetails;
        }

        public delegate Task<string> GetPreferredUserName();
        public static async Task<EctUser> GetUserFromHashOrProcessingUser(this EctDbContext dbContext, string hashedUserId, GetPreferredUserName getPreferredUsername)
        {
            string userEmail = await getPreferredUsername.Invoke();
            EctUser user;
            if (string.IsNullOrEmpty(hashedUserId))
            {
                user = dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
                return user;
            }

            user = dbContext.GetUserIdIfEmailIsTeamLead(userEmail, hashedUserId);
            return user;
        }

        public static EctUser GetUserIdIfEmailIsTeamLead(this EctDbContext dbContext, string email, string hashedUserId)
        {
            EctUser teamLead = dbContext.Users.Include(u => u.LeaderOf).FirstOrDefault(u => u.Email.Equals(email));
            EctUser userForHashedId = null;
            foreach (var team in teamLead.LeaderOf)                                                                                         // As of now leaders are assigned a single team and this loop runs only once
            {
                try
                {
                    var members = dbContext.Users.Where(u => u.MemberOfId == team.Id).ToList();                                             // Get all the users for that team.
                    userForHashedId = members.FirstOrDefault(u => hashedUserId.Equals(ComputeSha256Hash(u.Id.ToString())));                          // Check if any of the members' Id matches {hashedUserId} and return the user.
                    if (userForHashedId != null) return userForHashedId;
                }
                catch (Exception)
                {
                    // It's ok the hashedId didn't match any of the members.
                }
            }

            //bool isTeamLeadForUserWithId = dbContext.Users.Include(u => u.MemberOf)                                                       // This query may be slow as it hashes every user's Id until it finds a match. T(n) = n * [ComputeSha256Hash() + ...]
            //    .Any(u => hashedUserId.Equals(ComputeSha256Hash(u.Id.ToString()))                                                         // Does the user's Id match the requested {userId}?
            //        && u.MemberOf.LeaderId == teamLead.Id);                                                                               // Are they a member of a team led by {email}?

            return null;
        }


        /******************** Email notifications ********************/

        // this method should be triggered every Sunday at noon ----- See Startup.cs to configure the cron expression
        public static void ProcessNotifications(this EctDbContext dbContext, EctMailKit mailKit)
        {                                                                                       //                              Examples:
            DateTime currentWeekEnd = DateTime.Now.AddDays(1);                                  // Following Monday             March 1st
            DateTime currentWeekStart = currentWeekEnd.AddDays(-7);                             // Last week's Monday           Feb  22th

            DateTime pastWeekEnd = currentWeekStart;                                        // Last week's Monday           Feb  22th
            DateTime pastWeekStart = currentWeekStart.AddDays(-7);                          // The Monday before that       Feb  15th

            var teams = dbContext.Teams.Include(t => t.Members).ToList();
            foreach (var team in teams)
            {
                EmailContents emailContents = new(){
                    TeamName = team.Name,
                    PointsThreshold = team.PointsThreshold,
                    MarginDifference = team.MarginForNotification,
                    Members = new List<NotificationMemberData>()
                };

                var members = team.Members.ToList();
                foreach (var member in members)
                {
                    NotificationMemberData memberData = new() {
                        Name = member.FullName,
                        CurrentPoints = dbContext.GetCommunicationPointsForUser(member, currentWeekStart, currentWeekEnd),
                        PastPoints = dbContext.GetCommunicationPointsForUser(member, pastWeekStart, pastWeekEnd),
                        CurrentWeek = $"{currentWeekStart:dd, MMM, yyyy} - {currentWeekEnd:dd, MMM, yyyy}",
                        PastWeek = $"{pastWeekStart:dd MMM, yyyy} - {pastWeekEnd:dd MMM, yyyy}"
                    };

                    if (IsPotentiallyIsolated(team, memberData.CurrentTotal, memberData.PastTotal))
                        emailContents.Members.Add(memberData);
                }

                SendNotificationEmail(emailContents.ToString(), team, mailKit);
            }
        }

        public static List<int> GetCommunicationPointsForUser(this EctDbContext dbContext, EctUser user, DateTime fromDate, DateTime toDate)
        {
            List<ReceivedMail> receivedMail = dbContext.GetReceivedMailInDateRangeForUserId(user.Id, fromDate, toDate);
            List<SentMail> sentMail = dbContext.GetSentMailInDateRangeForUserId(user.Id, fromDate, toDate);
            List<CalendarEvent> calendarEvents = dbContext.GetCalendarEventsInDateRangeForUserId(user.Id, fromDate, toDate);

            List<int> pointsPerDay = new();

            while(fromDate < toDate)
            {
                int receivedMailOnDate = receivedMail.Where(rm => rm.ReceivedAt.Date == fromDate.Date).ToList().Count;
                int sentMailOnDate = sentMail.Where(sm => sm.SentAt.Date == fromDate.Date).ToList().Count;
                List<CalendarEvent> eventsOnDate = calendarEvents.Where(ce => ce.Start.Date == fromDate.Date).ToList();

                int eventMinutesOnDate = CalendarEvent.GetTotalMinutesFromEvents(eventsOnDate);
                int mailCount = receivedMailOnDate + sentMailOnDate;
                int totalPointsOnDate = dbContext.CalculateTotalCommunicationPoints(mailCount, eventMinutesOnDate);

                pointsPerDay.Add(totalPointsOnDate);
                fromDate = fromDate.AddDays(1);
            }

            return pointsPerDay;
        }
        private static int CalculateTotalCommunicationPoints(this EctDbContext dbContext, int mailCount, int minutesInMeetings)
        {
            IEnumerable<CommunicationPoint> commPoints = dbContext.CommunicationPoints;
            int emailPoints = CommunicationPoint.GetCommunicationPointForMedium(commPoints, "email").Points;
            int meetingPoints = CommunicationPoint.GetCommunicationPointForMedium(commPoints, "meeting").Points;

            int totalPoints = (int)(mailCount * emailPoints + minutesInMeetings / 10.0 * meetingPoints);

            return totalPoints;
        }
        private static bool IsPotentiallyIsolated(EctTeam team, int currentWeekPoints, int previousWeekPoints)
        {
            currentWeekPoints = currentWeekPoints > 0 ? currentWeekPoints : 1;                                                                              // If there are no points set as 1 to avoid division by 0.
            previousWeekPoints = previousWeekPoints > 0 ? previousWeekPoints : 1;

            double percentDifferenceInPoints = currentWeekPoints * 100 / previousWeekPoints - 100;
            double realPercentDifference = percentDifferenceInPoints * -1;

            if (currentWeekPoints <= team.PointsThreshold 
                || (percentDifferenceInPoints < 0 && realPercentDifference >= team.MarginForNotification))
                return true;

            return false;
        }
        
        
        private static void SendNotificationEmail(string messageContent, EctTeam team, EctMailKit mailKit)                                  // Not a DbContext extension method. Move to a more suitable place.
        {
            foreach (var recipient in team.AdditionalUsersToNotify)
            {
                mailKit.SendNotificationEmail(team.Name, recipient, messageContent);
            }
        }
    }
}
