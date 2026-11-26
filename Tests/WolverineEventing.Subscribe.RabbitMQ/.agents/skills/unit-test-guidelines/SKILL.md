---
name: unit-test-guidelines
description: Use when writing or changing unit tests in a *.UnitTests project, including filling in the generated test stubs for command, query, event handlers, domain services and service operations.
template-id: Intent.UnitTesting.UnitTestInstructionTemplate
contentHash: D494B385D394673927077C7FD909E92734234B35C519C3A2EA1CAF53F8C547D8
---
# Unit Tests

Fill in the generated unit test classes. The classes and their placeholders come from the Software Factory; the tests are yours.

## Code management

- A generated test class is merge-managed: test methods you add are kept, and its constructor and placeholder test are yours to change or delete. Keep the class's `[IntentManaged]` attributes and `using Intent.RoslynWeaver.Attributes;`.
- Add new tests rather than rewriting tests that are already written.

## Writing tests

- Use xUnit `[Fact]` with `// Arrange`, `// Act` and `// Assert` comments. Name handler tests `Handle_{Behavior}_When{Condition}` and service tests `{MethodName}_{Behavior}_When{Condition}`; drop "When" if the condition is obvious.
- Mock every dependency of the class under test.
- Use the real entity class, never an anonymous object. The repository interface names it: `IOrderRepository : IEFRepository<Order, Guid>` means `Order`.
- Build a DTO the way it defines: through its static `Create(...)` factory if it has one, otherwise with property initializers.
- Set only the values the test depends on. When a test asserts a change, make the before and after values differ, so it fails if the assignment is removed.
- Write one representative test per scenario. Skip tests that only check parameters, `CancellationToken` passing, or mock calls the happy path already verifies.

## Mocking repositories and mappers

- In predicates for `FindAllAsync` and `FindAllProjectToAsync<TDto>`, use the concrete entity type: `It.IsAny<Expression<Func<Order, bool>>>()`, not `Func<object, bool>`.
- For a generic `FindAllAsync` with a predicate, compile and apply the predicate so the filter is exercised:

```csharp
  _repositoryMock
      .Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Expression<Func<Order, bool>> predicate, CancellationToken _) =>
          testData.Where(predicate.Compile()).ToList());
```

- Mock a domain-specific repository method such as `FindCompletedOrdersAsync` directly; it filters internally.
- With AutoMapper, mock the singular `Map<TDto>(entity)`. The `MapTo…List()` extension methods call it once per item, so a `Map<List<TDto>>` setup is never hit:
  `_mapperMock.Setup(x => x.Map<OrderDto>(It.IsAny<Order>())).Returns((Order o) => new OrderDto { Id = o.Id });`
- For a create, capture the added entity and assign its Id when `SaveChangesAsync` runs:

```csharp
  Order? captured = null;
  _repositoryMock.Setup(x => x.Add(It.IsAny<Order>())).Callback<Order>(o => captured = o);
  _repositoryMock.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
      .Callback(() => captured!.Id = expectedId)
      .ReturnsAsync(1);
```

## What to cover

- **Command handlers:** the happy path; `NotFoundException` when `FindByIdAsync` returns `null`, for updates and deletes; the updated properties; the returned id for creates.
- **Query handlers:** the happy path, checking mapped values rather than only non-null; empty results; data that doesn't match the filter; not found for get-by-id, as the handler handles it.
- **Domain event handlers** (`INotificationHandler<DomainEventNotification<TDomainEvent>>`, data on `notification.DomainEvent`): entities created or changed from the event, the repository's `Add` and `Remove` calls, service calls, and `Publish` or `Send` on the event bus. When a dependency throws, assert the exception propagates and later calls don't happen. When order matters, record it in a `callOrder` list from each mock's `Callback`.
- **Integration event handlers** (`IIntegrationEventHandler<TMessage>.HandleAsync`, properties on the message itself): service calls with the message's values, event bus calls captured with `.Callback<T>(e => captured = e)`, exception propagation, and empty or default message values.
- **Domain services:** assert the entity's state after the call, not only that the repository was called. Also cover not found, invariant violations, and every aggregate the service changes. A domain service doesn't save, so don't verify `SaveChangesAsync`.
- **Service operations:** the command and query coverage above, for each public method, grouped by a `{MethodName}_` prefix.
