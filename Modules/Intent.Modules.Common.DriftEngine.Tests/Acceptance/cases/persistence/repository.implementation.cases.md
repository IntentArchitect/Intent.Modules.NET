# repository.implementation

A modelled Repository has an implementation: a class anywhere in scope that implements its interface, `X : IX`. `repository.implementation` is an existence kind (`presence`), so a repository with no implementation is contradicted, where Common.Drift treated it as a pending change. The kind arrives `later`, after phase 6, so these cases are its specification, not a phase gate.

## verified

`OrderRepository` in the infrastructure project implements `IOrderRepository` (ESC-DOM-72).

```json case
{ "phase": "later", "kinds": ["repository.implementation"], "traces": ["ESC-DOM-72"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "order-repository", "type": "Repository", "name": "OrderRepository", "typeRef": { "element": "order" } }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Domain/Repositories/IOrderRepository.cs
namespace Ordering.Domain.Repositories;

public interface IOrderRepository : IRepository<Order>
{
}
```

```csharp file src/Infrastructure/Repositories/OrderRepository.cs
using Ordering.Domain.Repositories;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
}
```

```json expect
{
  "claims": [
    { "kind": "repository.implementation", "element": "order-repository", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Infrastructure/Repositories/OrderRepository.cs:5" }
  ]
}
```

## contradicted-missing

No class implements `IOrderRepository`. The implementation is an existence claim, so it is contradicted: the code lacks it (R8), never a pending change.

```json case
{ "phase": "later", "base": "verified", "without": ["src/Infrastructure/Repositories/OrderRepository.cs"], "kinds": ["repository.implementation"], "traces": ["ESC-DOM-72"], "requirements": ["R1", "R8", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "repository.implementation", "element": "order-repository", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/repository.implementation", "severity": "error", "subject": "order-repository", "messageContains": "isn't in the code" }
  ]
}
```

## contradicted-not-implementing

A class named `OrderRepository` exists but doesn't implement `IOrderRepository`. A name alone isn't an implementation, so the claim is contradicted (ESC-DOM-72).

```json case
{ "phase": "later", "base": "verified", "kinds": ["repository.implementation"], "traces": ["ESC-DOM-72"], "requirements": ["R1", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Infrastructure/Repositories/OrderRepository.cs
using Ordering.Domain.Repositories;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository
{
}
```

```json expect
{
  "claims": [
    { "kind": "repository.implementation", "element": "order-repository", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/repository.implementation", "severity": "error", "subject": "order-repository" }
  ]
}
```
