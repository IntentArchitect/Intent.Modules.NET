using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Application.Customers;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomerById;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomers;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Invoices;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoices;
using GraphQL.CQRS.TestApplication.Application.Products;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryType", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.Queries
{
    [ExtendObjectType(OperationType.Query)]
    public class Query
    {
        public async Task<IReadOnlyList<InvoiceDto>> GetInvoicesAsync(
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetInvoicesQuery(), cancellationToken);
        }

        public async Task<IReadOnlyList<ProductDto>> GetProductsAsync([Service] IProductsService service)
        {
            return await service.FindProducts();
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid id, [Service] IProductsService service)
        {
            return await service.FindProductById(id);
        }

        public async Task<Customer> GetCustomer(
            Guid id,
            CancellationToken cancellationToken,
            [Service] ICustomerRepository repository)
        {
            var entity = await repository.FindByIdAsync(id, cancellationToken);
            return entity;
        }

        public async Task<IReadOnlyList<CustomerDto>> GetCustomers(
            CancellationToken cancellationToken,
            [Service] ICustomerRepository repository,
            [Service] IMapper mapper)
        {
            var entities = await repository.FindAllAsync(cancellationToken);
            return entities.MapToCustomerDtoList(mapper);
        }

        [IntentManaged(Mode.Ignore)]
        [UsePaging]
        [UseSorting]
        [UseFiltering]
        public IQueryable<Invoice> GetSpecialInvoices(
            [Service] IInvoiceRepository repository)
        {
            return repository.GetQueryable();
        }
    }
}