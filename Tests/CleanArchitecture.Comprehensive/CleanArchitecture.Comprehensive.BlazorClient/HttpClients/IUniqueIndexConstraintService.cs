using CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.UniqueIndexConstraint.ClassicMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients
{
    public interface IUniqueIndexConstraintService : IDisposable
    {
        Task<Guid> CreateAggregateWithUniqueConstraintIndexElementAsync(CreateAggregateWithUniqueConstraintIndexElementCommand command, CancellationToken cancellationToken = default);
        Task<Guid> CreateAggregateWithUniqueConstraintIndexStereotypeAsync(CreateAggregateWithUniqueConstraintIndexStereotypeCommand command, CancellationToken cancellationToken = default);
        Task DeleteAggregateWithUniqueConstraintIndexElementAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAggregateWithUniqueConstraintIndexStereotypeAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateAggregateWithUniqueConstraintIndexElementAsync(Guid id, UpdateAggregateWithUniqueConstraintIndexElementCommand command, CancellationToken cancellationToken = default);
        Task UpdateAggregateWithUniqueConstraintIndexStereotypeAsync(Guid id, UpdateAggregateWithUniqueConstraintIndexStereotypeCommand command, CancellationToken cancellationToken = default);
        Task<AggregateWithUniqueConstraintIndexElementDto> GetAggregateWithUniqueConstraintIndexElementByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<AggregateWithUniqueConstraintIndexElementDto>> GetAggregateWithUniqueConstraintIndexElementsAsync(CancellationToken cancellationToken = default);
        Task<AggregateWithUniqueConstraintIndexStereotypeDto> GetAggregateWithUniqueConstraintIndexStereotypeByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<AggregateWithUniqueConstraintIndexStereotypeDto>> GetAggregateWithUniqueConstraintIndexStereotypesAsync(CancellationToken cancellationToken = default);
        Task CreateViaContructorAsync(CreateViaContructorCommand command, CancellationToken cancellationToken = default);
    }
}