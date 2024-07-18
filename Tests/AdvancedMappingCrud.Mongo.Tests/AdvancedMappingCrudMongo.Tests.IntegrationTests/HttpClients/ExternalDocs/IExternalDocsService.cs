using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.ExternalDocs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.ExternalDocs
{
    public interface IExternalDocsService : IDisposable
    {
        Task<long> CreateExternalDocAsync(CreateExternalDocCommand command, CancellationToken cancellationToken = default);
        Task DeleteExternalDocAsync(long id, CancellationToken cancellationToken = default);
        Task UpdateExternalDocAsync(long id, UpdateExternalDocCommand command, CancellationToken cancellationToken = default);
        Task<ExternalDocDto> GetExternalDocByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<List<ExternalDocDto>> GetExternalDocsAsync(CancellationToken cancellationToken = default);
    }
}