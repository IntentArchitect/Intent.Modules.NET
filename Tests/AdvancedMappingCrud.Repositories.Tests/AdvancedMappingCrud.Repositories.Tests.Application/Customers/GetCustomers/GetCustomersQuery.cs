using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomers
{
    public class GetCustomersQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomersQuery(Func<IQueryable<CustomerDto>, IQueryable> transform)
        {
            Transform = transform;
        }

        public Func<IQueryable<CustomerDto>, IQueryable> Transform { get; }
    }
}