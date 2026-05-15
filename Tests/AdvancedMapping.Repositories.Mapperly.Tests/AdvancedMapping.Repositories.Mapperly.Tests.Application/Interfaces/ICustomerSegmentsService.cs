using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Interfaces
{
    public interface ICustomerSegmentsService
    {
        Task<Guid> CreateCustomerSegments(CreateCustomerSegmentsDto dto, CancellationToken cancellationToken = default);
        Task UpdateCustomerSegments(Guid id, UpdateCustomerSegmentsDto dto, CancellationToken cancellationToken = default);
        Task<CustomerSegmentsDto> FindCustomerSegmentsById(Guid id, CancellationToken cancellationToken = default);
        Task<List<CustomerSegmentsDto>> FindCustomerSegments(CancellationToken cancellationToken = default);
        Task DeleteCustomerSegments(Guid id, CancellationToken cancellationToken = default);
    }
}