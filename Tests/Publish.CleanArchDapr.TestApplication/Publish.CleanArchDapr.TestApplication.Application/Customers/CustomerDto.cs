using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Application.Common.Mappings;
using Publish.CleanArchDapr.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
        }

        public Guid Id { get; set; }

        public static CustomerDto Create(Guid id)
        {
            return new CustomerDto
            {
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}