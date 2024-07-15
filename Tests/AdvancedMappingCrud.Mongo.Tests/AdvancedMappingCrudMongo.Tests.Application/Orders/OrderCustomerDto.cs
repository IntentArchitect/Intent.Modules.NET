using AdvancedMappingCrudMongo.Tests.Application.Common.Mappings;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders
{
    public class OrderCustomerDto : IMapFrom<Customer>
    {
        public OrderCustomerDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }

        public static OrderCustomerDto Create(string name, string surname, string email, string id)
        {
            return new OrderCustomerDto
            {
                Name = name,
                Surname = surname,
                Email = email,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, OrderCustomerDto>();
        }
    }
}