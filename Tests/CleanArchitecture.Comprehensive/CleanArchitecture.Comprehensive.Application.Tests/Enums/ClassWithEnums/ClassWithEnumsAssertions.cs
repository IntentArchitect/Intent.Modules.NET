using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.CreateClassWithEnums;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.UpdateClassWithEnums;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Enums.ClassWithEnums
{
    public static class ClassWithEnumsAssertions
    {
        public static void AssertEquivalent(
            CreateClassWithEnumsCommand expectedDto,
            Domain.Entities.Enums.ClassWithEnums actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.EnumWithDefaultLiteral.Should().Be(expectedDto.EnumWithDefaultLiteral);
            actualEntity.EnumWithoutDefaultLiteral.Should().Be(expectedDto.EnumWithoutDefaultLiteral);
            actualEntity.EnumWithoutValues.Should().Be(expectedDto.EnumWithoutValues);
            actualEntity.NullibleEnumWithDefaultLiteral.Should().Be(expectedDto.NullibleEnumWithDefaultLiteral);
            actualEntity.NullibleEnumWithoutDefaultLiteral.Should().Be(expectedDto.NullibleEnumWithoutDefaultLiteral);
            actualEntity.NullibleEnumWithoutValues.Should().Be(expectedDto.NullibleEnumWithoutValues);
        }

        public static void AssertEquivalent(
            IEnumerable<ClassWithEnumsDto> actualDtos,
            IEnumerable<Domain.Entities.Enums.ClassWithEnums> expectedEntities)
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
                dto.EnumWithDefaultLiteral.Should().Be(entity.EnumWithDefaultLiteral);
                dto.EnumWithoutDefaultLiteral.Should().Be(entity.EnumWithoutDefaultLiteral);
                dto.EnumWithoutValues.Should().Be(entity.EnumWithoutValues);
                dto.NullibleEnumWithDefaultLiteral.Should().Be(entity.NullibleEnumWithDefaultLiteral.Value);
                dto.NullibleEnumWithoutDefaultLiteral.Should().Be(entity.NullibleEnumWithoutDefaultLiteral.Value);
                dto.NullibleEnumWithoutValues.Should().Be(entity.NullibleEnumWithoutValues.Value);
            }
        }

        public static void AssertEquivalent(
            ClassWithEnumsDto actualDto,
            Domain.Entities.Enums.ClassWithEnums expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.EnumWithDefaultLiteral.Should().Be(expectedEntity.EnumWithDefaultLiteral);
            actualDto.EnumWithoutDefaultLiteral.Should().Be(expectedEntity.EnumWithoutDefaultLiteral);
            actualDto.EnumWithoutValues.Should().Be(expectedEntity.EnumWithoutValues);
            actualDto.NullibleEnumWithDefaultLiteral.Should().Be(expectedEntity.NullibleEnumWithDefaultLiteral.Value);
            actualDto.NullibleEnumWithoutDefaultLiteral.Should().Be(expectedEntity.NullibleEnumWithoutDefaultLiteral.Value);
            actualDto.NullibleEnumWithoutValues.Should().Be(expectedEntity.NullibleEnumWithoutValues.Value);
        }

        public static void AssertEquivalent(
            UpdateClassWithEnumsCommand expectedDto,
            Domain.Entities.Enums.ClassWithEnums actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.EnumWithDefaultLiteral.Should().Be(expectedDto.EnumWithDefaultLiteral);
            actualEntity.EnumWithoutDefaultLiteral.Should().Be(expectedDto.EnumWithoutDefaultLiteral);
            actualEntity.EnumWithoutValues.Should().Be(expectedDto.EnumWithoutValues);
            actualEntity.NullibleEnumWithDefaultLiteral.Should().Be(expectedDto.NullibleEnumWithDefaultLiteral);
            actualEntity.NullibleEnumWithoutDefaultLiteral.Should().Be(expectedDto.NullibleEnumWithoutDefaultLiteral);
            actualEntity.NullibleEnumWithoutValues.Should().Be(expectedDto.NullibleEnumWithoutValues);
        }
    }
}