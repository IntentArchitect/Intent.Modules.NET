using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.DDD;
using CleanArchitecture.Comprehensive.Application.DDD.CreateCustomer;
using CleanArchitecture.Comprehensive.Application.Tests.DDD.Customers;
using CleanArchitecture.Comprehensive.Application.Tests.Extensions;
using CleanArchitecture.Comprehensive.Domain.DDD;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.DDD
{
    public class CreateCustomerCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateCustomerCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsCustomerToRepository(CreateCustomerCommand testCommand)
        {
            // Arrange
            var customerRepository = Substitute.For<ICustomerRepository>();
            var expectedCustomerId = new Fixture().Create<System.Guid>();
            Customer addedCustomer = null;
            customerRepository.OnAdd(ent => addedCustomer = ent);
            customerRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedCustomer.Id = expectedCustomerId);

            var sut = new CreateCustomerCommandHandler(customerRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedCustomerId);
            await customerRepository.UnitOfWork.Received(1).SaveChangesAsync();
            CustomerAssertions.AssertEquivalent(testCommand, addedCustomer);
        }
    }
}