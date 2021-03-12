using EctBlazorApp.Shared.Entities;

namespace EctBlazorApp.Server.Extensions
{
    public static class EctTeamExtensions
    {
        public static bool AreTeamNamesEqual(this EctTeam left, EctTeam right)
        {
            return left.Name.ToLower().Equals(right.Name.ToLower());
        }
    }
}
