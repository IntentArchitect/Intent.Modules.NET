# Intent.Application.Dtos.ObjectMapping

This module generates explicit C# object mapping extension methods for DTOs mapped from domain entities. It is a drop-in replacement for `Intent.Application.Dtos.AutoMapper` that produces readable, refactor-safe code with no runtime reflection or third-party mapping library dependency.

## What This Module Generates

- **`{DtoName}MappingExtensions.cs`** — one file per DTO with a `[map from]` domain mapping; contains a static class with two extension methods that fulfill the `Application.EntityDtoMappingExtensions` role.

## Mapping Extension Methods

For each DTO with a `[map from]` domain mapping in the Services designer, the module generates a static class with two extension methods on the source domain entity:

```csharp
public static class OrderDtoMappingExtensions
{
    public static OrderDto MapToOrderDto(this Order projectFrom)
    {
        return new OrderDto
        {
            Id = projectFrom.Id,
            CustomerId = projectFrom.CustomerId,
            OrderDate = projectFrom.OrderDate,
            Lines = projectFrom.Lines?.Select(x => x.MapToOrderLineDto()).ToList() ?? [],
            TagIds = projectFrom.Tags?.Select(x => x.Id).ToList() ?? []
        };
    }

    public static List<OrderDto> MapToOrderDtoList(this IEnumerable<Order> projectFrom)
        => projectFrom.Select(x => x.MapToOrderDto()).ToList();
}
```

### Flat and Nullable Properties

Flat scalar fields are assigned directly. Nullable navigation properties use `?.` to propagate nullability:

```csharp
// Flat scalar
RefNo = projectFrom.RefNo,

// Nullable navigation → nested DTO
ShippingAddress = projectFrom.ShippingAddress?.MapToAddressDto(),
```

### Nested DTO Composition

When a DTO field's type is itself a DTO with its own `[map from]` mapping, the generated code calls the nested type's own extension method recursively:

```csharp
// Single nested DTO
Customer = projectFrom.Customer.MapToCustomerDto(),

// Collection of nested DTOs
Lines = projectFrom.Lines?.Select(x => x.MapToOrderLineDto()).ToList() ?? [],
```

### FK Extraction

When a DTO field maps to an association end (a navigation property) but the field type is a primitive (typically `Guid`), the module extracts the FK instead:

```csharp
// Many-to-one: extract the local FK
CustomerId = projectFrom.CustomerId,

// One-to-many: extract IDs from related collection
TagIds = projectFrom.Tags?.Select(x => x.Id).ToList() ?? [],
```

### Enum Casts

When a DTO field maps to a domain property of a different enum type (e.g. `PaymentStatus` → `PaymentStatusDto`), an explicit cast is emitted:

```csharp
PaymentStatus = (PaymentStatusDto)projectFrom.PaymentStatus,
```

### Method Call Paths

If a mapping path includes an operation (parameterless method), the generated code calls it:

```csharp
DisplayName = projectFrom.GetDisplayName(),
```

## Null Path Handling

A mapping path can cross a **nullable hop** — an optional association or a nullable property — on its way to a value the DTO field declares as non-nullable. `Order.Coupon` is optional, but `OrderDto.CouponPercentOff` is a plain `int`. There is no answer that suits every application, so the choice is yours, made once per application under **Settings → Object Mapping → Null Path Handling**.

