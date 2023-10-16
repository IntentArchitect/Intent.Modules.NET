using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Application.Common.Mappings;
using TableStorage.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders
{
    public class OrderCustomerDto : IMapFrom<Customer>
    {
        public OrderCustomerDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static OrderCustomerDto Create(string name, Guid id)
        {
            return new OrderCustomerDto
            {
                Name = name,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, OrderCustomerDto>();
        }
    }
}