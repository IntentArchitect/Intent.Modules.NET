using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Application.People;
using RichDomain.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace RichDomain.Application.Tests.People
{
    public static class PersonAssertions
    {
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
                dto.FirstName.Should().Be(entity.FirstName);
                dto.DepartmentId.Should().Be(entity.DepartmentId);
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
            actualDto.FirstName.Should().Be(expectedEntity.FirstName);
            actualDto.DepartmentId.Should().Be(expectedEntity.DepartmentId);
        }
    }
}