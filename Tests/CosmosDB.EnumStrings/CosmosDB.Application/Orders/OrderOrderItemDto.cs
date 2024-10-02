using AutoMapper;
using CosmosDB.Application.Common.Mappings;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Orders
{
    public class OrderOrderItemDto : IMapFrom<OrderItem>
    {
        public OrderOrderItemDto()
        {
            OrderId = null!;
            Id = null!;
            Description = null!;
        }

        public string OrderId { get; set; }
        public string Id { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public static OrderOrderItemDto Create(string orderId, string id, int quantity, string description, decimal amount)
        {
            return new OrderOrderItemDto
            {
                OrderId = orderId,
                Id = id,
                Quantity = quantity,
                Description = description,
                Amount = amount
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrderItem, OrderOrderItemDto>();
        }
    }
}