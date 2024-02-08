using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
            AddressLine1 = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }

        public static CustomerDto Create(Guid id, string name, string addressLine1)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                AddressLine1 = addressLine1
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.AddressLine1, opt => opt.MapFrom(src => src.Address.Line1));
        }
    }
}