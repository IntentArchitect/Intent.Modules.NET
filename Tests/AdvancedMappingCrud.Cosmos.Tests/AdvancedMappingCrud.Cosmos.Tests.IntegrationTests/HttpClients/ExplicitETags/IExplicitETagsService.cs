using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.ExplicitETags;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.ExplicitETags
{
    public interface IExplicitETagsService : IDisposable
    {
        Task<string> CreateExplicitETagAsync(CreateExplicitETagCommand command, CancellationToken cancellationToken = default);
        Task UpdateExplicitETagAsync(string id, UpdateExplicitETagCommand command, CancellationToken cancellationToken = default);
        Task<ExplicitETagDto> GetExplicitETagByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}