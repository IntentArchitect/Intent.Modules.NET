---
name: intent-domain-interactions-expert
description: "Use when generating handler bodies (Command/Query/Event/Processing/Service handlers) that translate modelled designer interactions (query entity, create entity, update entity, delete entity, call domain/entity service, publish integration event, send integration command, processing actions) into C# inside the handler's method. USE FOR: authoring an IInteractionStrategy, registering it via InteractionStrategyProvider, calling method.ImplementInteractions(model), wiring CSharpMapping resolvers in handler factory extensions, and reading/handling association-end target-end models. DO NOT USE FOR: pure entity↔DTO field mapping inside a single template (use intent-mapping-architect); event publication infrastructure (IMessageBus, MessageBusPublishBehaviour) — that belongs to intent-module-orchestrator."
argument-hint: "[handler template id or role] [interaction kind — query/create/update/delete/callDomainService/processing/publish/send]"
---

# Intent Domain Interactions Expert

## Purpose

A handler template (e.g. `CommandHandler`, `EventHandler`, `ProcessingHandler`) is *not* responsible for hardcoding what the developer modelled as interactions on its target operation. Instead, the handler's factory extension calls `method.ImplementInteractions(model)`, which dispatches each modelled interaction (query entity, create order, publish event, etc.) to a registered `IInteractionStrategy`. This skill is for authoring those strategies and wiring them correctly.

```
Designer model:                                          Generated code (handler body):
  Operation "PlaceOrder"                                 ┌─ var customer = _customers.FindByIdAsync(...);
    ├─ QueryEntity → Customer            ──ImplementInteractions──►
    ├─ CreateEntity → Order                                ├─ _orders.Add(order);
    ├─ Publish → OrderPlacedEvent                          └─ _bus.Publish(new OrderPlacedEvent(...));
```

## Musts

1. **Implement `IInteractionStrategy`** from `Intent.Modules.Common.CSharp.Interactions`. The contract is exactly two methods: `IsMatch(IElement interaction)` and `ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)`.
2. **Register the strategy in a factory extension's `OnBeforeTemplateRegistrations`** via `InteractionStrategyProvider.Instance.Register(new MyStrategy());`. Never register from inside a template — registration must happen before any handler template is instantiated.
3. **Make `IsMatch` cheap and side-effect-free.** It's evaluated against every interaction on every operation; do only the typed predicate check (`interaction.IsXxxTargetEndModel()`) plus any quick model-level guard (e.g. `action.Mappings.Any()`).
4. **In `ImplementInteraction`, navigate from `IElement` → typed model via `.AsXxxTargetEndModel()`.** The cast `IAssociationEnd` may be required before the typed accessor.
5. **Emit code through `method.AddStatement(...)` / `method.AddStatements(...)` with explicit `ExecutionPhases`** — `Initialise`, `BusinessLogic`, `IntegrationEvents`, `Return`. Strategy ordering within a method comes from the phase, not from registration order.
6. **Use `method.GetMappingManager()` + `AddMappingResolver(...)`** when the interaction includes a mapping (e.g. mapping fields from the incoming command to a new entity). The resolver decides how each member-pair becomes a C# expression.
7. **Use `csharpMapping.GenerateCreationStatement(mapping)` or `GenerateUpdateStatement(mapping)`** to emit creation/update statements driven by the model's `Mappings` collection. Don't hand-construct object initializers when a Mapping is present.
8. **Register the strategy's mapping resolvers up-front inside `ImplementInteraction`**, scoped to the current method's mapping manager. Resolvers added later (e.g. via priority callbacks) miss interactions that ran in the same pass.
9. **Read `template.AddTypeSource(<templateId>)` for every template that produces a type the strategy may reference** (events, DTOs, enums, commands). Without `AddTypeSource`, `GetTypeName` returns the bare class name without bringing in the using.

## Must Nots

1. **Never register a strategy from inside a template constructor.** Templates instantiate per-model; registration must be singleton-scoped and earlier in the lifecycle.
2. **Never hardcode the handler's method name or signature** inside the strategy. The handler is passed in as `ICSharpClassMethodDeclaration`; everything you need is on that object plus the interaction element.
3. **Never call `template.CSharpFile.AfterBuild` from inside a strategy.** Strategies run synchronously as part of the handler's `OnBuildOnce`; deferring to `AfterBuild` breaks the ordering with other strategies and with mapping resolvers.
4. **Never branch on stereotype string names inside `IsMatch`.** Use the typed `Is<X>TargetEndModel()` predicates that the modeler provides — they're the only stable contract.
5. **Never call `method.AddStatement(...)` without a phase** when more than one strategy might attach to the same handler. Phaseless statements collide with phased statements in unpredictable ways.
6. **Never modify the handler's class structure (add fields, change constructor) directly from a strategy.** Use `@class.InjectService(...)` extensions which are idempotent and respect the existing DI pattern. The strategy's job is the method body — class-level changes belong in the handler factory extension.

