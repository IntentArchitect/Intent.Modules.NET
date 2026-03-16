using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Mappings;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Addresses
{
    public class AddressDto : IMapFrom<Address>
    {
        public AddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            PostalCode = null!;
        }

        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string PostalCode { get; set; }

        public static AddressDto Create(Guid id, string line1, string line2, string postalCode)
        {
            return new AddressDto
            {
                Id = id,
                Line1 = line1,
                Line2 = line2,
                PostalCode = postalCode
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Address, AddressDto>();
        }
    }
}