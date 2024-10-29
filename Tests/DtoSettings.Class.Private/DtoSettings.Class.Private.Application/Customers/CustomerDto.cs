using System;
using AutoMapper;
using DtoSettings.Class.Private.Application.Common.Mappings;
using DtoSettings.Class.Private.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.Customers
{
    public class CustomerDto : PersonDto, IMapFrom<Customer>
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

        public string Name { get; private set; }
        public string Surname { get; private set; }

        public static CustomerDto Create(Guid id, DateTime createdDate, string name, string surname)
        {
            return new CustomerDto(id, createdDate, name, surname);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}