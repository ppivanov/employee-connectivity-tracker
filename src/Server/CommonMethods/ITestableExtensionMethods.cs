using EctBlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public interface ITestableExtensionMethods
    {
        Task<GraphUserResponse> GetGraphUser(HttpClient client, string userId);
    }
}
