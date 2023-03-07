using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class UpdateAggregateRootCommandHandlerTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public async Task Handle_WithValidCommand_UpdatesExistingEntity(UpdateAggregateRootCommand testCommand, AggregateRoot existingEntity)
    {
        // Arrange
        var expectedAggregateRoot = CreateExpectedAggregateRoot(testCommand);
        
        var repository = Substitute.For<IAggregateRootRepository>();
        repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));
        
        var sut = new UpdateAggregateRootCommandHandler(repository);

        // Act
        await sut.Handle(testCommand, CancellationToken.None);
        
        // Assert
        expectedAggregateRoot.Should().BeEquivalentTo(existingEntity);
    }
    
    public static IEnumerable<object[]> GetTestData()
    {
        var fixture = new Fixture();
        var testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, CreateExpectedAggregateRoot(testCommand) };

        fixture = new Fixture();
        fixture.Customize<UpdateAggregateRootCommand>(comp => comp.Without(x => x.Composite));
        testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, CreateExpectedAggregateRoot(testCommand) };
        
        fixture = new Fixture();
        fixture.Customize<UpdateAggregateRootCommand>(comp => comp.Without(x => x.Composites));
        testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, CreateExpectedAggregateRoot(testCommand) };
    }
    
    private static AggregateRoot CreateExpectedAggregateRoot(UpdateAggregateRootCommand command)
    {
        return new AggregateRoot
        {
            Id = command.Id,
            AggregateAttr = command.AggregateAttr,
            Composite = command.Composite == null ? null : CreateExpectedCompositeSingleA(command.Composite),
            Composites = command.Composites?.Select(CreateExpectedCompositeManyB).ToList() ?? new List<CompositeManyB>(),
            AggregateId = command.Aggregate?.Id
        };
    }

    private static CompositeSingleA CreateExpectedCompositeSingleA(UpdateAggregateRootCompositeSingleADto dto)
    {
        return new CompositeSingleA
        {
            Id = dto.Id,
            CompositeAttr = dto.CompositeAttr,
            Composite = dto.Composite == null ? null : CreateExpectedCompositeSingleAA(dto.Composite),
            Composites = dto.Composites?.Select(CreateExpectedCompositeManyAA).ToList() ?? new List<CompositeManyAA>()
        };
    }

    private static CompositeManyAA CreateExpectedCompositeManyAA(UpdateAggregateRootCompositeSingleACompositeManyAADto dto)
    {
        return new CompositeManyAA
        {
            Id = dto.Id,
            CompositeAttr = dto.CompositeAttr,
            CompositeSingleAId = dto.CompositeSingleAId
        };
    }

    private static CompositeSingleAA CreateExpectedCompositeSingleAA(
        UpdateAggregateRootCompositeSingleACompositeSingleAADto dto)
    {
        return new CompositeSingleAA
        {
            Id = dto.Id,
            CompositeAttr = dto.CompositeAttr
        };
    }

    private static CompositeManyB CreateExpectedCompositeManyB(UpdateAggregateRootCompositeManyBDto dto)
    {
        return new CompositeManyB
        {
            Id = dto.Id,
            CompositeAttr = dto.CompositeAttr,
            SomeDate = dto.SomeDate,
            AggregateRootId = dto.AggregateRootId,
            Composite = dto.Composite == null ? null : CreateExpectedCompositeSinlgeBB(dto.Composite),
            Composites = dto.Composites?.Select(CreateExpectedCompositeManyBB).ToList() ?? new List<CompositeManyBB>()
        };
    }

    private static CompositeManyBB CreateExpectedCompositeManyBB(UpdateAggregateRootCompositeManyBCompositeManyBBDto dto)
    {
        return new CompositeManyBB
        {
            Id = dto.Id,
            CompositeAttr = dto.CompositeAttr,
            CompositeManyBId = dto.CompositeManyBId
        };
    }

    private static CompositeSingleBB CreateExpectedCompositeSinlgeBB(UpdateAggregateRootCompositeManyBCompositeSingleBBDto dto)
    {
        return new CompositeSingleBB
        {
            Id = dto.Id,
            CompositeAttr = dto.CompositeAttr
        };
    }
}