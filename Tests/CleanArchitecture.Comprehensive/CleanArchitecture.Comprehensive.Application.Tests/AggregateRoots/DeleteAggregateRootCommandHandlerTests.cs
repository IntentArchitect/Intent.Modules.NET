using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.DeleteAggregateRoot;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateRoots
{
    public class DeleteAggregateRootCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<AggregateRoot>();
            fixture.Customize<DeleteAggregateRootCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteAggregateRootCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesAggregateRootFromRepository(
            DeleteAggregateRootCommand testCommand,
            AggregateRoot existingEntity)
        {
            // Arrange
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            aggregateRootRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteAggregateRootCommandHandler(aggregateRootRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            aggregateRootRepository.Received(1).Remove(Arg.Is<AggregateRoot>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidAggregateRootId_ReturnsNotFound()
        {
            // Arrange
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateRootCommand>();
            aggregateRootRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateRoot>(default));


            var sut = new DeleteAggregateRootCommandHandler(aggregateRootRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}