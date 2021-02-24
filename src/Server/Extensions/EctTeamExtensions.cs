using EctBlazorApp.Server.MailKit;
using EctBlazorApp.Shared.Entities;
using System.Linq;
using System.Text;

namespace EctBlazorApp.Server.Extensions
{
    public static class EctTeamExtensions
    {        
        public static string ProcessPoints(this EctTeam team, EctUser user, int currentWeekPoints, int previousWeekPoints)
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

        public static void SendNotificationEmail(this EctTeam team, string messageContent, EctMailKit mailKit, EctDbContext dbContext)
        {
            var teamLead = dbContext.Users.First(u => u.Id.Equals(team.LeaderId));
            mailKit.SendNotificationEmail(teamLead, messageContent);
            // send email to additional recipients if specified
        }
    }
}
