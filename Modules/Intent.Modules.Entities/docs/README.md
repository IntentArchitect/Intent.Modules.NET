# Intent.Entities

A simple and extensible Entity pattern in C#. These entities are typically realized from modeling in the Domain Designer and represent the core objects within your domain that require persistence and identity management.

## What is an Entity?

An Entity is a domain object that represents a distinct, identifiable thing in your business domain. Unlike value objects, entities have a unique identity that persists throughout the entity's lifetime—even if all their attributes change. Two entities are considered equal if they have the same identity, regardless of whether their properties match.

### Key Characteristics of Entities

- **Persistent Identity**: Each entity has a unique identifier (e.g., an ID) that distinguishes it from all other entities, even if they have identical attributes
- **Mutable State**: Entities can change their attributes over time while maintaining the same identity
- **Lifecycle**: Entities have a life cycle—they are created, used, modified, and potentially deleted
- **Rich Behavior**: Entities often encapsulate business logic and domain behavior, not just data

### Entities as Aggregate Roots

By default, entities in Intent.Entities function as **Aggregate Roots**—or "Documents" in document database terminology. An aggregate root is an entity that is the primary gateway to a cluster of related objects. 

The aggregate root:
- Serves as the entry point for accessing a group of related entities
- Is responsible for maintaining invariants (business rules) across all objects within its aggregate
- Can reference other entities within its aggregate by their identity
- Transactions typically work at the aggregate root level making aggregates the consistency boundary

However, entities can also be part of a larger composition. When an entity is nested within another entity's aggregate, it becomes a child entity rather than a root, and is accessed only through its parent.

### Entities vs. Data Transfer Objects (DTOs)

Entities and DTOs serve different purposes and should **not be confused**:

| Aspect | Entity | DTO |
|--------|--------|-----|
| **Purpose** | Represents domain business logic and rules | Transfers data across boundaries (API, services, tiers) |
| **Identity** | Has a unique, persistent identity | No identity—only a data container |
| **Behavior** | Contains domain logic and methods | Typically just properties; no behavior |
| **Mutability** | Mutable; designed to change over time | Often immutable or simple containers |
| **Persistence** | Persisted to database | Not persisted; ephemeral |
| **Dependencies** | Can depend on other domain objects | Should have no dependencies |

**Best Practice**: Never pass entities directly across the wire (HTTP API, distributed calls, etc.). Instead, create DTOs specifically designed for that communication boundary and map entities to/from DTOs using tools like AutoMapper or Mapperly. This separation of concerns allows your DTOs to have different structures than your internal entities—you can flatten nested aggregates, expose only necessary fields, combine data from multiple entities, or evolve your API contract independently from your domain model.

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
