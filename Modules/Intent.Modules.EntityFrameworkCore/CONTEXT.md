# EntityFrameworkCore Module Context

> **Module:** `Intent.Modules.EntityFrameworkCore`
>
> **Purpose:** Durable architectural and implementation context for future work in this
> module. Read this before changing templates, factory extensions, or test applications
> related to EF Core.

---

## `HasDbTransaction` — External Transaction Co-existence

### What It Is

`HasDbTransaction()` is a method added by `UnitOfWorkExternalTransactionExtension` that lets
the MediatR `UnitOfWorkBehaviour` detect whether an external party (e.g. NServiceBus
`ITransactionalSession`, raw ADO.NET) has already enlisted an EF connection/transaction.
When it returns `true`, the behaviour skips wrapping in `TransactionScope` to avoid MSDTC
escalation.

### Where the Method Must Appear

The method must be present in **three places** for a project to compile:

1. **The unit-of-work interface** — either `IUnitOfWork` or `IApplicationDbContext` depending
   on which interface the project exposes. Both are patched:
   - `AddHasDbTransactionToInterface` → targets `Intent.Entities.Repositories.Api.UnitOfWorkInterface`
   - `AddHasDbTransactionToDbContextInterface` → targets `TemplateRoles.Application.Common.DbContextInterface`

2. **Every EF DbContext class** — both primary and secondary DbContexts must implement it.
   `AddHasDbTransactionToDbContext` targets **both** template roles:
   - `TemplateRoles.Infrastructure.Data.DbContext` (primary / `IsApplicationDbContext = true`)
   - `TemplateRoles.Infrastructure.Data.ConnectionStringDbContext` (secondary / multi-DbContext)
   The method is added unconditionally to every DbContext — no interface guard. This is
   intentional: the interface check was too narrow (a DbContext may implement `IApplicationDbContext`
   rather than `IUnitOfWork`), and an extra method on a class that doesn't need it is harmless.

3. **`UnitOfWorkBehaviour.Handle`** — the injected early-return guard. See below.

### Guard: Non-EF Unit-of-Work Backends

`ModifyUnitOfWorkBehaviour` must **not** inject the `HasDbTransaction` guard into projects
that don't use EF at all (e.g. Dapr, in-memory). Those projects have no `_dataSource` field
and the injected code won't compile.

Guard condition: skip injection if **no** template fulfills either
`TemplateRoles.Infrastructure.Data.DbContext` or
`TemplateRoles.Infrastructure.Data.ConnectionStringDbContext`.

### Guard: MediatR `next()` vs `next(cancellationToken)`

Older MediatR versions (used in .NET 6 projects) use `next()` with no argument. Newer
versions use `next(cancellationToken)`. The injected block must match the existing call style
already present in the Handle method — do not hardcode `next(cancellationToken)`.

Detection: inspect `handleMethod.Statements` for `"next(cancellationToken)"` and fall back
to `"next()"` if not found.

### Multi-DbContext Projects

In projects with more than one DbContext, only the primary one fulfills
`TemplateRoles.Infrastructure.Data.DbContext`. Secondary DbContexts only fulfill
`TemplateRoles.Infrastructure.Data.ConnectionStringDbContext`. Both roles must be targeted
when adding `HasDbTransaction()` to the implementation — use `FindTemplateInstances` (plural)
over both roles, not `FindTemplateInstance` (singular) over the primary role only.
