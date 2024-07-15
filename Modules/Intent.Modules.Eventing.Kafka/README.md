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

## Running Kafka locally using Docker

This guide will walk you through setting up Apache Kafka using Docker Compose, including the necessary steps to update your hosts file for local development purposes. We'll be using the `confluentinc` images for Zookeeper, Kafka, and Schema Registry, which are widely recognized for their ease of use and compatibility with Confluent Platform.

### Prerequisites

Before starting, ensure you have Docker and Docker Compose installed on your system. You can check if they're installed by running:

```bash
docker --version && docker-compose --version
```

If these commands return version numbers, you're good to go. If not, please install Docker and Docker Compose from the official websites.

### Update Your Hosts File

To allow your applications to communicate with Kafka using the hostname "kafka", you need to add an entry to your `/etc/hosts` file (on Linux/macOS) or `%SystemRoot%\System32\drivers\etc\hosts` file (on Windows). Open your hosts file with administrative privileges and add the following line:

```plaintext
127.0.0.1 kafka
```

This tells your system that whenever "kafka" is mentioned, it should resolve to `127.0.0.1`, which is your localhost.

**Note:** This step is crucial for local development environments where services need to communicate using hostnames instead of IP addresses.

### Start Kafka Services Using Docker Compose

Create and populate a `docker-compose.yml` file on your hard drive such as in `C:\Dev\Kafka`.

> [!NOTE]
> 
> This docker-compose.yml file is purely for local development purposes and is not recommended to be used outside that environment.

```yml
version: '3'

services:
  zookeeper:
    image: confluentinc/cp-zookeeper:latest
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181
      ZOOKEEPER_TICK_TIME: 2000
    ports:
      - "2181:2181"

  kafka:
    image: confluentinc/cp-kafka:latest
    depends_on:
      - zookeeper
    environment:
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:9092
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
    ports:
      - "9092:9092"
    links:
      - zookeeper

  schema-registry:
    image: confluentinc/cp-schema-registry:latest
    depends_on:
      - kafka
    environment:
      SCHEMA_REGISTRY_KAFKASTORE_CONNECTION_URL: zookeeper:2181
      SCHEMA_REGISTRY_HOST_NAME: schema-registry
      # Add the following line to specify the Kafka bootstrap servers explicitly
      SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS: kafka:9092
    ports:
      - "8081:8081"
    links:
      - zookeeper
```

With your `docker-compose.yml` file ready and your hosts file updated, you can now start the Kafka services. Navigate to the directory containing your `docker-compose.yml` file and run:

```bash
docker-compose up -d
```

The `-d` flag runs the containers in detached mode, meaning they'll run in the background.

### Verify Kafka Services Are Running

After starting the services, you can verify they're running correctly by checking the logs. Use the following command to view the logs for Kafka:

```bash
docker-compose logs kafka
```

Look for any error messages indicating issues with starting the service. If everything is set up correctly, you shouldn't see any errors related to connectivity or configuration.

### Creating Topics

Run the following command to create topics for this to work:

```powershell
docker exec -it <kafka-container-id> /usr/bin/kafka-topics --create  --replication-factor 1 --partitions 1 --topic <topic name> --bootstrap-server kafka:9092
```