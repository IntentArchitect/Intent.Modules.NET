using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.UpdateAggregateRoot;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateRoots
{
    public static class AggregateRootAssertions
    {
        public static void AssertEquivalent(
            CreateAggregateRootCompositeManyBCommand expectedDto,
            CompositeManyB actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.AggregateRootId.Should().Be(expectedDto.AggregateRootId);
            actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
            actualEntity.SomeDate.Should().Be(expectedDto.SomeDate);
            AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
            AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
        }

        public static void AssertEquivalent(
            CreateAggregateRootCompositeManyBCompositeSingleBBDto expectedDto,
            CompositeSingleBB actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        }

        public static void AssertEquivalent(
            IEnumerable<CreateAggregateRootCompositeManyBCompositeManyBBDto> expectedDtos,
            IEnumerable<CompositeManyBB> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
            }
        }

        public static void AssertEquivalent(
            IEnumerable<AggregateRootCompositeManyBDto> actualDtos,
            IEnumerable<CompositeManyB> expectedEntities)
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
                dto.CompositeAttr.Should().Be(entity.CompositeAttr);
                dto.SomeDate.Should().Be(entity.SomeDate.Value);
                dto.AggregateRootId.Should().Be(entity.AggregateRootId);
                dto.Id.Should().Be(entity.Id);
            }
        }

        public static void AssertEquivalent(AggregateRootCompositeManyBDto actualDto, CompositeManyB expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
            actualDto.SomeDate.Should().Be(expectedEntity.SomeDate.Value);
            actualDto.AggregateRootId.Should().Be(expectedEntity.AggregateRootId);
            actualDto.Id.Should().Be(expectedEntity.Id);
        }

        public static void AssertEquivalent(
            UpdateAggregateRootCompositeManyBCommand expectedDto,
            CompositeManyB actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.AggregateRootId.Should().Be(expectedDto.AggregateRootId);
            actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
            actualEntity.SomeDate.Should().Be(expectedDto.SomeDate);
            AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
            AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
        }

        public static void AssertEquivalent(
            UpdateAggregateRootCompositeManyBCompositeSingleBBDto expectedDto,
            CompositeSingleBB actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        }

        public static void AssertEquivalent(
            IEnumerable<UpdateAggregateRootCompositeManyBCompositeManyBBDto> expectedDtos,
            IEnumerable<CompositeManyBB> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
                entity.CompositeManyBId.Should().Be(dto.CompositeManyBId);
            }
        }

        public static void AssertEquivalent(CreateAggregateRootCommand expectedDto, AggregateRoot actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.AggregateAttr.Should().Be(expectedDto.AggregateAttr);
            AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
            AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
#warning Field not a composite association: Aggregate
            actualEntity.LimitedDomain.Should().Be(expectedDto.LimitedDomain);
            actualEntity.LimitedService.Should().Be(expectedDto.LimitedService);
        }

        public static void AssertEquivalent(
            IEnumerable<CreateAggregateRootCompositeManyBDto> expectedDtos,
            IEnumerable<CompositeManyB> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
                entity.SomeDate.Should().Be(dto.SomeDate);
                AssertEquivalent(dto.Composite, entity.Composite);
                AssertEquivalent(dto.Composites, entity.Composites);
            }
        }

        public static void AssertEquivalent(
            CreateAggregateRootCompositeSingleADto expectedDto,
            CompositeSingleA actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
            AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
            AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
        }

        public static void AssertEquivalent(
            CreateAggregateRootCompositeSingleACompositeSingleAADto expectedDto,
            CompositeSingleAA actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        }

        public static void AssertEquivalent(
            IEnumerable<CreateAggregateRootCompositeSingleACompositeManyAADto> expectedDtos,
            IEnumerable<CompositeManyAA> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
            }
        }

        public static void AssertEquivalent(
            IEnumerable<AggregateRootDto> actualDtos,
            IEnumerable<AggregateRoot> expectedEntities)
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
                dto.AggregateAttr.Should().Be(entity.AggregateAttr);
                dto.EnumType1.Should().Be(entity.EnumType1);
                dto.EnumType2.Should().Be(entity.EnumType2);
                dto.EnumType3.Should().Be(entity.EnumType3);
                dto.LimitedDomain.Should().Be(entity.LimitedDomain);
                dto.LimitedService.Should().Be(entity.LimitedService);
            }
        }

        public static void AssertEquivalent(AggregateRootDto actualDto, AggregateRoot expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.AggregateAttr.Should().Be(expectedEntity.AggregateAttr);
            actualDto.EnumType1.Should().Be(expectedEntity.EnumType1);
            actualDto.EnumType2.Should().Be(expectedEntity.EnumType2);
            actualDto.EnumType3.Should().Be(expectedEntity.EnumType3);
            actualDto.LimitedDomain.Should().Be(expectedEntity.LimitedDomain);
            actualDto.LimitedService.Should().Be(expectedEntity.LimitedService);
        }

        public static void AssertEquivalent(UpdateAggregateRootCommand expectedDto, AggregateRoot actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.AggregateAttr.Should().Be(expectedDto.AggregateAttr);
            AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
            AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
#warning Field not a composite association: Aggregate
            actualEntity.LimitedDomain.Should().Be(expectedDto.LimitedDomain);
            actualEntity.LimitedService.Should().Be(expectedDto.LimitedService);
        }

        public static void AssertEquivalent(
            IEnumerable<UpdateAggregateRootCompositeManyBDto> expectedDtos,
            IEnumerable<CompositeManyB> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
                entity.SomeDate.Should().Be(dto.SomeDate);
                entity.AggregateRootId.Should().Be(dto.AggregateRootId);
                AssertEquivalent(dto.Composites, entity.Composites);
                AssertEquivalent(dto.Composite, entity.Composite);
            }
        }

        public static void AssertEquivalent(
            UpdateAggregateRootCompositeSingleADto expectedDto,
            CompositeSingleA actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
            AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
            AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
        }

        public static void AssertEquivalent(
            UpdateAggregateRootCompositeSingleACompositeSingleAADto expectedDto,
            CompositeSingleAA actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        }

        public static void AssertEquivalent(
            IEnumerable<UpdateAggregateRootCompositeSingleACompositeManyAADto> expectedDtos,
            IEnumerable<CompositeManyAA> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
                entity.CompositeSingleAId.Should().Be(dto.CompositeSingleAId);
            }
        }
    }
}