using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot;
using CleanArchitecture.TestApplication.Domain.Entities;
using FluentAssertions;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public static class AggregateRootAssertions
{
    public static void AssertEquivalent(CreateAggregateRootCommand expectedDto, AggregateRoot actualEntity)
    {
        actualEntity.Should().NotBeNull();
        actualEntity.AggregateAttr.Should().Be(expectedDto.AggregateAttr);
        actualEntity.Composite.Should().NotBeNull();
        actualEntity.Composite.CompositeAttr.Should().Be(expectedDto.Composite.CompositeAttr);
        actualEntity.Composite.Composite.Should().NotBeNull();
        actualEntity.Composite.Composite.CompositeAttr.Should().Be(expectedDto.Composite.Composite.CompositeAttr);

        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
        AssertEquivalent(expectedDto.Composite.Composites, actualEntity.Composite.Composites);
    }

    public static void AssertEquivalent(
        IEnumerable<CreateAggregateRootCompositeManyBDto> expectedDtos, IEnumerable<CompositeManyB> actualEntities)
    {
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
            entity.SomeDate.Should().Be(dto.SomeDate);
            entity.Composite.CompositeAttr.Should().Be(dto.Composite.CompositeAttr);
            AssertEquivalent(dto.Composites, entity.Composites);
        }
    }

    public static void AssertEquivalent(IEnumerable<CreateAggregateRootCompositeManyBCompositeManyBBDto> expectedDtos, IEnumerable<CompositeManyBB> actualEntities)
    {
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
        }
    }

    public static void AssertEquivalent(
        IEnumerable<CreateAggregateRootCompositeSingleACompositeManyAADto> expectedDtos, IEnumerable<CompositeManyAA> actualEntities)
    {
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
        }
    }
    
    public static void AssertEquivalent(CreateAggregateRootCompositeManyBCommand expectedDto, CompositeManyB? actualEntity)
    {
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        actualEntity.SomeDate.Should().Be(expectedDto.SomeDate);
        actualEntity.Composite.CompositeAttr.Should().Be(expectedDto.Composite.CompositeAttr);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }
    
    public static void AssertEquivalent(UpdateAggregateRootCommand expectedDto, AggregateRoot actualEntity)
    {
        actualEntity.Should().NotBeNull();
        actualEntity.AggregateAttr.Should().Be(expectedDto.AggregateAttr);
        actualEntity.Composite.Should().NotBeNull();
        actualEntity.Composite.CompositeAttr.Should().Be(expectedDto.Composite.CompositeAttr);
        actualEntity.Composite.Composite.Should().NotBeNull();
        actualEntity.Composite.Composite.CompositeAttr.Should().Be(expectedDto.Composite.Composite.CompositeAttr);

        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
        AssertEquivalent(expectedDto.Composite.Composites, actualEntity.Composite.Composites);
    }

    private static void AssertEquivalent(
        IEnumerable<UpdateAggregateRootCompositeSingleACompositeManyAADto> expectedDtos, IEnumerable<CompositeManyAA> actualEntities)
    {
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
        }
    }

    private static void AssertEquivalent(
        IEnumerable<UpdateAggregateRootCompositeManyBDto> expectedDtos, IEnumerable<CompositeManyB> actualEntities)
    {
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
            entity.SomeDate.Should().Be(dto.SomeDate);
            entity.Composite.CompositeAttr.Should().Be(dto.Composite.CompositeAttr);
            AssertEquivalent(dto.Composites, entity.Composites);
        }
    }

    private static void AssertEquivalent(
        IEnumerable<UpdateAggregateRootCompositeManyBCompositeManyBBDto> expectedDtos, IEnumerable<CompositeManyBB> actualEntities)
    {
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
        }
    }
}