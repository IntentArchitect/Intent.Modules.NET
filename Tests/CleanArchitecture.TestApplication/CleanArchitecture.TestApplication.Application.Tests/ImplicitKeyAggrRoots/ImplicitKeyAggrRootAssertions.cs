using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public static class ImplicitKeyAggrRootAssertions
    {
        public static void AssertEquivalent(UpdateImplicitKeyAggrRootCommand expectedDto, ImplicitKeyAggrRoot actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
            AssertEquivalent(expectedDto.ImplicitKeyNestedCompositions, actualEntity.ImplicitKeyNestedCompositions);
        }

        public static void AssertEquivalent(
            IEnumerable<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto> expectedDtos,
            IEnumerable<ImplicitKeyNestedComposition> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(actualEntities);
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
                entity.Attribute.Should().Be(dto.Attribute);
            }
        }

        public static void AssertEquivalent(ImplicitKeyAggrRootDto actualDto, ImplicitKeyAggrRoot expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.Attribute.Should().Be(expectedEntity.Attribute);
            AssertEquivalent(actualDto.ImplicitKeyNestedCompositions, expectedEntity.ImplicitKeyNestedCompositions);
        }

        public static void AssertEquivalent(
            IEnumerable<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto> actualDtos,
            IEnumerable<ImplicitKeyNestedComposition> expectedEntities)
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
                dto.Attribute.Should().Be(entity.Attribute);
                dto.Id.Should().Be(entity.Id);
            }
        }

        public static void AssertEquivalent(
            IEnumerable<ImplicitKeyAggrRootDto> actualDtos,
            IEnumerable<ImplicitKeyAggrRoot> expectedEntities)
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
                AssertEquivalent(dto.ImplicitKeyNestedCompositions, entity.ImplicitKeyNestedCompositions);
            }
        }

        public static void AssertEquivalent(CreateImplicitKeyAggrRootCommand expectedDto, ImplicitKeyAggrRoot actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
            AssertEquivalent(expectedDto.ImplicitKeyNestedCompositions, actualEntity.ImplicitKeyNestedCompositions);
        }

        public static void AssertEquivalent(
            IEnumerable<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto> expectedDtos,
            IEnumerable<ImplicitKeyNestedComposition> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(actualEntities);
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
                entity.Attribute.Should().Be(dto.Attribute);
            }
        }

        public static void AssertEquivalent(
            UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand expectedDto,
            ImplicitKeyNestedComposition actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.ImplicitKeyAggrRootId.Should().Be(expectedDto.ImplicitKeyAggrRootId);
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
        }

        public static void AssertEquivalent(
            ImplicitKeyAggrRootImplicitKeyNestedCompositionDto actualDto,
            ImplicitKeyNestedComposition expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Attribute.Should().Be(expectedEntity.Attribute);
            actualDto.Id.Should().Be(expectedEntity.Id);
        }

        public static void AssertEquivalent(
            CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand expectedDto,
            ImplicitKeyNestedComposition actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.ImplicitKeyAggrRootId.Should().Be(expectedDto.ImplicitKeyAggrRootId);
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
        }
    }
}