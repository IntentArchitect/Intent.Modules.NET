using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Customers;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomerById;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomers;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryType", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.Queries
{
    [ExtendObjectType(OperationType.Query)]
    public class CustomerQueries
    {
        public async Task<CustomerDto> GetCustomerById(
            Guid id,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetCustomerByIdQuery(id: id), cancellationToken);
        }

        public async Task<IReadOnlyList<CustomerDto>> GetCustomers(
            string? name,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetCustomersQuery(name: name), cancellationToken);
        }
    }
}