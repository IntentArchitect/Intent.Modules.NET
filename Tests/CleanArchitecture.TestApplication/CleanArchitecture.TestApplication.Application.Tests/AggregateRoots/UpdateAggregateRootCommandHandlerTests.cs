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
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.UpdateCommandHandlerTests", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

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

    [Fact]
    public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
    {
        // Arrange
        var fixture = new Fixture();
        var testCommand = fixture.Create<UpdateAggregateRootCommand>();

        var repository = Substitute.For<IAggregateRootRepository>();
        repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<AggregateRoot>(null));

        var sut = new UpdateAggregateRootCommandHandler(repository);

        // Act
        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
        {
            await sut.Handle(testCommand, CancellationToken.None);
        });
    }

    public static IEnumerable<object[]> GetTestData()
    {
        var fixture = new Fixture();
        var testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, CreateExpectedAggregateRoot(testCommand) };

        fixture = new Fixture();
        fixture.Customize<UpdateAggregateRootCommand>(comp => comp.Without(x => x.Composites));
        testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, CreateExpectedAggregateRoot(testCommand) };

        fixture = new Fixture();
        fixture.Customize<UpdateAggregateRootCommand>(comp => comp.Without(x => x.Composite));
        testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, CreateExpectedAggregateRoot(testCommand) };
    }

    private static AggregateRoot CreateExpectedAggregateRoot(UpdateAggregateRootCommand dto)
    {
        return new AggregateRoot
        {
            AggregateAttr = dto.AggregateAttr,
            Composites = dto.Composites.Select(CreateExpectedCompositeManyB).ToList(),
            Composite = dto.Composite != null ? CreateExpectedCompositeSingleA(dto.Composite) : null,
#warning Field not a composite association: Aggregate
        };
    }

    private static CompositeSingleA CreateExpectedCompositeSingleA(UpdateAggregateRootCompositeSingleADto dto)
    {
        return new CompositeSingleA
        {
            CompositeAttr = dto.CompositeAttr,
            Composite = dto.Composite != null ? CreateExpectedCompositeSingleAA(dto.Composite) : null,
            Composites = dto.Composites.Select(CreateExpectedCompositeManyAA).ToList(),
        };
    }

    private static CompositeManyAA CreateExpectedCompositeManyAA(UpdateAggregateRootCompositeSingleACompositeManyAADto dto)
    {
        return new CompositeManyAA
        {
            CompositeAttr = dto.CompositeAttr,
            CompositeSingleAId = dto.CompositeSingleAId,
        };
    }

    private static CompositeSingleAA CreateExpectedCompositeSingleAA(UpdateAggregateRootCompositeSingleACompositeSingleAADto dto)
    {
        return new CompositeSingleAA
        {
            CompositeAttr = dto.CompositeAttr,
        };
    }

    private static CompositeManyB CreateExpectedCompositeManyB(UpdateAggregateRootCompositeManyBDto dto)
    {
        return new CompositeManyB
        {
            CompositeAttr = dto.CompositeAttr,
            SomeDate = dto.SomeDate,
            AggregateRootId = dto.AggregateRootId,
            Composites = dto.Composites.Select(CreateExpectedCompositeManyBB).ToList(),
            Composite = dto.Composite != null ? CreateExpectedCompositeSingleBB(dto.Composite) : null,
        };
    }

    private static CompositeManyBB CreateExpectedCompositeManyBB(UpdateAggregateRootCompositeManyBCompositeManyBBDto dto)
    {
        return new CompositeManyBB
        {
            CompositeAttr = dto.CompositeAttr,
            CompositeManyBId = dto.CompositeManyBId,
        };
    }

    private static CompositeSingleBB CreateExpectedCompositeSingleBB(UpdateAggregateRootCompositeManyBCompositeSingleBBDto dto)
    {
        return new CompositeSingleBB
        {
            CompositeAttr = dto.CompositeAttr,
        };
    }
}