using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ServiceToServiceInvocation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.ServiceToServiceInvocation
{
    public interface IExposedStoredProcService : IDisposable
    {
        Task<List<GetDataEntryDto>> GetDataAsync(CancellationToken cancellationToken = default);
    }
}