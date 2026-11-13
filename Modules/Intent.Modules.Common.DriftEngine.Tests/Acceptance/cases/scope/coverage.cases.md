# scope/coverage

The run lists what is out of scope, so a person can see what a green run didn't look at (R15): per row, its element, glob, scope, reason and how many files it decides; the built-in exclusions with counts; and the files in no row, counted per top-level folder. It is one `scope/coverage` finding of severity Info, which the Software Factory writes to its log and never to the drift report: the report holds only drift (decision 30).

eShopCustom's Domain, Services and Front End modules each gave a not-yet-adopted summary: a count of the files under their Sources that no Adopted entry covered, reported as a warning or a note rather than one finding per file. The listing's "not in any row" count is that summary; files outside scope are still never reported one by one.

All three cases share one fixture: three Root Folder rows, a file each row decides, two files in no row, and a build output file.

## rows-with-reasons

The run is green, and the listing still names each `Out` row with its reason, so the exclusions stay visible.

```json case
{ "phase": 1, "cases": ["scope/coverage"], "requirements": ["R15"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
          { "Glob": "src/**/*.Tests/**", "Scope": "Out", "Reason": "Test projects" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Domain" }
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

```csharp file src/Shop.Tests/OrderTests.cs
namespace Shop.Tests;

public class OrderTests
{
    [Fact]
    public void New_order_has_an_id() => Assert.NotEqual(Guid.Empty, new Order { Id = Guid.NewGuid() }.Id);
}
```

```csharp file src/Legacy/OrderService.cs
namespace Shop.Legacy;

public class OrderService
{
}
```

```yaml file build/pipeline.yml
trigger:
  - main

steps:
  - script: dotnet test
```

```sql file tools/seed-orders.sql
INSERT INTO Orders (Id) VALUES ('6f1c2a1e-0d5b-4a7e-9c1f-2b3d4e5f6a7b');
```

```csharp file src/Domain/obj/Debug/net8.0/Shop.Domain.AssemblyInfo.cs
[assembly: System.Reflection.AssemblyCompanyAttribute("Shop.Domain")]
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "scope/coverage", "severity": "info", "messageContains": "Test projects" },
    { "rule": "scope/coverage", "severity": "info", "messageContains": "Being replaced by src/Domain" },
    { "rule": "scope/coverage", "severity": "info", "messageContains": "src/Legacy/**" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Shop.Tests/**", "src/Legacy/**"] }
}
```

## built-in-exclusions

The listing counts what the built-in exclusions skipped, here one file under an `obj` folder, so even code no row could bring in is accounted for.

```json case
{ "phase": 1, "base": "rows-with-reasons", "cases": ["scope/coverage", "scope/builtin"], "requirements": ["R15", "R22"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "findings": [
    { "rule": "scope/coverage", "severity": "info", "messageContains": "obj" }
  ],
  "filesRead": { "exclude": ["**/obj/**"] }
}
```

## files-in-no-row

`build/pipeline.yml` and `tools/seed-orders.sql` are in no row, so they are out. They are counted in the listing as "not in any row", per top-level folder, and nothing else is reported about them: no finding per file, and nothing is read. This is the not-yet-adopted summary of eShopCustom's Domain, Services and Front End modules. The Front End module also downgraded undescribed code outside its Adopted entries to notes; here such code is simply out of scope, counted and never reported.

```json case
{ "phase": 1, "base": "rows-with-reasons", "cases": ["scope/coverage"], "traces": ["ESC-DOM-04", "ESC-SVC-05", "ESC-FE-05"], "requirements": ["R15", "R22"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "findings": [
    { "rule": "scope/coverage", "severity": "info", "messageContains": "not in any row" },
    { "rule": "scope/coverage", "count": 1, "note": "One listing, not a finding per file" }
  ],
  "filesRead": { "exclude": ["build/**", "tools/**"] }
}
```
