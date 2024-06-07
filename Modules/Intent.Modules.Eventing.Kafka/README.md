# Intent.Eventing.Kakfka

This module provides patterns for working with Apache Kafka as a publish/subscribe message system.

## What is Kafka?

Apache Kafka is a distributed event streaming platform that is used for building real-time data pipelines and streaming applications. Kafka is designed to handle large volumes of data in a scalable and fault-tolerant manner, making it ideal for use cases such as real-time analytics, data ingestion, and event-driven architectures.

At its core, Kafka is a distributed publish-subscribe messaging system. Data is written to Kafka topics by producers and consumed from those topics by consumers. Kafka topics can be partitioned, enabling the parallel processing of data, and topics can be replicated across multiple brokers for fault tolerance.

For more information on Kafka, check out their [official docs](https://docs.confluent.io/kafka/).

## Overview

This module generates code to work with Kafka's [`Confluent.Kafka` NuGet package](https://www.nuget.org/packages/Confluent.Kafka), in particular:

- Producers, for publishing messages.
- Consumers, for subscribing to and processing messages.
- Connecting to a Kafka Schema Registry so that message contracts are validated for correctness prior to being published.

A Kafka Producer is registered in dependency injection as a singleton for each message type the application is known to publish.

An ASP.NET Core [BackgroundService](https://learn.microsoft.com/aspnet/core/fundamentals/host/hosted-services) is added using [`AddHostedService<THostedService>(IServiceCollection)`](https://learn.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionhostedserviceextensions.addhostedservice) to run a Kafka Consumer for each message an application is subscribed to and ultimately dispatches incoming events to handlers which we generate and to which any additional services can be dependency injected into the constructor as is idiomatic when working with .NET.

To publish a message using manually written code, inject the `IEventBus` interface into the constructor where you want to use it and call the `Publish<T>(T)` method on it. Calling this method merely adds the message to an in-memory collection and is only dispatched to Kafka using a Producer once the `IEventBus`'s `FlushAllAsync(CancellationToken)` method is called. If you're using Intent Architect's standard architecture templates, they already ensure that `FlushAllAsync` is called at the end of each operation, typically after committing database transactions to minimize the possibility of messages being published and a transaction failing shortly after.

For each message type, an entry is added to the `appsettings.json` file allowing you to specify different configuration for different environments as per normal for .NET.

## Modeling Integration Events

This module automatically installs the `Intent.Modelers.Eventing` module which provides designer modeling capabilities for integration events. For details on modeling integration events, refer to its [README](https://github.com/IntentArchitect/Intent.Modules/blob/development/Modules/Intent.Modules.Modelers.Eventing/README.md).
