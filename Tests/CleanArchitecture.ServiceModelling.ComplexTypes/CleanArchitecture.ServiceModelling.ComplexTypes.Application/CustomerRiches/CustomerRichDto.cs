using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Mappings;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches
{
    public class CustomerRichDto : IMapFrom<CustomerRich>
    {
        public CustomerRichDto()
        {
            Address = null!;
        }

        public Guid Id { get; set; }
        public CustomerRichAddressDto Address { get; set; }

        public static CustomerRichDto Create(Guid id, CustomerRichAddressDto address)
        {
            return new CustomerRichDto
            {
                Id = id,
                Address = address
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerRich, CustomerRichDto>();
        }
    }
}