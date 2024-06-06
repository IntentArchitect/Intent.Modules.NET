using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Application.Common.Mappings;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TrainingModel.Tests.Application.Orders
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
        public Guid CustomerId { get; set; }

        public static OrderDto Create(Guid id, string refNo, DateTime orderDate, Guid customerId)
        {
            return new OrderDto
            {
                Id = id,
                RefNo = refNo,
                OrderDate = orderDate,
                CustomerId = customerId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}