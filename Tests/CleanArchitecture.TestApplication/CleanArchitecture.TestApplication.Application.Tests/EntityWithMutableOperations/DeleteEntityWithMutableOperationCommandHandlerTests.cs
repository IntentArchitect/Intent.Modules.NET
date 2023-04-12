using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.DeleteEntityWithMutableOperation;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithMutableOperations
{
    public class DeleteEntityWithMutableOperationCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<EntityWithMutableOperation>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<EntityWithMutableOperation>();
            fixture.Customize<DeleteEntityWithMutableOperationCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteEntityWithMutableOperationCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesEntityWithMutableOperationFromRepository(
            DeleteEntityWithMutableOperationCommand testCommand,
            EntityWithMutableOperation existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IEntityWithMutableOperationRepository>();
            repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existingEntity));

            var sut = new DeleteEntityWithMutableOperationCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            repository.Received(1).Remove(Arg.Is<EntityWithMutableOperation>(p => p.Id == testCommand.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteEntityWithMutableOperationCommand>();

            var repository = Substitute.For<IEntityWithMutableOperationRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<EntityWithMutableOperation>(default));
            repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

            var sut = new DeleteEntityWithMutableOperationCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}