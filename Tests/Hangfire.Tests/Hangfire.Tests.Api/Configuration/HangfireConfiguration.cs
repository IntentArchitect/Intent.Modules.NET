using System;
using Hangfire;
using Hangfire.Tests.Api.Filters;
using Hangfire.Tests.Api.HangfireJobs;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.Hangfire.HangfireConfiguration", Version = "1.0")]

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

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
        {
            var dashboardOptions = new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthFilter()]
            };

            app.UseHangfireDashboard("/hfjobs", dashboardOptions);
            app.ApplicationServices.GetService<IBackgroundJobClient>().Schedule<Delayed>(j => j.ExecuteAsync(), TimeSpan.FromMinutes(5));
            app.ApplicationServices.GetService<IBackgroundJobClient>().Enqueue<FireAndForget>(j => j.ExecuteAsync());
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<PriorityRecurring>("PriorityRecurring", "priority", j => j.ExecuteAsync(), "* * * * *");
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<PublishCommandJob>("PublishCommandJob", j => j.ExecuteAsync(), "* * * * *");
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<Recurring>("Recurring", j => j.ExecuteAsync(), "* * * * *");
            return app;
        }
    }
}