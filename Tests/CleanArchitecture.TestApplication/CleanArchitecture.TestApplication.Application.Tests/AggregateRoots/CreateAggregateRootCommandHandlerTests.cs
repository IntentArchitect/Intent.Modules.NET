using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
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
    [MemberData(nameof(GetValidAutoFixtures))]
    public async Task Handle_WithValidCommand_AddsAggregateRootToRepository(IFixture fixture)
    {
        // Arrange
        var command = fixture.Create<CreateAggregateRootCommand>();
        var expectedAggregateRoot = CreateExpectedAggregateRoot(command);

        var repository = Substitute.For<IAggregateRootRepository>();
        AggregateRoot addedAggregateRoot = null;
        repository.When(x => x.Add(Arg.Any<AggregateRoot>())).Do(ci => addedAggregateRoot = ci.Arg<AggregateRoot>());
        repository.UnitOfWork.When(async x => await x.SaveChangesAsync(CancellationToken.None)).Do(ci => addedAggregateRoot.Id = expectedAggregateRoot.Id);

        var sut = new CreateAggregateRootCommandHandler(repository);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(expectedAggregateRoot.Id);

        expectedAggregateRoot.Should().BeEquivalentTo(addedAggregateRoot, options => options
            .Excluding(x => x.Composite)
            .Excluding(x => x.Composites)
            .Excluding(x => x.Aggregate));
        expectedAggregateRoot.Composite.Should().BeEquivalentTo(addedAggregateRoot.Composite, options => options
            .Excluding(x => x.Composite)
            .Excluding(x => x.Composites));
        expectedAggregateRoot.Composites.Should().BeEquivalentTo(addedAggregateRoot.Composites);
    }
    
    public static IEnumerable<object[]> GetValidAutoFixtures()
    {
        var plainFixture = new Fixture();
        yield return new object[] { plainFixture };

        var noCompositeFixture = new Fixture();
        noCompositeFixture.Customize<CreateAggregateRootCommand>(comp => comp.Without(x => x.Composite));
        yield return new object[] { noCompositeFixture };
        
        var noCompositesFixture = new Fixture();
        noCompositesFixture.Customize<CreateAggregateRootCommand>(comp => comp.Without(x => x.Composites));
        yield return new object[] { noCompositesFixture };
    }

    private static AggregateRoot CreateExpectedAggregateRoot(CreateAggregateRootCommand command)
    {
        return new AggregateRoot
        {
            Id = Guid.NewGuid(),
            AggregateAttr = command.AggregateAttr,
            Composite = command.Composite == null
                ? null
                : new CompositeSingleA
                {
                    CompositeAttr = command.Composite.CompositeAttr,
                    Composite = new CompositeSingleAA { CompositeAttr = command.Composite.Composite?.CompositeAttr },
                    Composites = command.Composite.Composites?.Select(dto => new CompositeManyAA { CompositeAttr = dto.CompositeAttr }).ToList() ?? new List<CompositeManyAA>()
                },
            Composites = command.Composites?.Select(dto =>
                new CompositeManyB
                {
                    CompositeAttr = dto.CompositeAttr,
                    SomeDate = dto.SomeDate,
                    Composite = dto.Composite == null ? null : new CompositeSingleBB { CompositeAttr = dto.Composite.CompositeAttr },
                    Composites = dto.Composites?.Select(cdto => new CompositeManyBB { CompositeAttr = cdto.CompositeAttr }).ToList() ?? new List<CompositeManyBB>()
                }).ToList() ?? new List<CompositeManyB>()
        };
    }
}