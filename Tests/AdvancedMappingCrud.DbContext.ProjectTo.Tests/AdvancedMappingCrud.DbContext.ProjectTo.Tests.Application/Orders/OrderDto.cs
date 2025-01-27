using System;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            RefNo = null!;
        }

        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Guid CustomerId { get; set; }

        public static OrderDto Create(Guid id, string refNo, DateTime orderDate, OrderStatus orderStatus, Guid customerId)
        {
            return new OrderDto
            {
                Id = id,
                RefNo = refNo,
                OrderDate = orderDate,
                OrderStatus = orderStatus,
                CustomerId = customerId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}