---

## The Three Roles in This Module Family

```
┌──────────────────────────────────┐
│  Handler Factory Extension       │  ◄── lives in the handler's module (e.g. MediatR)
│  - OnBeforeTemplateExecution     │      finds handler templates, attaches mapping resolvers,
│    iterates handlers,            │      calls method.ImplementInteractions(handler.Model)
│    sets up mapping resolvers     │
└──────────────┬───────────────────┘
               │
               ▼
┌──────────────────────────────────┐
│  ImplementInteractions ext.      │  ◄── from Intent.Modules.Common.CSharp.Interactions
│  - iterates interaction elements │      foreach interaction in operation.OwnedAssociations:
│  - finds IsMatch strategy        │        InteractionStrategyProvider.Instance
│  - calls ImplementInteraction    │          .FirstOrDefault(s => s.IsMatch(interaction))
└──────────────┬───────────────────┘          .ImplementInteraction(method, interaction)
               │
               ▼
┌──────────────────────────────────┐
│  IInteractionStrategy            │  ◄── what THIS skill helps you author
│  - IsMatch(IElement)             │
│  - ImplementInteraction(...)     │
│    └─ emits phased statements    │
│       into method body           │
└──────────────────────────────────┘
```

The split matters: **factory extension** sets the stage (finds handlers, wires mappings); **`ImplementInteractions`** dispatches; **strategy** emits. Mixing roles produces brittle code.

## Strategy Skeleton

```csharp
public class MyInteractionStrategy : IInteractionStrategy
{
    public bool IsMatch(IElement interaction)
    {
        if (!interaction.IsMyInteractionKindTargetEndModel())
            return false;

        var action = interaction.AsMyInteractionKindTargetEndModel();
        return action?.TypeReference?.Element != null
            && action.Mappings.Any();   // example: only handle if a mapping is modelled
    }

    public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
    {
        ArgumentNullException.ThrowIfNull(method);

        var interaction = (IAssociationEnd)interactionElement;
        var action = interaction.AsMyInteractionKindTargetEndModel();
        var handlerClass = method.Class;
        var template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;

        // 1. Discover / inject any services this strategy needs on the handler class
        //    (DI parameters; introduces readonly field automatically)
        @class.InjectService(template.GetTypeName(SomeServiceTemplateId), "someService");

        // 2. Register any mapping resolvers this strategy contributes
        var mapping = method.GetMappingManager();
        mapping.AddMappingResolver(new MyMappingResolver(template));

        // 3. Pull in type sources for the templates whose generated types this strategy uses
        template.AddTypeSource(SomeReferencedTemplate.TemplateId);

        // 4. Emit the body, with explicit execution phases
        var creationStatement = mapping.GenerateCreationStatement(action.Mappings.Single());
        method.AddStatement(ExecutionPhases.BusinessLogic, creationStatement);
    }
}
```

Register in your factory extension's `OnBeforeTemplateRegistrations`:

```csharp
protected override void OnBeforeTemplateRegistrations(IApplication application)
{
    InteractionStrategyProvider.Instance.Register(new MyInteractionStrategy());
}
```

## Built-in Strategies (read before authoring a new one)

| Strategy | Module | Match predicate | Purpose |
|---|---|---|---|
| `QueryInteractionStrategy` | `Application.DomainInteractions` | `IsQueryEntityActionTargetEndModel` | Load entity (single or list) into a local var |
| `CreateEntityInteractionStrategy` | `Application.DomainInteractions` | `IsCreateEntityActionTargetEndModel` | New-up entity, mapping fields, add to repository |
| `UpdateEntityInteractionStrategy` | `Application.DomainInteractions` | `IsUpdateEntityActionTargetEndModel` | Load + apply mapping + (no save — UoW handles it) |
| `DeleteEntityInteractionStrategy` | `Application.DomainInteractions` | `IsDeleteEntityActionTargetEndModel` | Load + repository.Remove |
| `CallDomainServiceInteractionStrategy` | `Application.DomainInteractions` | `IsCallDomainServiceActionTargetEndModel` | Inject domain service + invoke method |
| `CallEntityServiceInteractionStrategy` | `Application.DomainInteractions` | `IsCallEntityServiceActionTargetEndModel` | Call an instance method on a previously-queried entity |
| `ODataQueryInteractionStrategy` | `Application.DomainInteractions` | `IsODataQueryEntityActionTargetEndModel` | OData query expansion |
| `ProcessingActionInteractionStrategy` | `Application.DomainInteractions` | `IsProcessingActionTargetEndModel` | Generic processing action (developer-extensible) |
| `PublishIntegrationMessageInteractionStrategy` | `Eventing.Contracts` | `IsPublishIntegrationEventTargetEndModel` OR `IsSendIntegrationCommandTargetEndModel` | Map the source → new message, then `_bus.Publish(...)` or `_bus.Send(...)` |

