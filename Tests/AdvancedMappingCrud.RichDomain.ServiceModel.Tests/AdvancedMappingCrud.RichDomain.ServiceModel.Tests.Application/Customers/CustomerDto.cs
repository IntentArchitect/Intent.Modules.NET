using System;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Login = null!;
        }

        public string Login { get; set; }
        public Guid UserId { get; set; }
        public Guid Id { get; set; }

        public static CustomerDto Create(string login, Guid userId, Guid id)
        {
            return new CustomerDto
            {
                Login = login,
                UserId = userId,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}