---
name: file-builder-expert
description: "Use when converting a standard C# class template or source file into an Intent Architect File Builder template, especially when asked to create a builder for this file, convert TransformText string output to CSharpFile fluent API, or generate matching template registration classes."
argument-hint: "[source file] [target template name] [single-file|file-per-model|custom]"
---

# File Builder Expert

## Musts
1. Inherit from `CSharpTemplateBase<TModel>` and implement `ICSharpFileBuilderTemplate`. Expose a `CSharpFile` property.
2. Construct `CSharpFile` in the constructor using `this.GetNamespace()` and `this.GetFolderPath()`. All structural fluent calls go here.
3. Implement `DefineFileConfig()` as `return CSharpFile.GetConfig();` and `TransformText()` as `return CSharpFile.ToString();` — nothing else.
4. Use specialized control flow builders (`AddIfStatement`, `AddForEachStatement`, `AddTryBlock`, etc.) for all logic. Use `CSharpInvocationStatement` for method calls. `CSharpMethodChainStatement` and `AddMethodChainStatement(...)` are `[Obsolete]` — never use either.
5. Register all `OnBuild` and `AfterBuild` callbacks during constructor setup. Always supply an explicit priority integer. Use the workspace band convention: **0=Core, 100=Enrichment, 500=Extension, 1000=Final**. `FileBuilderHelper.cs` is the authority on sort order: priority → template-type-name → template-id → model-id → creation order.
6. When a template must locate an element created by another template (`FindMethod`, `FindClass`, `FindStatement`), its callback **must** use a strictly higher priority number than the source template's callback.
7. Resolve model-driven types via the Type System APIs, never by guessing: `GetTypeName(...)` for model/type references, `GetTypeName(templateId, model)` for TemplateId-based references, and `UseType("Namespace.Type")` when a namespace should be introduced only because a specific concrete type is required. Do not use `UseType(...)` for types represented in the Intent model.
8. Generate members from model metadata when applicable: iterate `Model.Attributes` / `Model.Operations` and call `.AddProperty(...)` / `.AddMethod(...)` with resolved type names.
9. Advanced member discipline:
    - Properties: use `.Static()` for static members; use `.WithOptional(bool)` only when the target member API exposes it, otherwise model optionality must come from resolved type/nullability (`GetTypeName(...)`) and explicit property modifiers (for example `.Required()` when needed).
    - Constructors: DI parameters must call `param.IntroduceReadonlyField()` unless there is a deliberate, documented exception.
    - Async methods: when generated behavior is async (for example operations returning `Task`/`Task<T>`), call `.Async()` on the method builder.

## Must Nots
1. Never emit structural C# (classes, methods, namespaces) as raw strings in `TransformText` or anywhere outside the fluent API.
2. Never omit `ICSharpFileBuilderTemplate`. Never hardcode namespace or folder paths.
3. Never mismatch `TemplateId` between the template class and its registration class.
4. Never use `CSharpMethodChainStatement` or `AddMethodChainStatement(...)` — both are `[Obsolete]`.
5. Never add `else`, `else if`, `catch`, or `finally` as children of a block. They are sibling statements on the **parent method**.
6. Never use implicit priority (omitting the second argument, which defaults to 0) for reconciliation logic that depends on the existence of elements from other modules. Always supply an explicit integer.
7. Never hardcode type strings for types represented in the Intent model. Resolve them via `GetTypeName(...)` / `GetTypeName(templateId, model)` and only use `UseType(...)` for external fully qualified names.
8. Must not use raw string interpolation for Lambda arrows `=>` or Object Initializer braces `{}`. Use dedicated builder blocks (`CSharpLambdaBlock`, `CSharpObjectInitializerBlock`).

## Pattern Index

Read the relevant pattern file **before generating code** for that scenario:

| Scenario | File to read first |
|----------|--------------------|
| If/else, foreach, while, using, try/catch, assignments, invocation chains | `resources/patterns/control-flow.cs` |
| Generics, inheritance, attributes, XML docs, modifiers, nested types, metadata | `resources/patterns/advanced-types.cs` |
| OnBuild / AfterBuild priority, factory extensions, FindMethod, InsertAbove | `resources/patterns/lifecycle-hooks.cs` |
| Build errors, timing failures, metadata exceptions, registration mismatches | `resources/troubleshooting.md` |
| Quick API lookup (file setup, members, type declarations) | `resources/api-cheatsheet.md` |

