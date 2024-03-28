using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Application.Common.Mappings;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SqlServerImporterTests.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            RefNo = null!;
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string RefNo { get; set; }

        public static OrderDto Create(Guid id, Guid customerId, DateTime orderDate, string refNo)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId,
                OrderDate = orderDate,
                RefNo = refNo
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}