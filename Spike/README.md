# Mediator Pattern Spike — Golden Sample Reference

Implementation-pattern spike for replacing the **MediatR** (and **Wolverine**) dispatch stacks in
Intent's Clean Architecture output with the source-generated
[Mediator](https://github.com/martinothamar/Mediator) library (`Mediator.Abstractions` /
`Mediator.SourceGenerator` **3.0.2**, MIT). Modules come next. This folder is the evidence they
are derived from.

Design decisions and probe evidence: [DECISIONS.md](DECISIONS.md).

## Layout

```
Spike/
├─ MediatorSpike/                  Intent solution: THE BASELINE — real Software Factory output
│  ├─ intent/MediatorSpike.isln    open this in Intent Architect (designers are the source of truth)
│  ├─ CqrsSpike.Publisher/         Controllers host: CRUD, domain events, publishes + sends integration messages
│  └─ CqrsSpike.Subscriber/        FastEndpoints host: integration event/command handlers dispatch commands
├─ mediator/                       the same apps on Mediator; produced by tools/convert.py, never hand-edited
├─ tests/
│  ├─ Shared/                      scenario tests + fixtures, compiled unchanged into both test projects
│  ├─ CqrsSpike.Baseline.Tests/    shared scenarios → MediatorSpike (MediatR)
│  └─ CqrsSpike.Mediator.Tests/    shared scenarios → mediator (Mediator)
├─ probes/                         minimal repros of each Mediator constraint / failure mode
├─ tools/
│  ├─ convert.py                   MediatR output → Mediator variant; one rule per decision (proto-templates)
│  ├─ delta.py                     per-file classification of the baseline → mediator difference
│  └─ errs.py                      build-log error summariser
├─ CqrsSpike.Baseline.sln
└─ CqrsSpike.Mediator.sln
```

There is **one** copy of each app's source of truth. The baseline is generated from the designers
in `MediatorSpike.isln`. The Mediator variant is derived from it by `tools/convert.py`, so after
any model change you run:

```pwsh
# 1. in Intent Architect: run the Software Factory for both apps (MediatorSpike.isln)
python tools/convert.py      # 2. rebuild the Mediator variant from the fresh output
python tools/delta.py        # 3. inspect what the modules must emit
```

### What is modelled vs hand-written in the baseline

Everything is modelled in the Domain and Services designers:
- entities, the domain event, commands and queries with their domain interactions (create, update, delete, query);
- HTTP routes, `[Secured(Roles=Admin)]` and validations;
- the integration message the app owns, plus subscribe, publish and send;
- the cross-app package references.

Three method bodies are hand-written, which the Software Factory allows:

| File | Why |
|---|---|
| `Domain/Entities/Order.cs` constructor, `Update`, `MarkShipped` | Domain method bodies are always user code |
| `Application/EventHandlers/Orders/OrderPlacedDomainEventHandler.cs` `Handle` (`Body = Mode.Ignore`) | The MediatR domain-event-handler template cannot build a command from an interpolated expression (see finding 1) |

A fresh Software Factory run proposes **zero changes** to the Publisher, so the bodies are stable.
The Subscriber's only proposal is a cosmetic qualification change in `ExceptionProcessor`
(`ForbiddenAccessException` ↔ `Application.Common.Exceptions.ForbiddenAccessException`). It
alternates on every run: a type-resolution quirk, not missing protection.

## Running it

Needs Docker (or any RabbitMQ on `localhost:5672` that accepts `guest/guest`) and the .NET 8 runtime.

```pwsh
# runtime scenarios — the fixture reuses a broker on :5672 or starts rabbitmq:3.13 via Testcontainers
dotnet test tests/CqrsSpike.Baseline.Tests
dotnet test tests/CqrsSpike.Mediator.Tests

# real two-process run (needs a broker on :5672)
docker run -d --name cqrsspike-rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
dotnet run --no-launch-profile --project mediator/CqrsSpike.Publisher/CqrsSpike.Publisher.Api --urls http://localhost:5101
dotnet run --no-launch-profile --project mediator/CqrsSpike.Subscriber/CqrsSpike.Subscriber.Api --urls http://localhost:5102
curl -X POST localhost:5101/api/orders -H "Content-Type: application/json" -d '{"customerName":"Tess","total":42.5}'
```

## Scenario matrix — runtime results (2026-10-05)

The same `tests/Shared/Scenarios.cs` runs against both stacks over real RabbitMQ.

