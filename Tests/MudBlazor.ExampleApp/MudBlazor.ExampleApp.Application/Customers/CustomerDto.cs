using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Application.Common.Mappings;
using MudBlazor.ExampleApp.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
            AddressLine1 = null!;
            AddressCity = null!;
            AddressCountry = null!;
            AddressPostal = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? AccountNo { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressPostal { get; set; }

        public static CustomerDto Create(
            Guid id,
            string name,
            string? accountNo,
            string addressLine1,
            string? addressLine2,
            string addressCity,
            string addressCountry,
            string addressPostal)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                AccountNo = accountNo,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                AddressCity = addressCity,
                AddressCountry = addressCountry,
                AddressPostal = addressPostal
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.AddressLine1, opt => opt.MapFrom(src => src.Address.Line1))
                .ForMember(d => d.AddressLine2, opt => opt.MapFrom(src => src.Address.Line2))
                .ForMember(d => d.AddressCity, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(d => d.AddressCountry, opt => opt.MapFrom(src => src.Address.Country))
                .ForMember(d => d.AddressPostal, opt => opt.MapFrom(src => src.Address.Postal));
        }
    }
}