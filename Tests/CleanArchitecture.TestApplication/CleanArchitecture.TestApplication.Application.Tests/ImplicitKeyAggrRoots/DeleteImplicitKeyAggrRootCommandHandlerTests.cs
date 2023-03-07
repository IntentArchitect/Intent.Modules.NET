using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class DeleteImplicitKeyAggrRootCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WithValidCommand_DeletesImplicitKeyAggrRootFromRepository()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootCommand>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            var existingImplicitKeyAggrRoot = GetExistingImplicitKeyAggrRoot(testCommand);
            repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existingImplicitKeyAggrRoot));

            var sut = new DeleteImplicitKeyAggrRootCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            repository.Received(1).Remove(Arg.Is<ImplicitKeyAggrRoot>(p => p.Id == testCommand.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootCommand>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(default));
            repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

            var sut = new DeleteImplicitKeyAggrRootCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }

        private static ImplicitKeyAggrRoot GetExistingImplicitKeyAggrRoot(DeleteImplicitKeyAggrRootCommand testCommand)
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.With(x => x.Id, testCommand.Id));
            var existingImplicitKeyAggrRoot = fixture.Create<ImplicitKeyAggrRoot>();
            return existingImplicitKeyAggrRoot;
        }
    }
}