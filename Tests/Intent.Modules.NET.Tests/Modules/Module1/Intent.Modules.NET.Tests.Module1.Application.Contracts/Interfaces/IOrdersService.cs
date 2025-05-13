using Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Contracts.Interfaces
{
    public interface IOrdersService
    {
        Task<Guid> CreateOrder(OrderCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateOrder(Guid id, OrderUpdateDto dto, CancellationToken cancellationToken = default);
        Task<OrderDto> FindOrderById(Guid id, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> FindOrders(CancellationToken cancellationToken = default);
        Task DeleteOrder(Guid id, CancellationToken cancellationToken = default);
    }
}