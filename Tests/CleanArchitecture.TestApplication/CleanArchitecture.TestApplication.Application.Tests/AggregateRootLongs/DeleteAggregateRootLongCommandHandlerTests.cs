using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.DeleteAggregateRootLong;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRootLongs
{
    public class DeleteAggregateRootLongCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WithValidCommand_DeletesAggregateRootLongFromRepository()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateRootLongCommand>();

            var repository = Substitute.For<IAggregateRootLongRepository>();
            var existingAggregateRootLong = GetExistingAggregateRootLong(testCommand);
            repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existingAggregateRootLong));

            var sut = new DeleteAggregateRootLongCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            repository.Received(1).Remove(Arg.Is<AggregateRootLong>(p => p.Id == testCommand.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAggregateRootLongCommand>();

            var repository = Substitute.For<IAggregateRootLongRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<AggregateRootLong>(default));
            repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

            var sut = new DeleteAggregateRootLongCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }

        private static AggregateRootLong GetExistingAggregateRootLong(DeleteAggregateRootLongCommand testCommand)
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRootLong>(comp => comp.With(x => x.Id, testCommand.Id));
            var existingAggregateRootLong = fixture.Create<AggregateRootLong>();
            return existingAggregateRootLong;
        }
    }
}