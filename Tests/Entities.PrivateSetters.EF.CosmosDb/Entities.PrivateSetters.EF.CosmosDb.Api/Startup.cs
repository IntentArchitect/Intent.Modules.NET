using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.PrivateSetters.EF.CosmosDb.Api.Configuration;
using Entities.PrivateSetters.EF.CosmosDb.Api.Filters;
using Entities.PrivateSetters.EF.CosmosDb.Api.StartupJobs;
using Entities.PrivateSetters.EF.CosmosDb.Application;
using Entities.PrivateSetters.EF.CosmosDb.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Api
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
                });
            services.AddApplication();
            services.ConfigureProblemDetails();
            services.AddInfrastructure(Configuration);
            services.ConfigureSwagger(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwashbuckle(Configuration);

            if (Configuration.GetValue<bool>("Cosmos:EnsureDbCreated"))
            {
                app.EnsureDbCreationAsync().GetAwaiter().GetResult();
            }
        }
    }
}