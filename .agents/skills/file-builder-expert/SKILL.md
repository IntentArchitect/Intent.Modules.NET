---
name: file-builder-expert
description: Convert C# template files to Fluent CSharpFile builder API.
argument-hint: "[source file] [target template name]"
---

# File Builder Expert

> [!TIP]
> **Read more if you want to know about** builder APIs, patterns, or troubleshooting:
> *   [API Cheatsheet](./resources/api-cheatsheet.md) | [Patterns](./resources/builder-patterns.md) | [Troubleshooting](./resources/troubleshooting.md)
> *(To conserve tokens, avoid reading these for minor updates.)*

## Musts
1. Inherit from `CSharpTemplateBase<TModel>`, implement `ICSharpFileBuilderTemplate`, and expose `CSharpFile`.
2. Initialize `CSharpFile` structure in constructor.
3. Implement config/transform methods: `DefineFileConfig() => CSharpFile.GetConfig();` and `TransformText() => CSharpFile.ToString();`.
4. Use flow builders (`AddIfStatement`, `AddForEachStatement`) and `CSharpInvocationStatement`.
5. Register `OnBuild`/`AfterBuild` callbacks in constructor (**Core=0, Enrichment=100, Extension=500, Final=1000**).
6. Lookup callbacks must use higher priority than target template.
7. Resolve types inside callbacks/lambdas, never directly in constructor.
8. Resolve emitted type positions through the Type System APIs: `GetTypeName(...)` for model/type references, `GetTypeName(templateId, model)` for TemplateId-based references, and `UseType("Namespace.Type")` for framework/external types — **including method return/parameter types** (e.g. `method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken")`). Do not use `UseType(...)` for types represented in the Intent model.
9. For generated type declarations, put the final class/interface name on the template model/provider (for example `IHasName.Name`) and use the same model when resolving references via `GetTypeName(templateId, model)`; handle name collisions before `AddClass(...)`.
10. Inject DI parameters using `param.IntroduceReadonlyField()`.
11. For not-implemented handlers/method bodies, use:
    ```csharp
    method.AddStatement("// IntentInitialGen");
    method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
    method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
    ```

## Must Nots
1. Never emit structural C# as raw strings outside the fluent API.
2. Never use obsolete `CSharpMethodChainStatement` or `AddMethodChainStatement`.
3. Never add `else`/`catch`/`finally` as children of a block (must be siblings).
4. Never use raw string interpolation for lambda arrows `=>` or object initializer braces `{}`.
5. Never call obsolete `field.WithAssignment(string)` directly (use `WithAssignment(new CSharpStatement(...))`).
