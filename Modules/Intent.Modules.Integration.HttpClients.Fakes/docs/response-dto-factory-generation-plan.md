# Response DTO Factory Generation Plan

## Goal

Generate small, editable sample-data factories for fake HTTP clients. The generated data is intentionally boring and structural, like Swagger placeholder data, so developers and AI tools can see the response shape and replace it with realistic samples later.

## Scope

Generate factories only for DTO types returned by fake/integration-double operations:

- response DTOs
- nested response DTOs
- DTO item types inside returned lists

Do not generate factories merely because a DTO appears as a request, command, query, or input parameter.

## Factory Surface

Each generated response DTO factory is named `{DtoType}Factory` and always has these public methods:

```csharp
public static class OrderResponseFactory
{
    public static OrderResponse Create() => new()
    {
        Id = "string",
        Items =
        [
            OrderItemDtoFactory.Create()
        ]
    };

    public static OrderResponse Create(Action<OrderResponse> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var dto = Create();
        configure(dto);
        return dto;
    }
}
```

Generate `CreateList(int count, Action<T, int>? configure = null)` only for DTO factories that are actually used as list-shaped examples by generated fake/sample code. Do not generate `CreateMany`, `AddItem`, `WithItems`, or similar helpers.

```csharp
public static List<OrderResponse> CreateList(
    int count,
    Action<OrderResponse, int>? configure = null)
    => FactoryHelpers.List(Create, count, configure);
```

`FactoryHelpers` is generated only when at least one factory emits `CreateList`.

## Collection Rules

DTO factory collection properties should use `CreateList(1)` when the item is a DTO:

```csharp
Items = OrderItemDtoFactory.CreateList(1)
```

Fake client methods returning a collection should create the returned collection at the endpoint boundary:

```csharp
public async Task<List<OrderResponse>> GetOrdersAsync(CancellationToken cancellationToken = default)
{
    return await Task.FromResult<List<OrderResponse>>(OrderResponseFactory.CreateList(1));
}
```

Primitive collections may contain one primitive placeholder. Cyclic DTO collections should use an empty collection expression to avoid recursive factories.

## Placeholder Defaults

Initial placeholder mapping:

| Type | Default value |
| --- | --- |
| `Guid` | `Guid.Empty` |
| `string` | `"string"` |
| `int` | `0` |
| `long` | `0L` |
| `decimal` | `0m` |
| `bool` | `false` |
| `DateTime` | `DateTime.UnixEpoch` |
| `DateTimeOffset` | `DateTimeOffset.UnixEpoch` |
| nested DTO | `{NestedDto}Factory.Create()` |
| DTO collection property | `{NestedDto}Factory.CreateList(1)` |
| DTO collection return | `{Dto}Factory.CreateList(1)` |

Enums and unsupported types should start with `default` / `default!` unless a better module-wide rule is added later.

## Fake Method Bodies

Fake methods should return simple placeholder values:

```csharp
public async Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
{
    return await Task.FromResult<CustomerDto>(CustomerDtoFactory.Create());
}

public async Task<List<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default)
{
    return await Task.FromResult<List<CustomerDto>>(CustomerDtoFactory.CreateList(1));
}

public async Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
{
    return await Task.FromResult<Guid>(Guid.Empty);
}

public async Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default)
{
    await Task.CompletedTask;
}
```

Generated fake methods preserve the async method shape. Use `await Task.FromResult(...)` for `Task<T>` methods and `await Task.CompletedTask` for non-generic `Task` methods.

## Paged Results

`PagedResult<T>` is a generated wrapper, not a DTO factory target. Fake return generation should still create a structural wrapper:

```csharp
return await Task.FromResult<PagedResult<CustomerDto>>(new PagedResult<CustomerDto>
{
    TotalCount = 1,
    PageCount = 1,
    PageSize = 1,
    PageNumber = 1,
    Data = CustomerDtoFactory.CreateList(1)
});
```

## Implementation Tasks

1. Generate response-only DTO factories beside fake HTTP clients.
2. Keep factory surface to `Create()` and `Create(Action<T> configure)` by default.
3. Recursively collect nested response DTOs from endpoint return types.
4. Mark DTO factories that need `CreateList` when a generated list-shaped example uses them.
5. Generate conditional `FactoryHelpers` only when at least one factory needs `CreateList`.
6. Replace fake method `NotImplementedException` bodies with placeholder returns.
7. Handle `PagedResult<T>` as a wrapper, not as a factory target.
8. Validate the module build and generated fixture output.

## Edge Cases

- Duplicate DTO names in the same fake slice should produce unambiguous factory class names or fail clearly.
- The same DTO can appear in more than one fake slice; factory generation is keyed by service proxy plus DTO so placement remains vertical-slice friendly.
- Cyclic DTO graphs must not recurse forever. Use an empty collection/default placeholder for direct cycles.
- DTOs without public setters or a usable parameterless constructor may not support the object-initializer factory shape. Fail clearly rather than hiding unusable output.
- Nullable properties currently receive non-null placeholders when doing so exposes useful structure.
- Collection properties may be `List<T>`, `IEnumerable<T>`, arrays, or other collection interfaces. Prefer `CreateList(1)` for DTO collections where assignable, and fall back clearly for unsupported collection shapes.
- Generic DTOs need special care for factory class naming, type parameters, and nested generic response graphs.
- Fake method bodies are generated so initial `NotImplementedException` bodies are replaced. If handwritten fake behavior needs to be preserved later, introduce an explicit extension/override mechanism rather than `Body.Ignore`, which also preserves unwanted placeholder throws.
