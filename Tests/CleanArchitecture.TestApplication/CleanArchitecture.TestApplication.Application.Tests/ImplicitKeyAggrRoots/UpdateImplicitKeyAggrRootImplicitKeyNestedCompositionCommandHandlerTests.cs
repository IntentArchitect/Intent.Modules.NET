using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedUpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.Without(x => x.DomainEvents));
            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            var expectedEntity = existingOwnerEntity.ImplicitKeyNestedCompositions.First();
            expectedEntity.ImplicitKeyAggrRootId = existingOwnerEntity.Id;
            fixture.Customize<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>(comp => comp
            .With(x => x.Id, expectedEntity.Id)
            .With(x => x.ImplicitKeyAggrRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            yield return new object[] { testCommand, existingOwnerEntity, expectedEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand,
            ImplicitKeyAggrRoot existingOwnerEntity,
            ImplicitKeyNestedComposition existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            ImplicitKeyAggrRootAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidImplicitKeyAggrRootIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(null));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

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
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            testCommand.ImplicitKeyAggrRootId = owner.Id;

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}