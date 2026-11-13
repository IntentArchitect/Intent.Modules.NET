# scope/stale

A `Drift Scope` row that decides no file and excludes no type is stale (R24): a warning on the element that holds the row, naming it. It usually means a folder was renamed or removed and the row wasn't, so the scope no longer says what the team thinks it says. "Decides" is the word that matters: a row that matches files but never decides one, because a later row of its element or a row of a deeper element decides them all, is stale too (R27).

eShopCustom's modules each warned about their own adoption scope: a Source that holds no files (`scope.source-empty`) and an Adopted entry that matches nothing (`scope.stale`). With one scope on the Codebase Structure designer, Sources and Adopted entries are both `In` rows, and both warnings become this one finding.

## in-row-decides-no-file

Row 2 names `src/Ordering/**`, where the domain lived before it moved to `src/Domain`. No file is under it, so it decides nothing and is stale. Nothing else changes: `Order` is still found through row 1, and a stale row is a warning, never a reason to skip a claim.

The Domain, Services, Front End and Architecture modules each had this check for their own Sources and Adopted entries, and for their own file kinds (`.cs`, `.razor`). The engine has one scope for every designer, and a row is stale whatever kind of file it fails to decide.

```json case
{ "phase": 1, "cases": ["scope/stale"], "traces": ["ESC-DOM-02", "ESC-SVC-02", "ESC-SVC-03", "ESC-FE-02", "ESC-FE-03", "ESC-ARC-06"], "requirements": ["R20", "R24"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/Domain/**", "Scope": "In" },
          { "Glob": "src/Ordering/**", "Scope": "In" }
        ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "scope/stale", "severity": "warning", "subject": "root", "messageContains": "src/Ordering/**" },
    { "rule": "scope/stale", "messageContains": "src/Domain/**", "count": 0, "note": "Row 1 decides Order.cs" }
  ]
}
```

## out-row-decides-no-file

An `Out` row is stale on the same terms. Row 2 excludes `src/Legacy/**`, with a reason, but the legacy folder has since been deleted. Left in place, the coverage listing would go on reporting a deliberate exclusion of code that no longer exists.

```json case
{ "phase": 1, "base": "in-row-decides-no-file", "cases": ["scope/stale"], "requirements": ["R24"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Domain" }
        ] } } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "scope/stale", "severity": "warning", "subject": "root", "messageContains": "src/Legacy/**" }
  ]
}
```

## overridden-by-a-later-row

Row 1 matches `src/Domain/Order.cs`, but so does row 2, and within one element the last matching row decides. Row 1 matches a file yet decides none, so it is stale; row 2 isn't. Deleting row 1 changes nothing about what is read, which is exactly what the warning tells the team.

```json case
{ "phase": 1, "base": "in-row-decides-no-file", "cases": ["scope/stale", "scope/nested"], "requirements": ["R24", "R27"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/Domain/**", "Scope": "In" },
          { "Glob": "src/**", "Scope": "In" }
        ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "scope/stale", "severity": "warning", "subject": "root", "messageContains": "src/Domain/**" },
    { "rule": "scope/stale", "count": 1, "note": "Only row 1; row 2 decides Order.cs" }
  ]
}
```

## overridden-by-a-deeper-element

The Root Folder excludes `src/Api/Generated/**`, and so does the `Api` project (a `C# Project (.NET)` in the `src` folder, so its folder is `src/Api`), with its own row `Generated/**`. A file's scope is decided by the deepest element with a matching row, so the project's row decides `OrdersClient.cs` and the Root Folder's row 2 decides nothing. The warning is on the Root Folder, the element that holds the stale row; the project's row is not stale.

```json case
{ "phase": 1, "base": "in-row-decides-no-file", "cases": ["scope/stale", "scope/nested"], "requirements": ["R20", "R24", "R27"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Api/Generated/**", "Scope": "Out", "Reason": "NSwag client" }
        ] } },
        "elements": [
          { "id": "src", "type": "Folder", "name": "src",
            "children": [
              { "id": "api", "type": "C# Project (.NET)", "name": "Api",
                "stereotypes": { "Drift Scope": { "Entries": [
                  { "Glob": "Generated/**", "Scope": "Out", "Reason": "NSwag client, regenerated from the spec" }
                ] } } }
            ] }
        ] }]
    }
  }
}
```

```csharp file src/Api/Generated/OrdersClient.cs
// <auto-generated>
//     Generated using the NSwag toolchain.
// </auto-generated>
namespace Shop.Api.Generated;

public partial class OrdersClient
{
}
```

```json expect
{
  "findings": [
    { "rule": "scope/stale", "severity": "warning", "subject": "root", "messageContains": "src/Api/Generated/**" },
    { "rule": "scope/stale", "subject": "api", "count": 0 }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Api/Generated/**"] }
}
```

## types-row-excludes-no-type

Row 2 excludes the type `OrderTestData` from `src/Domain`, but the builder has since moved to the test project. A `Types` row never decides a file, so all it can do is exclude a type; excluding none, it is stale. eShopCustom's Adopted entries could name a type (or, in the Front End, a component) and were stale when it was gone; this is that case.

```json case
{ "phase": 1, "base": "in-row-decides-no-file", "cases": ["scope/stale", "scope/type"], "traces": ["ESC-DOM-02", "ESC-FE-03"], "requirements": ["R24", "R26"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Domain/**", "Types": ["OrderTestData"], "Scope": "Out", "Reason": "Test data builders kept beside the entities they build" }
        ] } } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "scope/stale", "severity": "warning", "subject": "root", "messageContains": "OrderTestData" }
  ]
}
```

## bare-folder-name

Row 2 was meant to bring the web project in, but a bare folder name matches the folder, not its contents (the File Classification glob rules, R20): `src/Web/**` would. The row decides no file, so it is stale, and `Program.cs` is not read.

```json case
{ "phase": 1, "base": "in-row-decides-no-file", "cases": ["scope/stale", "scope/notRead"], "requirements": ["R20", "R22", "R24"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/Domain/**", "Scope": "In" },
          { "Glob": "src/Web", "Scope": "In" }
        ] } } }]
    }
  }
}
```

```csharp file src/Web/Program.cs
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run();
```

```json expect
{
  "findings": [
    { "rule": "scope/stale", "severity": "warning", "subject": "root", "messageContains": "src/Web" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Web/**"] }
}
```
