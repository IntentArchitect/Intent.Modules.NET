---
name: domain-entity
description: guide coding agents to implement missing c# domain behaviour on a single domain entity or aggregate that lives in a dependency-free clean architecture domain project and may be persisted by a technology like ef core. use when a user shares a c# domain class with missing methods, not implemented exceptions, incomplete constructors, weak invariants, or unclear placement of business logic, and they want help finishing the domain behaviour while keeping persistence concerns secondary.
template-id: Intent.Entities.DomainEntitySkill
contentHash: 4E43B9360342E4B735EA73C59BB19DAAC30B7AB54A79176B26706F293AEE8E91
---
# Domain Entity

Implement or improve business behaviour on a single C# domain class.

Treat the class as part of a clean architecture domain model first and an persistence type second. Preserve domain intent, protect invariants, and avoid pushing business rules into application services unless the rule genuinely spans multiple aggregates or external systems.

## Default workflow

1. Read the entity or aggregate carefully.
2. Identify missing behaviour, especially methods throwing `NotImplementedException`, empty methods, placeholder guards, or constructors that do not fully establish a valid object.
3. Infer the domain intent from names, fields, properties, comments, existing guards, and related methods.
4. Implement the smallest coherent set of domain rules needed to make the type useful and internally consistent.
5. Explain assumptions briefly when the domain intent is uncertain.

## What to optimise for

- Keep business rules on the entity, aggregate root, or value object when they protect that type's own invariants.
- Prefer explicit domain operations over exposing setters.
- Make invalid states hard to create.
- Keep code usable in a domain-only project with no infrastructure dependencies.
- Align with the style already present in the user's codebase unless it is clearly harmful.

## Constructor guidance

When constructors are incomplete or missing, use these rules:

- Ensure a public constructor or factory establishes a valid domain object.
- Validate mandatory arguments.
- Normalise values only when the model already suggests it or when it is clearly required.

## Operation guidance

When implementing methods on the class:

- Enforce invariants at the point of state change.
- Check whether the method should be idempotent.
- Update all related fields together so the object cannot drift into a partially valid state.
- Prefer meaningful domain exceptions only if the codebase already uses them. Otherwise use standard argument or invalid operation exceptions.
- Return values only when the domain operation naturally produces one.
- Avoid side effects that belong to infrastructure.

## How to infer missing behaviour

When a method body is missing, infer intent from:

- method name and parameters
- property names and existing state
- nearby guard clauses
- comments, XML docs, or tests if provided
- common domain patterns already used in the class

If the intent is still ambiguous, choose the safest low-surprise implementation and state the assumption clearly.

## Implementation rules

- Preserve existing naming and coding style where reasonable.
- Do not introduce unnecessary abstractions.
- Do not add MediatR, repositories, domain services, or events unless the user already uses them or asks for them.
- Do not rewrite the entire model into a more opinionated DDD style unless the user requests that.
- Do not convert behaviour into extension methods.
- Do not expose mutable internals just to make the implementation easier.

## Review checklist

- Kept the business behaviour on the entity, aggregate root, or value object where it belongs.
- Implemented the missing methods, guards, or constructors that were clearly incomplete.
- Preserved or strengthened invariants so invalid state is harder to create.
- Validated mandatory inputs and rejected impossible state transitions.
- Updated related fields together so the object stays internally consistent.
- Avoided introducing infrastructure concerns into the domain model.
- Avoided adding unnecessary abstractions, patterns, or dependencies.
- Preserved the existing naming and coding style unless it was clearly harmful.
- Chosen standard exceptions unless the codebase already uses domain-specific ones.
- Stated any important assumptions briefly where domain intent was ambiguous.
- Returned only the code and explanation needed to complete the domain behaviour cleanly.

## EF Related Data Loading guidance

