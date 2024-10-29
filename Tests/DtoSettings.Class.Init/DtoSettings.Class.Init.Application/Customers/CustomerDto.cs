using System;
using AutoMapper;
using DtoSettings.Class.Init.Application.Common.Mappings;
using DtoSettings.Class.Init.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.Customers
{
    public class CustomerDto : PersonDto, IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; init; }
        public string Surname { get; init; }

        public static CustomerDto Create(Guid id, DateTime createdDate, string name, string surname)
        {
            return new CustomerDto
            {
                Id = id,
                CreatedDate = createdDate,
                Name = name,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}