using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces
{
    public interface ICustomersService : IDisposable
    {
        Task<Guid> CreateCustomer(CustomerCreateDto dto, CancellationToken cancellationToken = default);
        Task<CustomerDto> FindCustomerById(Guid id, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> FindCustomers(CancellationToken cancellationToken = default);
        Task DeleteCustomer(Guid id, CancellationToken cancellationToken = default);
    }
}