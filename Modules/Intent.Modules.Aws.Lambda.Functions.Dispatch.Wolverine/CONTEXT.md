# CONTEXT.md — Intent.Modules.Aws.Lambda.Functions.Dispatch.Wolverine

## Purpose

Generates Lambda function classes that dispatch Commands and Queries via Wolverine's `IMessageBus`. Groups operations by parent folder element (CQRS-by-folder pattern), producing one Lambda function class file per folder group.

---

## Key Architectural Decisions

### Registration class must be `public` (CRITICAL — do not make `internal`)

`CqrsLambdaFunctionClassTemplateRegistration` must be declared `public`.

The SF engine discovers template registrations via `Assembly.GetExportedTypes()`. Any `FilePerModelTemplateRegistration` or `TemplateRegistration` subclass that is `internal` is silently skipped — the SF run exits with 0 staged changes and no Lambda function files are generated. There is no error or warning; it simply looks like the template produced no output.

This is a silent failure mode that is easy to introduce and hard to diagnose. Always verify registration classes are `public`.

### Grouping strategy: one Lambda function class per parent folder element

`CqrsLambdaFunctionClassTemplateRegistration` groups Commands and Queries that have `HasHttpSettings()` by their parent folder element in the Services designer. One `CqrsLambdaFunctionContainerModel` is created per folder group, and one Lambda function class file is generated per container model.

This is the same grouping strategy used by Controllers.Dispatch, AzureFunctions.Dispatch, and is the Intent convention for CQRS-over-HTTP dispatch to serverless functions.

### Template filtering: skip non-CQRS `LambdaFunctionClassTemplate` instances

`WolverineEndpointExtension` filters `LambdaFunctionClassTemplate` instances using:
```csharp
if (containerTemplate.Model is not CqrsLambdaFunctionContainerModel) continue;
```

Non-CQRS Lambda functions (plain Lambda handlers not driven by Commands/Queries) are skipped entirely. This guard is load-bearing — removing it would cause the extension to attempt Wolverine dispatch injection into unrelated Lambda function classes.

### CancellationToken workaround (AWS Lambda Annotations AWSLambda0107)

AWS Lambda Annotations source generator raises diagnostic AWSLambda0107 if a Lambda function method has a `CancellationToken` parameter. The generated code works around this by injecting:
```csharp
var cancellationToken = CancellationToken.None;
```
at the start of each function method body, instead of accepting `CancellationToken` as a parameter.

Do not "fix" this by restoring `CancellationToken` as a method parameter. The source generator will refuse to process it and the Lambda function will fail to compile.

### Guid route parameter workaround (AWS Lambda Annotations converter limitation)

AWS Lambda Annotations has difficulty automatically converting string route parameters to `Guid`. The generated code works around this by:
1. Accepting all route parameters as `string`
2. Using `Guid.TryParse(routeParam, out var id)` at the start of the method
3. Returning `HttpResults.BadRequest(...)` on parse failure

Do not accept route parameters as `Guid` directly in the method signature. The source generator does not emit the correct converter and the Lambda will fail at runtime with a binding exception.

### Dispatch: constructor-injected `IMessageBus`

`IMessageBus` is injected via the Lambda function class constructor. Dispatch uses:
- `await _sender.InvokeAsync<T>(payload, cancellationToken)` — when result is expected
- `await _sender.InvokeAsync(payload, cancellationToken)` — void commands

This is consistent with the AzureFunctions and FastEndpoints dispatch modules in this family.

### `OnBuild` priority 10

The `WolverineEndpointExtension.OnBuild` callback runs at priority 10. This ensures it runs after the base `LambdaFunctionClassTemplate` (which builds the class structure and constructor) has completed.

---

## Interactions with Other Modules

| Module | Relationship |
|---|---|
| `Intent.Aws.Lambda.Functions` | Provides the `LambdaFunctionClassTemplate` that this extension hooks. |
| `Intent.Application.Wolverine` | Provides the `IMessageBus` abstraction used in generated dispatch code. |

---

## Anti-Patterns

- **Do not make `CqrsLambdaFunctionClassTemplateRegistration` internal.** The SF engine silently skips internal registration classes — 0 staged changes is the symptom, not an error message.
- **Do not add `CancellationToken` as a method parameter in Lambda function methods.** AWS Lambda Annotations source generator (AWSLambda0107) will reject it.
- **Do not accept `Guid` directly as a route parameter type.** The Annotations source generator does not emit the correct type converter; parse from `string` manually.
- **Do not change the grouping to one-class-per-operation.** The folder-grouping model is the Intent convention for CQRS-over-HTTP serverless dispatch.
