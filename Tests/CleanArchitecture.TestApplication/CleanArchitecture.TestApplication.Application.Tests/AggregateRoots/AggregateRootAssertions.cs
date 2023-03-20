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
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    private static void AssertEquivalent(CreateAggregateRootCompositeSingleADto expectedDto, CompositeSingleA actualEntity)
    {
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        actualEntity.Composite.Should().NotBeNull();
        actualEntity.Composite.CompositeAttr.Should().Be(expectedDto.Composite.CompositeAttr);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
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
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    private static void AssertEquivalent(CreateAggregateRootCompositeManyBCompositeSingleBBDto? expectedDto, CompositeSingleBB? actualEntity)
    {
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
    }

    public static void AssertEquivalent(UpdateAggregateRootCommand expectedDto, AggregateRoot actualEntity)
    {
        actualEntity.Should().NotBeNull();
        actualEntity.AggregateAttr.Should().Be(expectedDto.AggregateAttr);
        
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    private static void AssertEquivalent(UpdateAggregateRootCompositeSingleADto? expectedDto, CompositeSingleA? actualEntity)
    {
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    private static void AssertEquivalent(UpdateAggregateRootCompositeSingleACompositeSingleAADto? expectedDto, CompositeSingleAA? actualEntity)
    {
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
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
            AssertEquivalent(dto.Composite, entity.Composite);
            AssertEquivalent(dto.Composites, entity.Composites);
        }
    }

    private static void AssertEquivalent(UpdateAggregateRootCompositeManyBCompositeSingleBBDto? expectedDto, CompositeSingleBB? actualEntity)
    {
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
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

    public static void AssertEquivalent(AggregateRoot expectedEntity, AggregateRootDto actualDto)
    {
        actualDto.Id.Should().Be(expectedEntity.Id);
        actualDto.AggregateAttr.Should().Be(expectedEntity.AggregateAttr);
        AssertEquivalent(expectedEntity.Composite, actualDto.Composite);
        AssertEquivalent(expectedEntity.Composites, actualDto.Composites);
    }

    private static void AssertEquivalent(CompositeSingleA? expectedEntity, AggregateRootCompositeSingleADto? actualDto)
    {
        actualDto.Should().NotBeNull();
        actualDto.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
        AssertEquivalent(expectedEntity.Composite, actualDto.Composite);
        AssertEquivalent(expectedEntity.Composites, actualDto.Composites);
    }

    private static void AssertEquivalent(CompositeSingleAA? expectedEntity, AggregateRootCompositeSingleACompositeSingleAADto? actualDto)
    {
        actualDto.Should().NotBeNull();
        actualDto.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
    }

    private static void AssertEquivalent(IEnumerable<CompositeManyAA> expectedEntities, IEnumerable<AggregateRootCompositeSingleACompositeManyAADto> actualDtos)
    {
        actualDtos.Should().HaveSameCount(expectedEntities);
        for (int i = 0; i < actualDtos.Count(); i++)
        {
            var dto = actualDtos.ElementAt(i);
            var entity = expectedEntities.ElementAt(i);
            dto.CompositeAttr.Should().Be(entity.CompositeAttr);
        }
    }

    private static void AssertEquivalent(IEnumerable<CompositeManyB> expectedEntities, IEnumerable<AggregateRootCompositeManyBDto> actualDtos)
    {
        actualDtos.Should().HaveSameCount(expectedEntities);
        for (int i = 0; i < actualDtos.Count(); i++)
        {
            var dto = actualDtos.ElementAt(i);
            var entity = expectedEntities.ElementAt(i);
            dto.CompositeAttr.Should().Be(entity.CompositeAttr);
        }
    }


    public static void AssertEquivalent(CompositeManyB expectedEntity, AggregateRootCompositeManyBDto actualEntity)
    {
        actualEntity.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
        actualEntity.SomeDate.Should().Be(expectedEntity.SomeDate);
        AssertEquivalent(expectedEntity.Composite, actualEntity.Composite);
        AssertEquivalent(expectedEntity.Composites, actualEntity.Composites);
    }

    private static void AssertEquivalent(CompositeSingleBB? expectedEntity, AggregateRootCompositeManyBCompositeSingleBBDto? actualDto)
    {
        actualDto.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
    }
    
    private static void AssertEquivalent(IEnumerable<CompositeManyBB> expectedEntities, IEnumerable<AggregateRootCompositeManyBCompositeManyBBDto> actualDtos)
    {
        actualDtos.Should().HaveSameCount(expectedEntities);
        for (int i = 0; i < actualDtos.Count(); i++)
        {
            var dto = actualDtos.ElementAt(i);
            var entity = expectedEntities.ElementAt(i);
            dto.CompositeAttr.Should().Be(entity.CompositeAttr);
        }
    }
}