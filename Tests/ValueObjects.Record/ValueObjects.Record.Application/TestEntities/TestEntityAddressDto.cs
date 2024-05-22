using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.Application.Common.Mappings;
using ValueObjects.Record.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    public class TestEntityAddressDto : IMapFrom<Address>
    {
        public TestEntityAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Country = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public AddressType AddressType { get; set; }

        public static TestEntityAddressDto Create(
            string line1,
            string line2,
            string city,
            string country,
            AddressType addressType)
        {
            return new TestEntityAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Country = country,
                AddressType = addressType
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Address, TestEntityAddressDto>();
        }
    }
}