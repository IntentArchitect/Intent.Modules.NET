using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public class UserUserAddressDto : IMapFrom<Address>
    {
        public UserUserAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            AddressDetailsName = null!;
        }

        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public int Postal { get; set; }
        public AddressType AddressDetailsAddressType { get; set; }
        public AddressType? AddressDetailsAddressTypeNullable { get; set; }
        public string AddressDetailsName { get; set; }
        public int AddressDetailsNumber { get; set; }

        public static UserUserAddressDto Create(
            Guid id,
            string line1,
            string line2,
            string city,
            int postal,
            AddressType addressDetailsAddressType,
            AddressType? addressDetailsAddressTypeNullable,
            string addressDetailsName,
            int addressDetailsNumber)
        {
            return new UserUserAddressDto
            {
                Id = id,
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal,
                AddressDetailsAddressType = addressDetailsAddressType,
                AddressDetailsAddressTypeNullable = addressDetailsAddressTypeNullable,
                AddressDetailsName = addressDetailsName,
                AddressDetailsNumber = addressDetailsNumber
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Address, UserUserAddressDto>()
                .ForMember(d => d.AddressDetailsAddressType, opt => opt.MapFrom(src => src.AddressDetails != null ? src.AddressDetails!.AddressType : default))
                .ForMember(d => d.AddressDetailsAddressTypeNullable, opt => opt.MapFrom(src => src.AddressDetails != null ? src.AddressDetails!.AddressTypeNullable ?? default : default))
                .ForMember(d => d.AddressDetailsName, opt => opt.MapFrom(src => src.AddressDetails != null ? src.AddressDetails!.Name : null))
                .ForMember(d => d.AddressDetailsNumber, opt => opt.MapFrom(src => src.AddressDetails != null ? src.AddressDetails!.Number : (int?)null));
        }
    }
}