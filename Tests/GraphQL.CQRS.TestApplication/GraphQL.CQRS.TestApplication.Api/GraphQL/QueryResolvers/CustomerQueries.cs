using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Customers;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomerById;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomers;
using HotChocolate;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryResolver", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.QueryResolvers
{
    [ExtendObjectType(Name = "Query")]
    public class CustomerQueries
    {
        public async Task<IReadOnlyList<CustomerDto>> GetCustomers(
            GetCustomersQuery input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<CustomerDto> GetCustomersById(
            Guid id,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetCustomerByIdQuery { Id = id }, cancellationToken);
        }
    }
}