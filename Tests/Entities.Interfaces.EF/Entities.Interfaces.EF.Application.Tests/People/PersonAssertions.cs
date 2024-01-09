using System.Collections.Generic;
using System.Linq;
using Entities.Interfaces.EF.Application.People;
using Entities.Interfaces.EF.Application.People.CreatePerson;
using Entities.Interfaces.EF.Application.People.UpdatePerson;
using Entities.Interfaces.EF.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.People
{
    public static class PersonAssertions
    {
        public static void AssertEquivalent(CreatePersonCommand expectedDto, Person actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
        }

        public static void AssertEquivalent(IEnumerable<PersonDto> actualDtos, IEnumerable<Person> expectedEntities)
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

        public static void AssertEquivalent(PersonDto actualDto, Person expectedEntity)
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

        public static void AssertEquivalent(UpdatePersonCommand expectedDto, Person actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
        }
    }
}