using CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients
{
    public interface IAggregateRootsService : IDisposable
    {
        Task<Guid> CreateAggregateRootAsync(CreateAggregateRootCommand command, CancellationToken cancellationToken = default);
        Task<Guid> CreateAggregateRootCompositeManyBAsync(Guid aggregateRootId, CreateAggregateRootCompositeManyBCommand command, CancellationToken cancellationToken = default);
        Task DeleteAggregateRootAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAggregateRootCompositeManyBAsync(Guid aggregateRootId, Guid id, CancellationToken cancellationToken = default);
        Task UpdateAggregateRootAsync(Guid id, UpdateAggregateRootCommand command, CancellationToken cancellationToken = default);
        Task UpdateAggregateRootCompositeManyBAsync(Guid aggregateRootId, Guid id, UpdateAggregateRootCompositeManyBCommand command, CancellationToken cancellationToken = default);
        Task<AggregateRootDto> GetAggregateRootByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AggregateRootCompositeManyBDto> GetAggregateRootCompositeManyBByIdAsync(Guid aggregateRootId, Guid id, CancellationToken cancellationToken = default);
        Task<List<AggregateRootCompositeManyBDto>> GetAggregateRootCompositeManyBSAsync(Guid aggregateRootId, CancellationToken cancellationToken = default);
        Task<List<AggregateRootDto>> GetAggregateRootsAsync(CancellationToken cancellationToken = default);
    }
}