using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public interface ITestableExtensionMethods
    {
        Task<GraphUserResponse> GetGraphUser(HttpClient client, string userId);

        Task<GraphEventsResponse> GetMissingCalendarEvents(HttpClient client, EctUser user);
    }
}
