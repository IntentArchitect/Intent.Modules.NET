using System;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

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