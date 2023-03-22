using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
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

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.With(p => p.AggregateRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };
        }
        
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsCompositeManyBToRepository(CreateAggregateRootCompositeManyBCommand testCommand, AggregateRoot existingOwnerEntity)
        {
            // Arrange
            var expectedAggregateRootId = Guid.NewGuid();

            CompositeManyB addedCompositeManyB = null;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));
            repository.OnSaveChanges(
                () =>
                {
                    addedCompositeManyB = existingOwnerEntity.Composites.Single(p => p.Id == default);
                    addedCompositeManyB.Id = expectedAggregateRootId;
                    addedCompositeManyB.AggregateRootId = testCommand.AggregateRootId;
                });
            var sut = new CreateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedAggregateRootId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            AggregateRootAssertions.AssertEquivalent(testCommand, addedCompositeManyB);
        }
    }
}