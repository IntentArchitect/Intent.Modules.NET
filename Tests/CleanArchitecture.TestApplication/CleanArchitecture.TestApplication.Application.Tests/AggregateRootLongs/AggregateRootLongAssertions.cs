using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.CreateAggregateRootLong;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.UpdateAggregateRootLong;
using CleanArchitecture.TestApplication.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRootLongs
{
    public static class AggregateRootLongAssertions
    {
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
            AssertEquivalent(actualDto.CompositeOfAggrLong, expectedEntity.CompositeOfAggrLong);
        }

        public static void AssertEquivalent(
            AggregateRootLongCompositeOfAggrLongDto actualDto,
            CompositeOfAggrLong expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Attribute.Should().Be(expectedEntity.Attribute);
            actualDto.Id.Should().Be(expectedEntity.Id);
        }

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
    }
}