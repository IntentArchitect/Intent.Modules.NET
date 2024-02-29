using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Application.Common.Mappings;
using TableStorage.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders
{
    public class OrderOrderLineDto : IMapFrom<OrderLine>
    {
        public OrderOrderLineDto()
        {
            Description = null!;
        }

        public string Description { get; set; }
        public decimal Amount { get; set; }
        public Guid Id { get; set; }

        public static OrderOrderLineDto Create(string description, decimal amount, Guid id)
        {
            return new OrderOrderLineDto
            {
                Description = description,
                Amount = amount,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrderLine, OrderOrderLineDto>();
        }
    }
}