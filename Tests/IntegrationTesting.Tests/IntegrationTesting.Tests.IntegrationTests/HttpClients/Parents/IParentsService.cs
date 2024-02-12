using IntegrationTesting.Tests.IntegrationTests.Services.Parents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.Parents
{
    public interface IParentsService : IDisposable
    {
        Task<Guid> CreateParentAsync(CreateParentCommand command, CancellationToken cancellationToken = default);
        Task DeleteParentAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateParentAsync(Guid id, UpdateParentCommand command, CancellationToken cancellationToken = default);
        Task<ParentDto> GetParentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ParentDto>> GetParentsAsync(CancellationToken cancellationToken = default);
    }
}