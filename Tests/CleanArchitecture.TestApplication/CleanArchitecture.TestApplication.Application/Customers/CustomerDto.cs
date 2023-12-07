using System;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Email = null!;
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public static CustomerDto Create(Guid id, string email, string name, string surname)
        {
            return new CustomerDto
            {
                Id = id,
                Email = email,
                Name = name,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}