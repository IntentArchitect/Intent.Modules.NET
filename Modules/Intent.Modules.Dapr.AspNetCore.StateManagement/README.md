# Intent.Dapr.AspNetCore.StateManagement

This module provides patterns for working with Dapr (Distributed Application Runtime) State Management.

## What is Dapr State Management?

Dapr State Management is a component of the Dapr framework designed to simplify the process of managing and maintaining state within distributed and microservices-based applications. It provides an abstraction layer that enables developers to store and retrieve application state without being tightly coupled to specific storage technologies or providers. Dapr State Management supports various state stores, including databases, key-value stores, and cloud services, allowing developers to choose the most suitable option for their application's needs. By abstracting away the complexities of state management, Dapr allows developers to focus on building application logic without worrying about the underlying storage details, thereby promoting modularity and flexibility in developing distributed systems.

For more information on Dapr State Management, check out their [official docs](https://docs.dapr.io/developing-applications/building-blocks/state-management/state-management-overview/).

## What's in this module?

This module consumes your `Domain Model`, which you build in the `Domain Designer` and generates the corresponding Dapr State Management implementation:-

* Unit of Work and associated artifacts.
* Repositories and associated artifacts.
* `app.settings` configuration.
* Dependency Injection wiring.


## Domain Designer

When designing domain models for Dapr State Management your domain package must be annotated with the `Document Database` stereotype. If you have multiple Document DB technologies modules, you must explicitly indicate which Domain Packages contain CosmosDB domain models, by setting `Document Database`'s `Provider` property to Dapr.

![Configure Dapr provider](./docs/images/db-provider-dapr-db.png)
