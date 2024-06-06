using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.DeleteAggregateTestNoIdReturn;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateTestNoIdReturns
{
    public class DeleteAggregateTestNoIdReturnCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<AggregateTestNoIdReturn>();
            fixture.Customize<DeleteAggregateTestNoIdReturnCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteAggregateTestNoIdReturnCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesAggregateTestNoIdReturnFromRepository(
            DeleteAggregateTestNoIdReturnCommand testCommand,
            AggregateTestNoIdReturn existingEntity)
        {
            // Arrange
            var aggregateTestNoIdReturnRepository = Substitute.For<IAggregateTestNoIdReturnRepository>();
            aggregateTestNoIdReturnRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteAggregateTestNoIdReturnCommandHandler(aggregateTestNoIdReturnRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            aggregateTestNoIdReturnRepository.Received(1).Remove(Arg.Is<AggregateTestNoIdReturn>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidAggregateTestNoIdReturnId_ReturnsNotFound()
        {
            // Arrange
            var aggregateTestNoIdReturnRepository = Substitute.For<IAggregateTestNoIdReturnRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateTestNoIdReturnCommand>();
            aggregateTestNoIdReturnRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateTestNoIdReturn>(default));


            var sut = new DeleteAggregateTestNoIdReturnCommandHandler(aggregateTestNoIdReturnRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}