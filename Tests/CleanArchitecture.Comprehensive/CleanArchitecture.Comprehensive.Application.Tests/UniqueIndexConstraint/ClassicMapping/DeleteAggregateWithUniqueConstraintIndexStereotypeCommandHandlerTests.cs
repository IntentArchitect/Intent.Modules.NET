using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexStereotype;
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
    public class DeleteAggregateWithUniqueConstraintIndexStereotypeCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<AggregateWithUniqueConstraintIndexStereotype>();
            fixture.Customize<DeleteAggregateWithUniqueConstraintIndexStereotypeCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteAggregateWithUniqueConstraintIndexStereotypeCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesAggregateWithUniqueConstraintIndexStereotypeFromRepository(
            DeleteAggregateWithUniqueConstraintIndexStereotypeCommand testCommand,
            AggregateWithUniqueConstraintIndexStereotype existingEntity)
        {
            // Arrange
            var aggregateWithUniqueConstraintIndexStereotypeRepository = Substitute.For<IAggregateWithUniqueConstraintIndexStereotypeRepository>();
            aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteAggregateWithUniqueConstraintIndexStereotypeCommandHandler(aggregateWithUniqueConstraintIndexStereotypeRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            aggregateWithUniqueConstraintIndexStereotypeRepository.Received(1).Remove(Arg.Is<AggregateWithUniqueConstraintIndexStereotype>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidAggregateWithUniqueConstraintIndexStereotypeId_ReturnsNotFound()
        {
            // Arrange
            var aggregateWithUniqueConstraintIndexStereotypeRepository = Substitute.For<IAggregateWithUniqueConstraintIndexStereotypeRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateWithUniqueConstraintIndexStereotypeCommand>();
            aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateWithUniqueConstraintIndexStereotype>(default));


            var sut = new DeleteAggregateWithUniqueConstraintIndexStereotypeCommandHandler(aggregateWithUniqueConstraintIndexStereotypeRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}