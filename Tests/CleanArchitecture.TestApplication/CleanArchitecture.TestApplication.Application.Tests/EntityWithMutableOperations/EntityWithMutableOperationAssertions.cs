using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.CreateEntityWithMutableOperation;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.UpdateEntityWithMutableOperation;
using CleanArchitecture.TestApplication.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithMutableOperations
{
    public static class EntityWithMutableOperationAssertions
    {
        public static void AssertEquivalent(UpdateEntityWithMutableOperationCommand expectedDto, EntityWithMutableOperation actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
        }

        public static void AssertEquivalent(EntityWithMutableOperationDto actualDto, EntityWithMutableOperation expectedEntity)
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

        public static void AssertEquivalent(IEnumerable<EntityWithMutableOperationDto> actualDtos, IEnumerable<EntityWithMutableOperation> expectedEntities)
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

        public static void AssertEquivalent(CreateEntityWithMutableOperationCommand expectedDto, EntityWithMutableOperation actualEntity)
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