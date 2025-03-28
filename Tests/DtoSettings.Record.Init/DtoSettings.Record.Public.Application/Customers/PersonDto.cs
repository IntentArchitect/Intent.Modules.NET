using System;
using AutoMapper;
using DtoSettings.Record.Public.Application.Common.Mappings;
using DtoSettings.Record.Public.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Customers
{
    public record PersonDto : IMapFrom<Person>
    {
        public PersonDto()
        {
        }

        public Guid Id { get; init; }
        public DateTime CreatedDate { get; init; }

        public static PersonDto Create(Guid id, DateTime createdDate)
        {
            return new PersonDto
            {
                Id = id,
                CreatedDate = createdDate
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Person, PersonDto>();
        }
    }
}