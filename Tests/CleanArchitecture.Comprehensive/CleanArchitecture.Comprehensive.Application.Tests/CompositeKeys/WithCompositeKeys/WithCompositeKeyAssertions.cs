using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.CreateWithCompositeKey;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.UpdateWithCompositeKey;
using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.CompositeKeys.WithCompositeKeys
{
    public static class WithCompositeKeyAssertions
    {
        public static void AssertEquivalent(CreateWithCompositeKeyCommand expectedDto, WithCompositeKey actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
        }

        public static void AssertEquivalent(
            IEnumerable<WithCompositeKeyDto> actualDtos,
            IEnumerable<WithCompositeKey> expectedEntities)
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
                dto.Key1Id.Should().Be(entity.Key1Id);
                dto.Key2Id.Should().Be(entity.Key2Id);
                dto.Name.Should().Be(entity.Name);
            }
        }

        public static void AssertEquivalent(WithCompositeKeyDto actualDto, WithCompositeKey expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Key1Id.Should().Be(expectedEntity.Key1Id);
            actualDto.Key2Id.Should().Be(expectedEntity.Key2Id);
            actualDto.Name.Should().Be(expectedEntity.Name);
        }

        public static void AssertEquivalent(UpdateWithCompositeKeyCommand expectedDto, WithCompositeKey actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
            actualEntity.Key1Id.Should().Be(expectedDto.Key1Id);
            actualEntity.Key2Id.Should().Be(expectedDto.Key2Id);
        }
    }
}