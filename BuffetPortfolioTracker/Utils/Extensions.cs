using BuffetPortfolioTracker.BackgrounJobs;
using Quartz;
using System.Runtime.CompilerServices;
using static Quartz.Logging.OperationName;

namespace BuffetPortfolioTracker.Utils
{
    public static class Extensions
    {
        public static async Task<WebApplication> AddBackgroudJobs(this WebApplication app)
        {
            var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();

            var syncDataJob = JobBuilder.Create<SyncPortfolioJob>()
                .WithIdentity(name: "SyncPortfolioJob", group: "BackgroundJobs")
                .Build();

            var syncDataTrigger = TriggerBuilder.Create()
                .WithIdentity(name: "SyncPortfolioTrigger", group: "BackgrounJobTriggers")
                .WithSimpleSchedule(o => o
                    .RepeatForever()
                    .WithRepeatCount(3)
                    .WithIntervalInHours(12))
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(syncDataJob, syncDataTrigger);

            //await scheduler.Start();

            return app;
        }
    }
}
