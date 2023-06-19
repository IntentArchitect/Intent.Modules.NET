using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities.Nullability;
using CleanArchitecture.TestApplication.Domain.Nullability;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities
{
    public class TestNullablityDto : IMapFrom<TestNullablity>
    {
        public TestNullablityDto()
        {
            Str = null!;
        }

        public Guid Id { get; set; }
        public MyEnum MyEnum { get; set; }
        public string Str { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? NullableGuid { get; set; }
        public MyEnum? NullableEnum { get; set; }
        public Guid NullabilityPeerId { get; set; }

        public static TestNullablityDto Create(
            Guid id,
            MyEnum myEnum,
            string str,
            DateTime date,
            DateTime dateTime,
            Guid? nullableGuid,
            MyEnum? nullableEnum,
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
            profile.CreateMap<TestNullablity, TestNullablityDto>();
        }
    }
}