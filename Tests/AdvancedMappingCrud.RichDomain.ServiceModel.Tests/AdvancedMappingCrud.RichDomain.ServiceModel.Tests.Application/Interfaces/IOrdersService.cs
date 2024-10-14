using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces
{
    public interface IOrdersService : IDisposable
    {
        Task<Guid> CreateOrder(OrderCreateDto dto, CancellationToken cancellationToken = default);
        Task<OrderDto> FindOrderById(Guid id, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> FindOrders(CancellationToken cancellationToken = default);
        Task DeleteOrder(Guid id, CancellationToken cancellationToken = default);
        Task UpdateOrderItems(Guid id, UpdateOrderItemsDto dto, CancellationToken cancellationToken = default);
    }
}