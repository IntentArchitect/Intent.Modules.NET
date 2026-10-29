---
contentHash: 9C2EFC0BF38CCCC14AB7606CA0AA2C4FC673BD57F1FE0230C7B2E179DECE5491
---
# Entity Framework Core (`Intent.EntityFrameworkCore`)

## 0. Version compatibility

Verified against `Intent.EntityFrameworkCore` **5.2.1-pre.0**. The latest stable release on nuget.org is 5.2.0; the only difference is that 5.2.1 also adds the `HasDbTransaction()` guard to the `Intent.Application.Mediator` behaviour. These facts only hold from a particular version, so check the installed version (its `.imodspec`'s `<version>`) before trusting them:

- **Several DbContexts, `DbContextInstance` as the DbContext template's model, and the `Infrastructure.Data.ConnectionStringDbContext` / `Application.Common.ConnectionStringDbContextInterface` roles only exist from 5.0.4.** That version introduced one DbContext per `Database Settings` connection string name. Below it there is exactly one DbContext, and none of §3's per-connection-string roles exist.
- **The settings-aware overloads only exist from 5.1.0**: `DbContextManager.GetDbContexts(appId, metadataManager, settings)`, `DbContextManager.GetDbContext(classModel, settings)` and `new DbContextInstance(package, settings)`, along with the `Default Connection String Name` module setting. Called against an older install they throw `MissingMethodException` during the Software Factory run, so declare `<dependency id="Intent.EntityFrameworkCore" version="5.1.0" />` or higher when you use them. The older overloads still exist but are `[Obsolete]`, and always treat the literal `DefaultConnection` as primary.
- **`HasDbTransaction()` on every DbContext (§9) exists from 5.0.46.**
- **The `DBSet naming convention` setting exists from 5.0.32.** Before that, DbSet names were always pluralized.
- **Implicit owned-type detection exists from 5.0.27.** From then on an entity no longer needs a `Table` stereotype to get its own configuration, so before it the set of classes with an `EntityTypeConfiguration` (§9) was different.

Everything else is expected to hold for any 5.x release from 5.1.0. Outside that range, re-verify the specific fact against EF Core's own source.

## 1. What it generates, and what it deliberately leaves to other modules

`Intent.Modules.EntityFrameworkCore` generates:

- One `DbContext` class per distinct connection string name (§9).
- One `IEntityTypeConfiguration<T>` class per non-owned domain class in a package with the `Relational Database` stereotype.
- An optional `I{DbContext}` interface in the Application layer.
- An `AddDbContext<T>` registration plus a connection string per DbContext, in the Infrastructure `DependencyInjection` class.
- A `Migrations/README.md` holding EF CLI commands. There is no migrations template at all.
- `PagedList<T>`; a `UtcDateTimeOffsetConverter` on PostgreSQL only; a `DbInitializationExtensions` class on Cosmos DB only.

It deliberately does not generate:

- `SaveChanges` / `SaveChangesAsync` overrides. They only exist once a consumer creates them (§7).
- The unit of work. `Intent.EntityFrameworkCore.Repositories` makes every DbContext implement `IUnitOfWork` and registers it against the primary DbContext.
- Domain-event dispatch (`Intent.EntityFrameworkCore.Interop.DomainEvents`), the design-time factory (`Intent.EntityFrameworkCore.DesignTimeDbContextFactory`), auditing, soft delete, multi-tenancy, the outbox, or the entity classes themselves (`Intent.Entities`).

Each of those is a separate module that enriches EF Core's output, and they are the patterns this file documents.

## 2. Taking the dependency

```xml
<!-- .csproj -->
<PackageReference Include="Intent.Modules.EntityFrameworkCore" Version="5.1.*" />
```

```xml
<!-- .imodspec -->
<dependency id="Intent.EntityFrameworkCore" version="5.1.0" />
```

Both versions are illustrative floors (§0). Check `Intent.EntityFrameworkCore.imodspec`'s own `<version>` before writing yours.

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.EntityFrameworkCore` (published on nuget.org) |
| Intent module id | `Intent.EntityFrameworkCore`, without `Modules.` |
| Namespaces | `Intent.Modules.EntityFrameworkCore.Templates` (and `.DbContext`, `.DbContextInterface`, `.EntityTypeConfiguration`, `.DbMigrationsReadMe`), `Intent.Modules.EntityFrameworkCore.FactoryExtensions` (`AddDbContextStatement`), `Intent.Modules.EntityFrameworkCore.Settings` (setting accessors), and `Intent.EntityFrameworkCore.Api` (stereotype accessors, with no `Modules.`) |

In this repo, `EntityFrameworkCore.Repositories`, `Interop.DomainEvents` and `DesignTimeDbContextFactory` use a `PackageReference` at 5.1.0. Several others (`Entities.SoftDelete`, `Entities.BasicAuditing`, `EntityFrameworkCore.DiffAudit`, `EntityFrameworkCore.TemporalTables`, the eventing modules) take no reference at all and reach EF Core's output through role strings only. A module built outside this repo has only the `PackageReference` route.

What the reference gives you:

- `DbContextManager` / `DbContextInstance`: the only correct way to list DbContexts and to map a `ClassModel` to its DbContext.
- The typed templates `DbContextTemplate`, `EntityTypeConfigurationTemplate`, `DbContextInterfaceTemplate` and `DbMigrationsReadMeTemplate`.
- The configuration statement types `EfCoreFieldConfigStatement`, `EfCoreAssociationConfigStatement` and `EfCoreKeyMappingStatement`, and `AddDbContextStatement`.
- `IsOwned` / `ShouldOptOutOfCompositeModel` and `TemplateExtensions.GetDbContextName()`.
- The EF Core setting accessors over `DatabaseSettings`.

The builder types (`ICSharpFileBuilderTemplate`, `CSharpClass`, `CSharpStatement`, ...) come from `Intent.Modules.Common.CSharp`. `GetDatabaseSettings()` and the `DatabaseSettings` group class come from `Intent.Modules.Metadata.RDBMS`, and `ClassModel` / `AttributeModel` / `AssociationEndModel` / `DomainPackageModel` from `Intent.Modules.Modelers.Domain`. All arrive transitively.

`DbContextHelpers.GetSaveChangesAsyncMethod()` / `GetSaveChangesMethod()`, which nine in-repo modules use, are not in the package. They live in the shared project `Modules/Intent.Modules.EntityFrameworkCore.Shared`, compiled into each module that imports it. See §7.

## 3. Templates

Use the literal role strings below, not the `TemplateRoles` constants: `Intent.Modules.Constants` is not on nuget.org. EF Core's own `TemplateExtensions` methods (`GetDbContextName()` and so on) are safe to call, because the constant is compiled into EF Core's assembly.

| TemplateId | Lookup role string(s) |
|---|---|
| `Intent.EntityFrameworkCore.DbContext` | `Infrastructure.Data.ConnectionStringDbContext` (every DbContext); `Infrastructure.Data.DbContext` (primary only) |
| `Intent.EntityFrameworkCore.DbContextInterface` | `Application.Common.ConnectionStringDbContextInterface` (every); `Application.Common.DbContextInterface` (primary only) |
| `Intent.EntityFrameworkCore.EntityTypeConfiguration` | `Infrastructure.Data.EntityTypeConfiguration` |
| `Intent.EntityFrameworkCore.DbMigrationsReadMe` | `Infrastructure.Data` |
| `Intent.EntityFrameworkCore.PagedList` | `Application.Common.PagedList`, and the legacy `Repository.Implementation.PagedList` |
| `Intent.EntityFrameworkCore.UtcDateTimeOffsetConverter` | `Infrastructure.Data.EntityTypeConfiguration.Converters` |
| `Intent.EntityFrameworkCore.DbInitializationExtensions` | `Startup.DbInitializationExtensions`, `Azure.Api.DbInitializationService` |

The DbContext template's `.imodspec` `<role>` is `Infrastructure`, and the interface's is `Application`. Those place the output; they are not lookup roles, so never look a DbContext up by them. The two DbContext lookup roles are added at runtime.

## 4. Generated shape

The DbContext is named `Model.DbContextName`: `ApplicationDbContext` for the primary, derived from the connection string name for any other (§9).

```csharp
public class AlternateConnStrDefaultDbDbContext : DbContext /*, I{Name} when the interface is enabled */
{
    public AlternateConnStrDefaultDbDbContext(DbContextOptions<AlternateConnStrDefaultDbDbContext> options) : base(options) { }

    public DbSet<EntityAlternate> EntityAlternates { get; set; }          // added in AfterBuild

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // [modelBuilder.HasDefaultSchema("x");]        primary only, when "Default schema name" is set
        ConfigureModel(modelBuilder);
        // ...statements consumers add in OnBuild land here...
        modelBuilder.ApplyConfiguration(new EntityAlternateConfiguration()); // added in AfterBuild
    }

    [IntentManaged(Mode.Ignore)]
    private void ConfigureModel(ModelBuilder modelBuilder) { /* seed-data comment */ }
    // [protected override void ConfigureConventions(...)]  primary only, for enums-as-strings or PostgreSQL
    // [public async Task EnsureDbCreatedAsync()]           Cosmos only
}
```

Exactly one class per file, so reach it with `file.Classes.First()`. The constructor always exists, so add parameters to `Constructors.First()`.

The entity configuration is named `{Model.Name}Configuration`. It targets the `Domain.Entity.State` type when that exists, otherwise `Domain.Entity`:

```csharp
public class ContainerConfiguration : IEntityTypeConfiguration<Container>
{
    public void Configure(EntityTypeBuilder<Container> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ContainerNumber)
            .IsRequired();
        builder.OwnsMany(x => x.Vessels, ConfigureVessels);
        // ToTable/ToView/HasBaseType, HasIndex, HasOne/HasMany, check constraints, builder.Ignore(...)
    }

    public static void ConfigureVessels(OwnedNavigationBuilder<Container, Vessel> builder) { /* owned type, configured here */ }
}
```

DI registration is emitted inside `AddInfrastructure`, one statement per DbContext, in Domain package order:

```csharp
services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
    options.UseLazyLoadingProxies();
});
```

In Memory emits `UseInMemoryDatabase("name")` and Cosmos emits `UseCosmos(configuration["Cosmos:AccountEndpoint"], ...)`, both with no builder lambda. MySQL adds a `ServerVersion.Parse("8.0")` argument. The builder lambda can also gain `UseQuerySplittingBehavior` (split-queries setting), `MigrationsHistoryTable` (primary, when a default schema is set), `UseDateOnlyTimeOnly` (SQL Server on .NET 6/7) and `UseNetTopologySuite` (primary only). `UseLazyLoadingProxies()` is emitted when the module setting is on.

## 5. The model link

EF Core uses three mechanisms, depending on the node. There is no metadata on the DbContext or configuration classes themselves; use `template.Model` for those.

| Node | Key / mechanism | Type |
|---|---|---|
| DbContext template | `template.Model` | `DbContextInstance`; its `Id` is the connection string name |
| DbSet property | `model` | `ClassModel`, or `DataContractModel` for keyless DbSets added by `EntityFrameworkCore.Repositories` |
| `modelBuilder.ApplyConfiguration(...)` statement | `model` | `ClassModel` / `DataContractModel`, as above |
| `SaveChanges*` return statement | `save-changes` | `bool` (`true`); only exists once a consumer created the method |
| EntityTypeConfiguration template | `template.Model` | `ClassModel` |
| `Configure` method | `model` | `IElement` (the class's `InternalElement`), **not** `ClassModel` |
| owned `Configure{Member}` static method | `model` | `IElement` of the owned type |
| `builder.Property` / `OwnsOne` / `OwnsMany` statement for an attribute | `model` | `AttributeModel` (statement type `EfCoreFieldConfigStatement`) |
| relationship or owned-association statement | `model` | `AssociationEndModel` (statement type `EfCoreAssociationConfigStatement`) |
| `HasKey`, key `Property`, `ToTable`, `HasIndex`, check constraints, `Ignore` | none | find `EfCoreKeyMappingStatement` / `EfCoreKeyColumnPropertyStatement` by type; the others are plain strings |
| entity class | `primary-keys` (added by EF Core) | `CSharpProperty[]` |
| entity property (set by **you**) | `non-persistent`, read by EF Core | `bool`; when `true`, EF Core emits `builder.Ignore(e => e.X)` |
| `AddDbContextStatement` | none; use its `DbContextName` property | `string` |
| provider connection-string argument | `is-connection-string` | `bool` (`true`) |

`TryGetMetadata<T>` is an `is T` test, so `method.TryGetMetadata<ClassModel>("model", out _)` on `Configure` returns `false`. Use `TryGetMetadata<IElement>(...)` and then `.AsClassModel()`, or use `template.Model`.

## 6. Types: complete member reference

The full public surface a consumer calls. Verified against `Templates/DbContextManager.cs`, `Templates/DbContext/DbContextTemplatePartial.cs`, `Templates/DbContextInterface/DbContextInterfaceTemplatePartial.cs`, `Templates/EntityTypeConfiguration/EntityTypeConfigurationTemplatePartial.cs` and `EntityTypeConfigurationTemplateRegistration.cs`, the statement classes (`EFCoreFieldConfigStatement.cs`, `EfCoreAssociationConfigStatement.cs`, `EfCoreKeyMappingStatement.cs`, `EfCoreKeyColumnPropertyStatement.cs`, `RequiredEntityProperty.cs`), `Templates/TemplateExtensions.cs`, `Templates/Extensions.cs`, the two created-event classes, `Templates/DbMigrationsReadMe/DbMigrationsReadMeTemplatePartial.cs`, `FactoryExtensions/AddDbContextStatement.cs` and `DependencyInjectionExtension.cs`, `Settings/ModuleSettingsExtensions.cs` and `Api/Extensions/DomainPackageModelStereotypeExtensions.cs` (EF Core 5.2.1-pre.0). Also against `Intent.Modules.Metadata.RDBMS`'s settings, `CSharpMetadataBase.TryGetMetadata` / `CSharpStatement` in `Intent.Modules.Common.CSharp`, and `Modules/Intent.Modules.EntityFrameworkCore.Shared/DbContextHelpers.cs`.

### 6.1 `DbContextManager` / `DbContextInstance` (`Intent.Modules.EntityFrameworkCore.Templates`)

```csharp
public static class DbContextManager
{
    IList<DbContextInstance> GetDbContexts(string applicationId, IMetadataManager mm, IApplicationSettingsProvider settings);
    DbContextInstance GetDbContext(ClassModel classModel, IApplicationSettingsProvider settings); // the DbContext a class's package maps to
    [Obsolete] IList<DbContextInstance> GetDbContexts(string applicationId, IMetadataManager mm);  // assumes "DefaultConnection"
    [Obsolete] DbContextInstance GetDbContext(ClassModel classModel);
    // GetDatabaseProviderForDbContext(...) is INTERNAL; see §9
}

