using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Interfaces
{
    public interface IUploadDownloadService : IDisposable
    {
        Task<Guid> Upload(Stream content, string? filename, string? contentType, long? contentLength, CancellationToken cancellationToken = default);
        Task<FileDownloadDto> Download(Guid id, CancellationToken cancellationToken = default);
    }
}