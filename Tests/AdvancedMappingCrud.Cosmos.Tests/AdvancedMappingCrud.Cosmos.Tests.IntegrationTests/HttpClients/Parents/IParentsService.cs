using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Parents
{
    public interface IParentsService : IDisposable
    {
        Task<string> CreateParentAsync(CreateParentCommand command, CancellationToken cancellationToken = default);
        Task DeleteParentAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateParentAsync(string id, UpdateParentCommand command, CancellationToken cancellationToken = default);
        Task<ParentDto> GetParentByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<List<ParentDto>> GetParentsAsync(CancellationToken cancellationToken = default);
    }
}