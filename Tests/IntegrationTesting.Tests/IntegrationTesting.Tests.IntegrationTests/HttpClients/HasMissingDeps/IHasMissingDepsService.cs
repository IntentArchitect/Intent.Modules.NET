using IntegrationTesting.Tests.IntegrationTests.Services.HasMissingDeps;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.HasMissingDeps
{
    public interface IHasMissingDepsService : IDisposable
    {
        Task<Guid> CreateHasMissingDepAsync(CreateHasMissingDepCommand command, CancellationToken cancellationToken = default);
        Task UpdateHasMissingDepAsync(Guid id, UpdateHasMissingDepCommand command, CancellationToken cancellationToken = default);
        Task<HasMissingDepDto> GetHasMissingDepByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<HasMissingDepDto>> GetHasMissingDepsAsync(CancellationToken cancellationToken = default);
    }
}