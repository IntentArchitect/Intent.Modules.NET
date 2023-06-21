using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.AzureFunction.TestApplication.Application.Customers;
using GraphQL.AzureFunction.TestApplication.Application.Interfaces;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.MutationType", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Api.GraphQL.Mutations
{
    [ExtendObjectType(OperationType.Mutation)]
    public class CustomersMutations
    {
        public async Task<CustomerDto> CreateCustomer(CustomerCreateDto dto, [Service] ICustomersService service)
        {
            return await service.CreateCustomer(dto);
        }

        [UseMutationConvention]
        public async Task<CustomerDto> UpdateCustomer(Guid id, CustomerUpdateDto dto, [Service] ICustomersService service)
        {
            return await service.UpdateCustomer(id, dto);
        }

        public async Task<CustomerDto> DeleteCustomer(Guid id, [Service] ICustomersService service)
        {
            return await service.DeleteCustomer(id);
        }
    }
}