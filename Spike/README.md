# Mediator Pattern Spike — Golden Sample Reference

Implementation-pattern spike for replacing the **MediatR** (and **Wolverine**) dispatch stacks in
Intent's Clean Architecture output with the source-generated
[Mediator](https://github.com/martinothamar/Mediator) library (`Mediator.Abstractions` /
`Mediator.SourceGenerator` **3.0.2**, MIT). This folder is the golden sample the modules will be
derived from. Building those modules is a separate exercise with its own test apps.

Design decisions and probe evidence: [DECISIONS.md](DECISIONS.md).

> [!WARNING]
> **Never run the Software Factory on `MediatorSpike/intent/MediatorSpike.isln`.**
> The app code is hand-owned and carries no Intent code-management markers. Any Software Factory
> run would overwrite or delete it. The Intent solution is kept only as a record of how the apps
> were scaffolded. Its MediatR modules are uninstalled.

## Layout

```
Spike/
├─ MediatorSpike/                  the two apps, hand-owned source on Mediator
│  ├─ intent/MediatorSpike.isln    scaffolding record only; do NOT run the Software Factory (see above)
│  ├─ CqrsSpike.Publisher/         Controllers host: CRUD, domain events, publishes + sends integration messages
│  └─ CqrsSpike.Subscriber/        FastEndpoints host: integration event/command handlers dispatch commands
├─ tests/
│  ├─ Shared/                      scenario tests + fixtures, compiled into the test project
│  └─ CqrsSpike.Tests/             runs the scenarios against MediatorSpike
├─ probes/                         minimal repros of each Mediator constraint / failure mode
├─ tools/errs.py                   build-log error summariser
└─ CqrsSpike.sln                   the 8 app projects + the test project
```

Each app also keeps its own `CqrsSpike.<App>.sln`.

### How the code got here

The apps were **scaffolded with Intent Architect, then hand-owned** (decision D16 in
[DECISIONS.md](DECISIONS.md)):

1. Both apps were modelled in the Domain and Services designers and generated on MediatR by the
   Software Factory. That was the frozen baseline.
2. The baseline was converted to Mediator, and the shared scenarios passed on both stacks.
3. The MediatR copy and the conversion tooling were deleted. The Mediator code replaced the
   generated code in place, all Intent markers (`[IntentManaged]`, `[assembly: IntentTemplate]`,
   `Intent.RoslynWeaver.Attributes`, …) were stripped, and the Software Factory was switched off
   for good.

Git history is the record of both versions. Commit `bcfc0d8aa6` holds the MediatR baseline
(`Spike/MediatorSpike/`) and the Mediator copy (`Spike/mediator/`) side by side.

### Load-bearing details — don't "tidy" these

- `options.PipelineBehaviors` stays a **literal array** inside `AddMediator`. A non-literal value
  silently drops every behaviour (P5).
- `DomainEventNotification<T>` stays **abstract**, with a sealed subclass per event. A
  non-abstract generic breaks the source generator (P6).
- `Mediator.SourceGenerator` keeps `PrivateAssets="all"` and stays in the Application project only (P2, D12).
- `ServiceLifetime.Scoped` stays (D8).

## Running it

Needs Docker (or any RabbitMQ on `localhost:5672` that accepts `guest/guest`) and the .NET 8 runtime.

```pwsh
# build + runtime scenarios — the fixture reuses a broker on :5672 or starts rabbitmq:3.13 via Testcontainers
dotnet build CqrsSpike.sln
dotnet test CqrsSpike.sln

# real two-process run (needs a broker on :5672)
docker run -d --name cqrsspike-rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
dotnet run --no-launch-profile --project MediatorSpike/CqrsSpike.Publisher/CqrsSpike.Publisher.Api --urls http://localhost:5101
dotnet run --no-launch-profile --project MediatorSpike/CqrsSpike.Subscriber/CqrsSpike.Subscriber.Api --urls http://localhost:5102
curl -X POST localhost:5101/api/orders -H "Content-Type: application/json" -d '{"customerName":"Tess","total":42.5}'
```

## Scenario matrix — runtime results (2026-10-09)

`tests/Shared/Scenarios.cs` runs against both apps over real RabbitMQ. 8/8 pass. Before the
collapse, the same scenarios also passed unchanged against the generated MediatR baseline
(2026-10-05), which proved behavioural parity.

| # | Scenario | Proves | Result |
|---|---|---|---|
| S1 | CRUD via controller → `ISender` → handler → EF | command/query dispatch, void + value returns, NotFound | ✅ |
| S2 | Invalid command → 400, nothing buffered or committed | ValidationBehaviour runs before the handler | ✅ |
| S3 | Admin-only delete: anonymous 401, wrong role 403, Admin 200 | AuthorizationBehaviour | ✅ |
| S4 | Entity ctor raises domain event → handler sends `CreateAuditEntryCommand` | domain event dispatch + nested command from a notification handler | ✅ |
| S5/S6 | Order → `OrderCreatedEvent` (publish) + `RequestShipmentCommand` (send) → Subscriber integration **event** and **command** handlers dispatch commands → `ShipmentCreatedEvent` → Publisher integration handler dispatches `MarkOrderShippedCommand` | command publishes/sends integration messages; integration handlers dispatch via `ISender` in both apps | ✅ |
| S7 | Recorded pipeline order for a create | bus **flush happens after UoW commit** (`publish → send → commit → flush:2`) | ✅ |
| S8 | Query passes flush (`flush:0`) but never commits | UoW constrained to commands only | ✅ |
| S9 | FastEndpoints command (validation 400 / 201) and queries | FastEndpoints host dispatch | ✅ |

**Two-process run:** both apps were started with `dotnet run` as separate processes against a
RabbitMQ container and driven with curl.
- **Cross-app flow:** the order flowed Publisher → Subscriber (record + shipment) → Publisher, ending at `status: Shipped` with the matching `shipmentId`.
- **Domain event:** the audit entry `Order placed for Two-Process Tess` was created by the domain-event handler's nested command.
- **Other calls:** the invalid order returned 400, PUT returned 204, and the anonymous DELETE returned 401.

Logs show Mediator's `LoggingBehaviour` (a `MessagePreProcessor`) logging every request in both
processes, including the commands dispatched from MassTransit consumers. In an earlier run,
`PerformanceBehaviour` flagged a request that had been held up while the broker was unavailable.

**Generator output, inspected:** `options.PipelineBehaviors` is closed **per message at compile
time**. Commands get seven behaviours, queries six:
`UnitOfWorkBehaviour<,> where TRequest : IBaseCommand` is never registered for a query.

## The MediatR → Mediator delta = the modules' job description

Measured on 2026-10-05 between the generated MediatR baseline and the Mediator version (both in
commit `bcfc0d8aa6`): **112 files identical, 71 changed, 2 added, 4 removed.** Per template:

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

   Integration-event handlers map field-to-field invocations correctly. The baseline used a
   hand-written body here (`OrderPlacedDomainEventHandler.Handle`). **The Mediator domain-event
   module should fix this rather than inherit it.**
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
- **Opt-in behaviours (D9):** the spike registers all seven to prove each shape. The module's setting decides which are emitted. Authorization is auto-on when any request is secured.
