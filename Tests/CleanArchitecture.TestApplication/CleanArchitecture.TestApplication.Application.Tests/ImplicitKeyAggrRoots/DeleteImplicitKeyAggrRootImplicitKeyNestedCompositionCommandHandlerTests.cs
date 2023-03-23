using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedDeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.Without(x => x.DomainEvents));
            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            fixture.Customize<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>(comp => comp
            .With(x => x.Id, existingOwnerEntity.ImplicitKeyNestedCompositions.First().Id)
            .With(x => x.ImplicitKeyAggrRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };
        }
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesImplicitKeyNestedCompositionFromRepository(DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand, ImplicitKeyAggrRoot existingOwnerEntity)
        {
            // Arrange
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId).Returns(Task.FromResult(existingOwnerEntity));

            var sut = new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            existingOwnerEntity.ImplicitKeyNestedCompositions.Should().NotContain(p => p.Id == testCommand.Id);
        }

        [Fact]
        public async Task Handle_WithInvalidOwnerIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(default));

            var sut = new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

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
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            testCommand.ImplicitKeyAggrRootId = owner.Id;

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(default));

            var sut = new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}