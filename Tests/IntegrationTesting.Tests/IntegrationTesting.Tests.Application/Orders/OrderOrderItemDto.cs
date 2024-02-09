using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Orders
{
    public class OrderOrderItemDto : IMapFrom<OrderItem>
    {
        public OrderOrderItemDto()
        {
            Description = null!;
        }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid ProductId { get; set; }

        public static OrderOrderItemDto Create(Guid id, string description, Guid productId)
        {
            return new OrderOrderItemDto
            {
                Id = id,
                Description = description,
                ProductId = productId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrderItem, OrderOrderItemDto>();
        }
    }
}