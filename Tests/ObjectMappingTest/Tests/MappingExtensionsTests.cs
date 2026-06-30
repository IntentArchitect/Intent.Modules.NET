using ObjectMappingTest.Application.Customers;
using ObjectMappingTest.Application.Orders;
using ObjectMappingTest.Application.Products;
using ObjectMappingTest.Domain;
using ObjectMappingTest.Domain.Entities;

namespace ObjectMappingTest.Tests;

public class MappingExtensionsTests
{
    // ── Shape: flat primitives + nullable scalar ─────────────────────────────
    [Fact]
    public void MapToOrderLineDto_MapsAllFlatProperties()
    {
        var line = new OrderLine
        {
            Id = Guid.NewGuid(),
            ProductName = "Widget",
            Qty = 3,
            DiscountCode = "SAVE10",
            UnitPrice = 9.99m,
        };

        var dto = line.MapToOrderLineDto();

        Assert.Equal(line.Id, dto.Id);
        Assert.Equal(line.ProductName, dto.ProductName);
        Assert.Equal(line.Qty, dto.Qty);
        Assert.Equal(line.DiscountCode, dto.DiscountCode);
        Assert.Equal(line.UnitPrice, dto.UnitPrice);
    }

    [Fact]
    public void MapToOrderLineDto_NullableScalar_MapsNullWhenAbsent()
    {
        var line = new OrderLine { Id = Guid.NewGuid(), ProductName = "X", Qty = 1, UnitPrice = 1m };

        var dto = line.MapToOrderLineDto();

        Assert.Null(dto.DiscountCode);
    }

