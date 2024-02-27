using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using WindowsServiceHost.Tests.Jobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.QuartzConfiguation", Version = "1.0")]

namespace WindowsServiceHost.Tests.Configuration
{
    public static class QuartzConfiguration
    {
        public static IServiceCollection ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddQuartz(q =>
                {
                    if (configuration.GetValue<bool?>($"Quartz:Jobs:DispatchJob:Enabled") ?? true)
                    {
                        q.ScheduleJob<DispatchJob>(trigger =>
                        {
                            trigger.WithCronSchedule(configuration.GetValue<string?>($"Quartz:Jobs:DispatchJob:CronSchedule") ?? "*/15 * * * * ?");
                            trigger.WithIdentity("DispatchJob");
                        });
                    }

                    if (configuration.GetValue<bool?>($"Quartz:Jobs:RunEvery15Seconds:Enabled") ?? true)
                    {
                        q.ScheduleJob<RunEvery15Seconds>(trigger =>
                        {
                            trigger.WithCronSchedule(configuration.GetValue<string?>($"Quartz:Jobs:RunEvery15Seconds:CronSchedule") ?? "*/15 * * * * ?");
                            trigger.WithIdentity("RunEvery15Seconds");
                        });
                    }
                });

            services
                .AddQuartzHostedService(options =>
                {
                    options.WaitForJobsToComplete = true;
                }); ;

            return services;
        }
    }
}