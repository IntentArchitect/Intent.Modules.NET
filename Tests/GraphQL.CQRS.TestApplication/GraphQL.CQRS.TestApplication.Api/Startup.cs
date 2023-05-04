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
            services
                .AddGraphQLServer()
                //.AddQueryType<CustomerQueries>()
                //.AddQueryType<Query>()
                .AddQueryType(x => x.Name("Query"))
                .AddTypeExtension<Query>()
                //.AddTypeExtension<CustomerQueries>()
                //.AddTypeExtension<ProductQueries>()
                //.AddTypeExtension<InvoiceQueries>()
                //.AddType<InvoiceQueries>()
                .AddMutationType(x => x.Name("Mutation"))
                .AddTypeExtension<Mutation>()
                //.AddTypeExtension<CustomerMutations>()
                //.AddTypeExtension<ProductMutations>()
                //.AddTypeExtension<InvoiceMutations>()
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

    //[ExtendObjectType(typeof(Query))]
    [ExtendObjectType(Name = "Mutation")]
    public class CustomerMutations
    {
        public async Task<Guid> CreateCustomer(CreateCustomerCommand input, CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            var result = await mediator.Send(input, cancellationToken);
            return result;
        }

        public async Task<UpdateCustomerCommand> UpdateCustomer(UpdateCustomerCommand input, CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            await mediator.Send(input, cancellationToken);
            return input;
        }
    }

    [ExtendObjectType(Name = "Mutation")]
    public class ProductMutations
    {
        public async Task<Guid> CreateProduct(ProductCreateDto input, CancellationToken cancellationToken,
            [Service] IProductsService productService)
        {
            var result = await productService.CreateProduct(input);
            return result;
        }

        public async Task<ProductUpdateDto> UpdateCustomer(ProductUpdateDto input, CancellationToken cancellationToken,
            [Service] IProductsService productService)
        {
            await productService.UpdateProduct(input.Id, input);
            return input;
        }

        public async Task<Guid> DeleteCustomer(Guid id, CancellationToken cancellationToken,
            [Service] IProductsService productService)
        {
            await productService.DeleteProduct(id);
            return id;
        }
    }

    [ExtendObjectType(Name = "Mutation")]
    public class InvoiceMutations
    {
        public async Task<Guid> CreateInvoice(
            CreateInvoiceCommand input,
            CancellationToken cancellationToken,
            [Service] IInvoiceRepository invoiceRepository)
        {
            var invoice = new Invoice()
            {
                No = input.No,
                Created = input.Created,
                InvoiceLines = new List<InvoiceLine>(),
                CustomerId = input.CustomerId,
            };
            invoiceRepository.Add(invoice);
            await invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return invoice.Id;
        }
    }

    //[ExtendObjectType(typeof(Query))]
    [ExtendObjectType(Name = "Query")]
    public class CustomerQueries
    {
        public async Task<IReadOnlyList<CustomerDto>> GetCustomers(CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            var result = await mediator.Send(new GetCustomersQuery(), cancellationToken);
            return result;
        }

        public async Task<CustomerDto> GetCustomersById(Guid id, CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            var result = await mediator.Send(new GetCustomerByIdQuery() { Id = id }, cancellationToken);
            return result;
        }
    }

    [ExtendObjectType(Name = "Query")]
    public class ProductQueries
    {
        public async Task<IReadOnlyList<ProductDto>> GetProducts(CancellationToken cancellationToken,
            [Service] IProductsService productService)
        {
            var result = await productService.FindProducts();
            return result;
        }

        public async Task<ProductDto> GetProductById(Guid id, CancellationToken cancellationToken,
            [Service] IProductsService productService)
        {
            var result = await productService.FindProductById(id);
            return result;
        }
    }

    [ExtendObjectType(Name = "Query")]
    public class InvoiceQueries
    {
        public async Task<IReadOnlyList<InvoiceType>> GetInvoices(
            CancellationToken cancellationToken,
            [Service] IInvoiceRepository invoiceRepository)
        {
            var entities = await invoiceRepository.FindAllAsync(cancellationToken);
            return entities.Select(x => new InvoiceType(x)).ToList();
        }
    }

}