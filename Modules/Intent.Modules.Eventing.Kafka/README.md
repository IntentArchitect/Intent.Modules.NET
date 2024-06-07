# Intent.Eventing.Kakfka

This module provides patterns for working with Apache Kafka as a publish/subscribe message system.

## What is Kafka?

Apache Kafka is a distributed event streaming platform that is used for building real-time data pipelines and streaming applications. Kafka is designed to handle large volumes of data in a scalable and fault-tolerant manner, making it ideal for use cases such as real-time analytics, data ingestion, and event-driven architectures.

At its core, Kafka is a distributed publish-subscribe messaging system. Data is written to Kafka topics by producers and consumed from those topics by consumers. Kafka topics can be partitioned, enabling the parallel processing of data, and topics can be replicated across multiple brokers for fault tolerance.

For more information on Kafka, check out their [official docs](https://docs.confluent.io/kafka/).

## Modeling Integration Events and Commands

This module automatically installs the `Intent.Modelers.Eventing` module which provides designer modeling capabilities for integration events. For details on modeling integration events, refer to its [README](https://github.com/IntentArchitect/Intent.Modules/blob/development/Modules/Intent.Modules.Modelers.Eventing/README.md).
