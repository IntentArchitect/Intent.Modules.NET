using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.TestNullablities;
using CleanArchitecture.Comprehensive.Application.TestNullablities.UpdateTestNullablity;
using CleanArchitecture.Comprehensive.Domain.Entities.Nullability;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Nullability.TestNullablities
{
    public static class TestNullablityAssertions
    {

        public static void AssertEquivalent(
            IEnumerable<TestNullablityDto> actualDtos,
            IEnumerable<TestNullablity> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().BeNullOrEmpty();
                return;
            }

            actualDtos.Should().HaveSameCount(actualDtos);
            for (int i = 0; i < expectedEntities.Count(); i++)
            {
                var entity = expectedEntities.ElementAt(i);
                var dto = actualDtos.ElementAt(i);
                if (entity == null)
                {
                    dto.Should().BeNull();
                    continue;
                }

                dto.Should().NotBeNull();
                dto.Id.Should().Be(entity.Id);
                dto.MyEnum.Should().Be(entity.SampleEnum);
                dto.Str.Should().Be(entity.Str);
                dto.Date.Should().Be(entity.Date);
                dto.DateTime.Should().Be(entity.DateTime);
                dto.NullableGuid.Should().Be(entity.NullableGuid.Value);
                dto.NullableEnum.Should().Be(entity.NullableEnum.Value);
                dto.NullabilityPeerId.Should().Be(entity.NullabilityPeerId);
            }
        }

        public static void AssertEquivalent(TestNullablityDto actualDto, TestNullablity expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.MyEnum.Should().Be(expectedEntity.SampleEnum);
            actualDto.Str.Should().Be(expectedEntity.Str);
            actualDto.Date.Should().Be(expectedEntity.Date);
            actualDto.DateTime.Should().Be(expectedEntity.DateTime);
            actualDto.NullableGuid.Should().Be(expectedEntity.NullableGuid.Value);
            actualDto.NullableEnum.Should().Be(expectedEntity.NullableEnum.Value);
            actualDto.NullabilityPeerId.Should().Be(expectedEntity.NullabilityPeerId);
        }

        public static void AssertEquivalent(UpdateTestNullablityCommand expectedDto, TestNullablity actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.SampleEnum.Should().Be(expectedDto.SampleEnum);
            actualEntity.Str.Should().Be(expectedDto.Str);
            actualEntity.Date.Should().Be(expectedDto.Date);
            actualEntity.DateTime.Should().Be(expectedDto.DateTime);
            actualEntity.NullableGuid.Should().Be(expectedDto.NullableGuid);
            actualEntity.NullableEnum.Should().Be(expectedDto.NullableEnum);
            actualEntity.NullabilityPeerId.Should().Be(expectedDto.NullabilityPeerId);
            actualEntity.DefaultLiteralEnum.Should().Be(expectedDto.DefaultLiteralEnum);
        }
    }
}