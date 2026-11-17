using Intent.RoslynWeaver.Attributes;
using Quartz;
using TestMediator.Quartz.Api.Jobs.Maintenance;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.QuartzConfiguration", Version = "1.0")]

namespace TestMediator.Quartz.Api.Configuration
{
    public static class QuartzConfiguration
    {
        public static IServiceCollection ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddQuartz(q =>
                {
                    if (configuration.GetValue<bool?>($"Quartz:Jobs:PurgeExpiredSessionsJob:Enabled") ?? true)
                    {
                        q.ScheduleJob<PurgeExpiredSessionsJob>(trigger =>
                        {
                            trigger.WithCronSchedule(configuration.GetValue<string?>($"Quartz:Jobs:PurgeExpiredSessionsJob:CronSchedule") ?? "0 0 * * * ?");
                            trigger.WithIdentity("PurgeExpiredSessionsJob");
                        });
                    }
                });

            services
                .AddQuartzHostedService(options =>
                {
                    options.WaitForJobsToComplete = true;
                });

            return services;
        }
    }
}