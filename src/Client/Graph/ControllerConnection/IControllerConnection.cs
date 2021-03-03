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

        Task<string> GetAPITokenAsync();

        Task<IEnumerable<string>> GetUsersEligibleForMembers();

        Task<Boolean> IsProcessingUserAnAdmin();

        Task<Boolean> IsProcessingUserALeader();

        Task<(bool, string)> SubmitNotificationOptions(NotificationOptionsResponse notificationOptions);

        Task<(bool, string)> SubmitPoints(IEnumerable<CommunicationPoint> communicationPoints);

        Task<(bool, string)> SubmitTeamData(EctTeamRequestDetails teamDetails);

        Task<string> UpdateDatabaseRecords();
    }
}
