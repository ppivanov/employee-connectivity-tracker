using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Client.Graph
{
    public interface IControllerConnection
    {
        Task<string> UpdateDatabaseRecords();

        Task<string> GetAPITokenAsync();

        Task<Boolean> IsAdmin(HttpClient Http);
    }
}
