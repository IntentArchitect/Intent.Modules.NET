using System;
using AutoMapper;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Application.Common.Mappings;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CustomerDto Create(Guid id, string name)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}