using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.DeleteAggregateRoot;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class DeleteAggregateRootCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WithValidCommand_DeletesAggregateRootFromRepository()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateRootCommand>();

            var repository = Substitute.For<IAggregateRootRepository>();
            var existingAggregateRoot = GetExistingAggregateRoot(testCommand);
            repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existingAggregateRoot));

            var sut = new DeleteAggregateRootCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            repository.Received(1).Remove(Arg.Is<AggregateRoot>(p => p.Id == testCommand.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateRootCommand>();

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<AggregateRoot>(default));
            repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

            var sut = new DeleteAggregateRootCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }

        private static AggregateRoot GetExistingAggregateRoot(DeleteAggregateRootCommand testCommand)
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRoot>(comp => comp.With(x => x.Id, testCommand.Id));
            var existingAggregateRoot = fixture.Create<AggregateRoot>();
            return existingAggregateRoot;
        }
    }
}