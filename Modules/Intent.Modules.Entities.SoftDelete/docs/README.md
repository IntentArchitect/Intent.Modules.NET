# Intent.Entities.SoftDelete

This module implements a basic Soft Delete pattern for entities, compatible with both Entity Framework and CosmosDB.

It provides the necessary components to configure entities to follow the `soft delete`` pattern. With this approach, when an entity is deleted, it is not physically removed from the data store. Instead, it is flagged as deleted, and standard CRUD operations will treat the record as if it doesn't exist in the database.

## Configuraing Soft Delete

The `Soft Delete` stereotype can be applied to _Entities_ in the Domain Designer. When applied, a `bool` column named `IsDeleted` is automatically added to the entity. This column tracks whether the record is deleted and cannot be removed as long as the `Soft Delete` stereotype is active.


## Entity Framework and Inheritence

In certain cases, even if the `Soft Delete` stereotype is applied to an entity, the pattern may not be applied as expected. This behavior is intentional.

The pattern will not apply under the following conditions:

- A child entity inherits from a parent entity.
- The child entity has the `Soft Delete` stereotype applied.
- The parent entity does not have the `Soft Delete` stereotype applied.
- The parent entity has a generated configuration (e.g., it is not abstract or has the `Table` stereotype applied).

This design choice prevents runtime errors caused by incorrect Entity Framework Core configurations that would result from the above scenario.