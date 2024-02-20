using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients
{
    public interface IUploadDownloadService : IDisposable
    {
        Task<Guid> UploadAsync(Stream content, string? filename, string? contentType, long? contentLength, CancellationToken cancellationToken = default);
        Task<FileDownloadDto> DownloadAsync(Guid id, CancellationToken cancellationToken = default);
    }
}