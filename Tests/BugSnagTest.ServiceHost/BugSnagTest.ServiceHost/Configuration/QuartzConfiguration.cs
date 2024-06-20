using System;
using System.Collections.Generic;
using BugSnagTest.ServiceHost.Jobs;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.Matchers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.QuartzConfiguration", Version = "1.0")]

namespace BugSnagTest.ServiceHost.Configuration
{
    public static class QuartzConfiguration
    {
        public static IServiceCollection ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<BugSnagQuartzJobListener>();
            services
                .AddQuartz(q =>
                {
                    if (configuration.GetValue<bool?>($"Quartz:Jobs:ScheduledJob:Enabled") ?? true)
                    {
                        q.ScheduleJob<ScheduledJob>(trigger =>
                        {
                            trigger.WithCronSchedule(configuration.GetValue<string?>($"Quartz:Jobs:ScheduledJob:CronSchedule") ?? "*/10 * * * * ?");
                            trigger.WithIdentity("ScheduledJob");
                        });
                    }
                    q.AddJobListener(sp => sp.GetRequiredService<BugSnagQuartzJobListener>(), GroupMatcher<JobKey>.AnyGroup());
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