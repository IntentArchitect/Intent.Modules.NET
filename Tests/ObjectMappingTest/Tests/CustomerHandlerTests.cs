using NSubstitute;
using ObjectMappingTest.Application.Customers.GetCustomerById;
using ObjectMappingTest.Application.Customers.GetCustomerDetail;
using ObjectMappingTest.Domain.Common.Exceptions;
using ObjectMappingTest.Domain.Entities;
using ObjectMappingTest.Domain.Repositories;

namespace ObjectMappingTest.Tests;

public class CustomerHandlerTests
{
    [Fact]
    public async Task GetCustomerByIdHandler_CustomerExists_ReturnsMappedDto()
    {
        var customer = BuildCustomer();
        var repository = Substitute.For<ICustomerRepository>();
        repository.FindByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        var handler = new GetCustomerByIdHandler(repository);

        var dto = await handler.Handle(new GetCustomerById(customer.Id), CancellationToken.None);

        Assert.Equal(customer.Id, dto.Id);
        Assert.Equal(customer.Name, dto.Name);
        await repository.Received(1).FindByIdAsync(customer.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCustomerByIdHandler_CustomerMissing_ThrowsNotFoundException()
    {
        var customerId = Guid.NewGuid();
        var repository = Substitute.For<ICustomerRepository>();
        repository.FindByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns((Customer?)null);
        var handler = new GetCustomerByIdHandler(repository);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetCustomerById(customerId), CancellationToken.None));
    }

    [Fact]
    public async Task GetCustomerDetailHandler_CustomerExists_ReturnsMappedDto()
    {
        var customer = BuildCustomer();
        customer.Address = new Address { Street = "1 Test St", City = "Testville", PostalCode = "TST" };
        var repository = Substitute.For<ICustomerRepository>();
        repository.FindByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        var handler = new GetCustomerDetailHandler(repository);

        var dto = await handler.Handle(new GetCustomerDetail(customer.Id), CancellationToken.None);

        Assert.Equal(customer.Id, dto.Id);
        Assert.Equal("1 Test St", dto.Address?.Street);
    }

    [Fact]
    public async Task GetCustomerDetailHandler_CustomerMissing_ThrowsNotFoundException()
    {
        var customerId = Guid.NewGuid();
        var repository = Substitute.For<ICustomerRepository>();
        repository.FindByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns((Customer?)null);
        var handler = new GetCustomerDetailHandler(repository);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetCustomerDetail(customerId), CancellationToken.None));
    }

    private static Customer BuildCustomer() => new Customer
    {
        Id = Guid.NewGuid(),
        Name = "Test Customer",
        Email = "test@example.com",
    };
}
