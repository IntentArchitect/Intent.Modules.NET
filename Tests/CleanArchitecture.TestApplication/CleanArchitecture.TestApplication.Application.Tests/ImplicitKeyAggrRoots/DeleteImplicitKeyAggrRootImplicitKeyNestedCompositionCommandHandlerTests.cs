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
        [Fact]
        public async Task Handle_WithValidCommand_DeletesImplicitKeyNestedCompositionFromRepository()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            var testCommand = new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand();
            testCommand.Id = owner.ImplicitKeyNestedCompositions.First().Id;
            testCommand.ImplicitKeyAggrRootId = owner.Id;

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId).Returns(Task.FromResult(owner));

            var sut = new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            owner.ImplicitKeyNestedCompositions.Should().NotContain(p => p.Id == testCommand.Id);
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
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            testCommand.ImplicitKeyAggrRootId = owner.Id;

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(default));

            var sut = new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }
        // [Fact]
        // public async Task Handle_WithValidCommand_DeletesImplicitKeyNestedCompositionFromRepository()
        // {
        //     // Arrange
        //     var fixture = new Fixture();
        //     var testCommand = fixture.Create<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
        //
        //     var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
        //     var existingImplicitKeyNestedComposition = GetExistingImplicitKeyNestedComposition(testCommand);
        //     repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existingImplicitKeyNestedComposition));
        //
        //     var sut = new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);
        //
        //     // Act
        //     await sut.Handle(testCommand, CancellationToken.None);
        //
        //     // Assert
        //     repository.Received(1).Remove(Arg.Is<ImplicitKeyNestedComposition>(p => p.Id == testCommand.Id));
        // }
        //
        // [Fact]
        // public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        // {
        //     // Arrange
        //     var fixture = new Fixture();
        //     var testCommand = fixture.Create<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
        //
        //     var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
        //     repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyNestedComposition>(default));
        //     repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());
        //
        //     var sut = new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);
        //
        //     // Act
        //     // Assert
        //     await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        //     {
        //         await sut.Handle(testCommand, CancellationToken.None);
        //     });
        // }
        //
        // private static ImplicitKeyNestedComposition GetExistingImplicitKeyNestedComposition(DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand)
        // {
        //     var fixture = new Fixture();
        //     fixture.Register<DomainEvent>(() => null);
        //     fixture.Customize<ImplicitKeyNestedComposition>(comp => comp.With(x => x.Id, testCommand.Id));
        //     var existingImplicitKeyNestedComposition = fixture.Create<ImplicitKeyNestedComposition>();
        //     return existingImplicitKeyNestedComposition;
        // }
    }
}