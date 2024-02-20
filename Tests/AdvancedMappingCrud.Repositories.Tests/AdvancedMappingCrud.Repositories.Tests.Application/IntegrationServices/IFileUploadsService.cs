using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationServices.Contracts.Services.Common;
using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationServices.Contracts.Services.FileUploads;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.IntegrationServices
{
    public interface IFileUploadsService : IDisposable
    {
        Task<Guid> UploadFileAsync(string? contentType, long? contentLength, UploadFileCommand command, CancellationToken cancellationToken = default);
        Task<FileDownloadDto> DownloadFileAsync(Guid id, CancellationToken cancellationToken = default);
    }
}