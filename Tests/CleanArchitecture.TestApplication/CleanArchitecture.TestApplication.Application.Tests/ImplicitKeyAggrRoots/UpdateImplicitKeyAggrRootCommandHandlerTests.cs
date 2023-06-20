using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.ImplicitKeyAggrRoots;
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
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class UpdateImplicitKeyAggrRootCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<ImplicitKeyAggrRoot>();
            fixture.Customize<UpdateImplicitKeyAggrRootCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootCommand>();
            yield return new object[] { testCommand, existingEntity };
        }
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateImplicitKeyAggrRootCommand testCommand,
            ImplicitKeyAggrRoot existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new UpdateImplicitKeyAggrRootCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            ImplicitKeyAggrRootAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootCommand>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(null));

            var sut = new UpdateImplicitKeyAggrRootCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}