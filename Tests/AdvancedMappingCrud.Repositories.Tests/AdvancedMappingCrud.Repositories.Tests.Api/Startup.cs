using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Configuration;
using AdvancedMappingCrud.Repositories.Tests.Api.Filters;
using AdvancedMappingCrud.Repositories.Tests.Application;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OData.SimpleKey;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.ModelBuilder;
using Serilog;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api
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
                options.Filter().OrderBy().Expand().SetMaxTop(200);
                var odataBuilder = new ODataConventionModelBuilder();
                odataBuilder.EntitySet<ODataCustomer>("ODataCustomers");
                odataBuilder.EntitySet<ODataOrder>("ODataOrders");
                odataBuilder.EntitySet<ODataOrderLine>("ODataOrderLines");
                odataBuilder.EntitySet<ODataProduct>("ODataProducts");
                odataBuilder.EntitySet<ODataCustomer>("ODataCustomers").EntityType.Ignore(m => m.DomainEvents);
                odataBuilder.EntitySet<ODataProduct>("ODataProducts").EntityType.Ignore(m => m.DomainEvents);
                options.Select()
                    .Expand()
                    .Filter()
                    .OrderBy()
                    .SetMaxTop(100)
                    .Count()
                    .AddRouteComponents("odata", odataBuilder.GetEdmModel());
            });
            services.AddApplication(Configuration);
            services.ConfigureApplicationSecurity(Configuration);
            services.ConfigureHealthChecks(Configuration);
            services.ConfigureProblemDetails();
            services.ConfigureApiVersioning();
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
            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultHealthChecks();
                endpoints.MapControllers();
            });
            app.UseSwashbuckle(Configuration);
        }
    }
}