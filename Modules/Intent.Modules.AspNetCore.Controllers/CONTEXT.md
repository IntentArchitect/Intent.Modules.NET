# Context: Intent.AspNetCore.Controllers

## Purpose
Generates ASP.NET Core MVC controllers from Services designer elements, plus the supporting filters
(`ExceptionFilter`, `BinaryContentFilter`) and JSON response helpers they rely on.

## Architectural Decisions
- **`ExceptionFilter.OnException` is generated with `Body = Mode.Merge`** (since 7.1.16) — users routinely
  need to map their own exception types to HTTP results, and the natural place is another `case` in the
  generated `switch`. Under the class's default `Mode.Fully` any hand-added case was removed on the next
  Software Factory run. Merge keeps user cases while the template (and the extensions below) still own
  and refresh theirs. Rejected: `Body = Mode.Ignore` — that would stop modules installed later (FluentValidation,
  NotFound, ModularMonolith) from ever adding their cases to an existing filter.

## Invariants & Constraints
- **The `Body = Mode.Merge` on `OnException` needs RoslynWeaver 5.0.2 or later**, so the module depends on
  `Intent.OutputManager.RoslynWeaver` 5.0.2. Earlier versions merge the `switch` as one statement: a
  hand-added case makes the whole `switch` count as the developer's, and the generated `switch` is added
  again below it. That was reproduced on 5.0.1. 5.0.2 merges a `switch` case by case, so user cases,
  restored generated cases and module-contributed cases all end up in one `switch`. Don't lower this
  dependency while the filter is in Merge mode.
- The exception `switch` inside `OnException` carries the builder metadata `exception-switch`. Other modules
  find it with `FindMethod("OnException").FindStatement(p => p.HasMetadata("exception-switch"))` and append
  cases. Renaming the method or dropping the metadata silently stops those modules contributing cases.

## Module Interactions
- **Intent.ModularMonolith.Host** — adds a `ValidationException` case to the exception switch via the
  `exception-switch` metadata (in an `OnBuild` at priority 1000, skipped if a `case ValidationException` already exists).
- **Intent.Bugsnag** — enriches the generated `ExceptionFilter` to report exceptions to Bugsnag.
