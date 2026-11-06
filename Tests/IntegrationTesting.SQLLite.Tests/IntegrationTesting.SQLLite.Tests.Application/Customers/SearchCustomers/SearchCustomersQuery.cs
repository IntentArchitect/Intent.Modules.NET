using IntegrationTesting.SQLLite.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.Customers.SearchCustomers
{
    /// <summary>
    /// HTTP QUERY endpoint: the search criteria travel in the request body, and the request stays safe and idempotent.
    /// </summary>
    public class SearchCustomersQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public SearchCustomersQuery(string lastName)
        {
            LastName = lastName;
        }

        public string LastName { get; set; }
    }
}