using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.CustomerCTS;
using CleanArchitecture.TestApplication.Application.CustomerCTS.UpdateCustomerCT;
using CleanArchitecture.TestApplication.Domain.Entities.ComplexTypes;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ComplexTypes.CustomerCTs
{
    public static class CustomerCTAssertions
    {
        public static void AssertEquivalent(CustomerCTDto actualDto, CustomerCT expectedEntity)
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

        public static void AssertEquivalent(UpdateCustomerCTCommand expectedDto, CustomerCT actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
        }
    }
}