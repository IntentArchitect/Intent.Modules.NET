using System;
using CleanArchitecture.Comprehensive.Api.Filters;
using CleanArchitecture.Comprehensive.Api.HangfireJobs;
using Hangfire;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.Hangfire.HangfireConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.Configuration
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
            services.AddHangfireServer();
            return services;
        }

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
        {
            var dashboardOptions = new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthFilter()]
            };

            app.UseHangfireDashboard("/mycustomhangfire", dashboardOptions);
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<CommandJob>("CommandJob", j => j.ExecuteAsync(), "* * * * *");
            app.ApplicationServices.GetService<IBackgroundJobClient>().Schedule<Delayed1>(j => j.ExecuteAsync(), TimeSpan.FromMinutes(5));
            app.ApplicationServices.GetService<IBackgroundJobClient>().Enqueue<FireAndForget>(j => j.ExecuteAsync());
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<Recurring1>("Recurring1", "priority", j => j.ExecuteAsync(), "* * * * *");
            return app;
        }
    }
}