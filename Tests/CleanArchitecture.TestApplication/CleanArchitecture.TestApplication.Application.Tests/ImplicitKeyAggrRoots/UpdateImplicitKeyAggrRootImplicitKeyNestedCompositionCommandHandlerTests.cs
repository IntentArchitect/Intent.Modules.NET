using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
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
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            var existingEntity = existingOwnerEntity.ImplicitKeyNestedCompositions.First();
            existingEntity.ImplicitKeyAggrRootId = existingOwnerEntity.Id;
            fixture.Customize<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>(comp => comp
                .With(x => x.ImplicitKeyAggrRootId, existingOwnerEntity.Id)
                .With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            yield return new object[] { testCommand, existingOwnerEntity, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesImplicitKeyNestedComposition(
            UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand,
            ImplicitKeyAggrRoot existingOwnerEntity,
            ImplicitKeyNestedComposition existingEntity)
        {
            // Arrange
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            implicitKeyAggrRootRepository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(implicitKeyAggrRootRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            ImplicitKeyAggrRootAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidImplicitKeyAggrRootId_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            implicitKeyAggrRootRepository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None)!.Returns(Task.FromResult<ImplicitKeyAggrRoot>(default));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(implicitKeyAggrRootRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithInvalidImplicitKeyNestedCompositionId_ReturnsNotFound()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.With(p => p.ImplicitKeyNestedCompositions, new List<ImplicitKeyNestedComposition>()));
            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            fixture.Customize<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>(comp => comp
                .With(p => p.ImplicitKeyAggrRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            implicitKeyAggrRootRepository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(implicitKeyAggrRootRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}