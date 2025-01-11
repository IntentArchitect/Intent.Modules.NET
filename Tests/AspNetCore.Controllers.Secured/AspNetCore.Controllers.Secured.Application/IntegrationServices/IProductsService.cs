using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Controllers.Secured.Application.IntegrationServices.Contracts.Services.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.IntegrationServices
{
    public interface IProductsService : IDisposable
    {
        Task<Guid> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken = default);
        Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}