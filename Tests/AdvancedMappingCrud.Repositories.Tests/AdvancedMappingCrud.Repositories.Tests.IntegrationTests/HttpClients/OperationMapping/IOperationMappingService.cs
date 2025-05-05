using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.OperationMapping
{
    public interface IOperationMappingService : IDisposable
    {
        Task<Guid> CreateTaskItemAsync(Guid userId, Guid taskListId, CreateTaskItemCommand command, CancellationToken cancellationToken = default);
        Task CreateUserWithTaskItemAsync(CreateUserWithTaskItemCommand command, CancellationToken cancellationToken = default);
        Task CreateUserWithTaskItemContractAsync(CreateUserWithTaskItemContractCommand command, CancellationToken cancellationToken = default);
        Task DeleteTaskItemAsync(Guid userId, Guid taskListId, Guid id, CancellationToken cancellationToken = default);
        Task UpdateTaskItemAsync(Guid userId, Guid taskListId, Guid id, UpdateTaskItemCommand command, CancellationToken cancellationToken = default);
        Task<TaskItemDto> GetTaskItemByIdAsync(Guid userId, Guid taskListId, Guid id, CancellationToken cancellationToken = default);
        Task<List<TaskItemDto>> GetTaskItemsAsync(Guid userId, Guid taskListId, CancellationToken cancellationToken = default);
    }
}