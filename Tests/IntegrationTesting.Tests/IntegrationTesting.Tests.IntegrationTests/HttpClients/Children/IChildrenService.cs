using IntegrationTesting.Tests.IntegrationTests.Services.Children;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.Children
{
    public interface IChildrenService : IDisposable
    {
        Task<Guid> CreateChildAsync(CreateChildCommand command, CancellationToken cancellationToken = default);
        Task DeleteChildAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateChildAsync(Guid id, UpdateChildCommand command, CancellationToken cancellationToken = default);
        Task<ChildDto> GetChildByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ChildDto>> GetChildrenAsync(CancellationToken cancellationToken = default);
    }
}