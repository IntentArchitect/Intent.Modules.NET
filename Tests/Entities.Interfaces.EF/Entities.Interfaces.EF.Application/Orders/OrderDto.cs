using System;
using AutoMapper;
using Entities.Interfaces.EF.Application.Common.Mappings;
using Entities.Interfaces.EF.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            RefNo = null!;
        }

        public Guid Id { get; set; }
        public string RefNo { get; set; }

        public static OrderDto Create(Guid id, string refNo)
        {
            return new OrderDto
            {
                Id = id,
                RefNo = refNo
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}