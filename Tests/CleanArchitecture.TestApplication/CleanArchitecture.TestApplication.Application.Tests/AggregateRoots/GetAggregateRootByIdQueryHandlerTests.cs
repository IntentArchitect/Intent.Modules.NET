using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRootById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class GetAggregateRootByIdQueryHandlerTests
{
    private readonly IMapper _mapper;

    public GetAggregateRootByIdQueryHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(config =>
        {
            config.AddMaps(typeof(GetAggregateRootByIdQueryHandler));
        });
        _mapper = mapperConfiguration.CreateMapper();
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public async Task Handle_WithValidQuery_RetrievesAggregateRoot(AggregateRoot testEntity)
    {
        // Arrange
        var expectedDto = CreateExpectedAggregateRootDto(testEntity);
        
        var query = new GetAggregateRootByIdQuery { Id = testEntity.Id };
        var repository = Substitute.For<IAggregateRootRepository>();
        repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult(testEntity));

        var sut = new GetAggregateRootByIdQueryHandler(repository, _mapper);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }
    
    [Fact]
    public async Task Handle_WithInvalidIdQuery_ReturnsEmptyResult()
    {
        // Arrange
        var query = new GetAggregateRootByIdQuery();
        query.Id = Guid.NewGuid();
        
        var repository = Substitute.For<IAggregateRootRepository>();
        var fixture = new Fixture();
        fixture.Register<DomainEvent>(() => null);
        repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult<AggregateRoot>(default));

        var sut = new GetAggregateRootByIdQueryHandler(repository, _mapper);
        
        // Act
        var result = await sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().Be(null);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        var fixture = new Fixture();
        fixture.Register<DomainEvent>(() => null);
        fixture.Customize<AggregateRoot>(comp => comp.Without(x => x.DomainEvents));
        yield return new object[] { fixture.Create<AggregateRoot>() };
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