## Using Directives

1. Use `AddUsing("Namespace")` for file-level namespace imports.
2. `AddUsing(...)` can be called during `CSharpFile` construction, later in the constructor, in helper methods, or in event/reconciliation logic when namespaces are discovered dynamically.
3. Prefer `UseType("Namespace.Type")` when the namespace should only appear if that exact concrete type is referenced. This applies to any non-model-resolved type, not only framework/external types.
4. Prefer `AddUsing(...)` when the namespace is needed independently of a specific type reference, or when it is discovered from dependencies, events, or collections.
5. `AddUsingBlock(...)` is unrelated to namespace imports. It creates a C# `using (...) { }` statement inside a method body.

### 🔑 Builder inference rule
The builder can only track type references that go through its type system (`UseType`, `GetTypeName`). When you emit a type name as a **raw string** (e.g. `AddAttribute("DefaultValue(0)")`), the builder sees opaque text and cannot infer the namespace. In that case you must add the namespace manually with `AddUsing(...)`. **The correct fix is always to reach for the typed builder API first** so the namespace is introduced as a side-effect of the type reference:

```csharp
// BAD — raw string, builder cannot infer System.ComponentModel
prop.AddAttribute($"DefaultValue({property.Value})");
// That requires manually calling AddUsing("System.ComponentModel") elsewhere.

// GOOD — typed builder call, UseType introduces System.ComponentModel automatically
prop.AddAttribute(UseType("System.ComponentModel.DefaultValueAttribute"), attribute =>
{
    attribute.AddArgument(property.Value);
});
```

Apply this rule to every emitted type reference — attributes, base types, parameter types, generic arguments. Reach for `UseType` / `GetTypeName` before reaching for `AddUsing`.

## Conditional AddUsing Patterns

```csharp
// Branch-based:
if (useTopLevelStatements)
{
    CSharpFile.AddUsing(this.GetNamespace());
}

// Dependency-driven:
foreach (var templateDependency in @event.TemplateDependencies)
{
    var template = GetTemplate<IClassProvider>(templateDependency);
    if (template != null)
    {
        AddUsing(template.Namespace);
    }
}

// Namespace collection:
foreach (var ns in @event.RequiredNamespaces)
{
    AddUsing(ns);
}

// Only introduce the namespace when this exact type is needed:
method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
```

## Minimal Template Shape

```csharp
[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class SampleTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "My.Module.SampleTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public SampleTemplate(IOutputTarget outputTarget, object model = null)
        : base(TemplateId, outputTarget, model)
    {
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
            .AddUsing("System")
            .AddClass("Sample", @class =>
            {
                @class.AddConstructor(ctor =>
                    ctor.AddParameter("string", "value", p => p.IntroduceReadonlyField()));
                @class.AddMethod("void", "DoWork", method =>
                    method.AddStatement("// TODO"));
            });
    }

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }
    [IntentManaged(Mode.Fully)] protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();
    [IntentManaged(Mode.Fully)] public override string TransformText() => CSharpFile.ToString();
}
```

## Registration Quick-Ref

| Template type | Registration base |
|---------------|-------------------|
| Single output file | `SingleFileTemplateRegistration` |
| One file per model | `FilePerModelTemplateRegistration<TModel>` — override `GetModels` |
| Event/pipeline driven | `ITemplateRegistration` |

`TemplateId` must be defined as `public const string` in the template and referenced by name from the registration.

## Source of Truth

> **AI:** Read from the `/resources/` folder (pattern files, cheatsheet, troubleshooting guide) for all logic and examples.  
> **Human reference:** The canonical API lives in the public repo at https://github.com/IntentArchitect/Intent.Modules — see:
> - `Modules/Intent.Modules.Common.CSharp/Builder/CSharpFile.cs`
> - `Modules/Intent.Modules.Common/FileBuilders/FileBuilderHelper.cs`
> - `Modules/Intent.Modules.Common.CSharp/Builder/IHasCSharpStatements.cs`
> - `Modules/Intent.Modules.Common.CSharp/Builder/` (all individual statement classes)
