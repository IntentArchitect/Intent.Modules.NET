using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.DeleteAggregateRootCompositeManyB;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedDeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateRoots
{
    public class DeleteAggregateRootCompositeManyBCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            var existingEntity = existingOwnerEntity.Composites.First();
            fixture.Customize<DeleteAggregateRootCompositeManyBCommand>(comp => comp
                .With(x => x.AggregateRootId, existingOwnerEntity.Id)
                .With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesCompositeManyBFromAggregateRoot(
            DeleteAggregateRootCompositeManyBCommand testCommand,
            AggregateRoot existingOwnerEntity)
        {
            // Arrange
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            aggregateRootRepository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new DeleteAggregateRootCompositeManyBCommandHandler(aggregateRootRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            existingOwnerEntity.Composites.Should().NotContain(p => testCommand.Id == p.Id);
        }

        [Fact]
        public async Task Handle_WithInvalidAggregateRootId_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateRootCompositeManyBCommand>();
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            aggregateRootRepository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None)!.Returns(Task.FromResult<AggregateRoot>(default));

            var sut = new DeleteAggregateRootCompositeManyBCommandHandler(aggregateRootRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithInvalidCompositeManyBId_ReturnsNotFound()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            fixture.Customize<AggregateRoot>(comp => comp.With(p => p.Composites, new List<CompositeManyB>()));
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            fixture.Customize<DeleteAggregateRootCompositeManyBCommand>(comp => comp
                .With(p => p.AggregateRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<DeleteAggregateRootCompositeManyBCommand>();
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            aggregateRootRepository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new DeleteAggregateRootCompositeManyBCommandHandler(aggregateRootRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}