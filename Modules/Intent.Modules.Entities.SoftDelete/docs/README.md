# Intent.Entities.SoftDelete

This module implements a basic Soft Delete pattern for entities, compatible with both Entity Framework and CosmosDB.

It provides the necessary components to configure entities to follow the `soft delete`` pattern. With this approach, when an entity is deleted, it is not physically removed from the data store. Instead, it is flagged as deleted, and standard CRUD operations will treat the record as if it doesn't exist in the database.

## Configuraing Soft Delete

The `Soft Delete` stereotype can be applied to _Entities_ in the Domain Designer. When applied, a `bool` column named `IsDeleted` is automatically added to the entity. This column tracks whether the record is deleted and cannot be removed as long as the `Soft Delete` stereotype is active.

## Entity Framework and Inheritance

If the parent entity of a child entity has the `Soft Delete` pattern applied and an Entity Framework configuration (`IEntityTypeConfiguration`) is generated for the parent, the child entity's configuration will not explicitly include any `Soft Delete` functionality. Instead, this functionality is inherited from the parent entity's configuration.

On the other hand, if the parent entity does not have the `Soft Delete` pattern applied, and an Entity Framework configuration (`IEntityTypeConfiguration`) has been generated for the parent,  neither the child nor the parent will have any `Soft Delete` functionality applied. This behavior is intentional, as applying the pattern to the child in this case would result in runtime errors due to incorrect Entity Framework Core configuration.
