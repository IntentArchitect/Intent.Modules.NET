using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.CreateCommandHandlerTests", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class CreateAggregateRootCommandHandlerTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public async Task Handle_WithValidCommand_AddsAggregateRootToRepository(CreateAggregateRootCommand testCommand)
    {
        // Arrange
        var expectedAggregateRoot = CreateExpectedAggregateRoot(testCommand);

        AggregateRoot addedAggregateRoot = null;
        var repository = Substitute.For<IAggregateRootRepository>();
        repository.OnAdd(ent => addedAggregateRoot = ent);
        repository.OnSave(() => addedAggregateRoot.Id = expectedAggregateRoot.Id);

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
        fixture.Customize<CreateAggregateRootCommand>(comp => comp.Without(x => x.Composites));
        yield return new object[] { fixture.Create<CreateAggregateRootCommand>() };

        fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCommand>(comp => comp.Without(x => x.Composite));
        yield return new object[] { fixture.Create<CreateAggregateRootCommand>() };

    }

    public static AggregateRoot CreateExpectedAggregateRoot(CreateAggregateRootCommand dto)
    {
        return new AggregateRoot
        {
            AggregateAttr = dto.AggregateAttr,
            Composites = dto.Composites.Select(CreateExpectedCompositeManyB).ToList(),
            Composite = dto.Composite != null ? CreateExpectedCompositeSingleA(dto.Composite) : null,
#warning Field not a composite association: Aggregate
        };
    }



    private static CompositeSingleA CreateExpectedCompositeSingleA(CreateAggregateRootCompositeSingleADto dto)
    {
        return new CompositeSingleA
        {
            CompositeAttr = dto.CompositeAttr,
            Composite = dto.Composite != null ? CreateExpectedCompositeSingleAA(dto.Composite) : null,
            Composites = dto.Composites.Select(CreateExpectedCompositeManyAA).ToList(),
        };
    }

    private static CompositeManyAA CreateExpectedCompositeManyAA(CreateAggregateRootCompositeSingleACompositeManyAADto dto)
    {
        return new CompositeManyAA
        {
            CompositeAttr = dto.CompositeAttr,
        };
    }

    private static CompositeSingleAA CreateExpectedCompositeSingleAA(CreateAggregateRootCompositeSingleACompositeSingleAADto dto)
    {
        return new CompositeSingleAA
        {
            CompositeAttr = dto.CompositeAttr,
        };
    }

    private static CompositeManyB CreateExpectedCompositeManyB(CreateAggregateRootCompositeManyBDto dto)
    {
        return new CompositeManyB
        {
            CompositeAttr = dto.CompositeAttr,
            SomeDate = dto.SomeDate,
            Composite = dto.Composite != null ? CreateExpectedCompositeSingleBB(dto.Composite) : null,
            Composites = dto.Composites.Select(CreateExpectedCompositeManyBB).ToList(),
        };
    }

    private static CompositeSingleBB CreateExpectedCompositeSingleBB(CreateAggregateRootCompositeManyBCompositeSingleBBDto dto)
    {
        return new CompositeSingleBB
        {
            CompositeAttr = dto.CompositeAttr,
        };
    }

    private static CompositeManyBB CreateExpectedCompositeManyBB(CreateAggregateRootCompositeManyBCompositeManyBBDto dto)
    {
        return new CompositeManyBB
        {
            CompositeAttr = dto.CompositeAttr,
        };
    }
}