using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.IntegrationTests.Services.TestEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace ValueObjects.Class.IntegrationTests.HttpClients.TestEntities
{
    public interface ITestEntitiesService : IDisposable
    {
        Task<Guid> CreateTestEntityAsync(CreateTestEntityCommand command, CancellationToken cancellationToken = default);
        Task DeleteTestEntityAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateTestEntityAsync(Guid id, UpdateTestEntityCommand command, CancellationToken cancellationToken = default);
        Task<List<TestEntityDto>> GetTestEntitiesAsync(CancellationToken cancellationToken = default);
        Task<TestEntityDto> GetTestEntityByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}