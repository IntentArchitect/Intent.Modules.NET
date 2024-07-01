using IntegrationTesting.Tests.IntegrationTests.Services.Brands;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.Brands
{
    public interface IBrandsService : IDisposable
    {
        Task<Guid> CreateBrandAsync(CreateBrandCommand command, CancellationToken cancellationToken = default);
        Task DeleteBrandAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateBrandAsync(Guid id, UpdateBrandCommand command, CancellationToken cancellationToken = default);
        Task<BrandDto> GetBrandByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<BrandDto>> GetBrandsAsync(CancellationToken cancellationToken = default);
    }
}