using Hangfire;
using Hangfire.Tests.Api.Filters;
using Hangfire.Tests.Api.HangfireJobs;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Hangfire.HangfireConfiguration", Version = "1.0")]

namespace Hangfire.Tests.Api.Configuration
{
    public static class HangfireConfiguration
    {
        public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(cfg =>
            {
                cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
                cfg.UseSimpleAssemblyNameTypeSerializer();
                cfg.UseRecommendedSerializerSettings();
                cfg.UseInMemoryStorage();
            });
            services.AddHangfireServer(opt =>
            {
                opt.Queues = ["priority", "default"];
            });
            return services;
        }

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
        {
            var dashboardOptions = new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthFilter()]
            };

            app.UseHangfireDashboard("/hfjobs", dashboardOptions);
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<PriorityRecurring>("PriorityRecurring", "priority", j => j.ExecuteAsync(), configuration.GetValue<string?>($"Hangfire:Jobs:PriorityRecurring:CronSchedule") ?? "* * * * *");
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<PublishCommandJob>("PublishCommandJob", j => j.ExecuteAsync(), configuration.GetValue<string?>($"Hangfire:Jobs:PublishCommandJob:CronSchedule") ?? "* * * * *");
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<Recurring>("Recurring", j => j.ExecuteAsync(), configuration.GetValue<string?>($"Hangfire:Jobs:Recurring:CronSchedule") ?? "* * * * *");
            return app;
        }
    }
}