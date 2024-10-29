using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.MultiTenancy.SeperateDb.Application.Common.Mappings;
using MongoDb.MultiTenancy.SeperateDb.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public static CustomerDto Create(string id, string name)
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