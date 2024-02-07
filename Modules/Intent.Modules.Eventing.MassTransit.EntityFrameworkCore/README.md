# Intent.Eventing.MassTransit.EntityFrameworkCore

This module extends the `Intent.Eventing.MassTransit` module, by adding support for the Outbox pattern using EntityFramework Core.

For more info on the MassTransit Outbox pattern, check out their [documentation](https://masstransit.io/documentation/patterns/transactional-outbox).

## What is the Outbox pattern about?

The MassTransit Outbox pattern is a strategy used in distributed systems where messages to be sent between different components or services are temporarily stored in an "outbox" database table within the application's transactional context. These messages are then dispatched asynchronously by a separate process, ensuring reliable communication and data consistency without impacting the main transaction. This decoupling of message sending from the core transactional logic enhances performance and fault tolerance in event-driven architectures, and MassTransit is an open-source framework that supports implementing this pattern effectively within .NET applications.

## What's in this module?

This module enhances the `Intent.Eventing.MassTransit` module in the following ways:

* Configures Outbox infrastructure.
* Configures the `DbContext` class for the Outbox pattern.

### Configures Outbox infrastructure

```csharp
    services.AddMassTransit(x =>
    {
        ...
        x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
        {
            o.UseSqlServer();
            o.UseBusOutbox();
        });
    });
```

### Configures the `DbContext` class for the Outbox pattern

```csharp
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureModel(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
```
