using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public interface IMockableMethods
    {
        Task<GraphUserResponse> GetGraphUser(HttpClient client, string userId);

        Task<GraphEventsResponse> GetMissingCalendarEvents(HttpClient client, EctUser user);
        Task<GraphReceivedMailResponse> GetMissingReceivedMail(HttpClient client, EctUser user);
    }
}
