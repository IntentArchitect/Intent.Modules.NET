using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ParentWithAnemicChildren;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.ParentWithAnemicChildren
{
    public interface IParentWithAnemicChildrenService : IDisposable
    {
        Task<Guid> CreateParentWithAnemicChildAsync(CreateParentWithAnemicChildCommand command, CancellationToken cancellationToken = default);
        Task DeleteParentWithAnemicChildAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateParentWithAnemicChildAsync(Guid id, UpdateParentWithAnemicChildCommand command, CancellationToken cancellationToken = default);
        Task<ParentWithAnemicChildDto> GetParentWithAnemicChildByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ParentWithAnemicChildDto>> GetParentWithAnemicChildrenAsync(CancellationToken cancellationToken = default);
    }
}