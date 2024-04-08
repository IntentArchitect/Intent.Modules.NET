using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Optionals;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Optionals
{
    public interface IOptionalsService : IDisposable
    {
        Task<Guid> CreateOptionalAsync(CreateOptionalCommand command, CancellationToken cancellationToken = default);
        Task<OptionalDto?> GetOptionalByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}