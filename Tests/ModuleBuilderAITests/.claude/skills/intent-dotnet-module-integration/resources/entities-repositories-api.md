---
contentHash: A2B1F140CE748BB6159F5D00C6D9DC92AD026B70BA9C997998A4270DA9FD74F1
---
# Repository interfaces (`Intent.Entities.Repositories.Api`)

## 0. Version compatibility

Verified against `Intent.Entities.Repositories.Api` **5.2.2**, which is also the latest stable release on nuget.org. These facts only hold from a particular version, so check the installed version (its `.imodspec`'s `<version>`) before trusting them:

- **`IPagedList<T>` and the `Repository.Interface.PagedList` role only exist from 5.1.0.** Before that the interface was `IPagedResult<T>` (template `PagedResultInterfaceTemplate`, role `Repository.Interface.PagedResult`). At 5.2.2 the template still also carries the old role, which is why every in-repo caller resolves `Repository.Interface.PagedList` first and falls back to `Repository.Interface.PagedResult` (§7).
- **The `CustomRepositoryInterface` / `CustomRepository` templates, `CustomRepositoryHelper` and `CustomRepositoryMethodsExtension` only exist from 5.1.5.** Before that, each persistence module turned Repository elements into code itself.
- **`ICursorPagedList<T>` exists from 5.1.8.** 5.1.9 fixed a case where it was not generated when it should have been.
- **The `NotImplementedException` stub on unimplemented repository methods (§9) exists from 5.1.7.**
- **Before 5.1.8, a repository operation whose name ends in `Async` got two `CancellationToken` parameters.**
- **The `ElementException` raised when `Intent.Entities` templates are missing (§9) exists from 5.1.11.**

Everything else is expected to hold for 5.1.11 to 5.2.x. Release note 5.1.6 ("`FindById` overload with `queryOptions`") no longer describes this module's output: that overload now comes from `Intent.EntityFrameworkCore.Repositories` (§8). Outside these ranges, re-verify the specific fact against this module's own source.

## 1. What it generates, and what it leaves to implementing modules

This module generates the persistence **contracts** only:

- One `I{Entity}Repository` interface per aggregate root.
- The generic `IRepository<in TDomain>` it extends.
- `IUnitOfWork`.
- `IPagedList<T>` and `ICursorPagedList<T>`.
- For each Domain designer **Repository** element that is not typed to an entity, a stand-alone interface plus a stub implementation class.

It implements none of the entity repository interfaces and does not choose a database. That is the job of a persistence ("implementing") module: `EntityFrameworkCore.Repositories`, `CosmosDB`, `MongoDb`, `MongoDb.MongoFramework.Repositories`, `Aws.DynamoDB`, `Azure.TableStorage`, `Redis.Om.Repositories`, `Dapr.AspNetCore.StateManagement`. An implementer replaces the interface's base type with its own richer base interface (`IEFRepository<,>`, `ICosmosDBRepository<,>`, ...). That base carries the query methods (`FindAllAsync`, `FindAsync`, paging) **and the `UnitOfWork` property**, and the implementer emits the implementation class.

Two modules that look like implementers are not:

- `Ardalis.Repositories` sits on top of `EntityFrameworkCore.Repositories` and rewrites the interface (§9).
- `Dapper` does not reference this module at all. It ships its own copy, `Intent.Dapper.EntityRepositoryInterface`, under the same `Repository.Interface.Entity` role, so a Dapper application has no `IUnitOfWork` and no `IRepository` from this module.

## 2. Taking the dependency

This file has two audiences. An **implementer** (a persistence module) takes the typed dependency. A **consumer** (a handler, CRUD or dispatch module that injects repositories) needs no package reference at all: everything it needs resolves by role string (§7).

```xml
<!-- .csproj (implementers) -->
<PackageReference Include="Intent.Modules.Entities.Repositories.Api" Version="5.2.2" />
```

```xml
<!-- .imodspec -->
<dependency id="Intent.Entities.Repositories.Api" version="5.2.0" />
```

Both versions are illustrative. Check this module's `.imodspec` `<version>` when you write the dependency.

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.Entities.Repositories.Api` (published on nuget.org) |
| Intent module id | `Intent.Entities.Repositories.Api`, without `Modules.` |
| Namespaces | `Intent.Modules.Entities.Repositories.Api.Templates[.<TemplateName>]`, `Intent.Modules.Entities.Repositories.Api.FactoryExtensions`, `Intent.Modules.Entities.Repositories.Api` (`CustomRepositoryHelper`), and the odd one out, `Intent.Entities.Repositories.Api.Api` (`ClassModelStereotypeExtensions`, no `Modules.`) |

How in-repo modules take it:

- **Implementers use a `PackageReference` pinned well below the current version.** `EntityFrameworkCore.Repositories`, `CosmosDB`, `MongoDb`, `MongoDb.MongoFramework.Repositories` and `Aws.DynamoDB` compile against 4.1.3 while their `.imodspec` declares 5.x. So they reach anything added later by **string** TemplateId: `EntityFrameworkCore.Repositories` looks up `"Intent.Entities.Repositories.Api.CustomRepositoryInterface"` as a literal. If you need `CustomRepositoryHelper` or `GetCursorPagedListInterfaceName`, compile against a version that has them (§0).
- **Consumers take no reference.** `Application.MediatR.CRUD`, `Application.ServiceImplementations.CRUD`, `Application.DomainInteractions`, `AspNetCore.Controllers.JsonPatch` and `Application.Dtos.Pagination` resolve everything by role string. The `TemplateRoles.*` constants they use come from `Intent.Modules.Constants`, which is not on nuget.org, so use the literal role strings in §3.
- A module outside this repo has no `ProjectReference` option; take the `PackageReference` shape.

What the reference gives an implementer:

- The `TemplateId` constants and the `TemplateExtensions` helpers (`GetEntityRepositoryInterfaceName`, `GetRepositoryInterfaceName`, `GetUnitOfWorkInterfaceName`, ...).
- The typed `EntityRepositoryInterfaceTemplate`, whose `Model` is the `ClassModel`.
- `CustomRepositoryHelper`.
- `HasRepository()`, to mirror the interface's model filter in your own registration (§9).
- A `EntityRepositoryInterfaceTemplateRegistration` you can subclass.

It also brings `Intent.Modules.Modelers.Domain` (`ClassModel`, `OperationModel`) and `Intent.Modules.Modelers.Domain.Repositories` (`RepositoryModel`) transitively. Their members are documented in `intent-modelers-integration`, not here.

The 5.2.2 package builds against `Intent.Modules.Common` 3.11.0 and `Intent.SoftwareFactory.SDK` 3.14.0-pre.1, so referencing it can trigger the `NU1605` downgrade chain: raise the references restore names until it is clean.

## 3. Templates

| TemplateId | Lookup role(s) | Model |
|---|---|---|
| `Intent.Entities.Repositories.Api.EntityRepositoryInterface` | `Repository.Interface.Entity` | `ClassModel` (and also any typed `RepositoryModel`, see §5) |
| `Intent.Entities.Repositories.Api.RepositoryInterface` | `Repository.Interface.Contract` (nobody looks it up by role; use the TemplateId) | none |
| `Intent.Entities.Repositories.Api.UnitOfWorkInterface` | `Domain.UnitOfWork` | none |
| `Intent.Entities.Repositories.Api.PagedListInterface` | `Repository.Interface.PagedList`, and the old `Repository.Interface.PagedResult` | none |
| `Intent.Entities.Repositories.Api.CursorPagedListInterface` | `Repository.Interface.CursorPagedList` | none |
| `Intent.Entities.Repositories.Api.CustomRepositoryInterface` | `Repository.Interface.Entity` | `RepositoryModel` with no entity type |
| `Intent.Entities.Repositories.Api.CustomRepository` | `Repository.Implementation` | `RepositoryModel` with no entity type |

Two role facts:

- `Repository.Interface.Entity` is shared by **three** templates across modules: this module's entity interface, this module's custom-repository interface, and Dapper's copy (§9).
- The implementation role is `Repository.Implementation.Entity`. This module does not register it: implementers fill it, and this module's `CustomRepositoryMethodsExtension` looks it up (§7).

## 4. Generated shape

Interfaces land under the consumer's `Repositories` location, by default `{App}.Domain/Repositories`. `IUnitOfWork` lands in `Common/Interfaces`. `CustomRepository` classes land under the `Repository.Implementation` location, `{App}.Infrastructure/Repositories` in the test apps.

```csharp
[IntentManaged(Mode.Fully, Signature = Mode.Fully)]
public interface IRepository<in TDomain>
{
    void Add(TDomain entity);
    void Update(TDomain entity);
    void Remove(TDomain entity);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    // EntityFrameworkCore adds: bool HasDbTransaction();
}

public interface IPagedList<T> : IList<T> { int TotalCount { get; } int PageCount { get; } int PageNo { get; } int PageSize { get; } }
public interface ICursorPagedList<T> : IList<T> { int PageSize { get; } string? CursorToken { get; } }

// as this module emits it, before any implementer touches it:
[IntentManaged(Mode.Merge, Signature = Mode.Fully)]
public interface I{Entity}Repository<entity generic params...> : IRepository<{Domain.Entity.Interface type}<generic args>>
{
    // one method per Operation of a Repository element typed to this entity (§5)

    [IntentManaged(Mode.Fully)]
    Task<{Domain.Entity.Interface type}?> FindByIdAsync({PkType} {pkName}, CancellationToken cancellationToken = default);
    [IntentManaged(Mode.Fully)]
    Task<List<{Domain.Entity.Interface type}>> FindByIdsAsync({PkType}[] {pkNamePlural}, CancellationToken cancellationToken = default);
}
```

- The interface is named `I{Model.Name.ToPascalCase()}Repository`. There is exactly one interface per file: reach it with `file.Interfaces.First()`.
- `{Domain.Entity.Interface type}` is whatever the `Domain.Entity.Interface` role resolves to: `IOrder` when the Domain "create entity interfaces" setting is on, the entity class itself when it is off. Generated output shows `IEFRepository<IOrder, Order>` versus `IEFRepository<AggregateRoot1, AggregateRoot1>`.
- The `?` on `FindByIdAsync` is only emitted when the project has nullable enabled.
- A composite key turns `FindByIdAsync`'s parameter into a named tuple, `(Guid Key1Id, Guid Key2Id) id`, and leaves out `FindByIdsAsync`.
- In practice the bare `IRepository<...>` base is almost never seen: an implementer replaces it. The exception is a Document Database package whose provider is "Custom". `CustomDbProviderFactoryExtension` rewrites the base to `IRepository<{Domain.Entity class}>` (the class, not the interface), and nothing in this repo implements it.
- A `CustomRepositoryInterface` is `I{Name.EnsureSuffixedWith("Repository")}`, with **no base interface and no `[IntentManaged]` attribute**. Its `CustomRepository` is `{Name}Repository : I{Name}Repository`, with one method per Operation, each stubbed with a `NotImplementedException` body, and registers itself in DI for that interface.

## 5. The model link

| Node | Mechanism | Value |
|---|---|---|
| `EntityRepositoryInterfaceTemplate` | `template.Model` | `ClassModel` (`Intent.Modelers.Domain.Api`) |
| the entity interface | `AddMetadata("model", ...)` and `RepresentsModel(model)` | `ClassModel` |
| a method from a Repository element's Operation (on the entity interface, the custom interface **and** the implementation class) | `AddMetadata("model", ...)` and `RepresentsModel(operationModel)` | `OperationModel` (`Intent.Modelers.Domain.Api`) |
| those methods' parameters | none | |
| `FindByIdAsync` / `FindByIdsAsync` | none; only `[IntentManaged(Mode.Fully)]` | |
| `CustomRepositoryInterfaceTemplate` / `CustomRepositoryTemplate` | `template.Model` | `RepositoryModel` (`Intent.Modelers.Domain.Repositories.Api`); the interface and class carry no `"model"` metadata themselves |
| `IRepository`, `IUnitOfWork`, `IPagedList`, `ICursorPagedList` | none | |

How the pieces connect:

- **Typed Repository elements share the entity interface's template.** For each Repository element typed to a Class, `CustomRepositoryMethodsExtension` registers the entity's interface template under every one of its roles, and its TemplateId, **for the `RepositoryModel` too** (`application.RegisterTemplateInRoleForModel(role, repository, interfaceTemplate)`). So `TryGetTemplate("Repository.Interface.Entity", repositoryModel, ...)` returns the entity interface for a typed Repository, and the `CustomRepositoryInterface` for an untyped one. Its `Model` is still the `ClassModel`.
- **Calling a Repository operation from a handler goes through `RepresentsModel`, not the name.** `Application.DomainInteractions` calls `AddTypeSource(repositoryInterfaceTemplate.Id)` so the mapping system can resolve the operation's method name from the interface.
- **Metadata implementers set and consumers read.** These are conventions in this module's ecosystem, not set by this module: `"requires-explicit-update"` (`bool`) on `CSharpFile.Interfaces[0]`, and `"entity-state-template-id"` (`string`, a TemplateId) on the **file**'s `CSharpFile.Metadata`, not on the interface.
- **Read with `TryGetMetadata`.** `EntityFrameworkCore.Repositories` uses `GetMetadata`; don't copy that.

## 6. Types: complete member reference

The full public surface of this package. Verified against 5.2.2: every `Templates/*/*Partial.cs` and `*Registration.cs` (including the T4 `PagedListInterfaceTemplate.cs`), `Templates/TemplateExtensions.cs`, `CustomRepositoryHelper.cs`, `FactoryExtensions/CustomDbProviderFactoryExtension.cs` and `CustomRepositoryMethodsExtension.cs`, `Api/ClassModelStereotypeExtensions.cs`, the `.imodspec` and `release-notes.md`. Builder behaviour was checked against `Intent.Modules.Common`'s `IntentTemplateBase.FulfillsRole` and `FileBuilderHelper.cs`, and `Intent.Modules.Common.CSharp`'s `CSharpFile.AddInterface` / `AddClass` and `CSharpMetadataBase.RepresentsModel`.

### 6.1 Where each type lives

| Type(s) | Namespace | NuGet package |
|---|---|---|
| `TemplateExtensions` | `Intent.Modules.Entities.Repositories.Api.Templates` | `Intent.Modules.Entities.Repositories.Api` |
| `EntityRepositoryInterfaceTemplate`, `EntityRepositoryInterfaceTemplateRegistration` | `...Templates.EntityRepositoryInterface` | same |
| `RepositoryInterfaceTemplate`, `UnitOfWorkInterfaceTemplate`, `PagedListInterfaceTemplate`, `CursorPagedListInterfaceTemplate`, `CustomRepositoryInterfaceTemplate`, `CustomRepositoryTemplate` (each with its registration) | `...Templates.<TemplateName>` | same |
| `CustomRepositoryHelper` | `Intent.Modules.Entities.Repositories.Api` | same |
| `CustomDbProviderFactoryExtension`, `CustomRepositoryMethodsExtension` | `Intent.Modules.Entities.Repositories.Api.FactoryExtensions` | same |
| `ClassModelStereotypeExtensions` (and nested `Repository`) | `Intent.Entities.Repositories.Api.Api` | same |
| `ClassModel`, `OperationModel` | `Intent.Modelers.Domain.Api` | `Intent.Modules.Modelers.Domain` (transitive); members in `intent-modelers-integration` |
| `RepositoryModel`, `GetRepositoryModels()` | `Intent.Modelers.Domain.Repositories.Api` | `Intent.Modules.Modelers.Domain.Repositories` (transitive); members in `intent-modelers-integration` |

### 6.2 Template classes

Every template is `public` and derives `CSharpTemplateBase<TModel>`. Only the members each adds are listed; the `CSharpTemplateBase` surface (`Model`, `ExecutionContext`, `GetTypeName`, `TryGetTypeName`, `UseType`, `AddTypeSource`, ...) is not repeated.

```csharp
public partial class EntityRepositoryInterfaceTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Entities.Repositories.Api.EntityRepositoryInterface";
    public CSharpFile CSharpFile { get; }
    public string GenericTypeParameters { get; }   // "<T1, T2>" from Model.GenericTypes, or ""
    public string RepositoryInterfaceName { get; } // GetTypeName(RepositoryInterface); throws ElementException if absent
    public string EntityStateName { get; }         // "Domain.Entity" type + GenericTypeParameters; throws ElementException if absent
    public string EntityInterfaceName { get; }     // "Domain.Entity.Interface" type + GenericTypeParameters; throws ElementException if absent
    public string PrimaryKeyType { get; }          // entity template's "Surrogate Key Type", else System.Guid; single key only
    public string PrimaryKeyName { get; }          // first attribute with "Primary Key", PascalCased, else "Id"; single key only
}

public partial class RepositoryInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate { const string TemplateId = "Intent.Entities.Repositories.Api.RepositoryInterface"; CSharpFile CSharpFile { get; } }
public partial class UnitOfWorkInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate { const string TemplateId = "Intent.Entities.Repositories.Api.UnitOfWorkInterface"; CSharpFile CSharpFile { get; } }
public partial class CursorPagedListInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    const string TemplateId = "Intent.Entities.Repositories.Api.CursorPagedListInterface";
    CSharpFile CSharpFile { get; }
    override bool CanRunTemplate();  // base && some template fills "Application.Common.CursorPagedList"
}
public partial class PagedListInterfaceTemplate : CSharpTemplateBase<object>   // T4 text template, NOT ICSharpFileBuilderTemplate
{
    const string TemplateId = "Intent.Entities.Repositories.Api.PagedListInterface";
}
public partial class CustomRepositoryInterfaceTemplate : CSharpTemplateBase<RepositoryModel>, ICSharpFileBuilderTemplate { const string TemplateId = "Intent.Entities.Repositories.Api.CustomRepositoryInterface"; CSharpFile CSharpFile { get; } }
public partial class CustomRepositoryTemplate : CSharpTemplateBase<RepositoryModel>, ICSharpFileBuilderTemplate { const string TemplateId = "Intent.Entities.Repositories.Api.CustomRepository"; CSharpFile CSharpFile { get; } override void BeforeTemplateExecution(); }

public class EntityRepositoryInterfaceTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>
{
    public EntityRepositoryInterfaceTemplateRegistration(IMetadataManager metadataManager);
    public override string TemplateId { get; }
    public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model);
    public override IEnumerable<ClassModel> GetModels(IApplication application);
    // = Domain class models where (IsAggregateRoot() && (!IsAbstract || HasStereotype("Table"))) || HasRepository()
}
```

The other six templates have a matching registration: `SingleFileTemplateRegistration`, or `FilePerModelTemplateRegistration<RepositoryModel>` for the custom pair, whose `GetModels` keeps only Repository elements with `TypeReference.Element == null`.

`SharedKernel.Consumer` subclasses `EntityRepositoryInterfaceTemplateRegistration`: it overrides `GetModels` to read another application's metadata and sets `CanRun = false` at `AfterBuild` 100, so its types resolve but it writes no file.

### 6.3 `TemplateExtensions`, all of them

```csharp
// each wraps template.GetTypeName(<TemplateId>[, model]); GetTypeName throws when nothing matches
string GetEntityRepositoryInterfaceName<T>(this IIntentTemplate<T> template) where T : ClassModel;
string GetEntityRepositoryInterfaceName(this IIntentTemplate template, ClassModel model);
string GetEntityRepositoryInterfaceName<T>(this IntentTemplateBase<T> template) where T : ClassModel;
string GetEntityRepositoryInterfaceName(this IntentTemplateBase template, ClassModel model);
string GetCustomRepositoryInterfaceName<T>(this IIntentTemplate<T> template) where T : RepositoryModel;
string GetCustomRepositoryInterfaceName(this IIntentTemplate template, RepositoryModel model);
string GetCustomRepositoryName<T>(this IIntentTemplate<T> template) where T : RepositoryModel;
string GetCustomRepositoryName(this IIntentTemplate template, RepositoryModel model);
string GetPagedListInterfaceName(this IIntentTemplate template);
string GetCursorPagedListInterfaceName(this IIntentTemplate template);
string GetRepositoryInterfaceName<T>(this IIntentTemplate template);   // T is unused and cannot be inferred
string GetUnitOfWorkInterfaceName<T>(this IIntentTemplate template);   // T is unused and cannot be inferred
[Obsolete] string GetRepositoryInterfaceName<T>(this IntentTemplateBase<T> template);
[Obsolete] string GetUnitOfWorkInterfaceName<T>(this IntentTemplateBase<T> template);
[Obsolete] string GetPagedResultInterfaceName<T>(this IntentTemplateBase<T> template);  // resolves the PagedList template
```

`this.GetRepositoryInterfaceName()` / `this.GetUnitOfWorkInterfaceName()` without a type argument binds to the **`[Obsolete]`** overloads, which is what every in-repo caller does, at the cost of a CS0612 warning. To use the other pair, give the type argument explicitly: `GetRepositoryInterfaceName<object>()`. None of these adds generic arguments: a generic entity's interface name comes back without `<T>`, and callers append `GenericTypeParameters` themselves (CosmosDB does).

### 6.4 `CustomRepositoryHelper` (public static)

```csharp
// For each Operation of the RepositoryModel, adds a method to file.Interfaces.First() in an OnBuild at priority 1:
//   return type: void, or GetTypeName(ReturnType, "System.Collections.Generic.List<{0}>")
//   name: Name.ToPascalCase(); parameters: GetTypeName(p.TypeReference) p.Name
//   async (plus an optional CancellationToken) when the name ends in "Async" or the Operation has the "Asynchronous" stereotype
//   XML doc comments from the element; "model" metadata + RepresentsModel(OperationModel)
// Also AddTypeSource for Domain.Enum, Domain.Entity.Interface, Domain.DataContract, Domain.ValueObject.
void ApplyInterfaceMethods<TTemplate, TModel>(TTemplate template, RepositoryModel repositoryModel)
    where TTemplate : CSharpTemplateBase<TModel>, ICSharpFileBuilderTemplate;

// Adds the same methods to CSharpFile.Classes.First() IMMEDIATELY (not in a callback), so the class must already exist,
// and registers an AfterBuild at 1000 that stubs EVERY method of that class with no statements (see §9).
void ApplyImplementationMethods<TTemplate, TModel>(TTemplate template, RepositoryModel repositoryModel)
    where TTemplate : ICSharpFileBuilderTemplate;   // TModel unused; you must still supply it
```

### 6.5 Factory extensions and stereotype accessors

```csharp
public class CustomDbProviderFactoryExtension : FactoryExtensionBase   // Order 0
{
    public const string CustomDbProviderId = "3b436f4c-554e-4d97-8968-5167a725e5cb";
    public static bool FilterCustomDbProvider(ClassModel x);  // package has "Document Database" with Provider == CustomDbProviderId
}
public class CustomRepositoryMethodsExtension : FactoryExtensionBase { }  // Order 0; behaviour in §5 and §7

public static class ClassModelStereotypeExtensions   // namespace Intent.Entities.Repositories.Api.Api
{
    Repository GetRepository(this ClassModel model);           // null when absent
    bool HasRepository(this ClassModel model);
    bool TryGetRepository(this ClassModel model, out Repository stereotype);
    public class Repository { const string DefinitionId = "cdc3d50d-0edd-4ffd-996f-08cbdf39595c"; string Name { get; } }  // no properties
}
```

## 7. Extension points

### Implementer side: you are writing a persistence module

Wire everything from `OnAfterTemplateRegistrations` or a template's `AfterTemplateRegistration()`, following `intent-module-orchestrator`.

- **Find the interface for each entity.** All of them: `application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface"))`. One per model: `FindTemplateInstance<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, classModel)`. Then **filter to your own packages** (by package stereotype, such as `"Relational Database"` or your Document Database provider): the interface exists for every aggregate in every package (§9).
- **Replace the base interface** in an `AfterBuild`: `@interface.Interfaces.Clear(); @interface.ExtendsInterface($"{yourBase}<{domainType}, {persistenceType}>")`. EF Core uses priority 2000, CosmosDB 1000. Make your base interface extend `IRepository<TDomain>` (through `GetRepositoryInterfaceName()`), and give it a `UnitOfWork` property plus the query methods consumers call (§9).
- **Emit the implementation.** Call `FulfillsRole("Repository.Implementation.Entity")`: this module's `CustomRepositoryMethodsExtension` looks the implementation up by that role plus `ClassModel` and adds the Repository element's operations to it. Resolve the interface with `TryGetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, Model)`. Register in DI with `ContainerRegistrationRequest.ToRegister(this).ForConcern("Infrastructure").ForInterface(contractTemplate)`, where `contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model, accessibleTo: null)`.
- **Declare the persistence behaviour consumers read.** Set `CSharpFile.Interfaces[0].AddMetadata("requires-explicit-update", true)` when a loaded entity must be passed back to `Update()` (all document stores and Dapper do; EF Core does not). Set `CSharpFile.Metadata["entity-state-template-id"] = <your document TemplateId>` (in an `OnBuild`) when query filters must target a persistence type rather than the domain type (CosmosDB, TableStorage).
- **Unit of work.** EF Core: make the `DbContext` implement `GetUnitOfWorkInterfaceName()` in an `OnBuild`, and register the application `DbContext` as `IUnitOfWork` with `ContainerRegistrationRequest.ToRegister(dbContext).ForInterface(GetTemplate<IClassProvider>(UnitOfWorkInterfaceTemplate.TemplateId)).ForConcern("Infrastructure").WithResolveFromContainer().WithPerServiceCallLifeTime()`. Document stores ship their own `I{Store}UnitOfWork` and expose that through the repository's `UnitOfWork` property instead.
- **Post-process Repository element operations** (for example stored procedures) in an `OnBuild` **above priority 1**. Find the method by `Name.ToPascalCase()`, and when it is overloaded, by `TryGetMetadata<OperationModel>("model")`. EF Core uses priority 5. This module only adds `Operation` children; `Stored Procedure` children are yours to add.
- **Untyped Repository elements.** Reach `"Intent.Entities.Repositories.Api.CustomRepositoryInterface"` / `"...CustomRepository"` by TemplateId plus `RepositoryModel`, and add your provider-specific members to them.

### Consumer side: you are writing a handler, CRUD or dispatch module

- **Resolve the repository for an entity** without a package reference: `template.TryGetTypeName("Repository.Interface.Entity", classModel, out var repositoryInterface)`, then inject it. That matches this module's interface, and Dapper's. For a non-aggregate entity, `Application.DomainInteractions` walks `GetAssociationsToAggregateRoot()` and injects the **aggregate root's** repository instead. For a Repository element, pass the `RepositoryModel`, its `IElement` or its id with the same role (§5).
- **Call its operations through the mapping system.** `AddTypeSource(repositoryInterfaceTemplate.Id)` lets mapped calls resolve method names through `RepresentsModel`. Never build the method name yourself.
- **Save changes.** When `TryGetTemplate("Domain.UnitOfWork", ...)` succeeds, in-repo consumers emit synchronous `Add` / `Update` / `Remove` and then `await {repo}.UnitOfWork.SaveChangesAsync(cancellationToken);`. When it doesn't, they fall back to `AddAsync` / `UpdateAsync` / `RemoveAsync` with no save, which is the Dapper shape. For an application-level unit of work, resolve `"Domain.UnitOfWork"` (or the TemplateId `"Intent.Entities.Repositories.Api.UnitOfWorkInterface"`, as `Application.Wolverine` and `ModularMonolith.Module` do).
- **Explicit update.** When `repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var b) && b`, emit `{repo}.Update(entity);` after changing the entity.
- **Paged results.** Resolve `TryGetTypeName("Repository.Interface.PagedList", out var n) ? n : GetTypeName("Repository.Interface.PagedResult")` for `IPagedList`, and `"Repository.Interface.CursorPagedList"` for cursor paging.

## 8. Worked example: `EntityFrameworkCore.Repositories`

In emission order:

1. **Dependency.** `PackageReference` 4.1.3, `.imodspec` dependency 5.2.0. Templates added after 4.1.3 are reached by string TemplateId.
2. **`IEFRepository<TDomain, TPersistence> : IRepository<TDomain>`.** Its base comes from `this.GetRepositoryInterfaceName()`. It adds `IUnitOfWork UnitOfWork { get; }` (from `GetUnitOfWorkInterfaceName()`), `FindAsync` / `FindFirstAsync` / `FindAllAsync` (list and `IPagedList` overloads, using the PagedList role with the PagedResult fallback), `CountAsync` and `AnyAsync`, plus synchronous versions when the `AddSynchronousMethodsToRepositories` database setting is on.
3. **`RepositoryBase<TDomain, TPersistence, TDbContext>`** implements `IEFRepository<,>`, constrains `TDbContext : DbContext, IUnitOfWork`, and exposes `UnitOfWork => _dbContext`.
4. **`{Entity}Repository`.** Its registration mirrors this module's model filter but adds the package check: `(package HasRelationalDatabase() && IsAggregateRoot() && (!IsAbstract || HasTable())) || HasRepository()`. It calls `FulfillsRole("Repository.Implementation.Entity")`. In its class-configure callback (`OnBuild` 0) it gets this module's interface template for the same `ClassModel` and registers an `AfterBuild` at **2000** that, only if `Interfaces.Count == 1`, clears the base and extends `IEFRepository<{Domain.Entity.Interface}, {Domain.Entity}>`; otherwise it logs a warning. In `BeforeTemplateExecution` it publishes the DI registration for the interface.
5. **`EntityRepositoryInterfaceSyncExtension`** (Relational Database packages only) hooks the entity template's `AfterBuild`, reads the `"primary-keys"` metadata, and adds the `FindByIdAsync(id, Func<IQueryable<T>, IQueryable<T>> queryOptions, ct)` overload, plus `FindById` / `FindByIds` when the synchronous setting is on.
6. **`DbContextInterfaceExtension`** makes every `DbContext` implement `IUnitOfWork` and registers the application `DbContext` as `IUnitOfWork`: `services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>())`.
7. **`CustomRepositoryMethodsExtension`** (Order 2, after this module's Order 0). For typed Repository elements, at `OnBuild` **5** it rewrites stored-procedure operations already added at priority 1 (return type, async), adds `Stored Procedure` children, and fills the implementation bodies. For untyped ones, it does the same against `"Intent.Entities.Repositories.Api.CustomRepositoryInterface"` / `"...CustomRepository"` by string, injecting `_dbContext` when a stored procedure needs it.

Result: `public interface IAggregateRoot1Repository : IEFRepository<AggregateRoot1, AggregateRoot1>`, with the Repository element's operations first, then the `[IntentManaged(Mode.Fully)]` `FindByIdAsync` pair and `FindByIdsAsync`.

## 9. Traps

- **The interface is generated for every aggregate root in every Domain package, whatever its database.** `GetModels` has no package filter. An implementer that does not filter by package rewrites other stores' interfaces. An implementer whose own registration leaves out `HasRepository()` (CosmosDB's does) leaves a `[Repository]` class with an interface and no implementation. Not reproduced.
- **Nothing is on the interface until `OnBuild` priority 0.** `AddInterface`'s configure callback runs at `OnBuild` 0. Before that, `Interfaces[0]` exists but has no `"model"` metadata, no base interface and no generic parameters. In `AfterTemplateRegistration` / `OnAfterTemplateRegistrations`, use `template.Model`, not the metadata.
- **`FindByIdAsync` / `FindByIdsAsync` only appear during the `AfterBuild` phase.** They are added from the **entity** template's `AfterBuild` (order 0), and only when the entity class, or a base class, carries `"primary-keys"` metadata. To remove or rewrite them, use an `AfterBuild` above 0: CosmosDB removes `FindByIdsAsync` for string ids at 1000, so its `ICustomerRepository` has only `FindByIdAsync(string id, ...)`.
- **No primary key, no `FindByIdAsync`; a composite key gets a tuple `id` and no `FindByIdsAsync`.** Both are visible in the test apps (`IEntityWithNoPkRepository`, `IWithCompositeKeyRepository`). The template's `PrimaryKeyType` / `PrimaryKeyName` are single-key helpers; don't use them for composite keys.
- **`UnitOfWork` is not part of `IRepository<T>`.** Every consumer's `{repo}.UnitOfWork.SaveChangesAsync(cancellationToken)` only compiles because each implementer's base interface adds a `UnitOfWork` property, and its type differs: `IUnitOfWork` on EF Core, `ICosmosDBUnitOfWork` on CosmosDB. An implementer without that property breaks the CRUD and DomainInteractions output. Not reproduced.
- **`IUnitOfWork` is always generated, even when nothing implements it.** `AdvancedMappingCrud.Cosmos.Tests` has `IUnitOfWork` and no implementation. Consumers read "the `Domain.UnitOfWork` role resolves" as "use synchronous `Add` and `.UnitOfWork.SaveChangesAsync`". Never read it as "something is registered for `IUnitOfWork`".
- **EF Core only replaces the base when the interface has exactly one.** `Ardalis.Repositories` relies on this: at `AfterBuild` 1 it clears the interface's bases **and all its methods** (including `FindByIdAsync`) and extends three interfaces, so EF Core's 2000 replacement logs a warning and stops. That is visible in `Tests/Ardalis`: `IClientRepository : IEFRepository<Client>, IRepositoryBase<Client>, IClientReadRepository`, with no members. Add a second base interface before 2000 in an EF Core application and the interface loses `IEFRepository`'s methods; add extra bases above 2000. Not reproduced.
- **The `Repository.Interface.Entity` role matches three different templates.** Pass a `ClassModel` and you get the entity interface (this module's or Dapper's). Pass a `RepositoryModel` and you get the entity interface if the Repository is typed to a Class, the `CustomRepositoryInterface` if it is untyped, and **nothing** if it is typed to anything other than a Class. Not reproduced.
- **Async is decided by the name or a stereotype, and the name is kept as modelled.** A method is async when its name ends in `Async` **or** it has the `Asynchronous` stereotype. No `Async` suffix is added or removed. Parameters carry no `"model"` metadata; correlate at method level.
- **Hand-added members survive on the entity interface, not on the custom interface.** The entity interface is `[IntentManaged(Mode.Merge, Signature = Mode.Fully)]`. The `CustomRepositoryInterface` has **no** `[IntentManaged]` attribute under an assembly default of `Mode.Fully`, so hand-written members are removed on the next run.
- **`ApplyImplementationMethods` stubs every empty method.** Its `AfterBuild` at 1000 gives every method of the implementation class that has no statements `[IntentManaged(Mode.Fully, Body = Mode.Merge)]`, `// IntentInitialGen` and a `NotImplementedException`, not only the Repository element's methods (30 stubs in the test app's `AggregateRoot1Repository`). A method your module adds empty and fills after 1000 gets stubbed too; fill bodies earlier.
- **`IPagedList<T>` is a T4 template.** `PagedListInterfaceTemplate` is not an `ICSharpFileBuilderTemplate`, so you cannot add to it through `CSharpFile`.
- **`ICursorPagedList<T>` is only written when a template fills `Application.Common.CursorPagedList`** (in this repo, `Azure.TableStorage`'s `CursorPagedList`). Otherwise its `CanRunTemplate` is false, while the template instance still exists. Use the `t => t.CanRunTemplate()` predicate when you need the file to really exist.
- **No dependency on `Intent.Entities`.** The entity interface needs the `Domain.Entity` and `Domain.Entity.Interface` templates. When they are missing, the run fails with an `ElementException` whose message names a template `'Repository.Interface'` and suggests installing this module itself; the real cause is the missing entity templates.
- **`<interoperability>` raises versions; it installs nothing new.** It raises `Intent.Dapr.AspNetCore.StateManagement` and `Intent.EntityFrameworkCore.Repositories` to minimum versions only when they are already installed. Its declared dependencies (`Intent.Modelers.Domain`, `Intent.Modelers.Domain.Repositories`) do bring in the Repository element in the Domain designer.
- **`AddMetadata` throws on a duplicate key.** Two modules setting `"requires-explicit-update"` on the same interface throws `ArgumentException`. Check `HasMetadata` first. Not reproduced.
