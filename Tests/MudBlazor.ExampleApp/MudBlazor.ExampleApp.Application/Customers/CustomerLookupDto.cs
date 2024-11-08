using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Application.Common.Mappings;
using MudBlazor.ExampleApp.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Customers
{
    public class CustomerLookupDto : IMapFrom<Customer>
    {
        public CustomerLookupDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CustomerLookupDto Create(Guid id, string name)
        {
            return new CustomerLookupDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerLookupDto>();
        }
    }
}