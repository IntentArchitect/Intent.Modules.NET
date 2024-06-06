using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexElement;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.UniqueIndexConstraint.ClassicMapping
{
    public class DeleteAggregateWithUniqueConstraintIndexElementCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<AggregateWithUniqueConstraintIndexElement>();
            fixture.Customize<DeleteAggregateWithUniqueConstraintIndexElementCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteAggregateWithUniqueConstraintIndexElementCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesAggregateWithUniqueConstraintIndexElementFromRepository(
            DeleteAggregateWithUniqueConstraintIndexElementCommand testCommand,
            AggregateWithUniqueConstraintIndexElement existingEntity)
        {
            // Arrange
            var aggregateWithUniqueConstraintIndexElementRepository = Substitute.For<IAggregateWithUniqueConstraintIndexElementRepository>();
            aggregateWithUniqueConstraintIndexElementRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteAggregateWithUniqueConstraintIndexElementCommandHandler(aggregateWithUniqueConstraintIndexElementRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            aggregateWithUniqueConstraintIndexElementRepository.Received(1).Remove(Arg.Is<AggregateWithUniqueConstraintIndexElement>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidAggregateWithUniqueConstraintIndexElementId_ReturnsNotFound()
        {
            // Arrange
            var aggregateWithUniqueConstraintIndexElementRepository = Substitute.For<IAggregateWithUniqueConstraintIndexElementRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateWithUniqueConstraintIndexElementCommand>();
            aggregateWithUniqueConstraintIndexElementRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateWithUniqueConstraintIndexElement>(default));


            var sut = new DeleteAggregateWithUniqueConstraintIndexElementCommandHandler(aggregateWithUniqueConstraintIndexElementRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}