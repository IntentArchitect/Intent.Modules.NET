# Intent.EntityFrameworkCore.BulkOperations

This module provides patterns for doing Bulk data operation with Entity Framework Core using the `EFCore BulkExtensions`.

## What is EFCore BulkExtensions?

Entity Framework Core (EF Core) BulkExtensions is a library designed to enhance the bulk insert, update, and delete operations in Entity Framework Core. Traditional ORM (Object-Relational Mapping) frameworks can be inefficient when dealing with large datasets due to the overhead of individual database calls for each entity. EFCore BulkExtensions addresses this limitation by providing a set of extension methods that allow developers to perform bulk operations, manipulating multiple records in a more efficient and performant manner. This library is particularly useful when dealing with scenarios that involve a large number of database records, as it helps minimize the number of round-trips to the database and significantly improves the overall performance of bulk data manipulation operations within the context of Entity Framework Core.

For more information on EFCore BulkExtensions, read their [official docs](https://entityframework-extensions.net/).

> [!NOTE]
> `EFCore BulkExtensions` requires a paid license for commercial usage, refer to their [website](https://entityframework-extensions.net/) for details.

## What's in this module?

This module extends the Entity Framework Core Repository adding methods for bulk operation support.