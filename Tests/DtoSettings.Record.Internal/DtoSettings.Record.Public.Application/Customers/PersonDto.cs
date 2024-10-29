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