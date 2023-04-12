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
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class DeleteImplicitKeyAggrRootCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<ImplicitKeyAggrRoot>();
            fixture.Customize<DeleteImplicitKeyAggrRootCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteImplicitKeyAggrRootCommand>();
            yield return new object[] { testCommand, existingEntity };
        }
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesImplicitKeyAggrRootFromRepository(
            DeleteImplicitKeyAggrRootCommand testCommand,
            ImplicitKeyAggrRoot existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existingEntity));

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
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}