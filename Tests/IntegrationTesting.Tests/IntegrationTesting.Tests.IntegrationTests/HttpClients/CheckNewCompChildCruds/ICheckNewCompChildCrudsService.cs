using IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.CheckNewCompChildCruds
{
    public interface ICheckNewCompChildCrudsService : IDisposable
    {
        Task<Guid> CreateCheckNewCompChildCrudAsync(CreateCheckNewCompChildCrudCommand command, CancellationToken cancellationToken = default);
        Task<Guid> CreateCNCCChildAsync(Guid checkNewCompChildCrudId, CreateCNCCChildCommand command, CancellationToken cancellationToken = default);
        Task DeleteCheckNewCompChildCrudAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteCNCCChildAsync(Guid checkNewCompChildCrudId, Guid id, CancellationToken cancellationToken = default);
        Task UpdateCheckNewCompChildCrudAsync(Guid id, UpdateCheckNewCompChildCrudCommand command, CancellationToken cancellationToken = default);
        Task UpdateCNCCChildAsync(Guid checkNewCompChildCrudId, Guid id, UpdateCNCCChildCommand command, CancellationToken cancellationToken = default);
        Task<CheckNewCompChildCrudDto> GetCheckNewCompChildCrudByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CheckNewCompChildCrudDto>> GetCheckNewCompChildCrudsAsync(CancellationToken cancellationToken = default);
        Task<CNCCChildDto> GetCNCCChildByIdAsync(Guid checkNewCompChildCrudId, Guid id, CancellationToken cancellationToken = default);
        Task<List<CNCCChildDto>> GetCNCCChildrenAsync(Guid checkNewCompChildCrudId, CancellationToken cancellationToken = default);
    }
}