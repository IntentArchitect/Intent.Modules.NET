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
- **HTTP `QUERY` gets a generated `HttpQueryAttribute` (since 7.2.0)** — ASP.NET Core 10 ships
  `HttpMethods.Query` but no attribute, so the template mirrors `HttpPostAttribute` as an `HttpMethodAttribute`
  subclass, following the `BinaryContentAttribute` single-file pattern. The controller template already emits
  `[Http{Verb}]`, so `QUERY` needed no change there. The attribute is gated on any operation using `QUERY`
  (`HttpQueryHelper.NeedsHttpQueryInfrastructure`) so apps without one see no diff. It is not gated on .NET 10:
  routing accepts any method name, so on earlier targets the attribute uses `"QUERY"` and its comment drops
  the `HttpMethods.Query` cref (a cref to a missing member is a build warning).
  - **Retire it once ASP.NET Core ships its own.** Checked 2026-11-06: no `HttpQueryAttribute` in
    `Microsoft.AspNetCore.Mvc.Core` up to 10.0.14 or 11.0.0-rc.3 (MVC ships only in the shared framework, so
    there is no NuGet package to raise). Check the newest `Microsoft.AspNetCore.App.Ref` on nuget.org for
    `Microsoft.AspNetCore.Mvc.HttpQueryAttribute`. When it exists, stop generating ours for apps targeting that
    .NET version and up (`GetMaxNetAppVersion`) and let `[HttpQuery]` resolve to the framework's. Generating
    both makes `[HttpQuery]` ambiguous wherever `Microsoft.AspNetCore.Mvc` is imported. Rejected for now:
    `[AcceptVerbs("QUERY")]` — it needs no generated type, but breaks the controller template's uniform
    `[Http{Verb}]` emission for one verb.
- **`QUERY` is GET-like everywhere responses are decided** (`Utils` return switch, `CanReturnNotFound`, the XML
  and `ProducesResponseType` switches): a `QUERY` request is safe and idempotent, only with its criteria in the body.

## Invariants & Constraints
- **The `Body = Mode.Merge` on `OnException` needs RoslynWeaver 5.0.2 or later**, so the module depends on
  `Intent.OutputManager.RoslynWeaver` 5.0.2. Earlier versions merge the `switch` as one statement: a
  hand-added case makes the whole `switch` count as the developer's, and the generated `switch` is added
  again below it. That was reproduced on 5.0.1. 5.0.2 merges a `switch` case by case, so user cases,
  restored generated cases and module-contributed cases all end up in one `switch`. Don't lower this
  dependency while the filter is in Merge mode.
- **The `HttpQueryAttribute` template must add its own `System` usings.** Two test apps
  (FastEndpointsTest, Standard.AspNetCore.TestApplication) have no implicit usings, and that is where a
  missing `AddUsing` shows up; apps with implicit usings hide it.
- The exception `switch` inside `OnException` carries the builder metadata `exception-switch`. Other modules
  find it with `FindMethod("OnException").FindStatement(p => p.HasMetadata("exception-switch"))` and append
  cases. Renaming the method or dropping the metadata silently stops those modules contributing cases.

## Module Interactions
- **Intent.ModularMonolith.Host** — adds a `ValidationException` case to the exception switch via the
  `exception-switch` metadata (in an `OnBuild` at priority 1000, skipped if a `case ValidationException` already exists).
- **Intent.Bugsnag** — enriches the generated `ExceptionFilter` to report exceptions to Bugsnag.
- **Intent.Metadata.WebApi** — written against 4.8.0-pre.0, the first version with `HttpVerb.Query` and the
  `QUERY` option on the `Http Settings` verb. That is the floor for the HTTP `QUERY` support.
- **Intent.AspNetCore.Swashbuckle / Intent.AspNetCore.Scalar** — describe `QUERY` actions in the OpenAPI
  document on .NET 10 only. This module does not depend on either.
  - The `.imodspec` detect floor for Swashbuckle is 6.1.0-pre.0, its first version with a `QUERY` filter. Below
    it, one `QUERY` action makes the whole Swagger document return 500, at runtime. It is the only `QUERY`
    floor kept (Intent.Metadata.WebApi carries none, see its CONTEXT.md). It lives here because an app needs
    Controllers 7.2.0 before a `QUERY` action generates at all, and because FastEndpoints refuses Swashbuckle 6
    and later, so a floor in Intent.Metadata.WebApi would lift Swashbuckle in FastEndpoints apps too.
