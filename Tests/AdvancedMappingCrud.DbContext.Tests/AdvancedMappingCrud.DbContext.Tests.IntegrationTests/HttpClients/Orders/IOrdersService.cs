using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Orders
{
    public interface IOrdersService : IDisposable
    {
        Task<Guid> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken = default);
        Task<Guid> CreateOrderOrderItemAsync(CreateOrderOrderItemCommand command, CancellationToken cancellationToken = default);
        Task DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteOrderOrderItemAsync(Guid orderId, Guid id, CancellationToken cancellationToken = default);
        Task UpdateOrderAsync(Guid id, UpdateOrderCommand command, CancellationToken cancellationToken = default);
        Task UpdateOrderOrderItemAsync(Guid id, UpdateOrderOrderItemCommand command, CancellationToken cancellationToken = default);
        Task<OrderDto> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<OrderOrderItemDto> GetOrderOrderItemByIdAsync(Guid orderId, Guid id, CancellationToken cancellationToken = default);
        Task<List<OrderOrderItemDto>> GetOrderOrderItemsAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default);
    }
}