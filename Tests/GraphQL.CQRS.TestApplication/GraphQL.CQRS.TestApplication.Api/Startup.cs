using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Api.Configuration;
using GraphQL.CQRS.TestApplication.Api.Filters;
using GraphQL.CQRS.TestApplication.Api.GraphQL.MutationResolvers;
using GraphQL.CQRS.TestApplication.Api.GraphQL.QueryResolvers;
using GraphQL.CQRS.TestApplication.Application;
using GraphQL.CQRS.TestApplication.Application.Customers;
using GraphQL.CQRS.TestApplication.Application.Customers.CreateCustomer;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomerById;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomers;
using GraphQL.CQRS.TestApplication.Application.Customers.UpdateCustomer;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Invoices.CreateInvoice;
using GraphQL.CQRS.TestApplication.Application.Products;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using GraphQL.CQRS.TestApplication.Infrastructure;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api
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
                    opt.Filters.Add<FluentValidationFilter>();
                });
            services.ConfigureCors();
            services.ConfigureApplicationSecurity(Configuration);
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.ConfigureSwagger(Configuration);
            services.ConfigureGraphQL();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
            app.UseSwashbuckle();
        }
    }
}