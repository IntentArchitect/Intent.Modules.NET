using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Services.File;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.IntegrationServices
{
    public interface IFileService : IDisposable
    {
        Task FileUploadAsync(string? contentType, long? contentLength, FileUploadCommand command, CancellationToken cancellationToken = default);
    }
}