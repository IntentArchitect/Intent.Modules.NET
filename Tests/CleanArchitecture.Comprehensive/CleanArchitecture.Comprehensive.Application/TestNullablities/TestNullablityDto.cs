using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.Nullability;
using CleanArchitecture.Comprehensive.Domain.Nullability;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities
{
    public class TestNullablityDto : IMapFrom<TestNullablity>
    {
        public TestNullablityDto()
        {
            Str = null!;
        }

        public Guid Id { get; set; }
        public NoDefaultLiteralEnum MyEnum { get; set; }
        public string Str { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? NullableGuid { get; set; }
        public NoDefaultLiteralEnum? NullableEnum { get; set; }
        public Guid NullabilityPeerId { get; set; }

        public static TestNullablityDto Create(
            Guid id,
            NoDefaultLiteralEnum myEnum,
            string str,
            DateTime date,
            DateTime dateTime,
            Guid? nullableGuid,
            NoDefaultLiteralEnum? nullableEnum,
            Guid nullabilityPeerId)
        {
            return new TestNullablityDto
            {
                Id = id,
                MyEnum = myEnum,
                Str = str,
                Date = date,
                DateTime = dateTime,
                NullableGuid = nullableGuid,
                NullableEnum = nullableEnum,
                NullabilityPeerId = nullabilityPeerId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TestNullablity, TestNullablityDto>()
                .ForMember(d => d.MyEnum, opt => opt.MapFrom(src => src.SampleEnum));
        }
    }
}