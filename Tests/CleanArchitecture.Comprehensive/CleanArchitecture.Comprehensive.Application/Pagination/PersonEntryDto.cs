using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination
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