public class DbContextInstance : IMetadataModel
{
    DbContextInstance(DomainPackageModel pkg, IApplicationSettingsProvider settings);
    [Obsolete] DbContextInstance(DomainPackageModel pkg);
    string Id { get; }                       // == ConnectionStringName; role + model lookups match on this
    string ConnectionStringName { get; }     // the package's "Database Settings" value, else the default name
    DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum DbProvider { get; } // SQLServer, PostgreSQL, MySQL, Oracle, InMemory, Default
    DomainPackageModel DomainPackageModel { get; } // the FIRST package only; see §9
    bool IsApplicationDbContext { get; }     // DbContextName == "ApplicationDbContext"
    string DbContextName { get; }
    string GetTypeName(IIntentTemplate template, string? defaultDbContextName = null); // returns "" (or the default) when unresolved
    // Equals/GetHashCode: ConnectionStringName + DbProvider
}
```

`GetDbContexts` only considers packages with the `Relational Database` stereotype, and returns one instance per distinct (connection string name, provider) pair. It throws an `ElementException` when two packages share a connection string name but use different providers. `GetDbContext` throws a plain `Exception` when the class is not in a Domain package; it does not check for the `Relational Database` stereotype.

### 6.2 `DbContextTemplate` (`...Templates.DbContext`)

`public partial class DbContextTemplate : CSharpTemplateBase<DbContextInstance>, ICSharpFileBuilderTemplate`. Beyond the standard `CSharpTemplateBase` surface:

```csharp
public const string TemplateId = "Intent.EntityFrameworkCore.DbContext";
public CSharpFile CSharpFile { get; }
public bool UseLazyLoadingProxies { get; }          // TEMPLATE config "Use Lazy-Loading Proxies" (default true), not the module setting
public string GetEntityNameOnly(ClassModel model);
public string GetDbSetName(ClassModel model);       // naming convention only; no collision handling (§9)
public IEnumerable<string> GetInterfaces();
public string GetDbContextOptionsType();            // "DbContextOptions<{ClassName}>"
public string GetMappingClassName(ClassModel model);
public override void BeforeTemplateExecution();     // registers I{DbContext} -> DbContext when the interface is enabled
public override IEnumerable<INugetPackageInfo> GetNugetDependencies();
```

### 6.3 `DbContextInterfaceTemplate` (`...Templates.DbContextInterface`)

`CSharpTemplateBase<DbContextInstance>, ICSharpFileBuilderTemplate` with `TemplateId`, `CSharpFile`, `bool IsEnabled` and `CanRunTemplate()`. `IsEnabled` is the `Generate DbContext interface` setting **or** "no template fills `Domain.UnitOfWork`". The interface declares `Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)` plus one get-only `DbSet<T>` per DbSet on the DbContext.

### 6.4 `EntityTypeConfigurationTemplate` (`...Templates.EntityTypeConfiguration`)

```csharp
public partial class EntityTypeConfigurationTemplate : CSharpTemplateBase<ClassModel, ITemplateDecorator>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.EntityFrameworkCore.EntityTypeConfiguration";
    public CSharpFile CSharpFile { get; }
    public bool PublishCreatedEvent { get; set; } = true;   // false = no DbSet/ApplyConfiguration on any DbContext
    public IEnumerable<CSharpStatement> GetKeyMappings(ClassModel model, RelationshipType? ownedRelationship);
    public override void BeforeTemplateExecution();          // publishes EntityTypeConfigurationCreatedEvent
    protected string GetDefaultSurrogateKeyType();
}
```

`EntityTypeConfigurationTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>` is public. Its constructor takes `IMetadataManager`, and `GetModels` can be overridden (§7).

`EntityTypeConfigurationTemplateExtensions` (public static):

- `bool IsOwned(this ClassModel, ISoftwareFactoryExecutionContext)` and `bool IsOwned(this ICanBeReferencedType, ISoftwareFactoryExecutionContext)`.
- `bool ShouldOptOutOfCompositeModel(this ClassModel, ISoftwareFactoryExecutionContext)`. Cached per application; throws if the context is not an `IApplication`.
- `bool IsValueObject(this ICanBeReferencedType, ISoftwareFactoryExecutionContext)`.

### 6.5 Configuration statement types (`...Templates.EntityTypeConfiguration`)

```csharp
public class EfCoreFieldConfigStatement : CSharpStatement, IHasCSharpStatements
{
    IList<CSharpStatement> Statements { get; }           // fluent continuations, rendered on new lines
    static EfCoreFieldConfigStatement CreateProperty(AttributeModel a, DatabaseSettings dbSettings, int? columnOrder = null); // "builder.Property(x => x.{Name.ToPascalCase()})"
    static EfCoreFieldConfigStatement CreateOwnsOne(AttributeModel a);
    static EfCoreFieldConfigStatement CreateOwnsMany(AttributeModel a);
    EfCoreFieldConfigStatement AddStatement(CSharpStatement s);   // pass ".HasConversion(...)"-style text, no semicolon
    EfCoreFieldConfigStatement AddStatements(IEnumerable<CSharpStatement> s);
}

