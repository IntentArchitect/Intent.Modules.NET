using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.TestApplication.Application.AggregateRoots.DeleteAggregateRoot;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class DeleteAggregateRootCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_DeletesAggregateRootFromRepository()
    {
        // Arrange
        var testCommand = new DeleteAggregateRootCommand();
        testCommand.Id = Guid.NewGuid();
        
        var repository = Substitute.For<IAggregateRootRepository>();
        var fixture = new Fixture();
        fixture.Register<DomainEvent>(() => null);
        fixture.Customize<AggregateRoot>(comp => comp.With(x => x.Id, testCommand.Id));
        repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(fixture.Create<AggregateRoot>()));

        var sut = new DeleteAggregateRootCommandHandler(repository);
        
        // Act
        await sut.Handle(testCommand, CancellationToken.None);

        // Assert
        repository.Received(1).Remove(Arg.Is<AggregateRoot>(p => p.Id == testCommand.Id));
    }

    [Fact]
    public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
    {
        // Arrange
        var testCommand = new DeleteAggregateRootCommand();
        testCommand.Id = Guid.NewGuid();
        
        var repository = Substitute.For<IAggregateRootRepository>();
        var fixture = new Fixture();
        fixture.Register<DomainEvent>(() => null);
        repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult<AggregateRoot>(default));
        repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

        var sut = new DeleteAggregateRootCommandHandler(repository);
        
        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async() =>
        {
            await sut.Handle(testCommand, CancellationToken.None);
        });
    }
}