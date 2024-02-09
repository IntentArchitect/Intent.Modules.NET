using System;
using System.Collections.Generic;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            RefNo = null!;
            OrderItems = null!;
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string RefNo { get; set; }
        public List<OrderOrderItemDto> OrderItems { get; set; }

        public static OrderDto Create(Guid id, Guid customerId, string refNo, List<OrderOrderItemDto> orderItems)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId,
                RefNo = refNo,
                OrderItems = orderItems
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>()
                .ForMember(d => d.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
        }
    }
}