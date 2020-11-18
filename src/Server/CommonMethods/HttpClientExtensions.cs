﻿using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public static class HttpClientExtensions
    {
        private static IMockableMethods defaultImplementation = new GraphMethods();
        public static IMockableMethods Implementation { private get; set; }
          = defaultImplementation;
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
    }
}