| # | Scenario | Proves | MediatR (generated) | Mediator |
|---|---|---|---|---|
| S1 | CRUD via controller → `ISender` → handler → EF | command/query dispatch, void + value returns, NotFound | ✅ | ✅ |
| S2 | Invalid command → 400, nothing buffered or committed | ValidationBehaviour runs before the handler | ✅ | ✅ |
| S3 | Admin-only delete: anonymous 401, wrong role 403, Admin 200 | AuthorizationBehaviour | ✅ | ✅ |
| S4 | Entity ctor raises domain event → handler sends `CreateAuditEntryCommand` | domain event dispatch + nested command from a notification handler | ✅ | ✅ |
| S5/S6 | Order → `OrderCreatedEvent` (publish) + `RequestShipmentCommand` (send) → Subscriber integration **event** and **command** handlers dispatch commands → `ShipmentCreatedEvent` → Publisher integration handler dispatches `MarkOrderShippedCommand` | command publishes/sends integration messages; integration handlers dispatch via `ISender` in both apps | ✅ | ✅ |
| S7 | Recorded pipeline order for a create | bus **flush happens after UoW commit** (`publish → send → commit → flush:2`) | ✅ | ✅ |
| S8 | Query passes flush (`flush:0`) but never commits | UoW constrained to commands only | ✅ | ✅ |
| S9 | FastEndpoints command (validation 400 / 201) and queries | FastEndpoints host dispatch | ✅ | ✅ |

**Two-process run (Mediator):** both apps were started with `dotnet run` as separate processes against
a dedicated RabbitMQ container and driven with curl.
- **Cross-app flow:** the order flowed Publisher → Subscriber (record + shipment) → Publisher, ending at `status: Shipped` with the matching `shipmentId`.
- **Domain event:** the audit entry `Order placed for Two-Process Tess` was created by the domain-event handler's nested command.
- **Other calls:** the invalid order returned 400, PUT returned 204, and the anonymous DELETE returned 401.

Logs show Mediator's `LoggingBehaviour` (a `MessagePreProcessor`) logging every request in both
processes, including the commands dispatched from MassTransit consumers. `PerformanceBehaviour`
flagged a request that had been held up while the broker was unavailable.

**Generator output, inspected:** `options.PipelineBehaviors` is closed **per message at compile
time**. Commands get seven behaviours, queries six:
`UnitOfWorkBehaviour<,> where TRequest : IBaseCommand` is never registered for a query.

## The delta = the modules' job description

`python tools/delta.py` → **112 files identical, 71 changed, 2 added, 4 removed.** Per template:

| Artifact (template) | MediatR (generated) | Mediator | Change size |
|---|---|---|---|
| Command (`…CommandModels`) | `IRequest<T>, ICommand` (Intent marker) | `ICommand<T>` (Mediator) | base list + usings |
| Query (`…QueryModels`) | `IRequest<T>, IQuery` | `IQuery<T>` | base list + usings |
| Command handler | `IRequestHandler<C,T>` → `Task<T> Handle` | `ICommandHandler<C,T>` → `ValueTask<T> Handle` | signature only; **body unchanged** |
| Void command handler | `IRequestHandler<C>` → `Task Handle` | `ICommandHandler<C>` → `ValueTask<Unit> Handle` + `return Unit.Value;` | signature + **1 body line** |
| Query handler | `IRequestHandler<Q,T>` → `Task<T>` | `IQueryHandler<Q,T>` → `ValueTask<T>` | signature only |
| `ICommand` / `IQuery` marker templates | generated | **removed** (D5) | −2 files per app |
| Behaviours (×7) | `IPipelineBehavior` + `RequestHandlerDelegate<T>`, `next(ct)` | `IPipelineBehavior` + `MessageHandlerDelegate<TReq,T>`, `next(request, ct)`, `ValueTask` | a few lines each, bodies identical |
| UnitOfWork constraint | `where TRequest : notnull, ICommand` | `where TRequest : IBaseCommand` | 1 line |
| LoggingBehaviour | `IRequestPreProcessor<T>` (assembly-scanned) | `MessagePreProcessor<T,R>`, **listed first** in `PipelineBehaviors` | shape change |
| DI (`AddApplication`) | `AddMediatR(cfg => { RegisterServicesFromAssembly; AddOpenBehavior… })` | `AddMediator((MediatorOptions o) => { o.ServiceLifetime = Scoped; o.PipelineBehaviors = [ … ]; })` | block swap |
| Domain event notification | `DomainEventNotification<T>` (instantiable generic) | **abstract** generic base + **sealed per-event** notification + `DomainEventNotificationFactory` switch | +2 files (+1 per event) |
| Domain event handler | `INotificationHandler<DomainEventNotification<E>>` → `Task` | `INotificationHandler<ENotification>` → `ValueTask` | signature only; **body unchanged** (`notification.DomainEvent.X`) |
| DomainEventService | `Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType…)` | `DomainEventNotificationFactory.Create(e)` then `Publish` (no reflection) | rewritten |
| Controllers / FastEndpoints / integration handlers | `using MediatR;` + `ISender` | `using Mediator;` + `ISender` | **1 using line** |
| Validators, `IBypassPipelineValidation` | — | identical code | template id only |
| `.csproj` (Application) | `MediatR 12.5.0` | `Mediator.Abstractions` + `Mediator.SourceGenerator` (`PrivateAssets=all`) | packages |

