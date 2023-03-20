using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedUpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class UpdateAggregateRootCompositeManyBCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            var existingEntity = fixture.Create<CompositeManyB>();
            existingOwnerEntity.Composites.Add(existingEntity);
            fixture.Customize<UpdateAggregateRootCompositeManyBCommand>(comp => comp 
                .With(x => x.Id, existingEntity.Id)
                .With(x => x.AggregateRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, existingOwnerEntity, existingEntity };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            existingOwnerEntity = fixture.Create<AggregateRoot>();
            existingEntity = fixture.Create<CompositeManyB>();
            existingOwnerEntity.Composites.Add(existingEntity);
            fixture.Customize<UpdateAggregateRootCompositeManyBCommand>(comp => comp
                .With(x => x.Id, existingEntity.Id)
                .With(x => x.AggregateRootId, existingOwnerEntity.Id)
                .Without(x => x.Composite));
            testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, existingOwnerEntity, existingEntity };
        }
        
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(UpdateAggregateRootCompositeManyBCommand testCommand, AggregateRoot existingOwnerEntity, CompositeManyB existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));

            var sut = new UpdateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            AggregateRootAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidAggregateRootIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult<AggregateRoot>(null));

            var sut = new UpdateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            var owner = fixture.Create<AggregateRoot>();
            testCommand.AggregateRootId = owner.Id;

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new UpdateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}