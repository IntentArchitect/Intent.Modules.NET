using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ObjectMappingTest.Domain;
using ObjectMappingTest.Domain.Entities;

namespace ObjectMappingTest.Infrastructure.Persistence
{
    public class DataSeeder : IHostedService
    {
        public static readonly Guid CustomerId1       = new("aaaaaaaa-0000-0000-0000-000000000001");
        public static readonly Guid CustomerId2       = new("aaaaaaaa-0000-0000-0000-000000000002");
        public static readonly Guid OrderId1          = new("bbbbbbbb-0000-0000-0000-000000000001");
        public static readonly Guid OrderId2          = new("bbbbbbbb-0000-0000-0000-000000000002");
        public static readonly Guid DigitalProductId1 = new("cccccccc-0000-0000-0000-000000000001");

        private readonly IServiceProvider _services;

        public DataSeeder(IServiceProvider services)
        {
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync(cancellationToken);

            if (await db.Customers.AnyAsync(cancellationToken)) return;

            // DigitalProduct seed
            var digitalProduct1 = new DigitalProduct
            {
                Id = DigitalProductId1,
                Name = "C# Deep Dive",
                Price = 29.99m,
                DownloadUrl = "https://downloads.example.com/csharp-deep-dive.pdf",
            };
            db.DigitalProducts.Add(digitalProduct1);

            // Customer 1 — has billing address, no shipping address
            var customer1 = new Customer
            {
                Id = CustomerId1,
                Name = "Alice",
                Email = "alice@example.com",
                Address = new Address { Street = "10 Billing Rd", City = "Billville", PostalCode = "B1L" },
                ShippingAddress = null,
            };

            // Customer 2 — has both billing and shipping address
            var customer2 = new Customer
            {
                Id = CustomerId2,
                Name = "Bob",
                Email = null,
                Address = new Address { Street = "20 Home St", City = "Hometown", PostalCode = "H0M" },
                ShippingAddress = new Address { Street = "99 Ship Lane", City = "Shiptown", PostalCode = "S9P" },
            };

            db.Customers.AddRange(customer1, customer2);

            // Order 1 — belongs to customer1, has lines, tags, Status=Confirmed, PaymentStatus=Paid
            var order1 = new Order
            {
                Id = OrderId1,
                RefNo = "ORD-001",
                CustomerId = CustomerId1,
                Customer = customer1,
                Status = OrderStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                Lines =
                [
                    new OrderLine { Id = Guid.NewGuid(), ProductName = "Widget A", Qty = 3, UnitPrice = 9.99m },
                    new OrderLine { Id = Guid.NewGuid(), ProductName = "Widget B", Qty = 1, UnitPrice = 49.99m, DiscountCode = "SAVE10" },
                ],
                Tags =
                [
                    new Tag { Id = Guid.NewGuid(), Name = "urgent" },
                    new Tag { Id = Guid.NewGuid(), Name = "fragile" },
                ],
            };

            // Order 2 — belongs to customer2, empty lines, no tags, Status=Pending, PaymentStatus=Pending
            var order2 = new Order
            {
                Id = OrderId2,
                RefNo = "ORD-002",
                CustomerId = CustomerId2,
                Customer = customer2,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                Lines = [],
                Tags = [],
            };

            db.Orders.AddRange(order1, order2);
            await db.SaveChangesAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
