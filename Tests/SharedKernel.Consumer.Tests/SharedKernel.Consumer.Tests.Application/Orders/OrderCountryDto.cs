using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Consumer.Tests.Application.Common.Mappings;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Orders
{
    public class OrderCountryDto : IMapFrom<Country>
    {
        public OrderCountryDto()
        {
            Name = null!;
            Code = null!;
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public Guid Id { get; set; }

        public static OrderCountryDto Create(string name, string code, Guid id)
        {
            return new OrderCountryDto
            {
                Name = name,
                Code = code,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Country, OrderCountryDto>();
        }
    }
}