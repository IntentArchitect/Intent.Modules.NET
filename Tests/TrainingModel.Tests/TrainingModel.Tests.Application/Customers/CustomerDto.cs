using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Application.Common.Mappings;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TrainingModel.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
            Address = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<CustomerAddressDto> Address { get; set; }

        public static CustomerDto Create(
            Guid id,
            string name,
            string surname,
            string email,
            bool isActive,
            List<CustomerAddressDto> address)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                Email = email,
                IsActive = isActive,
                Address = address
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.Address));
        }
    }
}