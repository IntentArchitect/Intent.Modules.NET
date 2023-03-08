using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRoots;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.GetAllQueryHandlerTests", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class GetAggregateRootsQueryHandlerTests
{
    private readonly IMapper _mapper;

    public GetAggregateRootsQueryHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(
            config =>
            {
                config.AddMaps(typeof(GetAggregateRootsQueryHandler));
            });
        _mapper = mapperConfiguration.CreateMapper();
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public async Task Handle_WithValidQuery_RetrievesAggregateRoots(List<AggregateRoot> testEntities)
    {
        // Arrange
        var expectedDtos = testEntities.Select(CreateExpectedAggregateRootDto).ToArray();

        var query = new GetAggregateRootsQuery();
        var repository = Substitute.For<IAggregateRootRepository>();
        repository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

        var sut = new GetAggregateRootsQueryHandler(repository, _mapper);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        var fixture = new Fixture();
        fixture.Register<DomainEvent>(() => null);
        fixture.Customize<AggregateRoot>(comp => comp.Without(x => x.DomainEvents));
        yield return new object[] { fixture.CreateMany<AggregateRoot>().ToList() };
        yield return new object[] { fixture.CreateMany<AggregateRoot>(0).ToList() };
    }

    private static AggregateRootDto CreateExpectedAggregateRootDto(AggregateRoot entity)
    {
        return new AggregateRootDto
        {
            Id = entity.Id,
            AggregateAttr = entity.AggregateAttr,
            Composites = entity.Composites?.Select(CreateExpectedCompositeManyB).ToList() ?? new List<AggregateRootCompositeManyBDto>(),
            Composite = entity.Composite != null ? CreateExpectedCompositeSingleA(entity.Composite) : null,
            Aggregate = entity.Aggregate != null ? CreateExpectedAggregateSingleC(entity.Aggregate) : null,
        };
    }

    private static AggregateRootCompositeManyBDto CreateExpectedCompositeManyB(CompositeManyB entity)
    {
        return new AggregateRootCompositeManyBDto
        {
            CompositeAttr = entity.CompositeAttr,
            SomeDate = entity.SomeDate,
            AggregateRootId = entity.AggregateRootId,
            Id = entity.Id,
            Composite = entity.Composite != null ? CreateExpectedCompositeSingleBB(entity.Composite) : null,
            Composites = entity.Composites?.Select(CreateExpectedCompositeManyBB).ToList() ?? new List<AggregateRootCompositeManyBCompositeManyBBDto>(),
        };
    }

    private static AggregateRootCompositeManyBCompositeSingleBBDto CreateExpectedCompositeSingleBB(CompositeSingleBB entity)
    {
        return new AggregateRootCompositeManyBCompositeSingleBBDto
        {
            CompositeAttr = entity.CompositeAttr,
            Id = entity.Id,
        };
    }

    private static AggregateRootCompositeManyBCompositeManyBBDto CreateExpectedCompositeManyBB(CompositeManyBB entity)
    {
        return new AggregateRootCompositeManyBCompositeManyBBDto
        {
            CompositeAttr = entity.CompositeAttr,
            CompositeManyBId = entity.CompositeManyBId,
            Id = entity.Id,
        };
    }

    private static AggregateRootCompositeSingleADto CreateExpectedCompositeSingleA(CompositeSingleA entity)
    {
        return new AggregateRootCompositeSingleADto
        {
            CompositeAttr = entity.CompositeAttr,
            Id = entity.Id,
            Composite = entity.Composite != null ? CreateExpectedCompositeSingleAA(entity.Composite) : null,
            Composites = entity.Composites?.Select(CreateExpectedCompositeManyAA).ToList() ?? new List<AggregateRootCompositeSingleACompositeManyAADto>(),
        };
    }

    private static AggregateRootCompositeSingleACompositeSingleAADto CreateExpectedCompositeSingleAA(CompositeSingleAA entity)
    {
        return new AggregateRootCompositeSingleACompositeSingleAADto
        {
            CompositeAttr = entity.CompositeAttr,
            Id = entity.Id,
        };
    }

    private static AggregateRootCompositeSingleACompositeManyAADto CreateExpectedCompositeManyAA(CompositeManyAA entity)
    {
        return new AggregateRootCompositeSingleACompositeManyAADto
        {
            CompositeAttr = entity.CompositeAttr,
            CompositeSingleAId = entity.CompositeSingleAId,
            Id = entity.Id,
        };
    }

    private static AggregateRootAggregateSingleCDto CreateExpectedAggregateSingleC(AggregateSingleC entity)
    {
        return new AggregateRootAggregateSingleCDto
        {
            AggregationAttr = entity.AggregationAttr,
            Id = entity.Id,
        };
    }
}