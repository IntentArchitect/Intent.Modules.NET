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

## Co-existence with AutoMapper

This module will not activate when `Intent.Application.Dtos.AutoMapper` is installed in the same application. The guard is enforced at the template level via `CanRunTemplate()`. To switch from AutoMapper to this module, uninstall `Intent.Application.Dtos.AutoMapper` first.

## Related Modules

### [Intent.Application.Dtos](https://github.com/IntentArchitect/Intent.Modules.NET)
Provides the `DTOModel` designer elements and the `DtoModelTemplate` that this module reads to discover which DTOs have domain mappings and to resolve DTO type names.

### [Intent.Application.Dtos.AutoMapper](https://github.com/IntentArchitect/Intent.Modules.NET)
The AutoMapper-based mapping provider this module replaces. The two modules are mutually exclusive — only one can be active in a given application.

### [Intent.Application.Dtos.Mapperly](https://github.com/IntentArchitect/Intent.Modules.NET)
An alternative mapping provider based on Mapperly source generation. Also mutually exclusive with this module.
