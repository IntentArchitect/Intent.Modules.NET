using System.Collections.Generic;
using System.Linq;
using CosmosDB.Application.DerivedOfTS;
using CosmosDB.Application.DerivedOfTS.CreateDerivedOfT;
using CosmosDB.Application.DerivedOfTS.UpdateDerivedOfT;
using CosmosDB.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CosmosDB.Application.Tests.DerivedOfTs
{
    public static class DerivedOfTAssertions
    {
        public static void AssertEquivalent(CreateDerivedOfTCommand expectedDto, DerivedOfT actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.DerivedAttribute.Should().Be(expectedDto.DerivedAttribute);
            actualEntity.GenericAttribute.Should().Be(expectedDto.GenericAttribute);
        }

        public static void AssertEquivalent(
            IEnumerable<DerivedOfTDto> actualDtos,
            IEnumerable<DerivedOfT> expectedEntities)
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
                dto.DerivedAttribute.Should().Be(entity.DerivedAttribute);
                dto.GenericAttribute.Should().Be(entity.GenericAttribute);
            }
        }

        public static void AssertEquivalent(DerivedOfTDto actualDto, DerivedOfT expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.DerivedAttribute.Should().Be(expectedEntity.DerivedAttribute);
            actualDto.GenericAttribute.Should().Be(expectedEntity.GenericAttribute);
        }

        public static void AssertEquivalent(UpdateDerivedOfTCommand expectedDto, DerivedOfT actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.DerivedAttribute.Should().Be(expectedDto.DerivedAttribute);
            actualEntity.GenericAttribute.Should().Be(expectedDto.GenericAttribute);
        }
    }
}