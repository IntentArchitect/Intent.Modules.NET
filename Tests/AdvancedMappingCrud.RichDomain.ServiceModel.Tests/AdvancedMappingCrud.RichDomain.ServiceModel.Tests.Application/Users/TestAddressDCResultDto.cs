using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class TestAddressDCResultDto : IMapFrom<AddressDC>
    {
        public TestAddressDCResultDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public int Postal { get; set; }

        public static TestAddressDCResultDto Create(string line1, string line2, string city, int postal)
        {
            return new TestAddressDCResultDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AddressDC, TestAddressDCResultDto>();
        }
    }
}