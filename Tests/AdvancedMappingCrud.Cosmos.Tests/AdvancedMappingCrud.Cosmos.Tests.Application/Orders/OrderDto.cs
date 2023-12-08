using System;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            Id = null!;
            CustomerId = null!;
            RefNo = null!;
        }

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public static OrderDto Create(string id, string customerId, string refNo, DateTime orderDate, OrderStatus orderStatus)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId,
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}