using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.CreateAggregateRootLong;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.UpdateAggregateRootLong;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateRootLongs
{
    public static class AggregateRootLongAssertions
    {
        public static void AssertEquivalent(CreateAggregateRootLongCommand expectedDto, AggregateRootLong actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
            AssertEquivalent(expectedDto.CompositeOfAggrLong, actualEntity.CompositeOfAggrLong);
        }

        public static void AssertEquivalent(
            CreateAggregateRootLongCompositeOfAggrLongDto expectedDto,
            CompositeOfAggrLong actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
        }

        public static void AssertEquivalent(
            PagedResult<AggregateRootLongDto> actualDtos,
            IPagedList<AggregateRootLong> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().Match<PagedResult<AggregateRootLongDto>>(p => p == null || !p.Data.Any());
                return;
            }
            actualDtos.Data.Should().HaveSameCount(expectedEntities);
            actualDtos.PageSize.Should().Be(expectedEntities.PageSize);
            actualDtos.PageCount.Should().Be(expectedEntities.PageCount);
            actualDtos.PageNumber.Should().Be(expectedEntities.PageNo);
            actualDtos.TotalCount.Should().Be(expectedEntities.TotalCount);
            for (int i = 0; i < expectedEntities.Count(); i++)
            {
                var dto = actualDtos.Data.ElementAt(i);
                var entity = expectedEntities.ElementAt(i);
                if (entity == null)
                {
                    dto.Should().BeNull();
                    continue;
                }

                dto.Should().NotBeNull();
                dto.Id.Should().Be(entity.Id);
                dto.Attribute.Should().Be(entity.Attribute);
            }
        }

        public static void AssertEquivalent(AggregateRootLongDto actualDto, AggregateRootLong expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.Attribute.Should().Be(expectedEntity.Attribute);
        }

        public static void AssertEquivalent(UpdateAggregateRootLongCommand expectedDto, AggregateRootLong actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
            AssertEquivalent(expectedDto.CompositeOfAggrLong, actualEntity.CompositeOfAggrLong);
        }

        public static void AssertEquivalent(
            UpdateAggregateRootLongCompositeOfAggrLongDto expectedDto,
            CompositeOfAggrLong actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Attribute.Should().Be(expectedDto.Attribute);
        }
    }
}