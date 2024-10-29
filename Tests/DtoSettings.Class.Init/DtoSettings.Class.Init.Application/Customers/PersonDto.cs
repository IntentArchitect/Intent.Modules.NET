using System;
using AutoMapper;
using DtoSettings.Class.Init.Application.Common.Mappings;
using DtoSettings.Class.Init.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.Customers
{
    public class PersonDto : IMapFrom<Person>
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