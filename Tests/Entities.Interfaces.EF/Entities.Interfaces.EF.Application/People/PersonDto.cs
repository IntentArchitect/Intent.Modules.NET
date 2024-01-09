using System;
using AutoMapper;
using Entities.Interfaces.EF.Application.Common.Mappings;
using Entities.Interfaces.EF.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.People
{
    public class PersonDto : IMapFrom<Person>
    {
        public PersonDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static PersonDto Create(Guid id, string name)
        {
            return new PersonDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Person, PersonDto>();
        }
    }
}