using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class CreateAggregateRootCommandHandlerTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public async Task Handle_WithValidCommand_AddsAggregateRootToRepository(CreateAggregateRootCommand testCommand)
    {
        // Arrange
        var expectedAggregateRoot = CreateExpectedAggregateRoot(testCommand);

        var repository = Substitute.For<IAggregateRootRepository>();
        AggregateRoot addedAggregateRoot = null;
        repository.When(x => x.Add(Arg.Any<AggregateRoot>())).Do(ci => addedAggregateRoot = ci.Arg<AggregateRoot>());
        repository.UnitOfWork.When(async x => await x.SaveChangesAsync(CancellationToken.None)).Do(_ => addedAggregateRoot.Id = expectedAggregateRoot.Id);

        var sut = new CreateAggregateRootCommandHandler(repository);

        // Act
        var result = await sut.Handle(testCommand, CancellationToken.None);

        // Assert
        result.Should().Be(expectedAggregateRoot.Id);
        expectedAggregateRoot.Should().BeEquivalentTo(addedAggregateRoot);
    }
    
    public static IEnumerable<object[]> GetTestData()
    {
        var fixture = new Fixture();
        yield return new object[] { fixture.Create<CreateAggregateRootCommand>() };

        fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCommand>(comp => comp.Without(x => x.Composite));
        yield return new object[] { fixture.Create<CreateAggregateRootCommand>() };
        
        fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCommand>(comp => comp.Without(x => x.Composites));
        yield return new object[] { fixture.Create<CreateAggregateRootCommand>() };
    }

    private static AggregateRoot CreateExpectedAggregateRoot(CreateAggregateRootCommand command)
    {
        return new AggregateRoot
        {
            Id = Guid.NewGuid(),
            AggregateAttr = command.AggregateAttr,
            Composite = command.Composite == null ? null : CreateExpectedCompositeSingleA(command.Composite),
            Composites = command.Composites?.Select(CreateExpectedCompositeManyB).ToList() ?? new List<CompositeManyB>()
        };
    }

    private static CompositeSingleA CreateExpectedCompositeSingleA(CreateAggregateRootCompositeSingleADto dto)
    {
        return new CompositeSingleA
        {
            CompositeAttr = dto.CompositeAttr,
            Composite = dto.Composite == null ? null : CreateExpectedCompositeSingleAA(dto.Composite),
            Composites = dto.Composites?.Select(CreateExpectedCompositeManyAA).ToList() ?? new List<CompositeManyAA>()
        };
    }

    private static CompositeManyAA CreateExpectedCompositeManyAA(CreateAggregateRootCompositeSingleACompositeManyAADto dto)
    {
        return new CompositeManyAA
        {
            CompositeAttr = dto.CompositeAttr
        };
    }

    private static CompositeSingleAA CreateExpectedCompositeSingleAA(
        CreateAggregateRootCompositeSingleACompositeSingleAADto dto)
    {
        return new CompositeSingleAA
        {
            CompositeAttr = dto.CompositeAttr
        };
    }

    private static CompositeManyB CreateExpectedCompositeManyB(CreateAggregateRootCompositeManyBDto dto)
    {
        return new CompositeManyB
        {
            CompositeAttr = dto.CompositeAttr,
            SomeDate = dto.SomeDate,
            Composite = dto.Composite == null ? null : CreateExpectedCompositeSinlgeBB(dto.Composite),
            Composites = dto.Composites?.Select(CreateExpectedCompositeManyBB).ToList() ?? new List<CompositeManyBB>()
        };
    }

    private static CompositeManyBB CreateExpectedCompositeManyBB(CreateAggregateRootCompositeManyBCompositeManyBBDto dto)
    {
        return new CompositeManyBB
        {
            CompositeAttr = dto.CompositeAttr
        };
    }

    private static CompositeSingleBB CreateExpectedCompositeSinlgeBB(CreateAggregateRootCompositeManyBCompositeSingleBBDto dto)
    {
        return new CompositeSingleBB
        {
            CompositeAttr = dto.CompositeAttr
        };
    }
}