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
        public CustomerDto(Guid id, DateTime createdDate, string name, string surname) : base(id, createdDate)
        {
            Name = name;
            Surname = surname;
        }

        protected CustomerDto()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; internal set; }
        public string Surname { get; internal set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}