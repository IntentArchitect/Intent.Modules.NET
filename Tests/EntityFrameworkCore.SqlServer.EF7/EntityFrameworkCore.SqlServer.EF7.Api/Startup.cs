using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Api.Configuration;
using EntityFrameworkCore.SqlServer.EF7.Api.Filters;
using EntityFrameworkCore.SqlServer.EF7.Application;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.IO.Converters;
using Serilog;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Api
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
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
            });
            services.AddApplication(Configuration);
            services.ConfigureApplicationSecurity(Configuration);
            services.ConfigureProblemDetails();
            services.AddInfrastructure(Configuration);
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}