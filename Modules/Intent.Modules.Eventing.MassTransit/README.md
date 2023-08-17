# Intent.Eventing.MassTransit

This module provides patterns for working with MassTransit.

## What is MassTransit?

MassTransit is an open-source distributed application framework for building and managing message-based communication systems in .NET applications. It provides a comprehensive set of tools and abstractions to simplify the implementation of event-driven architectures, allowing components of a system to communicate seamlessly through messages. MassTransit abstracts away the complexities of managing message queues, routing, and serialization, enabling developers to focus on designing and developing the core functionality of their applications. It supports various messaging patterns like publish/subscribe, request/response, and more, making it a versatile choice for building scalable, decoupled, and maintainable systems.

For more information on MassTransit, check out their [official docs](https://masstransit.io//).

## What's in this module?

This module consumes your `Domain Model`, which you build in the `Domain Designer` and generates the corresponding CosmosDB implementation:-

* MassTransit ESB Implimentation
* Message Publishing.
* Message Consumption.
* Multitenancy support through Finbuckle.
* `app.settings` configuration.
* Dependency Injection wiring.

## Multitenancy Finbuckle Integration

If you have the `Intent.Modules.AspNetCore.MultiTenancy` module install, this module will add Multitenancy support to your MassTransit implementation. All outbound messages published will automatically include a tenant idenitifier in the header and all message consumers which encounter messages with a tenant idenitifier will set up the Finbuckle tenanct for the processing of the message.

Notable details of the implemenation:

* Tenancy Publshing Filter, this filter add's the current Tenant Identity to outbound messages.
* Tenancy Consuming Filter, reads the Tenant Identity in inbound messages and configures Finbuckle accordingly.
* Finbucke Message Header Tenancy Strategy, Finback integration with setting up Tencacy through Message headers.

You can configure the name of the header in your `appsettings.json`, by default the header will be "Tenant-Identifier". If you re-configure these make sure the configuration is done across publishers and consuimers.

```json
{
  "MassTransit": {
    "TenantHeader": "My-Tenant-Header"
  }
}
```

## Related Modules

### Intent.Eventing.MassTransit.EntityFrameworkCore

This modules provides patterns around using Entity Framework Core as the technology provider for the OutBox pattern.
