using AutoMapper;
using CosmosDB.Application.Common.Mappings;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Orders
{
    public class OrderCreatedDto : IMapFrom<Order>
    {
        public OrderCreatedDto()
        {
            Id = null!;
            WarehouseId = null!;
        }

        public string Id { get; set; }
        public string WarehouseId { get; set; }

        public static OrderCreatedDto Create(string id, string warehouseId)
        {
            return new OrderCreatedDto
            {
                Id = id,
                WarehouseId = warehouseId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderCreatedDto>();
        }
    }
}