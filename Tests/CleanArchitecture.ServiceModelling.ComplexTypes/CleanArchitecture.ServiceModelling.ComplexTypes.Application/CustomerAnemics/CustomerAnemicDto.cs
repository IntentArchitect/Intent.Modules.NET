using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Mappings;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics
{
    public class CustomerAnemicDto : IMapFrom<CustomerAnemic>
    {
        public CustomerAnemicDto()
        {
            Name = null!;
            Address = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public CustomerAnemicAddressDto Address { get; set; }

        public static CustomerAnemicDto Create(Guid id, string name, CustomerAnemicAddressDto address)
        {
            return new CustomerAnemicDto
            {
                Id = id,
                Name = name,
                Address = address
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerAnemic, CustomerAnemicDto>();
        }
    }
}