- NEVER use `Include` or `ThenInclude` in the Application Layer, these are only available in the Infrastructure layer.
- Lazy loading with proxies is enabled. 
- Entities are configured using the `Owns` apis, so compsitional children will be automatically loaded with their parents.
- You can rely on navigation properties being automatically loaded when accessed.
- (CRITICAL) If your implementation will cause a lot of Lazy loading consider other alternatives, like moving the data loading into the repository layer.

## Unit of Work guidance

- SaveChanges rule (STRICT): Do not call UnitOfWork.SaveChangesAsync(...) / SaveChangesAsync(...) in a handler/service method unless the operation returns a payload that requires DB-generated values, such as a generated Id, surrogate key, RowVersion/concurrency token, DB-generated timestamp, or computed column.
- If the operation returns Unit, void, Task, or IRequest with no result: do not call SaveChangesAsync.
- If the operation returns an identifier or DTO that needs generated fields: call SaveChangesAsync before returning.
- If unsure, omit SaveChangesAsync and assume an outer unit-of-work/pipeline commit.
- When reviewing code, remove SaveChangesAsync unless there is a clear generated-value or immediate-commit requirement.

## Entity Framework repository guidance

- Repository update rule (STRICT): Do not call repository.Update(...) / repo.Update(...) when using EF repositories.
- EF tracks loaded entities automatically. Modify the entity properties directly and let the Unit of Work persist the tracked changes.
- Only call Add/Create/Delete operations when inserting or removing entities.
- When reviewing code, remove unnecessary Update calls for entities loaded from an EF repository.

## Mapperly guidance

- Any read/query method, including MediatR query handlers and application services, that returns Application-layer DTOs (`*Dto`) derived from Domain entities **MUST** use Mapperly.
    - Do not manually construct DTOs (`new XxxDto { ... }`) on read/query paths..
- **Mapperly gate (absolute):** If a handler/service returns entity-shaped DTOs or uses any mapper call, you **MUST**:
    - verify a Mapperly mapper exists by locating a `[Mapper]` partial mapper class with the required mapping method, e.g. `CustomerToCustomerDto(Customer customer)`, **and cite file path + excerpt**, **OR**
    - if verification fails, **immediately create** the required Mapperly mapper(s), including all required nested mappers.
    - verify collection mappings when returning lists, e.g. `CustomerToCustomerDtoList(IEnumerable<Customer> customers)`.
    - verify nested mapper dependencies use `[UseMapper]` and constructor injection where needed.
- **Registration gate:**
    - If a mapper is injected into a handler/service, verify it is registered in Application DI.
    - Follow the existing registration style. Mapperly sample projects register mappers as singletons, e.g. `services.AddSingleton<CustomerDtoMapper>();`.
    - If registration is missing, add the minimal mapper registration, including nested mapper registrations.
- Manual DTO construction is allowed only when the DTO is a non-entity-shaped view model/aggregation and Mapperly is not reasonable.
    - This must include an inline code comment explaining why Mapperly is not reasonable.
    - “Mapping doesn’t exist yet” is not a valid exception.
- If you can't find any existing mappings, create them in the same project as the services under:
    - `./Mappings/<FeatureOrAggregate>/<Entity>DtoMapper.cs`
    - Example: `MyApp.Application/Mappings/Invoices/InvoiceDtoMapper.cs`        

**Example:**
```csharp
    [Mapper]
    public partial class OrderDtoMapper
    {
        [UseMapper]
        private readonly OrderLineDtoMapper _orderLineDtoMapper;

        public OrderDtoMapper(OrderLineDtoMapper orderLineDtoMapper)
        {
            _orderLineDtoMapper = orderLineDtoMapper;
        }

        [MapProperty(nameof(Order.Lines), nameof(OrderDto.OrderLines))]
        [MapPropertyFromSource(nameof(OrderDto.IsActive), Use = nameof(MapIsActive))]
        public partial OrderDto OrderToOrderDto(Order order);

        public partial List<OrderDto> OrderToOrderDtoList(IEnumerable<Order> orders);

        private bool MapIsActive(Order source) => source.IsActive();
}
```
