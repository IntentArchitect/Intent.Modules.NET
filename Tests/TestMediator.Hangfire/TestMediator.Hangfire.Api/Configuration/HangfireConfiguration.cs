using Hangfire;
using Intent.RoslynWeaver.Attributes;
using TestMediator.Hangfire.Api.Filters;
using TestMediator.Hangfire.Api.HangfireJobs.Maintenance;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Hangfire.HangfireConfiguration", Version = "1.0")]

namespace TestMediator.Hangfire.Api.Configuration
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
                opt.Queues = ["default"];
            });
            return services;
        }

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
        {
            var dashboardOptions = new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthFilter()]
            };

            app.UseHangfireDashboard("/hangfire", dashboardOptions);
            app.ApplicationServices.GetService<IRecurringJobManager>().AddOrUpdate<PurgeExpiredSessionsJob>("PurgeExpiredSessionsJob", j => j.ExecuteAsync(), configuration.GetValue<string?>($"Hangfire:Jobs:PurgeExpiredSessionsJob:CronSchedule") ?? "* * * * *");
            return app;
        }
    }
}