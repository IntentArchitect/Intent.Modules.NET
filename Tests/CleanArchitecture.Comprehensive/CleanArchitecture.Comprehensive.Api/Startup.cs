using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.Configuration;
using CleanArchitecture.Comprehensive.Api.FastEndpoints;
using CleanArchitecture.Comprehensive.Api.Filters;
using CleanArchitecture.Comprehensive.Application;
using CleanArchitecture.Comprehensive.Infrastructure;
using FastEndpoints;
using FastEndpoints.ApiExplorer;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.IO.Converters;
using Serilog;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api
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
            .AddOData(options =>
            {
                options.Filter().OrderBy().Select().SetMaxTop(200);
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
            });
            services.AddApplication(Configuration);
            services.ConfigureApplicationSecurity(Configuration);
            services.ConfigureCors(Configuration);
            services.ConfigureHealthChecks(Configuration);
            services.ConfigureProblemDetails();
            services.ConfigureApiVersioning();
            services.AddInfrastructure(Configuration);
            services.AddFastEndpoints(opt =>
            {
                opt.Assemblies = [typeof(Program).Assembly];
            });
            services.AddFastEndpointsApiExplorer();
            services.ConfigureSwagger(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultHealthChecks();
                endpoints.MapDefaultHealthChecksUI();
                endpoints.MapControllers();
                endpoints.MapFastEndpoints(c => c.Endpoints.Configurator = ep =>
                {
                    ep.PostProcessors(0, new ExceptionProcessor());
                });
            });
            app.UseSwashbuckle(Configuration);
        }
    }
}