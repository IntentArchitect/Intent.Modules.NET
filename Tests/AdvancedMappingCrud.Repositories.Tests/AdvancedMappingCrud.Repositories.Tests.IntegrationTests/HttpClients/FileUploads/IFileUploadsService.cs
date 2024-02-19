using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.FileUploads;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.FileUploads
{
    public interface IFileUploadsService : IDisposable
    {
        Task<Guid> UploadFileAsync(string? filename, string? contentType, UploadFile command, CancellationToken cancellationToken = default);
    }
}