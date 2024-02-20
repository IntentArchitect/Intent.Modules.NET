using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Common;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.FileUploads;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.FileUploads
{
    public interface IFileUploadsService : IDisposable
    {
        Task RestrictedUploadAsync(string? contentType, long? contentLength, RestrictedUploadCommand command, CancellationToken cancellationToken = default);
        Task<Guid> SimpleUploadAsync(SimpleUploadCommand command, CancellationToken cancellationToken = default);
        Task<Guid> UploadFileAsync(string? contentType, long? contentLength, UploadFileCommand command, CancellationToken cancellationToken = default);
        Task<FileDownloadDto> DownloadFileAsync(Guid id, CancellationToken cancellationToken = default);
        Task<SimpleFileDownloadDto> SimpleDownloadAsync(Guid id, CancellationToken cancellationToken = default);
    }
}