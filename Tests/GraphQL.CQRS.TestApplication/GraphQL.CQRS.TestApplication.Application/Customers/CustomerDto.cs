using System;
using System.Collections.Generic;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Application.Common.Mappings;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;

        public static CustomerDto Create(Guid id, string name, string surname, string email)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                Email = email
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}