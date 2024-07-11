using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    public class OrderOrderItemDto : IMapFrom<OrderItem>
    {
        public OrderOrderItemDto()
        {
            OrderId = null!;
            Id = null!;
            ProductId = null!;
        }

        public string OrderId { get; set; }
        public string Id { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; }

        public static OrderOrderItemDto Create(string orderId, string id, int quantity, decimal amount, string productId)
        {
            return new OrderOrderItemDto
            {
                OrderId = orderId,
                Id = id,
                Quantity = quantity,
                Amount = amount,
                ProductId = productId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrderItem, OrderOrderItemDto>();

            profile.CreateMap<IOrderItemDocument, OrderOrderItemDto>();
        }
    }
}