# Intent.Entities

A simple and extensible Entity pattern in C#. These entities are typically realized from modeling in the Domain Designer and often used as data contracts for persistence modules.

## What is the Entity Pattern?

The entity pattern refers to a fundamental concept in computer science and data modeling, focusing on the identification and classification of distinct objects, individuals, or elements within a system. This pattern involves recognizing and defining entities as unique, distinguishable units that possess attributes describing their characteristics. Entities can have relationships with one another, forming the basis of a structured representation of real-world scenarios or data. The entity pattern is commonly used in database design, where entities become tables and attributes become columns, facilitating efficient data storage, retrieval, and manipulation. By capturing the inherent structure and connections in a given domain, the entity pattern lays the groundwork for creating organized and meaningful information systems.

## What's in this module?

This module consumes your `Domain Model`, which you build in the `Domain Designer` and generates the corresponding implementations:-

* Entities (POCOs)
* Entity Contracts
* State behaviour seperatation

## Related Modules

### Intent.EntityFrameworkCore

This module provides persistence patterns based on Entity Framework Core.

### Intent.CosmosDB

This module provides persistence patterns based on CosmosDB.

### Intent.MongoDB

This module provides persistence patterns based on MongoDB.
