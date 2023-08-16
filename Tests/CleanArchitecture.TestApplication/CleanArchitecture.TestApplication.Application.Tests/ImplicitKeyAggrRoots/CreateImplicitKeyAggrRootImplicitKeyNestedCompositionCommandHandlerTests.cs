using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
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
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            var existingEntity = existingOwnerEntity.ImplicitKeyNestedCompositions.First();
            existingEntity.ImplicitKeyAggrRootId = existingOwnerEntity.Id;
            fixture.Customize<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>(comp => comp
                .With(x => x.ImplicitKeyAggrRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsImplicitKeyNestedCompositionToImplicitKeyAggrRoot(
            CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand,
            ImplicitKeyAggrRoot existingOwnerEntity)
        {
            // Arrange
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            implicitKeyAggrRootRepository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));
            var expectedImplicitKeyAggrRootId = new Fixture().Create<System.Guid>();
            ImplicitKeyNestedComposition addedImplicitKeyNestedComposition = null;
            implicitKeyAggrRootRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ =>
                {
                    addedImplicitKeyNestedComposition = existingOwnerEntity.ImplicitKeyNestedCompositions.Single(p => p.Id == default);
                    addedImplicitKeyNestedComposition.Id = expectedImplicitKeyAggrRootId;
                    addedImplicitKeyNestedComposition.ImplicitKeyAggrRootId = testCommand.ImplicitKeyAggrRootId;
                });

            var sut = new CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(implicitKeyAggrRootRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedImplicitKeyAggrRootId);
            await implicitKeyAggrRootRepository.UnitOfWork.Received(1).SaveChangesAsync();
            ImplicitKeyAggrRootAssertions.AssertEquivalent(testCommand, addedImplicitKeyNestedComposition);
        }
    }
}