---
name: file-builder-expert
description: "Use when converting a standard C# class template or source file into an Intent Architect File Builder template, especially when asked to create a builder for this file, convert TransformText string output to CSharpFile fluent API, or generate matching template registration classes. Also covers authoring StaticContentTemplateRegistration classes that ship static files (CSS / JS / Razor / images) from a module's content folder."
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
7. Resolve emitted type positions through the Type System APIs: `GetTypeName(...)` for model/type references, `GetTypeName(templateId, model)` for TemplateId-based references, and `UseType("Namespace.Type")` for framework/external types, including method return/parameter types. Do not use `UseType(...)` for types represented in the Intent model.
8. Generate members from model metadata when applicable: iterate `Model.Attributes` / `Model.Operations` and call `.AddProperty(...)` / `.AddMethod(...)` with resolved type names.
9. For generated type declarations, put the final class/interface name on the template model/provider (for example `IHasName.Name`) and use the same model when resolving references via `GetTypeName(templateId, model)`; handle name collisions before `AddClass(...)`.
10. Advanced member discipline:
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
9. Do not override `TemplateMetadata` or `Migrations` for brand-new templates. Only add them when there is a real `ITemplateMigration`.
10. When writing new code or changing a specific call site, do not introduce `[Obsolete]` API usage. Leave unrelated existing obsolete code alone.

## Comments

Default to **no comments**. A template's name, `TemplateId`, and fluent calls are self-documenting. Add a comment only when the **why** is non-obvious to a reader who knows the SDK:

- A workaround for a specific SDK or merger bug (reference the bug/issue if possible)
- A constraint that isn't visible from the code — e.g. "must run after X template because it reads metadata set there"
- A subtle invariant that looks wrong but is intentional

**Never comment:**
- What the code does — well-named builder calls already say that (`AddClass`, `WithFileExtension`, `IntentManagedMerge`)
- Why a template exists or what it pairs with — that belongs in SKILL.md or the Module Builder designer comment field, not the code
- Cross-template design patterns — a reader navigating templates can see the pairing from the names and the `GetCodeBehind()` call

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
The builder can only track type references that go through its type system (`UseType`, `GetTypeName`). Raw type strings in signatures or attributes are opaque, so prefer the typed builder API before adding `AddUsing(...)`:

```csharp
// BAD — raw string, builder cannot infer System.ComponentModel
prop.AddAttribute($"DefaultValue({property.Value})");
// That requires manually calling AddUsing("System.ComponentModel") elsewhere.

// GOOD — typed builder call, UseType introduces System.ComponentModel automatically.
// Trim Attribute only after type resolution when you want [DefaultValue(...)] output.
prop.AddAttribute(UseType("System.ComponentModel.DefaultValueAttribute").RemoveSuffix("Attribute"), attribute =>
{
    attribute.AddArgument(property.Value);
});

// GOOD — method signatures are type references too
method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
```

Apply this to emitted type references: method signatures, attributes, base types, generic arguments, properties, and fields.

### 🔑 Split-file / code-behind usings
When a template contributes members to a **file other than its own** — e.g. a `.razor` template writing its `@code` into a sibling `.razor.cs` code-behind — two things bite:

1. **No inherited imports.** A plain `.razor.cs` gets **none** of Razor's implicit `_Imports` (`Microsoft.AspNetCore.Components`, `System.Threading.Tasks`, `System.Collections.Generic`, `System.Linq`, `System.ComponentModel.DataAnnotations`, …). References that compiled inline now need explicit usings, so run **every** member type, return type, and attribute through the type system. Attributes must drop the `Attribute` suffix with `.RemoveSuffix("Attribute")`:

   ```csharp
   var code = GetCodeBehind();   // the code-behind class (IBuildsCSharpMembers)
   code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnSubmitAsync", ...);
   input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
   email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
   ```

   Types referenced only inside **raw statement/expression strings** (e.g. `Encoding`, `WebEncoders`, a `.Select(...)` in a getter, or an `IEnumerable<>` field type) can't be inferred — either interpolate a `code.Template.UseType("…")` into the string, or add the namespace explicitly on the code-behind file (`CSharpFile.AddUsing("System.Linq")`).

