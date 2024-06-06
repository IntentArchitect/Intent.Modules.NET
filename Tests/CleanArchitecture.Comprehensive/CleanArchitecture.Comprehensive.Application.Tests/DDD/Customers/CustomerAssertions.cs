using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.DDD;
using CleanArchitecture.Comprehensive.Application.DDD.CreateCustomer;
using CleanArchitecture.Comprehensive.Domain.DDD;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.DDD.Customers
{
    public static class CustomerAssertions
    {
        public static void AssertEquivalent(CreateCustomerCommand expectedDto, Customer actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
            actualEntity.Surname.Should().Be(expectedDto.Surname);
            AssertEquivalent(expectedDto.Address, actualEntity.Address);
            actualEntity.Email.Should().Be(expectedDto.Email);
        }

        public static void AssertEquivalent(CreateCustomerAddressDto expectedDto, Address actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Line1.Should().Be(expectedDto.Line1);
            actualEntity.Line2.Should().Be(expectedDto.Line2);
            actualEntity.City.Should().Be(expectedDto.City);
            actualEntity.Postal.Should().Be(expectedDto.Postal);
        }
    }
}