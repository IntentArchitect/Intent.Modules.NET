using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.TestNullablities;
using CleanArchitecture.TestApplication.Application.TestNullablities.UpdateTestNullablity;
using CleanArchitecture.TestApplication.Domain.Entities.Nullability;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.Nullability.TestNullablities
{
    public static class TestNullablityAssertions
    {
        public static void AssertEquivalent(TestNullablityDto actualDto, TestNullablity expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.MyEnum.Should().Be(expectedEntity.MyEnum);
            actualDto.Str.Should().Be(expectedEntity.Str);
            actualDto.Date.Should().Be(expectedEntity.Date);
            actualDto.DateTime.Should().Be(expectedEntity.DateTime);
            actualDto.NullableGuid.Should().Be(expectedEntity.NullableGuid);
            actualDto.NullableEnum.Should().Be(expectedEntity.NullableEnum);
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
            actualEntity.MyEnum.Should().Be(expectedDto.MyEnum);
            actualEntity.Str.Should().Be(expectedDto.Str);
            actualEntity.Date.Should().Be(expectedDto.Date);
            actualEntity.DateTime.Should().Be(expectedDto.DateTime);
            actualEntity.NullableGuid.Should().Be(expectedDto.NullableGuid);
            actualEntity.NullableEnum.Should().Be(expectedDto.NullableEnum);
            actualEntity.NullabilityPeerId.Should().Be(expectedDto.NullabilityPeerId);
        }
    }
}