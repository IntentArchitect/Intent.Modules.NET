using IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.BadSignatures
{
    public interface IBadSignaturesService : IDisposable
    {
        Task<Guid> CreateBadSignaturesAsync(CreateBadSignaturesCommand command, CancellationToken cancellationToken = default);
        Task DeleteBadSignaturesAsync(Guid id, string more, CancellationToken cancellationToken = default);
        Task UpdateBadSignaturesAsync(Guid id, UpdateBadSignaturesCommand command, CancellationToken cancellationToken = default);
        Task<BadSignaturesDto> GetBadSignaturesByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<BadSignaturesDto>> GetBadSignaturesAsync(string filter, CancellationToken cancellationToken = default);
    }
}