using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Id = null!;
            Name = null!;
            Surname = null!;
            ShippingAddress = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public CustomerAddressDto ShippingAddress { get; set; }

        public static CustomerDto Create(
            string id,
            string name,
            string surname,
            bool isActive,
            CustomerAddressDto shippingAddress)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                IsActive = isActive,
                ShippingAddress = shippingAddress
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();

            profile.CreateMap<ICustomerDocument, CustomerDto>();
        }
    }
}