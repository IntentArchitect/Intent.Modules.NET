using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Application.Common.Mappings;
using Publish.CleanArchDapr.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public static OrderDto Create(Guid id, Guid customerId)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}