using IntegrationTesting.Tests.IntegrationTests.Services.NoReturns;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.NoReturns
{
    public interface INoReturnsService : IDisposable
    {
        Task CreateNoReturnAsync(CreateNoReturnCommand command, CancellationToken cancellationToken = default);
        Task DeleteNoReturnAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateNoReturnAsync(Guid id, UpdateNoReturnCommand command, CancellationToken cancellationToken = default);
        Task<NoReturnDto> GetNoReturnByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<NoReturnDto>> GetNoReturnsAsync(CancellationToken cancellationToken = default);
    }
}