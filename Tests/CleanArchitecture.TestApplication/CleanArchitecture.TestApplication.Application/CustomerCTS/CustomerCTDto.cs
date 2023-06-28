using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities.ComplexTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.CustomerCTS
{
    public class CustomerCTDto : IMapFrom<CustomerCT>
    {
        public CustomerCTDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CustomerCTDto Create(Guid id, string name)
        {
            return new CustomerCTDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerCT, CustomerCTDto>();
        }
    }
}