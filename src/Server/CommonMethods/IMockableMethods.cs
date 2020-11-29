using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public interface IMockableGraphMethods
    {
        Task<GraphUserResponse> GetGraphUser(HttpClient client, string userId);

        Task<GraphEventsResponse> GetMissingCalendarEvents(HttpClient client, EctUser user);

        Task<GraphReceivedMailResponse> GetMissingReceivedMail(HttpClient client, EctUser user);

        Task<GraphSentMailResponse> GetMissingSentMail(HttpClient client, EctUser user);
    }

    public interface IMockableMisc
    {
        Task<string> GetPreferredUsername(HttpContext controllerContext);
    }
}
