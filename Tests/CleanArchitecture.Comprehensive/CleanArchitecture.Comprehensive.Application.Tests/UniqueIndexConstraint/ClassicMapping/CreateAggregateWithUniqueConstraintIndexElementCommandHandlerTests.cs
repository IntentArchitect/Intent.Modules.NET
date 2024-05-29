using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Tests.Extensions;
using CleanArchitecture.Comprehensive.Application.Tests.UniqueIndexConstraint.AggregateWithUniqueConstraintIndexElements;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.CreateAggregateWithUniqueConstraintIndexElement;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.UniqueIndexConstraint.ClassicMapping
{
    public class CreateAggregateWithUniqueConstraintIndexElementCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateAggregateWithUniqueConstraintIndexElementCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsAggregateWithUniqueConstraintIndexElementToRepository(CreateAggregateWithUniqueConstraintIndexElementCommand testCommand)
        {
            // Arrange
            var aggregateWithUniqueConstraintIndexElementRepository = Substitute.For<IAggregateWithUniqueConstraintIndexElementRepository>();
            var expectedAggregateWithUniqueConstraintIndexElementId = new Fixture().Create<System.Guid>();
            AggregateWithUniqueConstraintIndexElement addedAggregateWithUniqueConstraintIndexElement = null;
            aggregateWithUniqueConstraintIndexElementRepository.OnAdd(ent => addedAggregateWithUniqueConstraintIndexElement = ent);
            aggregateWithUniqueConstraintIndexElementRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedAggregateWithUniqueConstraintIndexElement.Id = expectedAggregateWithUniqueConstraintIndexElementId);

            var sut = new CreateAggregateWithUniqueConstraintIndexElementCommandHandler(aggregateWithUniqueConstraintIndexElementRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedAggregateWithUniqueConstraintIndexElementId);
            await aggregateWithUniqueConstraintIndexElementRepository.UnitOfWork.Received(1).SaveChangesAsync();
            AggregateWithUniqueConstraintIndexElementAssertions.AssertEquivalent(testCommand, addedAggregateWithUniqueConstraintIndexElement);
        }
    }
}