public class EfCoreAssociationConfigStatement : CSharpStatement   // Text is null
{
    RequiredEntityProperty[] RequiredProperties;          // public FIELD: the foreign key columns
    static EfCoreAssociationConfigStatement CreateOwnsOne / CreateOwnsMany / CreateHasOne(AssociationEndModel end, IElement targetType, ICSharpFileBuilderTemplate template);
    static EfCoreAssociationConfigStatement CreateHasMany(AssociationEndModel end, IElement targetType, ICSharpFileBuilderTemplate template, Func<string,string> getTableNameByConvention);
    EfCoreAssociationConfigStatement CreateWithOwner();
    EfCoreAssociationConfigStatement WithForeignKey(bool enabled = true); // throws for many-to-many
    EfCoreAssociationConfigStatement AddStatement(CSharpStatement s);     // appended to the chain
    EfCoreAssociationConfigStatement AddStatements(IEnumerable<CSharpStatement> s);
    bool HasDefaultAssociationSourceName();
}

public class EfCoreKeyMappingStatement : CSharpStatement          // Text is null; renders HasKey/HasNoKey or ""
{
    EfCoreKeyMappingStatement(ClassModel model, RelationshipType? ownedRelationship);
    RequiredEntityProperty[] KeyColumns { get; set; }
}