**Before authoring a new strategy, read the closest existing one.** Most of the API surface is illustrated by `UpdateEntityInteractionStrategy` (typed model navigation, mapping manager, `ExecutionPhases.BusinessLogic`) and `PublishIntegrationMessageInteractionStrategy` (multi-kind `IsMatch`, NotImplementedException replacement pattern).

## Mapping Resolvers

A `IMappingTypeResolver` decides how a single source-target member pair becomes C# code. Strategies typically need one or more resolvers in their mapping manager:

| Resolver | Module | When to use |
|---|---|---|
| `ProcessingHandlerDomainMappingTypeResolver` | `Eventing.Contracts` | Map a message-handler's incoming message into a domain entity |
| `ProcessingHandlerDomainUpdateMappingTypeResolver` | `Eventing.Contracts` | Same as above for update flows (preserves identity) |
| `InvocationMappingTypeResolver` | `Eventing.Contracts` | Map command arguments into a method invocation |
| `MessageCreationMappingTypeResolver` | `Application.MediatR.CRUD.Eventing` | Build a new integration event/command from existing context |
| `TypeConvertingMappingResolver` | `Eventing.Contracts` | Insert type conversions (`Guid.Parse`, `int.Parse`) where the model declares them |

Register resolvers in the **factory extension** (not in the strategy) when they're cross-cutting for the whole handler. Register them **in the strategy** when they're specific to that single interaction.

## Handler Discovery in a Factory Extension

When wiring strategies into handlers, the factory extension iterates handler templates and finds the per-handler method:

```csharp
protected override void OnBeforeTemplateExecution(IApplication application)
{
    var templates = application
        .FindTemplateInstances<ITemplate>(TemplateRoles.Application.Eventing.EventHandler)
        .OfType<ICSharpFileBuilderTemplate>();

    foreach (var template in templates)
    {
        foreach (var handler in template.CSharpFile.GetProcessingHandlers())
        {
            var method = handler.Method;
            var mappingManager = method.GetMappingManager();

            // Cross-cutting resolvers for this handler family — order matters
            mappingManager.AddMappingResolver(new ProcessingHandlerDomainUpdateMappingTypeResolver(template));
            mappingManager.AddMappingResolver(new InvocationMappingTypeResolver(template));
            mappingManager.AddMappingResolver(new ProcessingHandlerDomainMappingTypeResolver(template));
            mappingManager.AddMappingResolver(new TypeConvertingMappingResolver(template));

            template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            template.AddTypeSource(TemplateRoles.Domain.ValueObject);

            // Set the "from" replacement so mappings know how to refer to the input
            mappingManager.SetFromReplacement(handler.Model, "message");

            // Dispatch
            method.ImplementInteractions(handler.Model);
        }
    }
}
```

`GetProcessingHandlers()` is provided by `Intent.Modules.Common.CSharp` and returns all handler-shaped methods in the file. `handler.Model` is the typed model behind the handler (e.g. `IntegrationEventHandlerModel`).

## Execution Phases

The `ExecutionPhases` enum (`Intent.Modules.Common.CSharp.Interactions`) gives strategies an ordering contract within a method:

| Phase | Typical contents |
|---|---|
| `Initialise` | Variable declarations, guards, early returns |
| `BusinessLogic` | Domain queries, entity mutations, service calls |
| `IntegrationEvents` | `_bus.Publish(...)`, `_bus.Send(...)` |
| `Return` | The terminal `return` expression |

Strategies must always pass a phase when emitting statements that may coexist with other strategies' statements. The default phaseless `method.AddStatement(...)` is reserved for templates that own the entire method body.

## Source of Truth

Public repository: <https://github.com/IntentArchitect/Intent.Modules>

Key files:
- `Modules/Intent.Modules.Common.CSharp/Interactions/IInteractionStrategy.cs`
- `Modules/Intent.Modules.Common.CSharp/Interactions/InteractionStrategyProvider.cs`
- `Modules/Intent.Modules.Common.CSharp/Interactions/ImplementInteractionsExtensions.cs`
- `Modules/Intent.Modules.Common.CSharp/Interactions/ExecutionPhases.cs`
- `Modules/Intent.Modules.Application.DomainInteractions/InteractionStrategies/*.cs` — the canonical built-in set
- `Modules/Intent.Modules.Eventing.Contracts/InteractionStrategies/PublishIntegrationMessageInteractionStrategy.cs` — eventing example
- `Modules/Intent.Modules.Eventing.Contracts/MappingTypeResolvers/*.cs` — the resolver examples
