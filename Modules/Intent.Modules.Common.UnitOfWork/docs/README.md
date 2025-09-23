# Intent.Common.UnitOfWork

This module provides patterns for working with the **Unit of Work** pattern in your applications. It centralizes transaction handling and persistence control, ensuring that multiple repository operations can be committed or rolled back as a single atomic unit.

## What is the Unit of Work Pattern?

The **Unit of Work (UoW)** pattern is a design pattern that maintains a list of objects affected by a business transaction and coordinates the writing out of changes and the resolution of concurrency problems. It provides:

- **Consistency**: Ensures all database operations in a transaction succeed or none are applied.  
- **Transaction Management**: Encapsulates transaction handling in one place.  
- **Decoupling**: Keeps application and domain logic free of persistence details.  
- **Rollback Support**: Prevents partial updates in case of errors.  

For more information, see [Microsoft’s documentation on the Unit of Work pattern](https://learn.microsoft.com/ef/ef6/fundamentals/repository-pattern#unit-of-work).

## What's in this module?
 
- **Configuration options** for persistence and transaction control.  

## Settings

The module provides two key settings that influence how persistence and transactions are managed.

### Automatically Persist Unit of Work

#### Description

When enabled, the Unit of Work will automatically **persist changes** at the end of an application service or request handler. Developers do not need to explicitly call `SaveChangesAsync()`).  

#### Behavior

- **Enabled**  
  - Persistence is automatic when a request completes successfully.  

- **Disabled**  
  - Developers must explicitly call `SaveChangesAsync()` to persist changes.  
  - Useful in advanced workflows where you want fine-grained control over when persistence happens.


### Use Ambient Transactions

#### Description

When enabled, the Unit of Work enlists operations into an **ambient transaction** using .NET's `TransactionScope`. This allows multiple repositories, multiple `DbContext` instances, or external resources to participate in the same transactional boundary.

#### Behavior

- **Enabled**
  - Ensures **cross-`DbContext`** or distributed operations succeed or fail together.
  - Useful when multiple data sources must be updated atomically.

- **Disabled**
  - Each persistence layer manages its own transaction internally.
  - Recommended for **cloud-native applications**, since distributed transactions are not always supported.

