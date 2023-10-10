using System.Collections.Generic;
using System.Linq;
using CosmosDB.Application.ClassContainers;
using CosmosDB.Application.ClassContainers.CreateClassContainer;
using CosmosDB.Application.ClassContainers.UpdateClassContainer;
using CosmosDB.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CosmosDB.Application.Tests.ClassContainers
{
    public static class ClassContainerAssertions
    {
        public static void AssertEquivalent(CreateClassContainerCommand expectedDto, ClassContainer actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.ClassPartitionKey.Should().Be(expectedDto.ClassPartitionKey);
        }

        public static void AssertEquivalent(
            IEnumerable<ClassContainerDto> actualDtos,
            IEnumerable<ClassContainer> expectedEntities)
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
                dto.ClassPartitionKey.Should().Be(entity.ClassPartitionKey);
            }
        }

        public static void AssertEquivalent(ClassContainerDto actualDto, ClassContainer expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.ClassPartitionKey.Should().Be(expectedEntity.ClassPartitionKey);
        }

        public static void AssertEquivalent(UpdateClassContainerCommand expectedDto, ClassContainer actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.ClassPartitionKey.Should().Be(expectedDto.ClassPartitionKey);
        }
    }
}