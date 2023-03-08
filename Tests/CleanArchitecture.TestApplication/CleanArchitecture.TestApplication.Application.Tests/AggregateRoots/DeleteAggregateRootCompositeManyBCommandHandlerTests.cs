using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.DeleteAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedDeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class DeleteAggregateRootCompositeManyBCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WithValidCommand_DeletesCompositeManyBFromRepository()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var owner = fixture.Create<AggregateRoot>();
            var testCommand = new DeleteAggregateRootCompositeManyBCommand();
            testCommand.Id = owner.Composites.First().Id;
            testCommand.AggregateRootId = owner.Id;

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId).Returns(Task.FromResult(owner));

            var sut = new DeleteAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            owner.Composites.Should().NotContain(p => p.Id == testCommand.Id);
        }

        [Fact]
        public async Task Handle_WithInvalidOwnerIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateRootCompositeManyBCommand>();

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult<AggregateRoot>(default));

            var sut = new DeleteAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var testCommand = fixture.Create<DeleteAggregateRootCompositeManyBCommand>();
            var owner = fixture.Create<AggregateRoot>();
            testCommand.AggregateRootId = owner.Id;

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult<AggregateRoot>(default));

            var sut = new DeleteAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }
    }
}