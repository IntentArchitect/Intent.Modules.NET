using System;
using AutoMapper;
using DtoSettings.Class.Internal.Application.Common.Mappings;
using DtoSettings.Class.Internal.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Customers
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

        public Guid Id { get; internal set; }
        public DateTime CreatedDate { get; internal set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Person, PersonDto>();
        }
    }
}