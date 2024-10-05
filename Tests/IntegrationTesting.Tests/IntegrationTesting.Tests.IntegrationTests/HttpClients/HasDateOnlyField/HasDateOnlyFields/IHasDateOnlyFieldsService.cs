using IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.HasDateOnlyField.HasDateOnlyFields
{
    public interface IHasDateOnlyFieldsService : IDisposable
    {
        Task<Guid> CreateHasDateOnlyFieldAsync(CreateHasDateOnlyFieldCommand command, CancellationToken cancellationToken = default);
        Task DeleteHasDateOnlyFieldAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateHasDateOnlyFieldAsync(Guid id, UpdateHasDateOnlyFieldCommand command, CancellationToken cancellationToken = default);
        Task<HasDateOnlyFieldDto> GetHasDateOnlyFieldByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<HasDateOnlyFieldDto>> GetHasDateOnlyFieldsAsync(CancellationToken cancellationToken = default);
    }
}