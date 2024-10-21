# Intent.SharedKernel

## Shared Kernel Overview

A Shared Kernel is a pattern from Domain-Driven Design (DDD) where a set of common domain models, logic, and business rules are shared between two or more bounded contexts or applications. It represents the portion of the domain that multiple teams or systems collaborate on and use together, ensuring consistency in core functionality while allowing the rest of the domain to remain independent. Teams must communicate closely to coordinate changes in the shared kernel to avoid conflicts or issues in integration. This pattern is often used in situations where certain business rules or models are too critical or central to be duplicated across different systems but still need to be reused.

## Module Overview

This module sets up an Intent Architect application to be a `Shared Kernel` for other Intent Architect applications yo use. You can model and implement the following concepts in this Shared Kernel application.

- Domain Entities
- Domain Events
- Domain Event Handlers
- Domain Services
- Traditional Application Services

This application will produce 3 assemblies

- Domain assembly
- Infrastructure assembly
- Application assembly

These assemblies can then be shared / referenced in your consuming applications which on built on top of the Shared Kernel.

## How to set this up in Intent Architect

1. Create a new solution using our `Empty Visual Studio Solution` Template. The name of the Application will be the name of your Shared Kernel application.
2. Install the `Intent.SharedKernel` module into your `Shared Kernel application`.
3. Create a new application using the `Clean Architecture .NET` application template, this will be an application which uses the Shared Kernel, lets call it the `Consumer application`.
4. In the `Consumer application` , Install the `Intent.SharedKernel.Consumer` module.
5. In the `Consumer application`, set the `Shared Kernel Application Id` setting in the `Shared Kernel` section, to be the application Id on the `Shared Kernel application`. (You can get the applicationId from it's application settings in the `*application.config` file)
6. In the `Consumer application`, in the `Domain Designer` add a package reference to the Shared Kernel's domain package.
7. In the `Consumer application`, in the `Service Designer` add a package reference to the Shared Kernel's domain package.


## How does it work ?

The `Shared Kernel` application produces 3 assemblies for you to referencing in you consuming applications.

The `Shared Kernel Consumer` application will be set up with project references to the `Shared Kernel` assemblies.

The `Shared Kernel Consumer` is set up in such a way that you can use the `Shared Kernel` models in your `Shared Kernel Consumer` as if they were modeled in your `Shared Kernel Consumer` application but will resolve to the types in the Shared Kernel assemblies.


