using System;
using System.Collections.Generic;
using AutoMapper;
using GraphQL.AzureFunction.TestApplication.Application.Common.Mappings;
using GraphQL.AzureFunction.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
            LastName = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public static CustomerDto Create(Guid id, string name, string lastName)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                LastName = lastName
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}