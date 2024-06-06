using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.ODataAggs;
using CleanArchitecture.Comprehensive.Application.ODataAggs.CreateODataAgg;
using CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ODataQuery.ODataAggs
{
    public static class ODataAggAssertions
    {
        public static void AssertEquivalent(CreateODataAggCommand expectedDto, ODataAgg actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
        }

        public static void AssertEquivalent(ODataAggDto actualDto, ODataAgg expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.Name.Should().Be(expectedEntity.Name);
        }
    }
}