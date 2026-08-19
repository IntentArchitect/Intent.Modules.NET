using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using WebAndWorker.QuartzWorker.Jobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.QuartzConfiguration", Version = "1.0")]

namespace WebAndWorker.QuartzWorker.Configuration
{
    public static class QuartzConfiguration
    {
        public static IServiceCollection ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddQuartz(q =>
                {
                    if (configuration.GetValue<bool?>($"Quartz:Jobs:TestJob:Enabled") ?? true)
                    {
                        q.ScheduleJob<TestJob>(trigger =>
                        {
                            trigger.WithCronSchedule(configuration.GetValue<string?>($"Quartz:Jobs:TestJob:CronSchedule") ?? "*/5 * * * * ?");
                            trigger.WithIdentity("TestJob");
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