using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Ones
{
    public interface IOnesService : IDisposable
    {
        Task<Guid> CreateOneAsync(CreateOneCommand command, CancellationToken cancellationToken = default);
        Task DeleteOneAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateOneAsync(Guid id, UpdateOneCommand command, CancellationToken cancellationToken = default);
        Task<OneDto> GetOneByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<OneDto>> GetOnesAsync(CancellationToken cancellationToken = default);
    }
}