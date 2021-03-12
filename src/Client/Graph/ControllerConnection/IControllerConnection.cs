using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Graph
{
    public interface IControllerConnection
    {
        Task<IEnumerable<EctUser>> FetchAdminstrators();

        Task<NotificationOptionsResponse> FetchCurrentNotificationOptions();

        Task<(CommunicationPoint, CommunicationPoint)> FetchCommunicationPoints();
        
        Task<DashboardResponse> FetchDashboardResponse(string queryString);

        Task<TeamDashboardResponse> FetchTeamDashboardResponse(string queryString);

        Task<IEnumerable<EctTeam>> FetchAllTeams();

        Task<string> GetAPITokenAsync();

        Task<string> GetHashedTeamId();

        Task<IEnumerable<string>> GetUsersEligibleForMembers();

        Task<bool> IsProcessingUserAnAdmin();

        Task<bool> IsProcessingUserALeader();

        Task<EctTeamRequestDetails> IsProcessingUserLeaderForTeam(string hashedTeamId);

        Task<(bool, string)> SubmitMoveMemberTeams(IEnumerable<EctTeam> teams);

        Task<(bool, string)> SubmitNotificationOptions(NotificationOptionsResponse notificationOptions);

        Task<(bool, string)> SubmitPoints(IEnumerable<CommunicationPoint> communicationPoints);

        Task<(bool, string)> SubmitTeamData(bool isNewTeam, EctTeamRequestDetails teamDetails);

        Task<string> UpdateDatabaseRecords();
    }
}
