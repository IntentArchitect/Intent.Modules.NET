using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Subscribe.CleanArchDapr.TestApplication.Api.Configuration;
using Subscribe.CleanArchDapr.TestApplication.Api.Filters;
using Subscribe.CleanArchDapr.TestApplication.Application;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Api
{
    [IntentManaged(Mode.Merge)]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(
                opt =>
                {
                    opt.Filters.Add<ExceptionFilter>();
                })
            .AddDapr();
            services.AddApplication(Configuration);
            services.ConfigureApplicationSecurity(Configuration);
            services.ConfigureProblemDetails();
            services.AddInfrastructure(Configuration);
            services.ConfigureSwagger(Configuration);
            services.AddDaprSidekick(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCloudEvents();
            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });
            app.UseSwashbuckle(Configuration);
        }
    }
}