2. **Resolve via the target block's template, not the host.** `UseType`/`GetTypeName` add (and prune) usings on the file of *that template's* `RootCodeContext`. A bare `UseType(...)` on the `.razor` template resolves against the `.razor`; instead call **`code.Template.UseType(...)`** — `code.Template` is the template that owns the block (the code-behind when present, the inline `@code` otherwise), so the using lands on the correct file either way. For this to resolve, the code-behind template must expose its class as the context:

   ```csharp
   // on the code-behind (CSharpTemplateBase) template:
   public override ICSharpCodeContext RootCodeContext => CSharpFile.Classes.Single();
   ```

   Razor `@using`/`@inject` directives are managed by the `RazorFile` independently, so this does not disturb them.

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
| Static files from `content/` folder | `StaticContentTemplateRegistration` — set `ContentSubFolder` (see below) |
| Event/pipeline driven | `ITemplateRegistration` |

`TemplateId` must be defined as `public const string` in the template and referenced by name from the registration.

## Static Content Templates

A `StaticContentTemplateRegistration` ships every file under `content/<ContentSubFolder>` into the generated app. The base `Register` **already** locates the content folder (relative to the module assembly), enumerates files, splits binary vs text via `BinaryFileGlobbingPatterns`, and registers each one. Your subclass should only **declare** and **gate** — it must never re-implement that enumeration. Reference implementations: `SamplePagesStaticContentTemplateRegistration`, `BlazorSkillSampleFilesStaticContentTemplateRegistration`, `ThemeArtifactsStaticContentTemplateRegistration` (all in `Intent.Modules.Blazor`).

### Musts
1. Declare `ContentSubFolder` and `BinaryFileGlobbingPatterns`; leave the file walking to `base.Register`.
2. To ship a folder only under a condition (a setting, an installed module), override `Register`, guard, then call `base.Register(registry, application)` — e.g. `if (!application.GetSettings()....) return; base.Register(registry, application);`.
3. For **per-file** overwrite behaviour, override `CreateTemplate` / `CreateBinaryTemplate`. They receive `fileRelativePath` and the default `OverwriteBehaviour`, so branch on the path and call `base.CreateTemplate(...)`. For a **whole-registration** default, override `GetDefaultOverrideBehaviour(outputTarget)`.
4. To emit only a **subset** of a folder's files (e.g. ship the markup from one module and the code-behind from another, or skip a file a sibling module also ships), split the content into separate sub-folders — one `ContentSubFolder` + registration each. Each stays a trivial declare-and-gate class; the gating lives in `Register`.
5. Model every registration as a **Static Content Template** element in the Module Builder designer so its `.imodspec` `<template>` entry (id, role, location) is generated — never hand-author the imodspec. Protect any hand-added `Register` / `Create*` / `GetDefaultOverrideBehaviour` override against regeneration with `[IntentMerge]` on the class (preferred — matches siblings) or `[IntentIgnore]` on the member.

### Must Nots
1. **Never re-derive the content path** from `GetType().Assembly.Location` + `Path.Combine(.., "content", ContentSubFolder)`. That copies the framework's own private implementation: it silently drifts when the SDK changes its content resolution, and a wrong assumption emits *nothing* instead of failing. `base.Register` already does this correctly.
2. **Never reflect into the base class's private members** (e.g. invoking the private `GetBinaryFiles` via `BindingFlags.NonPublic`). If a genuinely custom path needs binary detection, match the simple `*.ext` globs directly (suffix compare) — do not reach into framework internals.
3. **Never hand-roll `Directory.EnumerateFiles` + a manual `RegisterTemplate` loop** to achieve conditional skipping or per-file overwrite. Use folder splits (Must #4) and the `Create*` hooks (Must #3) instead.
4. **Never silently `return` when a content folder is "missing"** — if a registration runs, its modelled folder must exist. Swallowing a missing folder hides a modelling/path error.

## Source of Truth

> **AI:** Read from the `/resources/` folder (pattern files, cheatsheet, troubleshooting guide) for all logic and examples.  
> **Human reference:** The canonical API lives in the public repo at https://github.com/IntentArchitect/Intent.Modules — see:
> - `Modules/Intent.Modules.Common.CSharp/Builder/CSharpFile.cs`
> - `Modules/Intent.Modules.Common/FileBuilders/FileBuilderHelper.cs`
> - `Modules/Intent.Modules.Common.CSharp/Builder/IHasCSharpStatements.cs`
> - `Modules/Intent.Modules.Common.CSharp/Builder/` (all individual statement classes)
