using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Server.MailKit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;


// Source code: https://codeburst.io/schedule-cron-jobs-using-hostedservice-in-asp-net-core-e17c47ba06
namespace EctBlazorApp.Server.CronJob
{
    public class NotificationCronJob : CronJobService
    {
        private readonly IServiceProvider _serviceProvider;
        public NotificationCronJob(IScheduleConfig<NotificationCronJob> config, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo) 
        {
            _serviceProvider = serviceProvider;
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EctDbContext>();
            var mailKit = scope.ServiceProvider.GetRequiredService<EctMailKit>();

            dbContext.ProcessNotifications(mailKit);
            return Task.CompletedTask;
        }
    }
}
