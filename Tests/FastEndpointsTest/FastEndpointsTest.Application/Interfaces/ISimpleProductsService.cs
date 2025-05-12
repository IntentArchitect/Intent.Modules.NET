using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Application.SimpleProducts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace FastEndpointsTest.Application.Interfaces
{
    public interface ISimpleProductsService
    {
        Task<Guid> CreateSimpleProduct(SimpleProductCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateSimpleProduct(Guid id, SimpleProductUpdateDto dto, CancellationToken cancellationToken = default);
        Task<SimpleProductDto> FindSimpleProductById(Guid id, CancellationToken cancellationToken = default);
        Task<List<SimpleProductDto>> FindSimpleProducts(CancellationToken cancellationToken = default);
        Task DeleteSimpleProduct(Guid id, CancellationToken cancellationToken = default);
    }
}