Unchanged: Domain, EF Core, repositories, `IMessageBus`, MassTransit consumer and bus, security,
filters, Api hosting.

## Findings

1. **The MediatR domain-event-handler template can't map a command from an expression.** When a domain
   event handler's *Perform Invocation* maps a command field with an interpolated expression
   (`Order placed for {Order.CustomerName}`), the output depends on the mapping:
   - with the root mapping, it emits `var command = domainEvent;`, which doesn't compile;
   - without it, generation fails with "Could not resolve source path".

   Integration-event handlers map field-to-field invocations correctly. The baseline uses a
   hand-written body here. **The Mediator domain-event module should fix this rather than inherit it.**
2. **The current generated surface differs from the older `Tests/MassTransit.RabbitMQ` app** that the
   first, hand-built baseline was copied from:
   - the bus interface is `IMessageBus` (legacy `IEventBus` is a setting);
   - anonymous callers get **401**, not 403;
   - controllers return the bare GUID from create, not `JsonResponse<Guid>`;
   - DTO mapping uses `MapToXDto(_mapper)` extensions.

   Regenerating instead of hand-assembling caught all of these.
3. **The dispatch modules are near-identical.** Controllers, FastEndpoints and integration-handler
   call sites differ only by `using MediatR;` → `using Mediator;`.
4. **Modular Monolith breaks generator-in-Application (probe P12).** Two Application assemblies that
   each run the generator build clean. At runtime the first one's mediator wins `ISender`, and the
   second module's commands throw `MissingMessageHandlerException`. D12 holds for single-Application
   Clean Architecture only.
5. **Validations must be modelled.** The generated validators carry only modelled rules
   (`Validations` stereotype). Without them, S2 and S9 pass invalid input straight to the handler.
6. **Designer scripting trap:** calling `pkg.addReference(...)` reloads the designer's packages
   mid-script. Elements created later in the same script lose their type references. Add references
   in a script of their own.
7. **`ISender.Send(object)` exists** in Mediator 3.0.2, so `MassTransit.RequestResponse`'s `Send(object)`
   consumer has a direct equivalent. Not exercised at runtime here.
8. **Authorization exceptions are logged by UnhandledExceptionBehaviour in both stacks**, because
   Authorization sits inside it. This is parity, not a regression.

## Open questions for the module phase

- **Modular Monolith:** where does the generator live when one host has several Application modules (finding 4)?
- **Migration of existing apps:** custom void handlers need `return Unit.Value;` because the bodies are user-owned. A module migration or a documented compile-fix?
- **Role consumers of the dropped marker templates** (`Application.Command.Interface` / `Application.Query.Interface`), e.g. Dapr PubSub `IEvent : IRequest, ICommand`.
- **Seams to replicate in other modules:**
  - EF Core `UnitOfWorkExternalTransactionExtension` (patches the UoW behaviour by template id);
  - the outbox strip of the flush behaviour in MassTransit / NServiceBus / Eventing.Wolverine;
  - Blazor `ScopedMediator` (Mediator's `ISender` surface differs);
  - string-matching extensions (ODataQuery `IRequest<…>`, JsonPatch `"IRequest"` / `_mediator.Send`).
- **Opt-in behaviours (D9):** the variant registers all seven to prove each shape. The module's setting decides which are emitted. Authorization is auto-on when any request is secured.
