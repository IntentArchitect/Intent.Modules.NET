using System;
using AutoMapper;
using FastEndpointsTest.Application.Common.Mappings;
using FastEndpointsTest.Domain.Entities.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.Pagination
{
    public class PersonEntryDto : IMapFrom<PersonEntry>
    {
        public PersonEntryDto()
        {
            FirstName = null!;
            LastName = null!;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static PersonEntryDto Create(Guid id, string firstName, string lastName)
        {
            return new PersonEntryDto
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PersonEntry, PersonEntryDto>();
        }
    }
}