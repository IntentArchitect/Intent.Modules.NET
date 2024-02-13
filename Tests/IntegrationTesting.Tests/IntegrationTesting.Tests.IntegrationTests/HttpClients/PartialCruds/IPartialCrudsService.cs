using IntegrationTesting.Tests.IntegrationTests.Services.PartialCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.PartialCruds
{
    public interface IPartialCrudsService : IDisposable
    {
        Task<Guid> CreatePartialCrudAsync(CreatePartialCrudCommand command, CancellationToken cancellationToken = default);
        Task UpdatePartialCrudAsync(Guid id, UpdatePartialCrudCommand command, CancellationToken cancellationToken = default);
        Task<PartialCrudDto> GetPartialCrudByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}