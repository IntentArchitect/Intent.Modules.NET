using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Mappings;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics
{
    public class CustomerAnemicAddressDto : IMapFrom<Address>
    {
        public CustomerAnemicAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public Guid Id { get; set; }

        public static CustomerAnemicAddressDto Create(string line1, string line2, string city, Guid id)
        {
            return new CustomerAnemicAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Address, CustomerAnemicAddressDto>();
        }
    }
}