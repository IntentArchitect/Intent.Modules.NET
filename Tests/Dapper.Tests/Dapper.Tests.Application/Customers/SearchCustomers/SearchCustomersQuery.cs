using System.Collections.Generic;
using Dapper.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Dapper.Tests.Application.Customers.SearchCustomers
{
    public class SearchCustomersQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public SearchCustomersQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public string SearchTerm { get; set; }
    }
}