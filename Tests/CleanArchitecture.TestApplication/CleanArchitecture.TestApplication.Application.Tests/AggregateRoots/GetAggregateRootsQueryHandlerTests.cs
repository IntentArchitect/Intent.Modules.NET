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
using NSubstitute;
using Xunit;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class GetAggregateRootsQueryHandlerTests
{
    private readonly IMapper _mapper;

    public GetAggregateRootsQueryHandlerTests()
    {
        AssertionOptions.FormattingOptions.MaxLines = 500;
        var mapperConfiguration = new MapperConfiguration(config =>
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
        var expectedDtos = testEntities.Select(testEntity => CreateExpectedAggregateRootDto(testEntity)).ToArray();
        
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
        
        fixture = new Fixture();
        fixture.Register<DomainEvent>(() => null);
        fixture.Customize<AggregateRoot>(comp => comp.Without(x => x.DomainEvents));
        yield return new object[] { fixture.CreateMany<AggregateRoot>(0).ToList() };
    }

    private static AggregateRootDto CreateExpectedAggregateRootDto(AggregateRoot entity)
    {
        return new AggregateRootDto
        {
            Id = entity.Id,
            AggregateAttr = entity.AggregateAttr,
            Aggregate = entity.Aggregate == null ? null : CreateExpectedAggregateRootAggregateSingleCDto(entity.Aggregate),
            Composite = entity.Composite == null ? null : CreateExpectedAggregateRootCompositeSingleADto(entity.Composite),
            Composites = entity.Composites?.Select(CreateExpectedAggregateRootCompositeManyBDto).ToList()
        };
    }

    private static AggregateRootCompositeManyBDto CreateExpectedAggregateRootCompositeManyBDto(CompositeManyB entity)
    {
        return new AggregateRootCompositeManyBDto
        {
            Id = entity.Id,
            CompositeAttr = entity.CompositeAttr,
            SomeDate = entity.SomeDate,
            Composite = entity.Composite == null ? null : CreateExpectedAggregateRootCompositeManyBCompositeSingleBBDto(entity.Composite),
            Composites = entity.Composites?.Select(CreateExpectedAggregateRootCompositeManyBCompositeManyBBDto).ToList(),
            AggregateRootId = entity.AggregateRootId
        };
    }

    private static AggregateRootCompositeManyBCompositeManyBBDto CreateExpectedAggregateRootCompositeManyBCompositeManyBBDto(CompositeManyBB entity)
    {
        return new AggregateRootCompositeManyBCompositeManyBBDto
        {
            Id = entity.Id, 
            CompositeAttr = entity.CompositeAttr, 
            CompositeManyBId = entity.CompositeManyBId
        };
    }

    private static AggregateRootCompositeManyBCompositeSingleBBDto CreateExpectedAggregateRootCompositeManyBCompositeSingleBBDto(CompositeSingleBB entity)
    {
        return new AggregateRootCompositeManyBCompositeSingleBBDto()
        {
            Id = entity.Id, 
            CompositeAttr = entity.CompositeAttr
        };
    }

    private static AggregateRootCompositeSingleADto CreateExpectedAggregateRootCompositeSingleADto(CompositeSingleA entity)
    {
        return new AggregateRootCompositeSingleADto
        {
            Id = entity.Id,
            CompositeAttr = entity.CompositeAttr,
            Composite = entity.Composite == null ? null : CreateExpectedAggregateRootCompositeSingleACompositeSingleAADto(entity.Composite),
            Composites = entity.Composites?.Select(CreateExpectedAggregateRootCompositeSingleACompositeManyAADto).ToList(),
        };
    }

    private static AggregateRootCompositeSingleACompositeManyAADto CreateExpectedAggregateRootCompositeSingleACompositeManyAADto(CompositeManyAA cdto)
    {
        return new AggregateRootCompositeSingleACompositeManyAADto
        {
            Id = cdto.Id, 
            CompositeAttr = cdto.CompositeAttr, 
            CompositeSingleAId = cdto.CompositeSingleAId
        };
    }

    private static AggregateRootCompositeSingleACompositeSingleAADto CreateExpectedAggregateRootCompositeSingleACompositeSingleAADto(CompositeSingleAA entity)
    {
        return new AggregateRootCompositeSingleACompositeSingleAADto()
        {
            Id = entity.Id, 
            CompositeAttr = entity.CompositeAttr
        };
    }

    private static AggregateRootAggregateSingleCDto CreateExpectedAggregateRootAggregateSingleCDto(AggregateSingleC entity)
    {
        return new AggregateRootAggregateSingleCDto
        {
            Id = entity.Id,
            AggregationAttr = entity.AggregationAttr
        };
    }
}