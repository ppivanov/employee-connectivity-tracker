// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

// Code adapted from: https://github.com/microsoftgraph/msgraph-training-aspnet-core/blob/master/demo/GraphTutorial/

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EctWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                // Use OpenId authentication
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                // <AddSignInSnippet>
                // Specify this is a web app and needs auth code flow
                .AddMicrosoftIdentityWebApp(options => {
                    Configuration.Bind("AzureAd", options);

                    options.Prompt = "select_account";

                    options.Events.OnTokenValidated = async context => {
                        ITokenAcquisition tokenAcquisition = context.HttpContext.RequestServices
                            .GetRequiredService<ITokenAcquisition>();

                        GraphServiceClient graphClient = new GraphServiceClient(
                            new DelegateAuthenticationProvider(async (request) => {
                                var token = await tokenAcquisition
                                    .GetAccessTokenForUserAsync(GraphConstants.Scopes, user:context.Principal);
                                request.Headers.Authorization =
                                    new AuthenticationHeaderValue("Bearer", token);
                            })
                        );

                        // Get user information from Graph
                        User user = await graphClient.Me.Request()
                            .Select(u => new {
                                u.DisplayName,
                                u.Mail,
                                u.UserPrincipalName,
                                u.MailboxSettings
                            })
                            .GetAsync();

                        context.Principal.AddUserGraphInfo(user);

                        // Get the user's photo
                        // If the user doesn't have a photo, this throws
                        try
                        {
                            System.IO.Stream photo = await graphClient.Me
                                .Photos["48x48"]
                                .Content
                                .Request()
                                .GetAsync();

                            context.Principal.AddUserGraphPhoto(photo);
                        }
                        catch (ServiceException ex)
                        {
                            if (ex.IsMatch("ErrorItemNotFound") ||
                                ex.IsMatch("ConsumerPhotoIsNotSupported"))
                            {
                                context.Principal.AddUserGraphPhoto(null);
                            }
                            else
                            {
                                throw ex;
                            }
                        }
                    };

                    options.Events.OnAuthenticationFailed = context => {
                        string error = WebUtility.UrlEncode(context.Exception.Message);
                        context.Response
                            .Redirect($"/Home/ErrorWithMessage?message=Authentication+error&debug={error}");
                        context.HandleResponse();

                        return Task.FromResult(0);
                    };

                    options.Events.OnRemoteFailure = context => {
                        if (context.Failure is OpenIdConnectProtocolException)
                        {
                            string error = WebUtility.UrlEncode(context.Failure.Message);
                            context.Response
                                .Redirect($"/Home/ErrorWithMessage?message=Sign+in+error&debug={error}");
                            context.HandleResponse();
                        }

                        return Task.FromResult(0);
                    };
                })
                // </AddSignInSnippet>
                // Add ability to call web API (Graph)
                // and get access tokens
                .EnableTokenAcquisitionToCallDownstreamApi(options => {
                    Configuration.Bind("AzureAd", options);
                }, GraphConstants.Scopes)
                // <AddGraphClientSnippet>
                // Add a GraphServiceClient via dependency injection
                .AddMicrosoftGraph(options => {
                    options.Scopes = string.Join(' ', GraphConstants.Scopes);
                })
                // </AddGraphClientSnippet>
                // Use in-memory token cache
                // See https://github.com/AzureAD/microsoft-identity-web/wiki/token-cache-serialization
                .AddInMemoryTokenCaches();

            // Require authentication
            services.AddControllersWithViews(options =>
            {
                AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            // Add the Microsoft Identity UI pages for signin/out
            .AddMicrosoftIdentityUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Index}/{start=0}/{end=0}");            // start => start date for the dashboard -  0 => today 12:00am
                                                                                                    // end   => end date for the dashboard   -  0 => (today 12:00am + 1 day) - 1 minute === 11:59pm
            });
        }
    }
}
