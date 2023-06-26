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
    public class OrderOrderItemDto : IMapFrom<OrderItem>
    {
        public OrderOrderItemDto()
        {
            Description = null!;
        }

        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public Guid Id { get; set; }

        public static OrderOrderItemDto Create(Guid orderId, string description, decimal amount, Guid id)
        {
            return new OrderOrderItemDto
            {
                OrderId = orderId,
                Description = description,
                Amount = amount,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrderItem, OrderOrderItemDto>();
        }
    }
}