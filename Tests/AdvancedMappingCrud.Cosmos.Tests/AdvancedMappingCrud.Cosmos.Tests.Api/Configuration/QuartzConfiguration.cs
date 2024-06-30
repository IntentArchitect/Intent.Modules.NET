using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Api.Jobs;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.QuartzConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Api.Configuration
{
    public static class QuartzConfiguration
    {
        public static IServiceCollection ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddQuartz(q =>
                {
                    if (configuration.GetValue<bool?>($"Quartz:Jobs:CommandDelegateJob:Enabled") ?? true)
                    {
                        q.ScheduleJob<CommandDelegateJob>(trigger =>
                        {
                            trigger.WithCronSchedule(configuration.GetValue<string?>($"Quartz:Jobs:CommandDelegateJob:CronSchedule") ?? "* * * * * ?");
                            trigger.WithIdentity("CommandDelegateJob");
                        });
                    }

                    if (configuration.GetValue<bool?>($"Quartz:Jobs:NewScheduledJob:Enabled") ?? true)
                    {
                        q.ScheduleJob<NewScheduledJob>(trigger =>
                        {
                            trigger.WithCronSchedule(configuration.GetValue<string?>($"Quartz:Jobs:NewScheduledJob:CronSchedule") ?? "* * * * * ?");
                            trigger.WithIdentity("NewScheduledJob");
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