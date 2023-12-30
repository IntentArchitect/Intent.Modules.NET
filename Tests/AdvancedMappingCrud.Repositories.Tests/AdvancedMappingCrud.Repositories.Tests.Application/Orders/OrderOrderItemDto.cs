using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders
{
    public class OrderOrderItemDto : IMapFrom<OrderItem>
    {
        public OrderOrderItemDto()
        {
        }

        public Guid OrderId { get; set; }
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public Guid ProductId { get; set; }

        public static OrderOrderItemDto Create(Guid orderId, Guid id, int quantity, decimal amount, Guid productId)
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
            profile.CreateMap<OrderItem, OrderOrderItemDto>()
                .ForMember(d => d.Amount, opt => opt.MapFrom(src => src.UnitPrice));
        }
    }
}