    // ── Shape: nullable navigation → nullable DTO (null-conditional ?.) ──────
    [Fact]
    public void MapToCustomerDto_WithAddress_MapsNullableNavigation()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Alice",
            Email = "alice@example.com",
            Address = new Address { Street = "1 Main St", City = "Springfield", PostalCode = "12345" },
        };

        var dto = customer.MapToCustomerDto();

        Assert.NotNull(dto.Address);
        Assert.Equal("1 Main St", dto.Address.Street);
        Assert.Equal("Springfield", dto.Address.City);
    }

    [Fact]
    public void MapToCustomerDto_WithoutAddress_ReturnsNullAddress()
    {
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Bob", Address = null };

        var dto = customer.MapToCustomerDto();

        Assert.Null(dto.Address);
    }

    // ── Shape: two named nullable navigations to the same type ───────────────
    [Fact]
    public void MapToCustomerDetailDto_WithBothAddresses_MapsDistinctNavs()
    {
        var billingAddr  = new Address { Street = "10 Billing Rd", City = "Billville", PostalCode = "B1L" };
        var shippingAddr = new Address { Street = "99 Ship Lane", City = "Shiptown", PostalCode = "S9P" };

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Charlie",
            Email = "charlie@example.com",
            Address = billingAddr,
            ShippingAddress = shippingAddr,
        };

        var dto = customer.MapToCustomerDetailDto();

        Assert.Equal("10 Billing Rd", dto.Address?.Street);
        Assert.Equal("99 Ship Lane", dto.ShippingAddress?.Street);
        Assert.NotEqual(dto.Address?.Street, dto.ShippingAddress?.Street);
    }

    [Fact]
    public void MapToCustomerDetailDto_NullShippingAddress_ReturnsNullShippingField()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Dave",
            Address = new Address { Street = "1 A St", City = "C", PostalCode = "P" },
            ShippingAddress = null,
        };

        var dto = customer.MapToCustomerDetailDto();

        Assert.NotNull(dto.Address);
        Assert.Null(dto.ShippingAddress);
    }

    // ── Shape: collection of nested DTOs ─────────────────────────────────────
    [Fact]
    public void MapToOrderDto_MapsCollectionViaNestedExtensionMethod()
    {
        var customerId = Guid.NewGuid();
        var order = new Order
        {
            Id = Guid.NewGuid(),
            RefNo = "ORD-001",
            CustomerId = customerId,
            Status = OrderStatus.Confirmed,
            Lines =
            [
                new OrderLine { Id = Guid.NewGuid(), ProductName = "A", Qty = 2, UnitPrice = 5m },
                new OrderLine { Id = Guid.NewGuid(), ProductName = "B", Qty = 1, UnitPrice = 10m, DiscountCode = "HALF" },
            ],
        };

        var dto = order.MapToOrderDto();

        Assert.Equal(order.Id, dto.Id);
        Assert.Equal("ORD-001", dto.RefNo);
        Assert.Equal(customerId, dto.CustomerId);
        Assert.Equal(2, dto.OrderLines.Count);
        Assert.Equal("HALF", dto.OrderLines[1].DiscountCode);
    }

    // ── Shape: enum mapping ───────────────────────────────────────────────────
    [Fact]
    public void MapToOrderDetailDto_MapsEnumField()
    {
        var customer = BuildCustomerWithAddress();
        var order = BuildOrder(customer, OrderStatus.Shipped);

        var dto = order.MapToOrderDetailDto();

        Assert.Equal(OrderStatus.Shipped, dto.Status);
    }

    // ── Shape: 2-hop path flattening (Order→Customer→Name) ───────────────────
    [Fact]
    public void MapToOrderDetailDto_FlattensCustomerNameViaTwoHopPath()
    {
        var customer = BuildCustomerWithAddress();
        customer.Name = "Eve";
        var order = BuildOrder(customer, OrderStatus.Pending);

        var dto = order.MapToOrderDetailDto();

        Assert.Equal("Eve", dto.CustomerName);
    }

    // ── Shape: 2-hop nullable scalar (Order→Customer→Email?) ─────────────────
    [Fact]
    public void MapToOrderDetailDto_MapsNullableEmailViaTwoHopPath()
    {
        var customer = BuildCustomerWithAddress();
        customer.Email = "eve@example.com";
        var order = BuildOrder(customer, OrderStatus.Pending);

        var dto = order.MapToOrderDetailDto();

        Assert.Equal("eve@example.com", dto.CustomerEmail);
    }

    [Fact]
    public void MapToOrderDetailDto_NullEmail_ReturnsNullCustomerEmail()
    {
        var customer = BuildCustomerWithAddress();
        customer.Email = null;
        var order = BuildOrder(customer, OrderStatus.Pending);

        var dto = order.MapToOrderDetailDto();

        Assert.Null(dto.CustomerEmail);
    }

    // ── Shape: 3-hop to nested DTO (Order→Customer→Address?) ─────────────────
    [Fact]
    public void MapToOrderDetailDto_FlattensCustomerAddressViaThreeHopPath()
    {
        var customer = BuildCustomerWithAddress();
        customer.Address = new Address { Street = "7 Oak Ave", City = "Treetown", PostalCode = "T7T" };
        var order = BuildOrder(customer, OrderStatus.Confirmed);

        var dto = order.MapToOrderDetailDto();

        Assert.NotNull(dto.CustomerAddress);
        Assert.Equal("7 Oak Ave", dto.CustomerAddress.Street);
    }

    [Fact]
    public void MapToOrderDetailDto_NullCustomerAddress_ReturnsNullCustomerAddressField()
    {
        var customer = BuildCustomerWithAddress();
        customer.Address = null;
        var order = BuildOrder(customer, OrderStatus.Pending);

        var dto = order.MapToOrderDetailDto();

        Assert.Null(dto.CustomerAddress);
    }

    // ── Shape: FK-list from collection (Order.Tags[*].Id) ────────────────────
    [Fact]
    public void MapToOrderDetailDto_ExtractsFkIdListFromCollection()
    {
        var tag1 = new Tag { Id = Guid.NewGuid(), Name = "urgent" };
        var tag2 = new Tag { Id = Guid.NewGuid(), Name = "fragile" };
        var customer = BuildCustomerWithAddress();
        var order = BuildOrder(customer, OrderStatus.Pending);
        order.Tags = [tag1, tag2];

        var dto = order.MapToOrderDetailDto();

        Assert.Equal(2, dto.TagIds.Count);
        Assert.Contains(tag1.Id, dto.TagIds);
        Assert.Contains(tag2.Id, dto.TagIds);
    }

    [Fact]
    public void MapToOrderDetailDto_EmptyTagCollection_ReturnsEmptyTagIds()
    {
        var customer = BuildCustomerWithAddress();
        var order = BuildOrder(customer, OrderStatus.Pending);
        order.Tags = [];

        var dto = order.MapToOrderDetailDto();

        Assert.Empty(dto.TagIds);
    }

    // ── Shape: non-nullable nested navigation (no ?.) ─────────────────────────
    [Fact]
    public void MapToOrderWithCustomerDto_EmbeddsFullCustomerDto()
    {
        var customer = BuildCustomerWithAddress();
        customer.Name = "Frank";
        var order = BuildOrder(customer, OrderStatus.Delivered);

        var dto = order.MapToOrderWithCustomerDto();

        Assert.NotNull(dto.Customer);
        Assert.Equal("Frank", dto.Customer.Name);
        Assert.Equal(customer.Id, dto.Customer.Id);
    }

    // ── Shape: collection of non-primitive nested DTOs (Tags → TagDto) ────────
    [Fact]
    public void MapToOrderWithCustomerDto_MapsTagsToTagDtoCollection()
    {
        var customer = BuildCustomerWithAddress();
        var order = BuildOrder(customer, OrderStatus.Confirmed);
        order.Tags = [
            new Tag { Id = Guid.NewGuid(), Name = "alpha" },
            new Tag { Id = Guid.NewGuid(), Name = "beta" },
        ];

        var dto = order.MapToOrderWithCustomerDto();

        Assert.Equal(2, dto.Tags.Count);
        Assert.Equal("alpha", dto.Tags[0].Name);
        Assert.Equal("beta", dto.Tags[1].Name);
    }

    // ── Shape: List extension method ─────────────────────────────────────────
    [Fact]
    public void MapToOrderDtoList_MapsAllItemsInEnumerable()
    {
        var customer = BuildCustomerWithAddress();
        var orders = new List<Order>
        {
            BuildOrder(customer, OrderStatus.Pending),
            BuildOrder(customer, OrderStatus.Shipped),
        };
        orders[0].RefNo = "A";
        orders[1].RefNo = "B";

        var dtos = orders.MapToOrderDtoList();

        Assert.Equal(2, dtos.Count);
        Assert.Equal("A", dtos[0].RefNo);
        Assert.Equal("B", dtos[1].RefNo);
    }

    // ── Shape: enum type mismatch → cast ─────────────────────────────────────
    [Fact]
    public void MapToOrderSummaryDto_CastsDomainEnumToApplicationEnum()
    {
        var customer = BuildCustomerWithAddress();
        var order = new Order
        {
            Id = Guid.NewGuid(),
            RefNo = "ORD-CAST",
            CustomerId = customer.Id,
            Customer = customer,
            Status = OrderStatus.Confirmed,
            PaymentStatus = PaymentStatus.Paid,
            Lines = [],
            Tags = [],
        };

        var dto = order.MapToOrderSummaryDto();

        Assert.Equal(PaymentStatusDto.Paid, dto.PaymentStatus);
    }

    // ── Shape: parameterless method call ─────────────────────────────────────
    [Fact]
    public void MapToOrderSummaryDto_CallsMethodForDisplayName()
    {
        var customer = BuildCustomerWithAddress();
        var order = new Order
        {
            Id = Guid.NewGuid(),
            RefNo = "ORD-METHOD",
            CustomerId = customer.Id,
            Customer = customer,
            Status = OrderStatus.Shipped,
            PaymentStatus = PaymentStatus.Pending,
            Lines = [],
            Tags = [],
        };

        var dto = order.MapToOrderSummaryDto();

        Assert.Equal("ORD-METHOD (Shipped)", dto.DisplayName);
    }

    // ── Shape: inheritance path filtering (generalization hop stripped) ───────
    [Fact]
    public void MapToDigitalProductDto_MapsInheritedPropertiesDirectly()
    {
        var product = new DigitalProduct
        {
            Id = Guid.NewGuid(),
            Name = "C# Deep Dive",
            Price = 29.99m,
            DownloadUrl = "https://downloads.example.com/csharp-deep-dive.pdf",
        };

        var dto = product.MapToDigitalProductDto();

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal("C# Deep Dive", dto.Name);
        Assert.Equal(29.99m, dto.Price);
        Assert.Equal("https://downloads.example.com/csharp-deep-dive.pdf", dto.DownloadUrl);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    private static Customer BuildCustomerWithAddress() => new Customer
    {
        Id = Guid.NewGuid(),
        Name = "Test Customer",
        Email = "test@example.com",
        Address = new Address { Street = "1 Test St", City = "Testville", PostalCode = "TST" },
        ShippingAddress = null,
    };

    private static Order BuildOrder(Customer customer, OrderStatus status) => new Order
    {
        Id = Guid.NewGuid(),
        RefNo = "ORD-TEST",
        CustomerId = customer.Id,
        Customer = customer,
        Status = status,
        Lines = [],
        Tags = [],
    };
}
