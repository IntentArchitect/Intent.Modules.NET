using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Orders
{
    public interface IOrdersService : IDisposable
    {
        Task<string> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken = default);
        Task<string> CreateOrderOrderItemAsync(CreateOrderOrderItemCommand command, CancellationToken cancellationToken = default);
        Task DeleteOrderAsync(string id, CancellationToken cancellationToken = default);
        Task DeleteOrderOrderItemAsync(string orderId, string id, CancellationToken cancellationToken = default);
        Task UpdateOrderAsync(string id, UpdateOrderCommand command, CancellationToken cancellationToken = default);
        Task UpdateOrderOrderItemAsync(string id, UpdateOrderOrderItemCommand command, CancellationToken cancellationToken = default);
        Task<OrderDto> GetOrderByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<OrderOrderItemDto> GetOrderOrderItemByIdAsync(string orderId, string id, CancellationToken cancellationToken = default);
        Task<List<OrderOrderItemDto>> GetOrderOrderItemsAsync(string orderId, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default);
    }
}