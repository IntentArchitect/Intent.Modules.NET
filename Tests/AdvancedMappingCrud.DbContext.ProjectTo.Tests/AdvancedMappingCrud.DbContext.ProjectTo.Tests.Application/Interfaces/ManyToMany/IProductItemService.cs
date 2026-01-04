using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Interfaces.ManyToMany
{
    public interface IProductItemService
    {
        Task CreateProductItem(CreateProductItemDto dto, CancellationToken cancellationToken = default);
        Task UpdateProductItem(UpdateProductItemDto dto, Guid id, CancellationToken cancellationToken = default);
    }
}