using EctBlazorApp.Client.Graph;
using EctBlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient(
                "EctBlazorApp.ServerAPI", 
                client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("EctBlazorApp.ServerAPI"));

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("api://5f468f03-5a1f-4571-9e1e-9606014e5728/API.Access");
                options.ProviderOptions.AdditionalScopesToConsent.Add("https://graph.microsoft.com/User.Read");
                options.ProviderOptions.AdditionalScopesToConsent.Add("https://graph.microsoft.com/Calendars.Read");
                options.ProviderOptions.AdditionalScopesToConsent.Add("https://graph.microsoft.com/Mail.Read");
            });

            builder.Services.AddScoped<IControllerConnection, ControllerConnection>();

            builder.Services.AddSingleton<CustomAuthState>();
            builder.Services.AddSingleton<DashboardState>();

            await builder.Build().RunAsync();
        }
    }
}
