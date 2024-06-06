using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.CreateConcreteClass;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.UpdateConcreteClass;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Inheritance.ConcreteClasses
{
    public static class ConcreteClassAssertions
    {
        public static void AssertEquivalent(CreateConcreteClassCommand expectedDto, ConcreteClass actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.ConcreteAttr.Should().Be(expectedDto.ConcreteAttr);
            actualEntity.BaseAttr.Should().Be(expectedDto.BaseAttr);
        }

        public static void AssertEquivalent(
            IEnumerable<ConcreteClassDto> actualDtos,
            IEnumerable<ConcreteClass> expectedEntities)
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
                dto.ConcreteAttr.Should().Be(entity.ConcreteAttr);
                dto.BaseAttr.Should().Be(entity.BaseAttr);
            }
        }

        public static void AssertEquivalent(ConcreteClassDto actualDto, ConcreteClass expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.ConcreteAttr.Should().Be(expectedEntity.ConcreteAttr);
            actualDto.BaseAttr.Should().Be(expectedEntity.BaseAttr);
        }

        public static void AssertEquivalent(UpdateConcreteClassCommand expectedDto, ConcreteClass actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.ConcreteAttr.Should().Be(expectedDto.ConcreteAttr);
            actualEntity.BaseAttr.Should().Be(expectedDto.BaseAttr);
        }
    }
}