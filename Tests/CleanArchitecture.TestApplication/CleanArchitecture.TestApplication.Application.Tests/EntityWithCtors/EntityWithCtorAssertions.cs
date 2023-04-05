using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.EntityWithCtors;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.CreateEntityWithCtor;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.UpdateEntityWithCtor;
using CleanArchitecture.TestApplication.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithCtors
{
    public static class EntityWithCtorAssertions
    {
        public static void AssertEquivalent(UpdateEntityWithCtorCommand expectedDto, EntityWithCtor actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
        }

        public static void AssertEquivalent(EntityWithCtorDto actualDto, EntityWithCtor expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.Name.Should().Be(expectedEntity.Name);
        }

        public static void AssertEquivalent(IEnumerable<EntityWithCtorDto> actualDtos, IEnumerable<EntityWithCtor> expectedEntities)
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
                dto.Name.Should().Be(entity.Name);
            }
        }

        public static void AssertEquivalent(CreateEntityWithCtorCommand expectedDto, EntityWithCtor actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
        }
    }
}