using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.DeleteConcreteClass;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Inheritance.ConcreteClasses
{
    public class DeleteConcreteClassCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<ConcreteClass>();
            fixture.Customize<DeleteConcreteClassCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteConcreteClassCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesConcreteClassFromRepository(
            DeleteConcreteClassCommand testCommand,
            ConcreteClass existingEntity)
        {
            // Arrange
            var concreteClassRepository = Substitute.For<IConcreteClassRepository>();
            concreteClassRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteConcreteClassCommandHandler(concreteClassRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            concreteClassRepository.Received(1).Remove(Arg.Is<ConcreteClass>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidConcreteClassId_ReturnsNotFound()
        {
            // Arrange
            var concreteClassRepository = Substitute.For<IConcreteClassRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteConcreteClassCommand>();
            concreteClassRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<ConcreteClass>(default));


            var sut = new DeleteConcreteClassCommandHandler(concreteClassRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}