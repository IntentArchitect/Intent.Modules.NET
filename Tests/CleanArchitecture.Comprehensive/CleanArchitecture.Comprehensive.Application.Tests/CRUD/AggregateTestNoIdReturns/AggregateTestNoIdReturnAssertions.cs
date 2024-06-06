using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.CreateAggregateTestNoIdReturn;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.UpdateAggregateTestNoIdReturn;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateTestNoIdReturns
{
    public static class AggregateTestNoIdReturnAssertions
    {
        public static void AssertEquivalent(
            CreateAggregateTestNoIdReturnCommand expectedDto,
            AggregateTestNoIdReturn actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
        }

        public static void AssertEquivalent(
            IEnumerable<AggregateTestNoIdReturnDto> actualDtos,
            IEnumerable<AggregateTestNoIdReturn> expectedEntities)
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
                dto.Attribute.Should().Be(entity.Attribute);
            }
        }

        public static void AssertEquivalent(AggregateTestNoIdReturnDto actualDto, AggregateTestNoIdReturn expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.Attribute.Should().Be(expectedEntity.Attribute);
        }

        public static void AssertEquivalent(
            UpdateAggregateTestNoIdReturnCommand expectedDto,
            AggregateTestNoIdReturn actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
        }
    }
}