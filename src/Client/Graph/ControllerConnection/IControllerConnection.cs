using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Graph
{
    public interface IControllerConnection
    {
        Task<string> UpdateDatabaseRecords();

        Task<string> GetAPITokenAsync();

        Task<Boolean> IsProcessingUserAnAdmin();

        Task<Boolean> IsProcessingUserALeader();

        Task<(CommunicationPoint, CommunicationPoint)> FetchCommunicationPoints();
        
        Task<DashboardResponse> FetchDashboardResponse(string queryString);

        Task<(bool, string)> SubmitPoints(List<CommunicationPoint> communicationPoints);
    }
}
