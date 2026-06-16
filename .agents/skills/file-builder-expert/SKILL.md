---
name: file-builder-expert
description: Convert C# template files to Fluent CSharpFile builder API.
argument-hint: "[source file] [target template name]"
---

# File Builder Expert

> [!IMPORTANT]
> **Resource Read Constraint:** You are forbidden from reading the resource files under `/resources/` unless a `dotnet build` fails or a type resolution error occurs.

## Musts
1. Inherit from `CSharpTemplateBase<TModel>`, implement `ICSharpFileBuilderTemplate`, and expose `CSharpFile`.
2. Construct `CSharpFile` in constructor (with all structural fluent calls).
3. Implement `DefineFileConfig() => CSharpFile.GetConfig();` and `TransformText() => CSharpFile.ToString();`.
4. Use flow builders (`AddIfStatement`, `AddForEachStatement`, `AddTryBlock`) and `CSharpInvocationStatement` for method calls.
5. Register `OnBuild`/`AfterBuild` callbacks in constructor: **0=Core, 100=Enrichment, 500=Extension, 1000=Final**.
6. Target template lookup callbacks must use a strictly higher priority than the target template's priority.
7. Resolve types via: `GetTypeName(...)` (models), `GetTypeName(templateId, model)` (TemplateId), `UseType("Ns.Type")` (external).
8. **GetTypeName Timing:** Never call `GetTypeName`/`UseType` directly in the constructor before `CSharpFile` is defined. Execute inside lambdas or callbacks.
9. Inject DI parameters using `param.IntroduceReadonlyField()`.
10. When generating a not-implemented handler or method body, always use the following pattern:
    ```csharp
    method.AddStatement("// IntentInitialGen");
    method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
    method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
    ```

## Must Nots
1. Never emit structural C# as raw strings outside the fluent API.
2. Never use `CSharpMethodChainStatement` or `AddMethodChainStatement` (Obsolete).
3. Never add `else`, `else if`, `catch`, or `finally` as children of a block (must be siblings).
4. Never use raw string interpolation for lambda arrows `=>` or object initializer braces `{}`. Use `CSharpLambdaBlock` / `CSharpObjectInitializerBlock`.
5. Never call `field.WithAssignment(string)` directly (Obsolete); use `WithAssignment(new CSharpStatement("value"))`.
