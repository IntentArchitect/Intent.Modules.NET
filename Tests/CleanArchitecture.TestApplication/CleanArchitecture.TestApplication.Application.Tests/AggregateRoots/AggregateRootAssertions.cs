using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Domain.Entities;
using FluentAssertions;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public static class AggregateRootAssertions
{
    public static void AssertEquivalent(CreateAggregateRootCommand? expectedDto, AggregateRoot actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }

        actualEntity.Should().NotBeNull();
        actualEntity.AggregateAttr.Should().Be(expectedDto.AggregateAttr);
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    public static void AssertEquivalent(CreateAggregateRootCompositeSingleADto? expectedDto, CompositeSingleA actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    private static void AssertEquivalent(CreateAggregateRootCompositeSingleACompositeSingleAADto? expectedDto, CompositeSingleAA actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
    }

    public static void AssertEquivalent(
        IEnumerable<CreateAggregateRootCompositeManyBDto>? expectedDtos, IEnumerable<CompositeManyB> actualEntities)
    {
        if (expectedDtos == null)
        {
            actualEntities.Should().BeNullOrEmpty();
            return;
        }
        
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.Should().NotBeNull();
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
            entity.SomeDate.Should().Be(dto.SomeDate);
            AssertEquivalent(dto.Composite, entity.Composite);
            AssertEquivalent(dto.Composites, entity.Composites);
        }
    }

    public static void AssertEquivalent(IEnumerable<CreateAggregateRootCompositeManyBCompositeManyBBDto>? expectedDtos, IEnumerable<CompositeManyBB> actualEntities)
    {
        if (expectedDtos == null)
        {
            actualEntities.Should().BeNullOrEmpty();
            return;
        }
        
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.Should().NotBeNull();
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
        }
    }

    public static void AssertEquivalent(
        IEnumerable<CreateAggregateRootCompositeSingleACompositeManyAADto>? expectedDtos, IEnumerable<CompositeManyAA> actualEntities)
    {
        if (expectedDtos == null)
        {
            actualEntities.Should().BeNullOrEmpty();
            return;
        }
        
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.Should().NotBeNull();
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
        }
    }
    
    public static void AssertEquivalent(CreateAggregateRootCompositeManyBCommand? expectedDto, CompositeManyB? actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        actualEntity.SomeDate.Should().Be(expectedDto.SomeDate);
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    public static void AssertEquivalent(CreateAggregateRootCompositeManyBCompositeSingleBBDto? expectedDto, CompositeSingleBB? actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
    }

    public static void AssertEquivalent(UpdateAggregateRootCommand? expectedDto, AggregateRoot actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.AggregateAttr.Should().Be(expectedDto.AggregateAttr);
        
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }
    
    public static void AssertEquivalent(UpdateAggregateRootCompositeManyBCommand? expectedDto, CompositeManyB actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        actualEntity.SomeDate.Should().Be(expectedDto.SomeDate);
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    public static void AssertEquivalent(UpdateAggregateRootCompositeSingleADto? expectedDto, CompositeSingleA? actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
        AssertEquivalent(expectedDto.Composite, actualEntity.Composite);
        AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
    }

    public static void AssertEquivalent(UpdateAggregateRootCompositeSingleACompositeSingleAADto? expectedDto, CompositeSingleAA? actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
    }

    public static void AssertEquivalent(
        IEnumerable<UpdateAggregateRootCompositeSingleACompositeManyAADto>? expectedDtos, IEnumerable<CompositeManyAA> actualEntities)
    {
        if (expectedDtos == null)
        {
            actualEntities.Should().BeNullOrEmpty();
            return;
        }
        
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.Should().NotBeNull();
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
        }
    }

    public static void AssertEquivalent(
        IEnumerable<UpdateAggregateRootCompositeManyBDto>? expectedDtos, IEnumerable<CompositeManyB> actualEntities)
    {
        if (expectedDtos == null)
        {
            actualEntities.Should().BeNullOrEmpty();
            return;
        }
        
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.Should().NotBeNull();
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
            entity.SomeDate.Should().Be(dto.SomeDate);
            AssertEquivalent(dto.Composite, entity.Composite);
            AssertEquivalent(dto.Composites, entity.Composites);
        }
    }

    public static void AssertEquivalent(UpdateAggregateRootCompositeManyBCompositeSingleBBDto? expectedDto, CompositeSingleBB? actualEntity)
    {
        if (expectedDto == null)
        {
            actualEntity.Should().BeNull();
            return;
        }
        
        actualEntity.Should().NotBeNull();
        actualEntity.CompositeAttr.Should().Be(expectedDto.CompositeAttr);
    }

    public static void AssertEquivalent(
        IEnumerable<UpdateAggregateRootCompositeManyBCompositeManyBBDto>? expectedDtos, IEnumerable<CompositeManyBB> actualEntities)
    {
        if (expectedDtos == null)
        {
            actualEntities.Should().BeNullOrEmpty();
            return;
        }
        
        actualEntities.Should().HaveSameCount(actualEntities);
        for (int i = 0; i < expectedDtos.Count(); i++)
        {
            var dto = expectedDtos.ElementAt(i);
            var entity = actualEntities.ElementAt(i);
            entity.Should().NotBeNull();
            entity.CompositeAttr.Should().Be(dto.CompositeAttr);
        }
    }

    public static void AssertEquivalent(AggregateRoot? expectedEntity, AggregateRootDto actualDto)
    {
        if (expectedEntity == null)
        {
            actualDto.Should().BeNull();
            return;
        }
        
        actualDto.Should().NotBeNull();
        actualDto.Id.Should().Be(expectedEntity.Id);
        actualDto.AggregateAttr.Should().Be(expectedEntity.AggregateAttr);
        AssertEquivalent(expectedEntity.Composite, actualDto.Composite);
        AssertEquivalent(expectedEntity.Composites, actualDto.Composites);
    }

    public static void AssertEquivalent(CompositeSingleA? expectedEntity, AggregateRootCompositeSingleADto? actualDto)
    {
        if (expectedEntity == null)
        {
            actualDto.Should().BeNull();
            return;
        }
        
        actualDto.Should().NotBeNull();
        actualDto.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
        AssertEquivalent(expectedEntity.Composite, actualDto.Composite);
        AssertEquivalent(expectedEntity.Composites, actualDto.Composites);
    }

    public static void AssertEquivalent(CompositeSingleAA? expectedEntity, AggregateRootCompositeSingleACompositeSingleAADto? actualDto)
    {
        if (expectedEntity == null)
        {
            actualDto.Should().BeNull();
            return;
        }
        
        actualDto.Should().NotBeNull();
        actualDto.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
    }

    public static void AssertEquivalent(IEnumerable<CompositeManyAA>? expectedEntities, IEnumerable<AggregateRootCompositeSingleACompositeManyAADto> actualDtos)
    {
        if (expectedEntities == null)
        {
            actualDtos.Should().BeNullOrEmpty();
            return;
        }
        
        actualDtos.Should().HaveSameCount(expectedEntities);
        for (int i = 0; i < actualDtos.Count(); i++)
        {
            var dto = actualDtos.ElementAt(i);
            var entity = expectedEntities.ElementAt(i);
            dto.Should().NotBeNull();
            dto.CompositeAttr.Should().Be(entity.CompositeAttr);
        }
    }

    public static void AssertEquivalent(IEnumerable<CompositeManyB>? expectedEntities, IEnumerable<AggregateRootCompositeManyBDto> actualDtos)
    {
        if (expectedEntities == null)
        {
            actualDtos.Should().BeNullOrEmpty();
            return;
        }
        
        actualDtos.Should().HaveSameCount(expectedEntities);
        for (int i = 0; i < actualDtos.Count(); i++)
        {
            var dto = actualDtos.ElementAt(i);
            var entity = expectedEntities.ElementAt(i);
            dto.Should().NotBeNull();
            dto.CompositeAttr.Should().Be(entity.CompositeAttr);
        }
    }


    public static void AssertEquivalent(CompositeManyB? expectedEntity, AggregateRootCompositeManyBDto actualDto)
    {
        if (expectedEntity == null)
        {
            actualDto.Should().BeNull();
            return;
        }
        
        actualDto.Should().NotBeNull();
        actualDto.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
        actualDto.SomeDate.Should().Be(expectedEntity.SomeDate);
        AssertEquivalent(expectedEntity.Composite, actualDto.Composite);
        AssertEquivalent(expectedEntity.Composites, actualDto.Composites);
    }

    public static void AssertEquivalent(CompositeSingleBB? expectedEntity, AggregateRootCompositeManyBCompositeSingleBBDto? actualDto)
    {
        if (expectedEntity == null)
        {
            actualDto.Should().BeNull();
            return;
        }
        
        actualDto.Should().NotBeNull();
        actualDto.CompositeAttr.Should().Be(expectedEntity.CompositeAttr);
    }
    
    public static void AssertEquivalent(IEnumerable<CompositeManyBB>? expectedEntities, IEnumerable<AggregateRootCompositeManyBCompositeManyBBDto> actualDtos)
    {
        if (expectedEntities == null)
        {
            actualDtos.Should().BeNullOrEmpty();
            return;
        }
        
        actualDtos.Should().HaveSameCount(expectedEntities);
        for (int i = 0; i < actualDtos.Count(); i++)
        {
            var dto = actualDtos.ElementAt(i);
            var entity = expectedEntities.ElementAt(i);
            dto.Should().NotBeNull();
            dto.CompositeAttr.Should().Be(entity.CompositeAttr);
        }
    }

    public static void AssertEquivalent(IEnumerable<AggregateRoot>? expectedEntities, IEnumerable<AggregateRootDto> actualDtos)
    {
        if (expectedEntities == null)
        {
            actualDtos.Should().BeNullOrEmpty();
            return;
        }
        
        actualDtos.Should().HaveSameCount(expectedEntities);
        for (int i = 0; i < actualDtos.Count(); i++)
        {
            var dto = actualDtos.ElementAt(i);
            var entity = expectedEntities.ElementAt(i);
            dto.Should().NotBeNull();
            dto.Id.Should().Be(entity.Id);
            dto.AggregateAttr.Should().Be(entity.AggregateAttr);
            AssertEquivalent(entity.Composite, dto.Composite);
            AssertEquivalent(entity.Composites, dto.Composites);
        }
    }
}