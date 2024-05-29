using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateRoots;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedCreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            var existingEntity = existingOwnerEntity.Composites.First();
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp
                .With(x => x.AggregateRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            existingOwnerEntity = fixture.Create<AggregateRoot>();
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp
                .Without(x => x.Composite));
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
            var compositeManyBsSnapshot = existingOwnerEntity.Composites.ToArray();
            aggregateRootRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ =>
                {
                    addedCompositeManyB = existingOwnerEntity.Composites.Except(compositeManyBsSnapshot).Single();
                    addedCompositeManyB.Id = expectedAggregateRootId;
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