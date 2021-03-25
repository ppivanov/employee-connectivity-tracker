using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;

namespace EctBlazorApp.Server.Extensions
{
    public static class EctTeamExtensions
    {
        public static bool AreTeamNamesEqual(this EctTeam left, EctTeam right)
        {
            return left.Name.ToLower().Equals(right.Name.ToLower());
        }

        public static void SetNotificationOptions(this EctTeam team, NotificationOptionsResponse notificationOptions)
        {
            team.PointsThreshold = notificationOptions.PointsThreshold;
            team.MarginForNotification = notificationOptions.MarginForNotification;
            team.AdditionalUsersToNotify = notificationOptions.UsersToNotify;
        }
    }
}
