using NSubstitute;
using ObjectMappingTest.Application.Orders;
using ObjectMappingTest.Application.Orders.GetOrderById;
using ObjectMappingTest.Application.Orders.GetOrderDetail;
using ObjectMappingTest.Application.Orders.GetOrderSummaryById;
using ObjectMappingTest.Application.Orders.GetOrderWithCustomer;
using ObjectMappingTest.Domain;
using ObjectMappingTest.Domain.Common.Exceptions;
using ObjectMappingTest.Domain.Entities;
using ObjectMappingTest.Domain.Repositories;

namespace ObjectMappingTest.Tests;

public class OrderHandlerTests
{
    [Fact]
    public async Task GetOrderByIdHandler_OrderExists_ReturnsMappedDto()
    {
        var order = BuildOrder(BuildCustomer());
        var repository = Substitute.For<IOrderRepository>();
        repository.FindByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var handler = new GetOrderByIdHandler(repository);

        var dto = await handler.Handle(new GetOrderById(order.Id), CancellationToken.None);

        Assert.Equal(order.Id, dto.Id);
        Assert.Equal(order.RefNo, dto.RefNo);
        await repository.Received(1).FindByIdAsync(order.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrderByIdHandler_OrderMissing_ThrowsNotFoundException()
    {
        var orderId = Guid.NewGuid();
        var repository = Substitute.For<IOrderRepository>();
        repository.FindByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns((Order?)null);
        var handler = new GetOrderByIdHandler(repository);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetOrderById(orderId), CancellationToken.None));
    }

    [Fact]
    public async Task GetOrderDetailHandler_OrderExists_ReturnsMappedDto()
    {
        var customer = BuildCustomer();
        var order = BuildOrder(customer);
        var repository = Substitute.For<IOrderRepository>();
        repository.FindByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var handler = new GetOrderDetailHandler(repository);

        var dto = await handler.Handle(new GetOrderDetail(order.Id), CancellationToken.None);

        Assert.Equal(order.Id, dto.Id);
        Assert.Equal(customer.Name, dto.CustomerName);
    }

    [Fact]
    public async Task GetOrderDetailHandler_OrderMissing_ThrowsNotFoundException()
    {
        var orderId = Guid.NewGuid();
        var repository = Substitute.For<IOrderRepository>();
        repository.FindByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns((Order?)null);
        var handler = new GetOrderDetailHandler(repository);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetOrderDetail(orderId), CancellationToken.None));
    }

    [Fact]
    public async Task GetOrderSummaryByIdHandler_OrderExists_ReturnsMappedDto()
    {
        var order = BuildOrder(BuildCustomer());
        order.PaymentStatus = PaymentStatus.Paid;
        var repository = Substitute.For<IOrderRepository>();
        repository.FindByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var handler = new GetOrderSummaryByIdHandler(repository);

        var dto = await handler.Handle(new GetOrderSummaryById(order.Id), CancellationToken.None);

        Assert.Equal(order.Id, dto.Id);
        Assert.Equal(PaymentStatusDto.Paid, dto.PaymentStatus);
    }

    [Fact]
    public async Task GetOrderSummaryByIdHandler_OrderMissing_ThrowsNotFoundException()
    {
        var orderId = Guid.NewGuid();
        var repository = Substitute.For<IOrderRepository>();
        repository.FindByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns((Order?)null);
        var handler = new GetOrderSummaryByIdHandler(repository);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetOrderSummaryById(orderId), CancellationToken.None));
    }

    [Fact]
    public async Task GetOrderWithCustomerHandler_OrderExists_ReturnsMappedDto()
    {
        var customer = BuildCustomer();
        var order = BuildOrder(customer);
        var repository = Substitute.For<IOrderRepository>();
        repository.FindByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var handler = new GetOrderWithCustomerHandler(repository);

        var dto = await handler.Handle(new GetOrderWithCustomer(order.Id), CancellationToken.None);

        Assert.Equal(order.Id, dto.Id);
        Assert.Equal(customer.Id, dto.Customer.Id);
    }

    [Fact]
    public async Task GetOrderWithCustomerHandler_OrderMissing_ThrowsNotFoundException()
    {
        var orderId = Guid.NewGuid();
        var repository = Substitute.For<IOrderRepository>();
        repository.FindByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns((Order?)null);
        var handler = new GetOrderWithCustomerHandler(repository);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetOrderWithCustomer(orderId), CancellationToken.None));
    }

    private static Customer BuildCustomer() => new Customer
    {
        Id = Guid.NewGuid(),
        Name = "Test Customer",
        Email = "test@example.com",
    };

    private static Order BuildOrder(Customer customer) => new Order
    {
        Id = Guid.NewGuid(),
        RefNo = "ORD-TEST",
        CustomerId = customer.Id,
        Customer = customer,
        Status = OrderStatus.Pending,
        Lines = [],
        Tags = [],
    };
}
