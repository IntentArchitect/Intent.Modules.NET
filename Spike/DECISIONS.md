# Mediator (martinothamar/Mediator) — Pattern Spike: Decision Log

Goal: establish the implementation pattern for a Clean Architecture CQRS stack on the
source-generated **Mediator** library that completely replaces the MediatR and Wolverine
dispatch stacks. Modules come later; this spike is the golden sample they will be derived from.

> **Never run the Software Factory on `MediatorSpike/intent/MediatorSpike.isln`.** The app code is
> hand-owned and unmarked (D16); a run would overwrite or delete it.

Driver: licensing exit from MediatR (12.5.0 is the frozen Apache line; 13+ is RPL/commercial)
**and** a "no baggage" stack — clients want the minimum components needed.

Library: `Mediator.Abstractions` + `Mediator.SourceGenerator` **3.0.2** (latest stable, 2026-03-22).
Target framework: **net8.0** (Abstractions targets netstandard2.0 + net8.0).

## Decisions (grilling session 2026-10-03)

| # | Decision | Chosen | Rejected / why |
|---|---|---|---|
| D1 | Replacement model | **Replace wholesale** — one dispatcher per app (same as Wolverine) | Per-message coexistence: doubles behaviours/DI/dispatch logic |
| D2 | Baseline | *Superseded by D16 (2026-10-09).* **Intent-generated MediatR app**, frozen, then hand-converted; the diff is the module's job description | Hand-written baseline drifts from real generated output |
| D3 | Scenarios | CRUD commands/queries · command publishes integration event + sends integration command · integration handler dispatches a command · domain events (incl. a handler that sends a command) | — |
| D4 | Broker | **MassTransit** (richest MediatR interop today: flush-behaviour stripping, RequestResponse `Send(object)`) | Wolverine eventing — later |
| D5 | Message contracts | **Mediator-native CQRS**: `ICommand` / `ICommand<T>` / `IQuery<T>`, `ICommandHandler<,>` / `IQueryHandler<,>`. Intent's own `ICommand`/`IQuery` marker templates are **dropped** (they collide by name with Mediator's). UnitOfWork constrains on `IBaseCommand` | `IRequest<T>` + Intent markers (MediatR-like, keeps a permanent name collision) |
| D6 | Handler return types | **`ValueTask<T>`; void commands return `ValueTask<Unit>`** (`return Unit.Value;`) — idiomatic Mediator | Adapter base class keeping `Task` bodies: extra indirection, loses ValueTask benefit. Migration cost (custom void bodies need `return Unit.Value;`) to be measured in the spike |
| D7 | Domain events | **Per-event sealed wrapper**: abstract generic `DomainEventNotification<TDomainEvent>` base (generator skips abstract generics) + generated `sealed class XDomainEventNotification`; DomainEventService maps event → wrapper via a generated switch. Domain stays Mediator-free; handler bodies keep `notification.DomainEvent.X` | Generic wrapper (breaks the generator — PROBE 6); events implement `INotification` (Domain → Mediator dependency, handler bodies change); own dispatcher (we'd maintain a mini-mediator) |
| D8 | Lifetime | **`ServiceLifetime.Scoped`** (matches scoped DbContext/UoW; Mediator's own clean-arch sample). This makes pipeline behaviours Scoped too: one instance per message type per scope, unlike MediatR's transient behaviours, so a behaviour must keep no per-call state in fields | Singleton (captive scoped deps); Transient (allocations, no benefit) |
| D9 | Behaviour set | **Functional core** always generated when its dependents exist: UnitOfWork, MessageBusFlush, Validation. **Authorization auto-on whenever any request has `[Authorize]` modelled** (never silently off). Opt-in via setting: UnhandledException, Performance, Logging | Full parity (baggage); nothing-by-default |
| D10 | Behaviour registration | **Literal `options.PipelineBehaviors = [ ... ]`** inside `AddMediator`, priority-ordered (first = outermost). This is Mediator's own mechanism: the generator reads the array from source and closes each behaviour per message at compile time. Modules must NEVER emit a non-literal there: P5 builds with 0 warnings and every behaviour is dropped | Open-generic `services.AddScoped(typeof(IPipelineBehavior<,>), …)` — works (P10), but sidesteps the library's documented mechanism. AOT is not a reason either way: this stack (EF lazy-loading proxies, AutoMapper, FluentValidation assembly scanning) is not AOT-compatible |
| D11 | Dispatch hosts in spike | ASP.NET Core **Controllers** and **FastEndpoints** | Serverless (Functions/Lambda) — works in principle (no runtime codegen), proven later in module test apps |
| D12 | Generator placement | *Superseded by D17 (2026-10-09).* **Application project**, `PrivateAssets=all`; `AddMediator` called inside `AddApplication()` | Per-host: duplicated mediators, hosts become Mediator-aware |
| D13 | Behaviour location | **`Application/Common/Behaviours`**. The original reason (the literal array lives in Application) no longer holds after D17, but the behaviours stay here: they are application concerns, and Infrastructure references Application, so the array in Infrastructure can still name them | Infrastructure (Wolverine-style) — possible since D17, not taken |
| D14 | App topology | **Two apps, Pub/Sub**: Publisher (Controllers) — CRUD, domain events, publishes integration event + sends integration command; Subscriber (FastEndpoints) — integration handlers dispatch commands. Cross-process transport needed (MassTransit in-memory is per-process) | One self-subscribing app; one app per scenario |
| D15 | Transport | **RabbitMQ** (Docker). The test fixture reuses any broker on `localhost:5672` (guest/guest; official images allow guest from the host), otherwise starts `rabbitmq:3.13-management` via Testcontainers | Azure Service Bus emulator (heavier) |
| D16 | Ownership of the spike code (2026-10-09) | **Scaffold with Intent, then hand-own.** One set of apps in `MediatorSpike/`, on Mediator, with no MediatR in source or packages. The Software Factory is off **permanently** for `MediatorSpike.isln`; all Intent code-management markers are stripped and the MediatR modules uninstalled (plus `Intent.FastEndpoints` on the Subscriber; FastEndpoints itself is still used, as a plain NuGet package). One solution (`CqrsSpike.sln`) and one test project (`tests/CqrsSpike.Tests`). Git history (commit `bcfc0d8aa6`) is the record of the MediatR baseline and the before/after delta. Supersedes D2, A2, A6, A7 | Keeping a generated MediatR baseline beside a converted copy: two copies of every app, and a convert/delta toolchain that only made sense while the modules were the deliverable. Building modules that reproduce this code is a **separate, later exercise** with its own test apps |
| D17 | Mediator registration (2026-10-09) | **`Mediator.SourceGenerator` (`PrivateAssets=all`) and `AddMediator` move to the Infrastructure project**, inside `AddInfrastructure()`, with `options.Assemblies = [typeof(Application.DependencyInjection)]` so the generator scans the Application assembly for messages and handlers. Application keeps only `Mediator.Abstractions`. The literal `PipelineBehaviors` array (D10) moves with it. Supersedes D12 | Generator in Application (D12): Application then owns the composition of the pipeline, which is wiring, not application logic |
| D18 | Bus flush ownership (2026-10-09) | **Only the outermost owner in a DI scope flushes the scoped `IMessageBus`.** A scoped `MessageBusFlushOwner` counts nesting: `MessageBusPublishBehaviour` flushes only when it is outermost, and `IntegrationEventConsumer` claims ownership before dispatching, flushing after its transaction completes. Nested dispatches (a domain-event handler's command, a consumer's command) share the outer buffer and must not flush it before the outer commit. **Limit:** ownership is dispatch depth, not transaction ownership. Code that wraps dispatches in its own transaction must claim the owner itself, or a top-level dispatch flushes before that caller commits; only a transactional outbox removes the gap (open question for the module phase). Found by adversarial review; S7 asserts a single flush after the final commit, S7b that a dispatch defers to an outer owner | Flush on every dispatch (the MediatR baseline's behaviour): a nested dispatch sends the outer work's messages before the outer commit. Skip the flush while `Transaction.Current` is set: falsified in review, because it never flushes a top-level send made inside a caller's own transaction (messages lost), and nested dispatches on the explicit-EF-transaction path (no `TransactionScope`) still flush the outer buffer early |

### Setup choices made autonomously (user handed over after D15)

| # | Choice | Why |
|---|---|---|
| A1 | App names `CqrsSpike.Publisher` / `CqrsSpike.Subscriber` | Must not start with `Mediator` (root-namespace collisions hide missing `using`s) |
| A2 | *Superseded by D16 (2026-10-09).* Baseline **generated by the Software Factory** in `MediatorSpike/intent/MediatorSpike.isln` (opened through the standalone Intent Architect connection, with the user's go-ahead), fully modelled in the Domain and Services designers. The earlier hand-assembled baseline was deleted, so there is no duplicate output | A hand-built baseline drifted from current templates (see README finding 2) |
| A3 | Keep AutoMapper (architecture default, pre-commercial lock) in both stacks | The diff isolates the dispatcher. Mapper choice is orthogonal |
| A4 | Lean apps: Application Services, Swashbuckle, Serilog, Health Checks, Pagination, Value Objects, Unit Testing and Controllers versioning switched off at creation. MassTransit on RabbitMQ with outbox **None** (keeps the explicit bus-flush behaviour S7 measures). Enums serialised as strings | Not dispatcher-related; reduces diff noise |
| A5 | Mediator variant registers **all seven** behaviours | Proves every behaviour shape; the D9 opt-in setting is a module concern |
| A6 | *Superseded by D16 (2026-10-09).* Shared scenario tests compiled into two test projects (now one: `tests/CqrsSpike.Tests`) | Identical assertions prove behavioural parity |
| A7 | *Superseded by D16 (2026-10-09).* Mediator variant is **produced by `tools/convert.py`** from the generated baseline, never edited by hand | One source of truth; each conversion rule is a proto-template rule tagged with its decision id |
| A8 | Integration message ownership follows the designer rules: the publisher owns events, the receiver owns integration commands; the other side references the package | `OrderCreatedEvent` lives in the Publisher; `ShipmentCreatedEvent` and `RequestShipmentCommand` live in the Subscriber |

## Probe evidence (Mediator 3.0.2, throwaway projects)

| # | Probe | Result |
|---|---|---|
| P1 | Generator in Application, `AddMediator` in `AddApplication`, consumed by a host | ✅ |
| P2 | Generator referenced **without** `PrivateAssets=all` | ⚠️ host generates a second full `Mediator` + `AddMediator`; only CS0436 "type conflicts with imported type" **warnings**, no error |
| P3 | `ServiceLifetime.Scoped` with a scoped "DbContext", `ValidateScopes`/`ValidateOnBuild` on | ✅ handler + behaviour share the scope instance |
| P4 | Constrained open-generic behaviour (`where TMessage : <marker>`) via `options.PipelineBehaviors` | ✅ applied to matching messages only; array order = nesting order |
| P5 | `options.PipelineBehaviors = someStaticField` (non-literal) | ❌ **all behaviours silently dropped** — green build, nothing runs |
| P6 | Generic `DomainEventNotification<T> : INotification` (MediatR stack's shape) | ❌ generator emits non-compiling code (CS0246 `TDomainEvent`) |
| P7 | Domain events implementing `INotification`; `Publish((object)evt)` / `Publish(baseTypedEvt)` | ✅ polymorphic dispatch to runtime-type handlers (needs `Assemblies` to include Domain) |
| P8 | Abstract generic base + sealed per-event subclass | ✅ generator skips the abstract generic; Domain stays clean |
| P9 | Intent marker `ICommand` alongside `Mediator.ICommand` | ✅ only when fully qualified — superseded by D5 |
| P10 | Open-generic DI behaviour registration | ✅ constraints honoured by MS DI, order = registration order |
| P11 | Publish with no handler | ✅ no-op at runtime; MSG0005 warning at build |
| P12 | **Two** Application assemblies, each with the generator, in one host (Modular Monolith shape) | ❌ builds clean; first module's mediator wins `ISender` (`TryAdd`), second module's commands throw `MissingMessageHandlerException` at runtime. D12/D17 hold for single-Application Clean Architecture only. A generator in each module's Infrastructure would fail the same way. Not yet probed: one generator in a single composition project with `options.Assemblies = [ModA, ModB]`, the shape D17 makes natural and the likely fix |

Reproducible copies of P2, P5, P6, P7, P8 and P12 live in `probes/` (net8.0; `dotnet run --project Host`).
Runtime results of the full spike, the per-template delta, and further findings are in [README.md](README.md).

## Known integration seams the modules will need (from repo research)

- `EntityFrameworkCore/FactoryExtensions/UnitOfWorkExternalTransactionExtension.cs` patches MediatR's `UnitOfWorkBehaviour` and Wolverine's `UnitOfWorkMiddleware` by template id — needs a Mediator equivalent.
- MassTransit / NServiceBus / Eventing.Wolverine strip the bus-flush behaviour when a durable outbox is selected (match on `mediatr-config` metadata / statement text / `eventbus-flush` metadata).
- `MassTransit.RequestResponse` calls `ISender.Send(object)`.
- Blazor `ScopedMediator` implements MediatR's full `ISender` surface.
- String-matching extensions: ODataQuery (`IRequest<…>`), JsonPatch (interface containing "IRequest", `_mediator.Send`).
- Role-based consumers keep working if the new stack fills the same roles (`Application.Command`, `Application.Command.Handler`, `Application.Query`, `Application.Query.Handler`, …): Application.CQRS.CRUD, unit tests, DomainInteractions.
- Dropping the marker templates (D5) affects consumers of the `Application.Command.Interface` / `Application.Query.Interface` roles (e.g. Dapr PubSub `IEvent : IRequest, ICommand`).
- Test app names must not start with `Mediator` (enclosing-namespace lookup hides missing `using` bugs).
