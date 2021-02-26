using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;


// Source code: https://codeburst.io/schedule-cron-jobs-using-hostedservice-in-asp-net-core-e17c47ba06
namespace EctBlazorApp.Server.CronJob
{
    public class NotificationCronJob : CronJobService
    {
        public NotificationCronJob(IScheduleConfig<NotificationCronJob> config)
            : base(config.CronExpression, config.TimeZoneInfo) {  }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} CronJob is working.");
            return Task.CompletedTask;
        }
    }
}
