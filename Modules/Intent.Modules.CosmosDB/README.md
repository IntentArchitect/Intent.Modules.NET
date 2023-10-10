# Intent.CosmosDB

This module provides patterns for working with CosmosDB.

## What is CosmosDB?

Azure Cosmos DB is a globally distributed, multi-model database service provided by Microsoft's Azure cloud platform. It offers a highly scalable, low-latency, and globally distributed data storage solution for modern applications that require seamless scalability and high availability. Cosmos DB supports multiple data models, including document, key-value, graph, column-family, and time-series, allowing developers to choose the best model for their specific application needs. It offers automatic and configurable data replication across Azure regions, ensuring data durability and availability even in the face of regional outages. Cosmos DB also provides comprehensive SLAs for performance, availability, and latency, making it well-suited for applications with demanding requirements. Its multi-model and globally distributed capabilities make Cosmos DB a versatile and robust choice for building responsive and resilient applications on the cloud.

For more information on CosmosDB, check out their [official docs](https://learn.microsoft.com/azure/cosmos-db/).

## What's in this module?

This module consumes your `Domain Model`, which you build in the `Domain Designer` and generates the corresponding CosmosDB implementation:-

* Unit of Work and associated artifacts.
* Cosmos DBDocuments and associated artifacts.
* Repositories and associated artifacts.
* `app.settings` configuration.
* Dependency Injection wiring.

These CosmosDB patterns are realized using [Azure Cosmos DB Repository .NET SDK](https://github.com/IEvangelist/azure-cosmos-dotnet-repository).

## Domain Designer

When designing domain models for CosmosDB your domain package must be annotated with the `Document Database` stereotype. If you have multiple Document DB technologies modules, you must explicitly indicate which Domain Packages contain CosmosDB domain models, by setting `Document Database`'s `Provider` property to CosmosDB.

![Configure CosmosDB provider](./docs/images/db-provider-cosmos-db.png)

### Controlling the Cosmos DB Container and Partition Key for entities

A `Container` Stereotype can applied to `Class`es, `Folder`s and `Domain Package`s to control the Cosmos DB container to be used for particular Classes.

![The Container Stereotype](docs/images/container-stereotype.png)

To determine the Container settings for a particular `Class`, the elements are checked in the following priority for the presence of `Container` stereotype: the `Class` itself, the any parent folders from most deeply to less deeply nested for the class, and then finally the `Domain Package` itself is checked.

Once the stereotype is applied, you can specify settings for it:

![Container stereotype properties](docs/images/container-stereotype-properties.png)

If the "Partition Key" is left blank the class's "id" is used, if specified then you need to ensure that all classes under the container have an attribute named to match the stereotype's "Partition Key" value. The Software Factory will output warnings when classes do not have a matching attribute for its partition key.

If no `Container` stereotype is found as per the above method for a particular `Class`, it uses a default container called "Container" with `id` for the partition key.

## Related Modules

### Intent.Metadata.DocumentDB

This modules provides Document DB related stereotypes for extending the Domain Designer with Document DB technology specific data.

### Intent.Entities

This module generates domain entities as C# classes, which are used by this model.
