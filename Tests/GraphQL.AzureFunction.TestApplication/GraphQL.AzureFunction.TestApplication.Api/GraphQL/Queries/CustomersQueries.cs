using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.AzureFunction.TestApplication.Application.Common.Pagination;
using GraphQL.AzureFunction.TestApplication.Application.Customers;
using GraphQL.AzureFunction.TestApplication.Application.Interfaces;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryType", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Api.GraphQL.Queries
{
    [ExtendObjectType(OperationType.Query)]
    public class CustomersQueries
    {
        public async Task<CustomerDto> FindCustomerById(Guid id, [Service] ICustomersService service)
        {
            return await service.FindCustomerById(id);
        }

        public async Task<IReadOnlyList<CustomerDto>> FindCustomers([Service] ICustomersService service)
        {
            return await service.FindCustomers();
        }

        public async Task<PagedResult<CustomerDto>> GetCustomersPaged(
            int pageNo,
            int pageCount,
            IReadOnlyList<Guid> ids,
            [Service] ICustomersService service)
        {
            return await service.GetCustomersPaged(pageNo, pageCount, ids.ToList());
        }
    }
}