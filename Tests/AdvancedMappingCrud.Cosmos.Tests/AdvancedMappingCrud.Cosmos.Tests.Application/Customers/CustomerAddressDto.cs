using System;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers
{
    public class CustomerAddressDto : IMapFrom<Address>
    {
        public CustomerAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            PostalCode = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public Guid Id { get; set; }

        public static CustomerAddressDto Create(string line1, string line2, string city, string postalCode, Guid id)
        {
            return new CustomerAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                PostalCode = postalCode,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Address, CustomerAddressDto>();
        }
    }
}