using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.AggregateRoots;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
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
            var command = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            command.AggregateRootId = existingOwnerEntity.Id;
            yield return new object[] { command, existingOwnerEntity };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.Without(x => x.Composite));
            existingOwnerEntity = fixture.Create<AggregateRoot>();
            command = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            command.AggregateRootId = existingOwnerEntity.Id;
            yield return new object[] { command, existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsCompositeManyBToRepository(
            CreateAggregateRootCompositeManyBCommand testCommand,
            AggregateRoot existingOwnerEntity)
        {
            // Arrange
            var expectedAggregateRootId = new Fixture().Create<System.Guid>();
            CompositeManyB addedCompositeManyB = null;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));
            repository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ =>
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