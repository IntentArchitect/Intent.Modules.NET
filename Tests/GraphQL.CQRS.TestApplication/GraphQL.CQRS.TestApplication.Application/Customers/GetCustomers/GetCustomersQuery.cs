using System;
using System.Collections.Generic;
using GraphQL.CQRS.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Customers.GetCustomers
{
    public class GetCustomersQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomersQuery(string? name)
        {
            Name = name;
        }
        public string? Name { get; set; }
    }
}