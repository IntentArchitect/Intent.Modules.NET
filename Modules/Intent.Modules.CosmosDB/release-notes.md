﻿### Version 1.1.2

- New Feature: Added support for separate database multi tenancy.

### Version 1.1.1

- Fixed: If key types other than `string` were being used for an entity and `Use Optimistic Concurrency` was enabled then uncompilable code would be generated.

### Version 1.1.0

- New Feature: Added support for Optimistic Concurrency using `ETag` for `CosmosDB` module.
- Improvement: Added `FindAsync` method to repository.

### Version 1.0.3

- Fixed: Fixed an issue around nullable collections not being realized correctly.

### Version 1.0.2

- Fixed: Fixed an issue where modeling `Value Object`s was not working.

### Version 1.0.0

- New Feature: CosmosDB module.