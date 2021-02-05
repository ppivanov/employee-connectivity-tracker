using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Shared.Entities;
using EctBlazorApp.Shared.GraphModels;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Extensions
{
    public static class HttpClientExtensions
    {
        private static IMockableGraphMethods defaultImplementation = new GraphMethods();
        public static IMockableGraphMethods Implementation { private get; set; } = defaultImplementation;
        public static void RevertToDefaultImplementation()
        {
            Implementation = defaultImplementation;
        }

        public static Task<GraphUserResponse> GetGraphUser(this HttpClient client, string userId)
        {
            return Implementation.GetGraphUser(client, userId);
        }

        public static Task<GraphEventsResponse> GetMissingCalendarEvents(this HttpClient client, EctUser user)
        {
            return Implementation.GetMissingCalendarEvents(client, user);
        }

        public static Task<GraphReceivedMailResponse> GetMissingReceivedMail(this HttpClient client, EctUser user)
        {
            return Implementation.GetMissingReceivedMail(client, user);
        }

        public static Task<GraphSentMailResponse> GetMissingSentMail(this HttpClient client, EctUser user)
        {
            return Implementation.GetMissingSentMail(client, user);
        }
    }

    public static class HttpContextExtensions
    {
        private static IMockableMisc defaultImplementation = new Miscellaneous();
        public static IMockableMisc Implementation { private get; set; } = defaultImplementation;
        public static void RevertToDefaultImplementation()
        {
            Implementation = defaultImplementation;
        }

        public static Task<string> GetPreferredUsername(this HttpContext httpContext)
        {
            return Implementation.GetPreferredUsername(httpContext);
        }
    }
}
