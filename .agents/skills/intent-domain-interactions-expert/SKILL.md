---
name: intent-domain-interactions-expert
description: Translate modelled designer interactions into C# handler bodies.
argument-hint: "[handler template id or role] [interaction kind]"
---

# Intent Domain Interactions Expert

> [!IMPORTANT]
> **Resource Read Constraint:** You are forbidden from reading the resource files under `/resources/` unless a `dotnet build` tool execution fails or an explicit type resolution error occurs.

## Musts
1. **Implement `IInteractionStrategy`:** Expose `IsMatch(IElement interaction)` and `ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)`.
2. **Early Registration:** Register strategies in factory extensions' `OnBeforeTemplateRegistrations` via `InteractionStrategyProvider.Instance.Register(new MyStrategy());`. Never register from inside template constructors.
3. **Cheap Match:** Keep `IsMatch` cheap and side-effect-free (e.g. check `interaction.IsXxxTargetEndModel()`).
4. **Phased Statements:** Emit statements via `method.AddStatement(...)` with explicit `ExecutionPhases` (`Initialise`, `BusinessLogic`, `IntegrationEvents`, `Return`).
5. **Mapping Resolution:** Use `method.GetMappingManager()` and add resolvers up-front inside `ImplementInteraction`. Use `csharpMapping.GenerateCreationStatement` / `GenerateUpdateStatement` for mapping-driven statements.
6. **Register Type Sources:** Call `template.AddTypeSource(...)` for templates producing types the strategy may reference.

## Must Nots
1. Never register a strategy from inside a template constructor.
2. Never hardcode the handler's method name or signature inside the strategy.
3. Never call `template.CSharpFile.AfterBuild` from inside a strategy.
4. Never branch on stereotype string names inside `IsMatch` (use typed predicates).
5. Never call `method.AddStatement(...)` without a phase when multiple strategies attach to the same handler.
6. Never modify the handler's class structure (e.g. constructor/fields) directly from a strategy; use `@class.InjectService(...)` instead.
