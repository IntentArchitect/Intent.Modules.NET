using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
            CompanyName = null!;
            CompanyContactDetailsVOS = null!;
            Addresses = null!;
            ContactDetailsVOCell = null!;
            ContactDetailsVOEmail = null!;
        }

        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<UserContactDetailsVODto> CompanyContactDetailsVOS { get; set; }
        public List<UserAddressDto> Addresses { get; set; }
        public string ContactDetailsVOCell { get; set; }
        public string ContactDetailsVOEmail { get; set; }

        public static UserDto Create(
            Guid id,
            Guid companyId,
            string companyName,
            List<UserContactDetailsVODto> companyContactDetailsVOS,
            List<UserAddressDto> addresses,
            string contactDetailsVOCell,
            string contactDetailsVOEmail)
        {
            return new UserDto
            {
                Id = id,
                CompanyId = companyId,
                CompanyName = companyName,
                CompanyContactDetailsVOS = companyContactDetailsVOS,
                Addresses = addresses,
                ContactDetailsVOCell = contactDetailsVOCell,
                ContactDetailsVOEmail = contactDetailsVOEmail
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(d => d.CompanyContactDetailsVOS, opt => opt.MapFrom(src => src.Company.ContactDetailsVOS))
                .ForMember(d => d.Addresses, opt => opt.MapFrom(src => src.Addresses))
                .ForMember(d => d.ContactDetailsVOCell, opt => opt.MapFrom(src => src.ContactDetails.Cell))
                .ForMember(d => d.ContactDetailsVOEmail, opt => opt.MapFrom(src => src.ContactDetails.Email));
        }
    }
}