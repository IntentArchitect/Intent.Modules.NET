using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Application.Common.Mappings;
using RichDomain.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace RichDomain.Application.People
{
    public class PersonDto : IMapFrom<Person>
    {
        public PersonDto()
        {
            FirstName = null!;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public Guid DepartmentId { get; set; }

        public static PersonDto Create(Guid id, string firstName, Guid departmentId)
        {
            return new PersonDto
            {
                Id = id,
                FirstName = firstName,
                DepartmentId = departmentId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Person, PersonDto>();
        }
    }
}