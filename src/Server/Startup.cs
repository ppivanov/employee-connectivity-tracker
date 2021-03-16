using EctBlazorApp.Server.CronJob;
using EctBlazorApp.Server.MailKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;

namespace EctBlazorApp.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        cors =>
                        {
                            cors.WithOrigins("*")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                });

            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(
                    options =>
                    {
                        options.Authority = Configuration["JwtAuthority"];
                        options.Audience = Configuration["JwtAudience"];
                    });

            services.AddDbContext<EctDbContext>(options =>
            {
                options.UseSqlServer(Configuration["DefaultDbConnection"]);
            });

            services.AddSingleton(
                Configuration.GetSection("MailKitMetadata").Get<EctMailKit>()
            );

            services.AddCronJob<NotificationCronJob>(c =>
            {
                //c.CronExpression = @"0 12 * * Sun";                           // run job every Sunday at 12:00pm
                c.CronExpression = @"0 12 * * *";                               // run job every day at 12:00pm
                c.TimeZoneInfo = TimeZoneInfo.Local;
            });

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
