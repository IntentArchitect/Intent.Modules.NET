using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedCreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            var command = fixture.Create<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            command.ImplicitKeyAggrRootId = existingOwnerEntity.Id;
            yield return new object[] { command, existingOwnerEntity };
        }
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsImplicitKeyNestedCompositionToRepository(CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand, ImplicitKeyAggrRoot existingOwnerEntity)
        {
            // Arrange
            var expectedImplicitKeyAggrRootId = new Fixture().Create<System.Guid>();

            ImplicitKeyNestedComposition addedImplicitKeyNestedComposition = null;
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));
            repository.OnSaveChanges(
                () =>
                {
                    addedImplicitKeyNestedComposition = existingOwnerEntity.ImplicitKeyNestedCompositions.Single(p => p.Id == default);
                    addedImplicitKeyNestedComposition.Id = expectedImplicitKeyAggrRootId;
                    addedImplicitKeyNestedComposition.ImplicitKeyAggrRootId = testCommand.ImplicitKeyAggrRootId;
                });
            var sut = new CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedImplicitKeyAggrRootId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            ImplicitKeyAggrRootAssertions.AssertEquivalent(testCommand, addedImplicitKeyNestedComposition);
        }
    }
}