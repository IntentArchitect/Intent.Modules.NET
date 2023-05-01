using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Api.Configuration;
using GraphQL.CQRS.TestApplication.Api.Filters;
using GraphQL.CQRS.TestApplication.Application;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using GraphQL.CQRS.TestApplication.Infrastructure;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
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
            services
                .AddGraphQLServer()
                //.AddQueryType<CustomerQueries>()
                .AddQueryType<Query>()
                .AddTypeExtension<CustomerQueries>()
                //.AddType<InvoiceQueries>()
                .AddMutationType<CustomerMutations>()
                //.AddSubscriptionType(d => d.Name("Subscription"))
                //.AddAuthorizeDirectiveType()
                .BindRuntimeType<string, StringType>()
                .BindRuntimeType<Guid, IdType>();
            ;
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
            app.UseCors();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
            app.UseSwashbuckle();
        }
    }

    public class Query
    {
    }

    //[ExtendObjectType(typeof(Query))]
    public class CustomerMutations
    {
        public async Task<Customer> CreateCustomer(CustomerInput input, [Service] ICustomerRepository customerRepository)
        {
            var customer = new Customer()
            {
                Name = input.Name,
                Surname = input.Surname,
                Email = input.Email,
            };
            customerRepository.Add(customer);
            await customerRepository.UnitOfWork.SaveChangesAsync();
            return customer;
        }
    }

    public class CustomerInput
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }

    [ExtendObjectType(typeof(Query))]
    public class CustomerQueries
    {
        public async Task<IReadOnlyList<Customer>> GetCustomers(
            CancellationToken cancellationToken,
            [Service] ICustomerRepository customerRepository) =>
            await customerRepository.FindAllAsync(cancellationToken);
    }

    public class InvoiceQueries
    {
        public async Task<IReadOnlyList<Invoice>> GetInvoices(
            CancellationToken cancellationToken,
            [Service] IInvoiceRepository invoiceRepository) =>
            await invoiceRepository.FindAllAsync(cancellationToken);
    }
}