public class EfCoreKeyColumnPropertyStatement : CSharpStatement   // Text is null
{
    EfCoreKeyColumnPropertyStatement(AttributeModel model, int? implicitOrder, DatabaseSettingsExtensions.PrimaryKeyValueProviderOptionsEnum pkValueProvider);
    static bool RequiresConfiguration(AttributeModel a, DatabaseSettingsExtensions.PrimaryKeyValueProviderOptionsEnum pkValueProvider);
}

public record RequiredEntityProperty(ICanBeReferencedType Class, string Name, ICanBeReferencedType Type,
    bool IsNullable = false, bool IsCollection = false, Action<CSharpProperty> ConfigureProperty = null);
```

### 6.6 `AddDbContextStatement` (`Intent.Modules.EntityFrameworkCore.FactoryExtensions`)

`public class AddDbContextStatement : CSharpStatement, IHasCSharpStatements`:

- Constructor `AddDbContextStatement(string dbContextName)`; its text is `services.AddDbContext<{name}>((sp, options) =>`.
- `string DbContextName`.
- `IList<CSharpStatement> Statements`: the `options.UseX(...)` / `options.UseLazyLoadingProxies();` statements inside the lambda.
- `AddConfigOptionStatement(CSharpStatement)` and `AddConfigOptionStatements(...)`.
- `FindAndReplace`, which also searches its child statements.

Inside the lambda, `sp` is the `IServiceProvider` and `options` is the options builder.

### 6.7 Helpers, events and settings

- `TemplateExtensions` (`...Templates`): `GetDbContextName()` (resolves the **primary** DbContext), `GetDbContextInterfaceName()` (primary only), `GetEntityTypeConfigurationName(ClassModel)`, `GetPagedListName()`, `GetUtcDateTimeOffsetConverterName()` and `GetDbInitializationExtensionsName()`.
- `Extensions` (`...Templates`): `FindSchema(ClassModel)`, `GetRelationshipType(AssociationModel)`, `Relationship(AssociationEndModel)`, `MultiplicityString`, `RelationshipString`, `IdentifierType`, `IdentifierName`, and `enum RelationshipType { OneToOne, OneToMany, ManyToOne, ManyToMany }`.
- `EntityTypeConfigurationCreatedEvent { EntityTypeConfigurationTemplate Template { get; set; } }` and `DbMigrationsReadMeCreatedEvent { DbMigrationsReadMeTemplate Template { get; } }`. Both are published from the template's `BeforeTemplateExecution`, so subscribe in `OnAfterTemplateRegistrations`.
- `DbMigrationsReadMeTemplate`: settable `PackageManagerConsoleSupported`, `IncludeStartupProjectArguments`, `IncludeDbContextArguments`, `ExtraArguments` (`List<string>`) and `ExtraComments`; read-only `BoundedContextName`, `MigrationProject`, `StartupProject` and `DbContext`.
- `Intent.Modules.EntityFrameworkCore.Settings.DatabaseSettingsExtensions`: extension methods on `Intent.Modules.Metadata.RDBMS.Settings.DatabaseSettings` (from `settings.GetDatabaseSettings()`). `DatabaseProvider()` (`.AsEnum()` gives `InMemory, SqlServer, Postgresql, MySql, Cosmos, Oracle, SqlLite`, each with an `IsX()`), `TableNamingConvention()`, `LazyLoadingWithProxies()`, `GenerateDbContextInterface()`, `EnableSplitQueriesGlobally()`, `StoreEnumsAsStrings()`, `EnumCheckConstraints()`, `DefaultSchemaName()`, `MaintainColumnOrdering()`, `PrimaryKeyValueProvider()`, `DBSetNamingConvention()`, `SuppressConnectionStringSetting()` and `DefaultConnectionStringName()`.
- `Intent.EntityFrameworkCore.Api.DomainPackageModelStereotypeExtensions`: `GetDatabaseSettings(this DomainPackageModel)`, the per-package stereotype with `ConnectionStringName()` and `DatabaseProvider()`, plus `GetCosmosDBContainerSettings`. `AttributeModel` gains `GetPartitionKey` / `GetRowVersion`.

## 7. Extension points

All are wired from `FactoryExtensionBase.OnAfterTemplateRegistrations`. Priorities refer to `OnBuild` / `AfterBuild`; the timing rules are in `intent-module-orchestrator`.

- **Enrich every DbContext.** Loop over `DbContextManager.GetDbContexts(application.Id, application.MetadataManager, application.Settings)` and call `application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.Data.ConnectionStringDbContext", dbContext)` for each. Then change it in `OnBuild`: constructor parameters through `Constructors.First().AddParameter(..., p => p.IntroduceReadonlyField())`, methods, interfaces. Use `"Infrastructure.Data.DbContext"` only when you really mean the primary DbContext, and then treat a missing one as normal, not an error (§9).
- **Hook `SaveChanges`.** EF Core emits no overrides. The in-repo convention is the shared `DbContextHelpers`. It finds the override with the most parameters by name, and if there is none it inserts `SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)` (body `return await base.SaveChangesAsync(...)`) and `int SaveChanges(bool acceptAllChangesOnSuccess)` at index 0, with the return statement tagged `save-changes`. Then call `InsertStatement(0, ...)` or `FindStatement(s => s.HasMetadata("save-changes")).InsertAbove(...)`. A module outside this repo must copy that helper exactly (name, parameter shape, metadata key) so it cooperates with every other module that uses it. The alternative is an EF interceptor registered in DI, as `Entities.SoftDelete` and `SecondLevelCaching` do.
- **Add `modelBuilder` calls.** `FindMethod("OnModelCreating")` and `AddStatement(...)`. Added in `OnBuild`, they land after `ConfigureModel(modelBuilder);` and before every `ApplyConfiguration`. Added in an `AfterBuild` at priority 1 or above, they land after the `ApplyConfiguration` statements.
- **Add conventions.** `ConfigureConventions` exists only on the primary DbContext, and only when EF Core needed it (enums as strings, or PostgreSQL). Use `FindMethod("ConfigureConventions") ?? AddMethod(...)` in an `OnBuild` at priority 1 or above, and include `base.ConfigureConventions(configurationBuilder);` when you create it. There is no in-repo precedent; not reproduced.
- **Add DbSets for your own types.** The pattern is `EntityFrameworkCore.Repositories`' `CustomRepositoryMethodsExtension`: in an `AfterBuild`, `AddProperty($"DbSet<{type}>", name, p => p.AddMetadata("model", yourModel))`, and add `modelBuilder.ApplyConfiguration(new X());` with the same `model` metadata. Guard against duplicates. They are not added for you: `EntityTypeConfigurationCreatedEvent` needs an `EntityTypeConfigurationTemplate`.
- **Change an entity configuration.** Find the templates with `FindTemplateInstances<IIntentTemplate<ClassModel>>(TemplateDependency.OnTemplate("Infrastructure.Data.EntityTypeConfiguration"))` and cast them to `ICSharpFileBuilderTemplate`. In an `AfterBuild`, `FindMethod("Configure")`, then append a fluent continuation to a property (`configure.Statements.OfType<EfCoreFieldConfigStatement>()` matched on its `model` `AttributeModel.Id`, then `.AddStatement(".HasConversion(...)")`), add whole statements like `Entities.SoftDelete`'s `builder.HasQueryFilter(...)`, or rewrite `builder.ToTable(...)` like `TemporalTables`. Owned types live in the owner's `Configure{Member}` methods (§9).
- **Configure `AddDbContext`.** In the Infrastructure `"Infrastructure.DependencyInjection"` template, in an `OnBuild` at priority 1 or above (in-repo modules use 10), `FindMethod("AddInfrastructure")`, walk `method.Statements.OfType<AddDbContextStatement>()`, filter on `DbContextName`, and call `AddConfigOptionStatement("options.AddInterceptors(...);")`. To change the connection string, replace the argument tagged `is-connection-string`. Finish before `AfterBuild` 1000 (§9).
- **Keep a property out of the database.** On your property in the entity template, `property.AddMetadata("non-persistent", true)` in `OnBuild`. EF Core then emits `builder.Ignore(e => e.X);`. `Intent.DomainEvents` does this for `DomainEvents`.
- **Generate configurations for other models.** Subclass `EntityTypeConfigurationTemplateRegistration` and override `GetModels`. To use the instances only so their types resolve, set `PublishCreatedEvent = false` and `CanRun = false` in an `AfterBuild` at 100, as `AspNetCore.Identity`'s `EntityTypeConfigurationTemplateStubRegistration` does.
- **Adjust the migrations README.** Subscribe to `DbMigrationsReadMeCreatedEvent` in `OnAfterTemplateRegistrations` and set its properties, like `DesignTimeDbContextFactory`'s `MigrationsReadMeExtension`.
- **Learn the connection string name.** EF Core publishes a `ConnectionStringRegistrationRequest(name: ConnectionStringName, ...)` per DbContext, unless `Suppress Connection String Setting` is on; Cosmos instead gets `AppSettingRegistrationRequest`s for `Cosmos:AccountEndpoint`, `:AccountKey`, `:DatabaseName` and `:EnsureDbCreated`. It also publishes an `InfrastructureRegisteredEvent` named `SqlServer`, `PostgreSql`, `MySql`, `CosmosDb` or `Oracle`, with the property `ConnectionStringName`; SQLite and In Memory get none. Subscribe the way `Intent.Infrastructure.Constants` does.

## 8. Worked example: `EntityFrameworkCore.Interop.DomainEvents`

This module is the cleanest consumer that handles several DbContexts correctly. EF Core's `<interoperability>` block installs it whenever `Intent.DomainEvents` is present.

1. Dependency: `PackageReference` to `Intent.Modules.EntityFrameworkCore` 5.1.0, `<dependency id="Intent.EntityFrameworkCore" version="5.1.0" />`, and an import of `Intent.Modules.EntityFrameworkCore.Shared.projitems` for the `SaveChanges` helpers.
2. `DomainEventsDbContextExtension` (`Order => 0`) calls the settings-aware `DbContextManager.GetDbContexts(application.Id, application.MetadataManager, application.Settings)`.
3. Per instance, it calls `application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.Data.ConnectionStringDbContext", context)` and null-checks the result.
4. In one `OnBuild` at priority 0, it adds the usings; adds `Constructors.First().AddParameter(GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId), "domainEventService", p => p.IntroduceReadonlyField())`; calls `GetSaveChangesMethod().InsertStatement(0, "DispatchEventsAsync().GetAwaiter().GetResult();")` and `GetSaveChangesAsyncMethod().InsertStatement(0, "await DispatchEventsAsync(cancellationToken);")`; and adds a private `DispatchEventsAsync` that loops over `ChangeTracker.Entries<IHasDomainEvent>()`.
5. Result: in `Tests/EntityFrameworkCore.MultiDbContext.WithDefaultDbContext`, both `ApplicationDbContext` and the secondary `AlternateConnStrDefaultDbDbContext` get the constructor parameter, both `SaveChanges` overrides and the dispatch method.

Compare `Entities.BasicAuditing`, `EntityFrameworkCore.DiffAudit` and `EntityFrameworkCore.DataMasking`: they target `"Infrastructure.Data.DbContext"`, so in that same test app only `ApplicationDbContext` gets `SetAuditableFieldsAsync()` / `LogDiffAudit()`. That is only right if the feature really is primary-only.

## 9. Traps

- **Primary versus every DbContext.** `Infrastructure.Data.DbContext` is filled only by the DbContext whose `ConnectionStringName` equals the `Default Connection String Name` setting (blank means `DefaultConnection`). When every package sets a custom name there is no primary: `FindTemplateInstance("Infrastructure.Data.DbContext")` returns `null`, `TemplateExtensions.GetDbContextName()` fails to resolve, and `DesignTimeDbContextFactory` does not run. Seen in `Tests/EntityFrameworkCore.MultiDbContext.NoDefaultDbContext`, which has `Db1DbContext` to `Db3DbContext`, no `ApplicationDbContext` and no `IUnitOfWork` registration.
- **One DbContext per connection string, not per package.** `GetDbContexts` de-duplicates on (connection string name, provider), so packages without `Database Settings`, or sharing a name, collapse into one DbContext. `DbContextInstance.DomainPackageModel` is only the first of them, so never use it to list a DbContext's entities; check `DbContextManager.GetDbContext(classModel, settings).Equals(instance)` per class. Seen in `WithDefaultDbContext`, where `ApplicationDbContext` holds DbSets from two packages.
- **DbContext naming.** A non-primary name strips `ConnectionString` / `Connection`, PascalCases the result, strips a `DbContext` suffix and appends `DbContext`, so `AlternateConnStrDefaultDb` becomes `AlternateConnStrDefaultDbDbContext`. Resolve the type with `dbContextInstance.GetTypeName(template)`, never by building the name yourself. That call returns `""` (not `null`, no exception) when nothing resolves.
- **Finding `services.AddDbContext` by text hits only the first DbContext, and the first is not necessarily the primary.** The order follows Domain packages. In `WithDefaultDbContext`, `Entities.SoftDelete`'s interceptor and `AspNetCore.MultiTenancy`'s connection-string rewrite landed on the secondary `AlternateConnStrDefaultDbDbContext`, not on `ApplicationDbContext`. Use `OfType<AddDbContextStatement>()` and filter on `DbContextName`.
- **`AddDbContextStatement` stops existing at `AfterBuild` 1000.** `Intent.Infrastructure.Constants`, which EF Core auto-installs, replaces every statement with an `is-connection-string` argument by plain re-parsed statements. After that, `OfType<AddDbContextStatement>()` and the metadata are gone. `AspNetCore.IntegrationTesting`'s fixture does the same at 10000. Make DI changes in an `OnBuild` at priority 1 or above, or an `AfterBuild` below 1000.
- **DbSets and `ApplyConfiguration` statements appear late.** An `AfterBuild` at priority 0 adds them, and it is registered only when the first `EntityTypeConfigurationCreatedEvent` fires in `BeforeTemplateExecution`. Read or change them from an `AfterBuild` at priority 1 or above (`DataMasking` uses 1000). Find DbSets by `model` metadata, never by name: a colliding name is re-prefixed with its folder names or replaced with the type name, and `DbContextTemplate.GetDbSetName()` does not account for that. Not reproduced.
- **Configuration statements are added at `OnBuild` 100.** In an earlier callback, `Configure` is empty. `builder.Ignore(...)` lines come later still, from an `AfterBuild` at 0 on the **entity** template's file.
- **`Text` is `null` on `EfCoreKeyMappingStatement`, `EfCoreAssociationConfigStatement` and `EfCoreKeyColumnPropertyStatement`.** A predicate like `s => s.Text.StartsWith(...)` over `Configure` throws a `NullReferenceException`. Use `s.Text?.` (as `TemporalTables` does) or a type test.
- **Owned and composite classes have no configuration template.** `GetModels` leaves out owned classes and abstract classes that are not opted out. Value objects and composites are configured inside the owner's `public static void Configure{Member}(OwnedNavigationBuilder<TOwner,TOwned> builder)`, whose `model` is the owned type's `IElement`. A lookup by role and `ClassModel` for an owned class returns `null`. Use `IsOwned` / `ShouldOptOutOfCompositeModel` rather than re-deriving ownership.
- **Two types are named `DatabaseSettings`, and two enums `DatabaseProviderOptionsEnum`.** The module-setting group is in `Intent.Modules.Metadata.RDBMS.Settings`, with members `SqlServer`, `Postgresql`, `Cosmos`, `SqlLite` and so on. The per-package stereotype is `Intent.EntityFrameworkCore.Api.DomainPackageModelStereotypeExtensions.DatabaseSettings`, with `SQLServer`, `PostgreSQL`, `Default` and so on, and no Cosmos or SQLite. A package can override the provider, but the mapping method `GetDatabaseProviderForDbContext` is `internal`: map `DbContextInstance.DbProvider` yourself, treating `Default` as the app setting. Much of EF Core reads only the app-wide setting: entity configurations, index `IncludeProperties`, conventions and the design-time factory.
- **Cosmos changes the shape.** No `ToTable` statements; `ToContainer` / `HasPartitionKey` come first in `Configure`; no builder lambda in `UseCosmos`; an `EnsureDbCreatedAsync()` method; and a `DbInitializationExtensions` class that targets the legacy `Distribution.WebApi.Startup` `Configure` method.
- **The DbContext interface template is always registered**, one per DbContext, even when disabled; it just doesn't run. So `FindTemplateInstance("Application.Common.DbContextInterface")` is non-null even when no file is generated. Check `DbContextInterfaceTemplate.IsEnabled`, as `PagedList` does. It is enabled by the setting, or automatically whenever nothing fills `Domain.UnitOfWork`. UNVERIFIED: whether type resolution skips a disabled instance.
- **Constructor parameters are padded with `null!` at `AfterBuild` 1100.** `DesignTimeDbContextFactory` passes one `null!` for each primary-DbContext constructor parameter beyond the options, giving `new ApplicationDbContext(optionsBuilder.Options, null!, null!)`. Add constructor parameters before 1100.
- **`HasDbTransaction()` is added to every DbContext at `AfterBuild` 500**, and to `IUnitOfWork` and the primary `I{DbContext}` interface. Call `FindMethod("HasDbTransaction")` before adding your own.
- **The shared `SaveChanges` helper uses `SingleOrDefault` after ordering.** If any module adds a second `SaveChangesAsync` overload (for example `(CancellationToken)`), every module that uses the helper throws "Sequence contains more than one matching element". Not reproduced. Don't add overloads; hook the existing one.
- **Lazy-loading proxies are controlled in two places.** The module setting drives `options.UseLazyLoadingProxies()` and makes navigation properties `virtual` (entity `OnBuild` 1000). The `Microsoft.EntityFrameworkCore.Proxies` NuGet package follows the template configuration `Use Lazy-Loading Proxies` (default true) instead. The two can disagree. Not reproduced.
- **Auto-installs.** EF Core's `<interoperability>` block installs `Interop.DomainEvents` (when `Intent.DomainEvents` is present), `EntityFrameworkCore.Repositories`, `Entities`, `AspNetCore.HealthChecks` and `Infrastructure.Constants`. So an application with EF Core usually already has the unit of work, domain-event dispatch, and the connection-string rewrite of the DI registration.
- **Don't copy `DataMasking`'s lookups.** It finds `builder.Property(x => x.{attribute.Name})` by text without PascalCasing, while EF Core emits `ToPascalCase()`, and it matches `ApplyConfiguration` statements on `ClassModel.Name`. Match on statement type plus `model` metadata `Id` instead.
