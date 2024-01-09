using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Entities.Interfaces.EF.Application.People.CreatePerson;
using Entities.Interfaces.EF.Application.Tests.Extensions;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.People
{
    public class CreatePersonCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreatePersonCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsPersonToRepository(CreatePersonCommand testCommand)
        {
            // Arrange
            var personRepository = Substitute.For<IPersonRepository>();
            var expectedPersonId = new Fixture().Create<System.Guid>();
            Person addedPerson = null;
            personRepository.OnAdd(ent => addedPerson = (Person)ent);
            personRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedPerson.Id = expectedPersonId);

            var sut = new CreatePersonCommandHandler(personRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedPersonId);
            await personRepository.UnitOfWork.Received(1).SaveChangesAsync();
            PersonAssertions.AssertEquivalent(testCommand, addedPerson);
        }
    }
}