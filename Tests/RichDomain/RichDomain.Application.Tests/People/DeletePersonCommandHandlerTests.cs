using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using RichDomain.Application.People.DeletePerson;
using RichDomain.Domain.Common;
using RichDomain.Domain.Common.Exceptions;
using RichDomain.Domain.Entities;
using RichDomain.Domain.Repositories;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace RichDomain.Application.Tests.People
{
    public class DeletePersonCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<Person>();
            fixture.Customize<DeletePersonCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeletePersonCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesPersonFromRepository(
            DeletePersonCommand testCommand,
            Person existingEntity)
        {
            // Arrange
            var personRepository = Substitute.For<IPersonRepository>();
            personRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<IPerson>(existingEntity));

            var sut = new DeletePersonCommandHandler(personRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            personRepository.Received(1).Remove(Arg.Is<Person>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidPersonId_ReturnsNotFound()
        {
            // Arrange
            var personRepository = Substitute.For<IPersonRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeletePersonCommand>();
            personRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<IPerson>(default));


            var sut = new DeletePersonCommandHandler(personRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}