| Value | Emitted for a nullable hop into a non-nullable field | Runtime consequence when that hop is null |
| --- | --- | --- |
| **`Strict`** (default) | `projectFrom.Coupon!.PercentOff` | Throws `NullReferenceException`. The mapping fails loudly and returns no DTO at all — never a partially populated one. |
| **`Lenient`** | `projectFrom.Coupon?.PercentOff ?? default!` | Returns a DTO whose affected field holds the CLR default for its type (`0`, `Guid.Empty`, `null`, the enum's zero value); every other field is populated normally. |

`Strict` is the default and applies when the setting has never been set. The decision is made **per hop**, not per field:

- A **non-nullable** hop always emits a plain `.` — the setting does not touch it. `projectFrom.Customer.Address` keeps its `.` on `Customer` under both values.
- A nullable hop into a **nullable** DTO field emits `?.` under both values — there is nothing to disagree about; the null simply propagates.
- Only the nullable-hop-into-non-nullable-field combination differs between the two values.

The `Lenient` guard is always a single expression inside its initializer entry — no local variable, no helper method. `default!` is emitted uniformly, including on `int` and enum targets, so the generated file raises no new nullable-reference warnings under either value.

## Expression Mappings and the Prefix Form

A mapping can be authored as an expression rather than a path — for example `src.OrderNumber + " / " + src.Status`. The module normalises the prefix so the generated code always refers to its own parameter name, `projectFrom`:

- **`src.`** is **rewritten** to `projectFrom.` — every occurrence, not just the leading one, so a multi-reference expression such as `src.Amount > 0 ? src.Amount : 0` normalises correctly throughout.
- An expression already written in the **`projectFrom.`** form is left exactly as it is.
- Anything else has `projectFrom.` prepended to the expression **as a whole**. This matches `Intent.Application.Dtos.AutoMapper` byte for byte, including the cases where prepending to the whole expression is not what the author meant. There is no design-time diagnostic for it.

The two recognised prefix forms are interchangeable: `src.OrderNumber + " / " + src.Status` and `projectFrom.OrderNumber + " / " + projectFrom.Status` produce byte-identical generated code.

## Call Sites — what Domain Interactions generates

When `Intent.Application.DomainInteractions` (v1.2.10-pre.0 or later) is installed alongside this module, it recognises the module and generates query handler bodies that call these extension methods directly. No `IMapper` is injected into any handler.

| Query shape | Generated Call Site |
| --- | --- |
| Single entity | `return order.MapToOrderDto();` |
| Collection | `return orders.MapToOrderDtoList();` |
| Nullable single | `return order?.MapToOrderDto();` |
| Offset-paged | `return orders.MapToPagedResult(x => x.MapToOrderDto());` |

The nullable form is driven by the **Query Entity Action's** own multiplicity, not by the DTO's nullability. A query that may legitimately find nothing must have its Query Entity Action end modelled as `0..1`; left at `1`, Domain Interactions emits a `NotFoundException` guard and the handler throws instead of returning null.

Every Query Entity Action must also carry a **Query Entity Mapping** — for a single-entity lookup, the request's key mapped onto the entity's (`Id → Id`); for a get-all query, an empty mapping. A Query Entity Action with **no** mapping at all is silently not matched: the handler generates as a `NotImplementedException` stub with no repository injected, and no error is reported.

If a DTO has no domain mapping, no Mapping Extension Class is generated for it — and the Call Site strategy detects this and emits nothing, rather than a call to a class that does not exist.

### The `IQueryable` projection trade-off

This module's Call Site strategy reports `HasProjectTo() == false`. The mapping methods are ordinary C# operating on materialised objects; they are not expression trees and cannot be translated into SQL by a LINQ provider.

The practical consequence: setting the application's **Default Query Implementation** to `ProjectTo` is incompatible with this module, and Domain Interactions raises a clear error on the offending element rather than generating a query that fails at runtime. Use `Default`, or use `Intent.Application.Dtos.AutoMapper` if `ProjectTo` matters more to you than explicit mapping code. Under `Default`, entities are loaded and then mapped in memory — for a query returning a narrow projection of a wide aggregate, that reads more columns than a `ProjectTo` query would.

## Co-existence with other mapping providers

This module no longer stands down when `Intent.Application.Dtos.AutoMapper` is installed. Both can be installed in the same application and both will generate; no error or warning is raised about the other's presence. This makes an incremental migration possible, but while both are installed you are generating two mapping implementations over the same DTOs — remove the one you are migrating away from once the migration is done.

## Known limitations

- **Cursor paging is not verified.** The offset-paged Call Site is exercised end to end; the cursor-paged equivalent is not. Nothing in this repository fulfils the `Application.Common.CursorPagedList` template role except `Intent.Modules.Azure.TableStorage`, a persistence provider that would compete with EF Core for the repository roles, so a cursor-paged query could not be modelled to test against. Cursor-paged output may work, but it is untested.
- **Multi-PK collection fields.** A field that projects the primary keys of a to-many association (e.g. `LineIds`) was previously detected by a `GetTypeInfo(field.TypeReference).IsPrimitive` guard. That call resolves a collection field to `List<Guid>` and reports it non-primitive, so the field fell through to the bare navigation path and emitted code that does not compile. This module now uses a collection-aware check instead. **`Intent.Application.Dtos.AutoMapper` carries the identical unguarded form and very likely has the same defect** — it has not been fixed there, and this note is the only record of it.
- **Branches with no test coverage.** The following are implemented but not exercised by either verification application, so they rest on inspection rather than on a passing test: the casing-conversion branch of `PascalCasePropertyAccesses`; the prepend-the-whole-expression branch for an unrecognised prefix form; a nullable hop occurring *inside* a collection projection; and a nullable hop feeding an enum cast.

## Related Modules

### [Intent.Application.Dtos](https://github.com/IntentArchitect/Intent.Modules.NET)
Provides the `DTOModel` designer elements and the `DtoModelTemplate` that this module reads to discover which DTOs have domain mappings and to resolve DTO type names.

### [Intent.Application.Dtos.AutoMapper](https://github.com/IntentArchitect/Intent.Modules.NET)
The AutoMapper-based mapping provider this module replaces. The two modules are mutually exclusive — only one can be active in a given application.

### [Intent.Application.Dtos.Mapperly](https://github.com/IntentArchitect/Intent.Modules.NET)
An alternative mapping provider based on Mapperly source generation. Also mutually exclusive with this module.
