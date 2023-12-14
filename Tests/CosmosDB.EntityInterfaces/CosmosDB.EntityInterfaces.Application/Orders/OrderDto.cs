using System;
using AutoMapper;
using CosmosDB.EntityInterfaces.Application.Common.Mappings;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            Id = null!;
            WarehouseId = null!;
            RefNo = null!;
        }

        public string Id { get; set; }
        public string WarehouseId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }

        public static OrderDto Create(string id, string warehouseId, string refNo, DateTime orderDate)
        {
            return new OrderDto
            {
                Id = id,
                WarehouseId = warehouseId,
                RefNo = refNo,
                OrderDate = orderDate
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}