using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.VariantTypesClasses;
using CleanArchitecture.TestApplication.Application.VariantTypesClasses.CreateVariantTypesClass;
using CleanArchitecture.TestApplication.Application.VariantTypesClasses.UpdateVariantTypesClass;
using CleanArchitecture.TestApplication.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.VariantTypesClasses
{
    public static class VariantTypesClassAssertions
    {
        public static void AssertEquivalent(UpdateVariantTypesClassCommand expectedDto, VariantTypesClass actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.StrCollection.Should().BeEquivalentTo(expectedDto.StrCollection);
            actualEntity.IntCollection.Should().BeEquivalentTo(expectedDto.IntCollection);
            actualEntity.StrNullCollection.Should().BeEquivalentTo(expectedDto.StrNullCollection);
            actualEntity.IntNullCollection.Should().BeEquivalentTo(expectedDto.IntNullCollection);
            actualEntity.NullStr.Should().Be(expectedDto.NullStr);
            actualEntity.NullInt.Should().Be(expectedDto.NullInt);
        }

        public static void AssertEquivalent(VariantTypesClassDto actualDto, VariantTypesClass expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.StrCollection.Should().BeEquivalentTo(expectedEntity.StrCollection);
            actualDto.IntCollection.Should().BeEquivalentTo(expectedEntity.IntCollection);
            actualDto.StrNullCollection.Should().BeEquivalentTo(expectedEntity.StrNullCollection);
            actualDto.IntNullCollection.Should().BeEquivalentTo(expectedEntity.IntNullCollection);
            actualDto.NullStr.Should().Be(expectedEntity.NullStr);
            actualDto.NullInt.Should().Be(expectedEntity.NullInt);
        }

        public static void AssertEquivalent(IEnumerable<VariantTypesClassDto> actualDtos, IEnumerable<VariantTypesClass> expectedEntities)
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
                dto.StrCollection.Should().BeEquivalentTo(entity.StrCollection);
                dto.IntCollection.Should().BeEquivalentTo(entity.IntCollection);
                dto.StrNullCollection.Should().BeEquivalentTo(entity.StrNullCollection);
                dto.IntNullCollection.Should().BeEquivalentTo(entity.IntNullCollection);
                dto.NullStr.Should().Be(entity.NullStr);
                dto.NullInt.Should().Be(entity.NullInt);
            }
        }

        public static void AssertEquivalent(CreateVariantTypesClassCommand expectedDto, VariantTypesClass actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.StrCollection.Should().BeEquivalentTo(expectedDto.StrCollection);
            actualEntity.IntCollection.Should().BeEquivalentTo(expectedDto.IntCollection);
            actualEntity.StrNullCollection.Should().BeEquivalentTo(expectedDto.StrNullCollection);
            actualEntity.IntNullCollection.Should().BeEquivalentTo(expectedDto.IntNullCollection);
            actualEntity.NullStr.Should().Be(expectedDto.NullStr);
            actualEntity.NullInt.Should().Be(expectedDto.NullInt);
        }
    }
}