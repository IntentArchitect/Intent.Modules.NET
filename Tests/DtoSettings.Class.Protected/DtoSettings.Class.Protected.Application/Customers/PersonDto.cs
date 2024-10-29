using System;
using AutoMapper;
using DtoSettings.Class.Protected.Application.Common.Mappings;
using DtoSettings.Class.Protected.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Customers
{
    public class PersonDto : IMapFrom<Person>
    {
        public PersonDto(Guid id, DateTime createdDate)
        {
            Id = id;
            CreatedDate = createdDate;
        }

        protected PersonDto()
        {
        }

        public Guid Id { get; protected set; }
        public DateTime CreatedDate { get; protected set; }

        public static PersonDto Create(Guid id, DateTime createdDate)
        {
            return new PersonDto(id, createdDate);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Person, PersonDto>();
        }
    }
}