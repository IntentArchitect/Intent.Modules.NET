using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Orders
{
    public interface IOrdersService : IDisposable
    {
        Task<string> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken = default);
        Task DeleteOrderAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateOrderAsync(string id, UpdateOrderCommand command, CancellationToken cancellationToken = default);
        Task<OrderDto> GetOrderByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<OrderDto> GetOrderByRefNoAsync(string refNo, string external, CancellationToken cancellationToken = default);
        Task<OrderDto> GetOrderByRefAsync(string? refNo, string? externalRefNo, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> GetOrdersByRefNoAsync(string? refNo, string? externalRefNo, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default);
    }
}