using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Application.Common.Mappings;
using TrainingModel.Tests.Domain;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TrainingModel.Tests.Application.Customers
{
    public class CustomerAddressDto : IMapFrom<Address>
    {
        public CustomerAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Postal = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }
        public AddressType AddressType { get; set; }
        public Guid CustomersId { get; set; }
        public Guid Id { get; set; }

        public static CustomerAddressDto Create(
            string line1,
            string line2,
            string city,
            string postal,
            AddressType addressType,
            Guid customersId,
            Guid id)
        {
            return new CustomerAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal,
                AddressType = addressType,
                CustomersId = customersId,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Address, CustomerAddressDto>();
        }
    }
}