using System;
using AutoMapper;
using DtoSettings.Record.Public.Application.Common.Mappings;
using DtoSettings.Record.Public.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Customers
{
    public record CustomerDto : PersonDto, IMapFrom<Customer>
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