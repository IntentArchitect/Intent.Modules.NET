using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.DeleteAggregateRootLong;
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

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateRootLongs
{
    public class DeleteAggregateRootLongCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<AggregateRootLong>();
            fixture.Customize<DeleteAggregateRootLongCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteAggregateRootLongCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesAggregateRootLongFromRepository(
            DeleteAggregateRootLongCommand testCommand,
            AggregateRootLong existingEntity)
        {
            // Arrange
            var aggregateRootLongRepository = Substitute.For<IAggregateRootLongRepository>();
            aggregateRootLongRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteAggregateRootLongCommandHandler(aggregateRootLongRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            aggregateRootLongRepository.Received(1).Remove(Arg.Is<AggregateRootLong>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidAggregateRootLongId_ReturnsNotFound()
        {
            // Arrange
            var aggregateRootLongRepository = Substitute.For<IAggregateRootLongRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateRootLongCommand>();
            aggregateRootLongRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateRootLong>(default));


            var sut = new DeleteAggregateRootLongCommandHandler(aggregateRootLongRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}