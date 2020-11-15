using EctBlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public static class HttpClientExtensions
    {
        private static ITestableExtensionMethods defaultImplementation = new GraphMethods();
        public static ITestableExtensionMethods Implementation { private get; set; }
          = defaultImplementation;
        public static void RevertToDefaultImplementation()
        {
            Implementation = defaultImplementation;
        }

        public static Task<GraphUserResponse> GetGraphUser(this HttpClient client, string userId)
        {
            return Implementation.GetGraphUser(client, userId);
        }
    }
}
