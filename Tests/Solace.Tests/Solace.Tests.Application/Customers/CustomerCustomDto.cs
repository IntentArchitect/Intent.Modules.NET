using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application.Common.Mappings;
using Solace.Tests.Domain.Contracts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Solace.Tests.Application.Customers
{
    public class CustomerCustomDto : IMapFrom<CustomerCustom>
    {
        public CustomerCustomDto()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }

        public static CustomerCustomDto Create(string name, string surname)
        {
            return new CustomerCustomDto
            {
                Name = name,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerCustom, CustomerCustomDto>();
        }
    }
}