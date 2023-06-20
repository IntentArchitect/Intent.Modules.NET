using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.DeleteAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
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
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRoot>(comp => comp.Without(x => x.DomainEvents));
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            fixture.Customize<DeleteAggregateRootCompositeManyBCommand>(comp => comp
            .With(x => x.Id, existingOwnerEntity.Composites.First().Id)
            .With(x => x.AggregateRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<DeleteAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesCompositeManyBFromRepository(
            DeleteAggregateRootCompositeManyBCommand testCommand,
            AggregateRoot existingOwnerEntity)
        {
            // Arrange
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId).Returns(Task.FromResult(existingOwnerEntity));

            var sut = new DeleteAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            existingOwnerEntity.Composites.Should().NotContain(p => p.Id == testCommand.Id);
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
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
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
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}