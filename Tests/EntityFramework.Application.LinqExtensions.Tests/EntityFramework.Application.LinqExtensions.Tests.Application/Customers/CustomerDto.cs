using AutoMapper;
using EntityFramework.Application.LinqExtensions.Tests.Application.Common.Mappings;
using EntityFramework.Application.LinqExtensions.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFramework.Application.LinqExtensions.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }

        public static CustomerDto Create(Guid id, string name, string surname, bool isActive)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                IsActive = isActive
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>();
        }
    }
}