using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Common.Mappings;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            Number = null!;
            OrderItems = null!;
        }

        public Guid Id { get; set; }
        public string Number { get; set; }
        public List<OrderOrderItemDto> OrderItems { get; set; }

        public static OrderDto Create(Guid id, string number, List<OrderOrderItemDto> orderItems)
        {
            return new OrderDto
            {
                Id = id,
                Number = number,
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