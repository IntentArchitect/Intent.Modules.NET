using EntityFrameworkCore.SQLLite.Application.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Application.Interfaces
{
    public interface IOrdersService
    {
        Task<Guid> CreateOrder(CreateOrderDto dto, CancellationToken cancellationToken = default);
        Task UpdateOrder(Guid id, UpdateOrderDto dto, CancellationToken cancellationToken = default);
        Task<OrderDto> FindOrderById(Guid id, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> FindOrders(CancellationToken cancellationToken = default);
        Task DeleteOrder(Guid id, CancellationToken cancellationToken = default);
    }
}