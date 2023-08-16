using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.AggregateRoots;
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

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            var existingEntity = existingOwnerEntity.Composites.First();
            existingEntity.AggregateRootId = existingOwnerEntity.Id;
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp
                .With(x => x.AggregateRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            existingOwnerEntity = fixture.Create<AggregateRoot>();
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp
                .Without(x => x.Composite)
                .With(x => x.AggregateRootId, existingOwnerEntity.Id));
            testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsCompositeManyBToAggregateRoot(
            CreateAggregateRootCompositeManyBCommand testCommand,
            AggregateRoot existingOwnerEntity)
        {
            // Arrange
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            aggregateRootRepository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));
            var expectedAggregateRootId = new Fixture().Create<System.Guid>();
            CompositeManyB addedCompositeManyB = null;
            aggregateRootRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ =>
                {
                    addedCompositeManyB = existingOwnerEntity.Composites.Single(p => p.Id == default);
                    addedCompositeManyB.Id = expectedAggregateRootId;
                    addedCompositeManyB.AggregateRootId = testCommand.AggregateRootId;
                });

            var sut = new CreateAggregateRootCompositeManyBCommandHandler(aggregateRootRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedAggregateRootId);
            await aggregateRootRepository.UnitOfWork.Received(1).SaveChangesAsync();
            AggregateRootAssertions.AssertEquivalent(testCommand, addedCompositeManyB);
        }
    }
}