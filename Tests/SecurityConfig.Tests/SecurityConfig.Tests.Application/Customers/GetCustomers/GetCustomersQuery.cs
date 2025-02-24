using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SecurityConfig.Tests.Application.Common.Interfaces;
using SecurityConfig.Tests.Application.Common.Security;
using SecurityConfig.Tests.Application.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Customers.GetCustomers
{
    [Authorize(Policy = Permissions.PolicyCustomer)]
    public class GetCustomersQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomersQuery()
        {
        }
    }
}