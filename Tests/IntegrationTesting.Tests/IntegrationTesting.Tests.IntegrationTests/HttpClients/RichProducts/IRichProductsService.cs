using IntegrationTesting.Tests.IntegrationTests.Services.RichProducts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.RichProducts
{
    public interface IRichProductsService : IDisposable
    {
        Task<Guid> CreateRichProductAsync(CreateRichProductCommand command, CancellationToken cancellationToken = default);
        Task DeleteRichProductAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateRichProductAsync(Guid id, UpdateRichProductCommand command, CancellationToken cancellationToken = default);
        Task<RichProductDto> GetRichProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<RichProductDto>> GetRichProductsAsync(CancellationToken cancellationToken = default);
    }
}