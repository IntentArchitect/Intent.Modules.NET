# Context: Intent.Services.Guardrails.Contracts

## Purpose

Guards a brownfield codebase's application layer. Each Services designer element that was imported from existing code (services, commands, queries, DTOs, enums) is merged back into its existing file, so every modelled type and member keeps existing.

## Architectural Decisions

- **File default is plain `Mode.Merge`.** Each file carries only `[assembly: DefaultIntentManaged(Mode.Merge)]`, from `IntentManagedMerge()`. The user chose this, the same as in `Intent.Domain.Guardrails.Contracts`, whose CONTEXT.md records the rejected targeted Body/Signature Ignore design and why it must stay targeted if reinstated. Consequence, from decompiled RoslynWeaver 5.0.1: `MergeSignature` rewrites the modifiers, return types and parameters of matched members, and `ManageStatementBlock(..., Mode.Merge)` runs on existing bodies, which is why generated bodies are always empty.
- **Every contract is a concrete type.** Services are emitted as a class (or record), never an interface. Brownfield application services are matched by their concrete class name, which is used verbatim. The rejected option was mirroring `Intent.Modules.Application.Contracts`' `IXService` interface, which would never merge onto the existing class.
- **Type Configuration stereotype selects class or record.** Defined in this module's package and installed into the Services designer ("Include in Module" + "Reference in Designer" = Services). Applies to Service, Command, Query and DTO. Empty or absent means class. `ContractTypeKind.cs` reads it through the typed accessors, and `AddContractType` is the single dispatch to `AddClass` / `AddRecord`. A DTO whose kind differs from its parent DTO's raises an `ElementException`. The parent is read through `ParentDtoTypeReference`, because `ParentDto` throws when there are multiple generalizations.
  - **Struct was requested and then dropped by the user**, because `Intent.Modules.Common.CSharp` (up to 3.10.11) can't declare a struct. Revisit when the builder supports it.
  - RoslynWeaver matches type declarations with `kindsMustMatch: true`, so the configured kind must match the existing type on disk. A mismatch adds a second declaration with the same name.
- **Commands and queries are plain property types** with no `IRequest` or MediatR interfaces. Adding interfaces would be implementation, and would assume a mediator library the codebase may not use.
- **Async: name convention plus an explicit Async Configuration stereotype (user decision).** This is the same rule as the Domain module, whose CONTEXT.md records the reasoning and rejected options. In short: a name ending in `Async` means async, and this module's own Async Configuration stereotype on Service operations (Is Async, Include Cancellation Token) overrides it. Async operations emit the `async` keyword, because plain `Mode.Merge` otherwise strips it from existing async/await methods. The earlier by-id reads of Intent.Application.Contracts' Asynchronous/Synchronous stereotypes were removed. Keep this stereotype identical to the Domain module's.
- **Methods always have empty bodies: no statements, no `NotImplementedException` (user decision).** The empty body merges with whatever is on disk and leaves it untouched. The cost is that a new value-returning method won't compile until implemented. Same rule as the Domain module; don't reintroduce a statement.
- **Type resolution uses role strings.** The module registers its own five roles plus the Domain module's Entity, Enum, ValueObject and DataContract roles (`GuardrailsContractsServicesRoles`), with a `List<T>` formatter. That way DTO and command fields typed as Domain types resolve without an assembly reference to the Domain contract module.

- **Ships the `reverse-engineer-services` AI skill (user decision).** `ReverseEngineerServicesSkill` is a Single File, Markdown File Builder template with role `AI.Context.Skills`, emitting `skills/reverse-engineer-services/SKILL.md`. Its content is the user's, embedded in a non-interpolated raw string, verbatim except that its two `struct` mentions were removed at the user's request (the C# builder can't emit structs). It points agents at `reverse-engineer-domain` (from the Domain module) first, and describes this module's template behaviour, so a template behaviour change must update the skill in the same change.

## Invariants & Constraints

- **No casing changes, ever (user decision).** The designer is reverse-engineered from the code and is the authoritative view of it, so every generated name must be exactly the designer name: types, members, parameters, literals and namespaces. Always pass `element.Name` as a string to the builder. Its model overloads (e.g. `AddProperty(type, model)`) Pascal-case the name, so `lowerProp` becomes `LowerProp`. Under Merge that would add a second, renamed member beside the real one. The string overloads, `AddClass`/`AddRecord`/`AddEnum`/`AddLiteral` and `ToCSharpNamespace` were all probed (Common.CSharp 3.10.5) and keep names verbatim.
- The role strings are a shared contract with `Intent.Domain.Guardrails.Contracts` and with the Output Anchors in consumer applications. Changing any of them needs a synchronised bump of both modules.
- The Type Configuration stereotype is defined separately here and in the Domain module, because neither module can reference the other. Keep the options identical.
- Never call `RepresentsModel` on a `CSharpEnum` or `CSharpEnumLiteral`. It throws in Intent.Modules.Common.CSharp 3.10.5 to 3.10.11 and blocks the consumer's whole Software Factory run (see the Domain module's CONTEXT.md for the probe details).
- `Templates/TemplateExtensions.cs` is Fully managed. Shared builder logic lives in the hand-written `ContractBuilderExtensions.cs` and `ContractTypeKind.cs`.

## Module Interactions

- **Intent.Domain.Guardrails.Contracts**: four of its roles are resolved by string. There is no package or module dependency between the two.

## Superseded

- **The `Intent.Guardrails.Contracts` entry module and its `reverse-engineer-designers` skill.** Removed by the user; this module is installed directly. It used to bundle the designers, both contract modules and a skill that imported existing code and applied the Type Configuration and Async Configuration stereotypes. Each contract module now ships its own skill instead (see Architectural Decisions).
- **Targeted Body/Signature Ignore member default, and Signature = Ignore as the plan's fallback.** Removed by the user in favour of plain `Mode.Merge`.
- **"Records are not supported; every DTO, command and query is a class."** Replaced by the Type Configuration stereotype.
