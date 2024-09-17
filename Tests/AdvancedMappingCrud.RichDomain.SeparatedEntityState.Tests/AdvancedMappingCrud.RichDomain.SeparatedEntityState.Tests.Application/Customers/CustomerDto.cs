using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Login = null!;
        }

        public Guid Id { get; set; }
        public string Login { get; set; }
        public Guid UserId { get; set; }

        public static CustomerDto Create(Guid id, string login, Guid userId)
        {
            return new CustomerDto
            {
                Id = id,
                Login = login,
                UserId = userId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}