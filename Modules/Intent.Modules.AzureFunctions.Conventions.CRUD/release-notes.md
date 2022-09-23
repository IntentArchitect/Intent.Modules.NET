### Version 3.3.7

- New: CRUD Creation scripts now automatically return the surrogate key type in the create operation if it is not a composite key. This will return the Entity's ID on creation. Explicit and Implicit keys are respected.
- New: Those surrogate key types will be automatically wrapped in a `application/json` wrapper object to aid clients consuming that service operation with parsing valid JSON.
