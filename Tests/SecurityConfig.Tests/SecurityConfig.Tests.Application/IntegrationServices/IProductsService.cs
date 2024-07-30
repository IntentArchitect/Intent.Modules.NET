using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SecurityConfig.Tests.Application.IntegrationServices.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace SecurityConfig.Tests.Application.IntegrationServices
{
    public interface IProductsService : IDisposable
    {
        Task<Guid> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken = default);
        Task<ProductDto> FindProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> FindProductsAsync(CancellationToken cancellationToken = default);
        Task UpdateProductAsync(Guid id, ProductUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    }
}