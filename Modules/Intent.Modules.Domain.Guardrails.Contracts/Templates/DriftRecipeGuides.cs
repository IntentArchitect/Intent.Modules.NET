namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The configure-drift-check skill's guides to the Domain designer's recipes for EF Core, MongoDB and Azure Cosmos DB:
    /// a tested recipe file per technology, the decisions that change it, how each code form is modelled, and how to
    /// adapt it. Written beside the skill, in <c>configure-drift-check/references/</c>, and read only when the code uses
    /// the technology. The recipe files are tested against code in every style a guide covers (the test project's
    /// <c>DomainGuideTests</c>), so change one only with its tests. Edit the text with a plain-text editor: Intent's file
    /// tools re-indent raw strings.
    /// </summary>
    internal static class DriftRecipeGuides
    {
        public const string EfCore = """"
# EF Core: the Domain recipe file

Start `guardrails/domain.drift.json` from this guide when the code maps its entities with EF Core. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, set `files` to the projects that hold the entities, the `DbContext` and its configuration classes, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

A `Microsoft.EntityFrameworkCore` package, or a provider such as `Microsoft.EntityFrameworkCore.SqlServer` or `Npgsql.EntityFrameworkCore.PostgreSQL`, and a class deriving from `DbContext`. EF Core configures an entity in up to three styles, which a codebase often mixes, even for one entity:

- **In the context:** `modelBuilder.Entity<Order>(entity => { … })` in `OnModelCreating`, as database-first scaffolding writes it.
- **In configuration classes:** `class OrderConfiguration : IEntityTypeConfiguration<Order>` and its `Configure(EntityTypeBuilder<Order> builder)`, applied by `ApplyConfiguration` or `ApplyConfigurationsFromAssembly`.
- **With data annotations** on the entity class: `[Table]`, `[Key]`, `[Column]`, `[MaxLength]`, `[Required]`, `[Precision]`, `[Index]`.

The recipes cover all three. Each looks for the entity's block in `OnModelCreating` first, then its configuration class, then the entity class, and reads the entity class's annotations after the fluent configuration, as EF Core applies them: fluent configuration overrides annotations, and annotations override EF Core's conventions.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Are nullable reference types on for the entities? | `<Nullable>enable</Nullable>` in the entities' csproj, or `#nullable enable` in their files | On: nothing. Off: replace `column.required` with the version below |
| Which database? | `UseSqlServer`, `UseNpgsql`, `UseSqlite` or `UseMySql` where the context is registered | Not SQL Server: change `column.textType`'s `unstated` to name its default text type (`text` on PostgreSQL and SQLite) |
| Any global conventions? | `ConfigureConventions` (`configurationBuilder.Properties<string>().HaveMaxLength(256)`, `.AreUnicode(false)`, `Properties<decimal>().HavePrecision(18, 2)`), or a loop over `modelBuilder.Model.GetEntityTypes()` that sets them | Set that fact's `default` and `absent` to the convention's value (`256`, `VARCHAR`, `18` and `2`), and its `unstated` to say where it comes from |

Nullable reference types off, where a string is optional unless it says otherwise:

```json
"column.required": {
  "find": [
    "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
    "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
    "class:{Entity} > property:{Attribute}"
  ],
  "read": [
    { "match": "\\.IsRequired\\(false\\)", "value": "false" },
    { "match": "\\.IsRequired\\((?:true)?\\)", "value": "true" },
    { "part": "declaration", "match": "\\[Required\\b", "value": "true" }
  ],
  "inherit": ["class:{Entity} > property:{Attribute}"],
  "default": "false",
  "unstated": "no `IsRequired()` or `[Required]` for `{Attribute}`, so with nullable reference types off EF Core maps it as nullable"
}
```

## The recipe file

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "table.name": {
      "find": [
        "statement:Entity<{Entity}>",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure",
        "class:{Entity}"
      ],
      "read": [
        { "match": "\\.To(?:Table|View)\\(\"([^\"]+)\"" },
        { "part": "declaration", "match": "\\[Table\\(\"([^\"]+)\"" },
        { "match": "\\.To(?:Table|View)\\((?!\"|\\w+=>)[^)]*\\)", "unreadable": true },
        { "match": "DbSet<{Entity}>(\\w+)(?:\\{|=>)" }
      ],
      "inherit": ["class:{Entity}", "property with DbSet<{Entity}>"],
      "default": "{Entity}",
      "unstated": "no `ToTable`, `[Table]` or `DbSet<{Entity}>` property, so EF Core names the table after the class"
    },
    "table.schema": {
      "find": [
        "statement:Entity<{Entity}>",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure",
        "class:{Entity}"
      ],
      "read": [
        { "match": "\\.To(?:Table|View)\\(\"[^\"]+\",(?:schema:)?\"([^\"]+)\"" },
        { "part": "declaration", "match": "\\[Table\\(\"[^\"]+\",Schema=\"([^\"]+)\"" },
        { "match": "\\.HasDefaultSchema\\(\"([^\"]+)\"" }
      ],
      "inherit": ["class:{Entity}", "statement with \\.HasDefaultSchema\\("],
      "default": "",
      "unstated": "no schema in `ToTable` or `[Table]`, and no `HasDefaultSchema`, so the database's default schema"
    },
    "table.kind": {
      "find": [
        "statement:Entity<{Entity}>",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure",
        "class:{Entity}"
      ],
      "read": [{ "match": "\\.ToView\\(", "value": "View" }],
      "default": "Table",
      "unstated": "no `ToView`, so EF Core maps `{Entity}` to a table"
    },
    "key": {
      "find": [
        "statement:Entity<{Entity}>",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure",
        "class:{Entity}"
      ],
      "read": [
        { "match": "\\.HasKey\\(\\w+=>new\\{([^}]*)\\}" },
        { "match": "\\.HasKey\\(\\w+=>\\w+\\.(\\w+)\\)" },
        { "match": "\\.HasKey\\(((?:\"\\w+\",)*\"\\w+)\"\\)" },
        { "match": "\\.HasNoKey\\(\\)|\\[Keyless\\]", "value": "" },
        { "match": "\\[PrimaryKey\\(((?:(?:nameof\\([\\w.]+\\)|\"\\w+\"),)*(?:nameof\\([\\w.]+|\"\\w+))[)\"]" },
        { "match": "\\[Key\\b[^\\]]*\\][^{;]*?(\\w+)(?:\\{|=>)" },
        { "match": "\\b((?i:id|{Entity}id))(?:\\{|=>)" }
      ],
      "replace": [
        { "match": "nameof\\(([\\w.]+)\\)?", "with": "$1" },
        { "match": "\"", "with": "" },
        { "match": "\\b\\w+\\.", "with": "" }
      ],
      "inherit": ["class:{Entity}"],
      "default": "",
      "unstated": "no `HasKey`, `[PrimaryKey]`, `[Key]`, or `Id` or `{Entity}Id` property on `{Entity}` itself, so its key comes from a base class, which the model doesn't state"
    },
    "column.name": {
      "find": [
        "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.HasColumnName\\(\"([^\"]+)\"" },
        { "part": "declaration", "match": "\\[Column\\(\"([^\"]+)\"" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no `HasColumnName` or `[Column]` name for `{Attribute}`, so EF Core names the column after the property"
    },
    "column.type": {
      "find": [
        "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "HasColumnType\\(\"(?!(?i:n?varchar|n?text|character varying|decimal|numeric)\\b)([^\"]+)\"" },
        { "part": "declaration", "match": "TypeName=\"(?!(?i:n?varchar|n?text|character varying|decimal|numeric)\\b)([^\"]+)\"" },
        { "match": "^(?=.*\\.IsFixedLength\\(\\))(?=.*\\.IsUnicode\\(false\\)).*?\\.HasMaxLength\\((\\d+)\\)", "value": "char($1)" },
        { "match": "^(?=.*\\.IsFixedLength\\(\\)).*?\\.HasMaxLength\\((\\d+)\\)", "value": "nchar($1)" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no `HasColumnType` or `[Column(TypeName)]` for `{Attribute}`, so EF Core maps it by its type"
    },
    "column.textType": {
      "find": [
        "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.IsFixedLength\\(\\)|(?:HasColumnType\\(|TypeName=)\"(?i:n?char)\\(", "value": "DEFAULT" },
        { "match": "(?:HasColumnType\\(|TypeName=)\"((?i:n?varchar|n?text))\\b", "case": "upper" },
        { "match": "(?:HasColumnType\\(|TypeName=)\"(?i:character varying)\\b", "value": "VARCHAR" },
        { "match": "\\.IsUnicode\\(false\\)|\\[Unicode\\(false\\)\\]", "value": "VARCHAR" },
        { "match": "\\.IsUnicode\\((?:true)?\\)|\\[Unicode(?:\\(true\\))?\\]", "value": "NVARCHAR" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "DEFAULT",
      "absent": "DEFAULT",
      "unstated": "no `IsUnicode`, `[Unicode]` or text column type for `{Attribute}`, so SQL Server's default, nvarchar"
    },
    "column.maxLength": {
      "find": [
        "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.IsFixedLength\\(\\)|(?:HasColumnType\\(|TypeName=)\"(?i:n?char)\\(", "value": "" },
        { "match": "\\.HasMaxLength\\((\\d+)\\)" },
        { "part": "declaration", "match": "\\[(?:MaxLength|StringLength)\\((\\d+)" },
        { "match": "(?:HasColumnType\\(|TypeName=)\"(?i:n?varchar|character varying)\\((\\d+)\\)" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no `HasMaxLength`, `[MaxLength]` or `[StringLength]` for `{Attribute}`, so no maximum length"
    },
    "column.required": {
      "find": [
        "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.IsRequired\\(false\\)", "value": "false" },
        { "match": "\\.IsRequired\\((?:true)?\\)", "value": "true" },
        { "part": "declaration", "match": "\\[Required\\b", "value": "true" },
        { "part": "declaration", "match": "\\?{Attribute}\\b", "value": "false" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "true",
      "unstated": "`{Attribute}` isn't nullable and nothing says `IsRequired(false)`, so with nullable reference types on EF Core makes it required"
    },
    "column.precision": {
      "find": [
        "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.HasPrecision\\((\\d+)" },
        { "part": "declaration", "match": "\\[Precision\\((\\d+)" },
        { "match": "(?:HasColumnType\\(|TypeName=)\"(?i:decimal|numeric)\\((\\d+)" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no `HasPrecision`, `[Precision]` or `decimal(p,s)` column type for `{Attribute}`, so the database's default precision"
    },
    "column.scale": {
      "find": [
        "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.HasPrecision\\(\\d+,(\\d+)\\)" },
        { "part": "declaration", "match": "\\[Precision\\(\\d+,(\\d+)\\)" },
        { "match": "(?:HasColumnType\\(|TypeName=)\"(?i:decimal|numeric)\\(\\d+,\\s*(\\d+)\\)" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no scale in `HasPrecision`, `[Precision]` or a `decimal(p,s)` column type for `{Attribute}`, so the database's default scale"
    },
    "column.default": {
      "find": [
        "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)"
      ],
      "read": [
        { "match": "\\.HasDefaultValueSql\\(\"((?:[^\"\\\\]|\\\\.)*)\"\\)" },
        { "match": "\\.HasDefaultValue\\(((?:[^()]|\\([^()]*\\))*)\\)" }
      ],
      "default": "",
      "absent": "",
      "unstated": "no `HasDefaultValue` or `HasDefaultValueSql` for `{Attribute}`, so the column has no default"
    },
    "relationship.multiplicity": {
      "find": [
        "statement:Entity<{Source}> > chain with (?:Has|Owns)(?:One|Many)\\(\\w+=>\\w+\\.{TargetEnd}[,)]",
        "statement:Entity<{Target}> > chain with Has(?:One|Many)\\(\\w+=>\\w+\\.{SourceEnd}\\)",
        "class with IEntityTypeConfiguration<{Source}> > method:Configure > chain with (?:Has|Owns)(?:One|Many)\\(\\w+=>\\w+\\.{TargetEnd}[,)]",
        "class with IEntityTypeConfiguration<{Target}> > method:Configure > chain with Has(?:One|Many)\\(\\w+=>\\w+\\.{SourceEnd}\\)",
        "class:{Source} > property:{TargetEnd}"
      ],
      "read": [
        { "match": "(?<=OwnsOne\\(\\w+=>\\w+\\.{TargetEnd})[,)]", "value": "1->1" },
        { "match": "(?<=OwnsMany\\(\\w+=>\\w+\\.{TargetEnd})[,)]", "value": "1->*" },
        { "match": "(?<=HasMany\\(\\w+=>\\w+\\.{TargetEnd}\\))\\.WithMany\\(", "value": "*->*" },
        { "match": "(?<=HasMany\\(\\w+=>\\w+\\.{TargetEnd}\\))\\.WithOne\\((?=.*\\.IsRequired\\(false\\))", "value": "0..1->*" },
        { "match": "(?<=HasMany\\(\\w+=>\\w+\\.{TargetEnd}\\))\\.WithOne\\(", "value": "1->*" },
        { "match": "(?<=HasOne\\(\\w+=>\\w+\\.{TargetEnd}\\))\\.WithMany\\((?=.*\\.IsRequired\\(false\\))", "value": "*->0..1" },
        { "match": "(?<=HasOne\\(\\w+=>\\w+\\.{TargetEnd}\\))\\.WithMany\\((?=.*\\.IsRequired\\(\\))", "value": "*->1" },
        { "match": "(?<=HasOne\\(\\w+=>\\w+\\.{TargetEnd}\\))\\.WithOne\\((?=.*\\.IsRequired\\(false\\))", "value": "1->0..1" },
        { "match": "(?<=HasOne\\(\\w+=>\\w+\\.{TargetEnd}\\))\\.WithOne\\(", "value": "1->1" },
        { "match": "(?<=HasMany\\(\\w+=>\\w+\\.{SourceEnd}\\))\\.WithMany\\(", "value": "*->*" },
        { "match": "(?<=HasMany\\(\\w+=>\\w+\\.{SourceEnd}\\))\\.WithOne\\((?=.*\\.IsRequired\\(false\\))", "value": "*->0..1" },
        { "match": "(?<=HasMany\\(\\w+=>\\w+\\.{SourceEnd}\\))\\.WithOne\\((?=.*\\.IsRequired\\(\\))", "value": "*->1" },
        { "match": "(?<=HasOne\\(\\w+=>\\w+\\.{SourceEnd}\\))\\.WithMany\\((?=.*\\.IsRequired\\(false\\))", "value": "0..1->*" },
        { "match": "(?<=HasOne\\(\\w+=>\\w+\\.{SourceEnd}\\))\\.WithMany\\(", "value": "1->*" },
        { "match": "(?<=HasOne\\(\\w+=>\\w+\\.{SourceEnd}\\))\\.WithOne\\(", "value": "1->1" },
        { "part": "declaration", "match": "<{Target}>{TargetEnd}\\b", "value": "1->*" },
        { "part": "declaration", "match": "\\b{Target}\\?{TargetEnd}\\b", "value": "*->0..1" },
        { "part": "declaration", "match": "\\?{TargetEnd}(?<=\\w)Id\\b", "value": "*->0..1" },
        { "part": "declaration", "match": "\\b{Target} {TargetEnd}\\b", "value": "*->1" }
      ],
      "inherit": ["class:{Source} > property:{TargetEnd}Id", "class:{Source} > property:{TargetEnd}"],
      "default": "*->1",
      "unstated": "nothing makes the relationship optional (no `IsRequired(false)`, nullable `{TargetEnd}Id` or `{Target}?` navigation), so EF Core makes it required"
    },
    "relationship.foreignKey": {
      "find": [
        "statement:Entity<{Source}> > chain with (?:Has|Owns)(?:One|Many)\\(\\w+=>\\w+\\.{TargetEnd}[,)]",
        "statement:Entity<{Target}> > chain with Has(?:One|Many)\\(\\w+=>\\w+\\.{SourceEnd}\\)",
        "class with IEntityTypeConfiguration<{Source}> > method:Configure > chain with (?:Has|Owns)(?:One|Many)\\(\\w+=>\\w+\\.{TargetEnd}[,)]",
        "class with IEntityTypeConfiguration<{Target}> > method:Configure > chain with Has(?:One|Many)\\(\\w+=>\\w+\\.{SourceEnd}\\)",
        "class:{Source} > property:{TargetEnd}"
      ],
      "read": [
        { "match": "\\.HasForeignKey(?:<\\w+>)?\\(\\w+=>new\\{([^}]*)\\}" },
        { "match": "\\.HasForeignKey(?:<\\w+>)?\\(\\w+=>\\w+\\.(\\w+)\\)" },
        { "match": "\\.HasForeignKey(?:<\\w+>)?\\(((?:\"\\w+\",)*\"\\w+)\"\\)" },
        { "part": "declaration", "match": "\\[ForeignKey\\((?:nameof\\()?\"?([\\w.]+?)\"?\\)?\\)\\]" },
        { "part": "declaration", "match": "\\b({TargetEnd}(?<=\\w)Id)\\b" },
        { "part": "declaration", "match": "\\b({SourceEnd}(?<=\\w)Id)\\b" }
      ],
      "replace": [
        { "match": "\"", "with": "" },
        { "match": "\\b\\w+\\.", "with": "" }
      ],
      "inherit": ["class:{Source} > property:{TargetEnd}", "class:{Source} > property:{TargetEnd}Id", "class:{Target} > property:{SourceEnd}Id"],
      "default": "",
      "unstated": "no `HasForeignKey`, `[ForeignKey]` or `{TargetEnd}Id` property, so EF Core adds a shadow foreign key, which the model doesn't state"
    },
    "index.columns": {
      "find": [
        "statement:Entity<{Entity}> > chain with HasIndex\\([^;]*?\"{Index}\"\\)",
        "statement:Entity<{Entity}> > chain with (?:HasDatabaseName|HasName)\\(\"{Index}\"\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with HasIndex\\([^;]*?\"{Index}\"\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with (?:HasDatabaseName|HasName)\\(\"{Index}\"\\)",
        "class with \\[Index\\([^\\]]*Name=\"{Index}\""
      ],
      "read": [
        { "match": "HasIndex\\(\\w+=>new\\{([^}]*)\\}" },
        { "match": "HasIndex\\(\\w+=>\\w+\\.(\\w+)[,)]" },
        { "part": "declaration", "match": "\\[Index\\(((?:(?:nameof\\([\\w.]+\\)|\"\\w+\"),)*(?:nameof\\([\\w.]+|\"\\w+))[)\"](?=[^\\]]*Name=\"{Index}\")" }
      ],
      "replace": [
        { "match": "nameof\\(([\\w.]+)\\)?", "with": "$1" },
        { "match": "\"", "with": "" },
        { "match": "\\b\\w+\\.", "with": "" }
      ],
      "absent": "",
      "unstated": "no index named `{Index}` on `{Entity}`"
    },
    "index.unique": {
      "find": [
        "statement:Entity<{Entity}> > chain with HasIndex\\([^;]*?\"{Index}\"\\)",
        "statement:Entity<{Entity}> > chain with (?:HasDatabaseName|HasName)\\(\"{Index}\"\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with HasIndex\\([^;]*?\"{Index}\"\\)",
        "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with (?:HasDatabaseName|HasName)\\(\"{Index}\"\\)",
        "class with \\[Index\\([^\\]]*Name=\"{Index}\""
      ],
      "read": [
        { "match": "\\.IsUnique\\((?:true)?\\)", "value": "true" },
        { "part": "declaration", "match": "\\[Index\\((?=[^\\]]*Name=\"{Index}\")(?=[^\\]]*IsUnique=true)", "value": "true" }
      ],
      "default": "false",
      "absent": "",
      "unstated": "no `IsUnique()` or `IsUnique = true` on the index named `{Index}`"
    }
  },
  "own": [
    { "kind": "entity configuration", "find": "statement with \\bEntity<(\\w+)>\\(", "name": "$1" },
    { "kind": "entity configuration", "find": "class with IEntityTypeConfiguration<(\\w+)>", "name": "$1" },
    { "kind": "index", "find": ["chain with HasIndex\\([^;]*?\"(\\w+)\"\\)", "chain with (?:HasDatabaseName|HasName)\\(\"(\\w+)\"\\)"], "name": "$1" }
  ]
}
```

## How each form is modelled

The recipes read EF Core in the Domain designer's terms, so the model must use the same terms. Model each form as this table says. The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` or `absent` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `table.name` | `ToTable("Orders")` or `[Table("Orders")]`, else the `DbSet` property's name: `Table` → `Name` | Code. `default` (the class's name) only for an entity with no `DbSet` |
| `table.schema` | `ToTable("Orders", "sales")`, `[Table("Orders", Schema = "sales")]` or `HasDefaultSchema("sales")`: `Table` → `Schema`, or `Schema` on the package | Code, or `default` where none of them says |
| `table.kind` | `ToView` → `View`; anything else → `Table` | Code for a view, `default` for a table |
| `key` | `HasKey`, `[PrimaryKey]`, `[Key]`, or an `Id` or `{Entity}Id` property: `Primary Key` on those attributes, in that order. `HasNoKey()` or `[Keyless]`: none | Code. `default` only where the key is on a base class, which the model doesn't state |
| `column.name` | `HasColumnName("…")` or `[Column("…")]`: `Column` → `Name` | Code where the column is renamed, else `default` |
| `column.type` | `HasColumnType("…")` or `[Column(TypeName = "…")]`, except text and decimal types: `Column` → `Type`. Fixed-length text (`IsFixedLength()`): `char(n)` or `nchar(n)` | Code where stated, else `default` |
| `column.textType` | `IsUnicode(false)`, `[Unicode(false)]` or `varchar`: `VARCHAR`. `IsUnicode()` or `nvarchar`: `NVARCHAR`. `text`, `ntext`: `TEXT`, `NTEXT`. Fixed-length text: `DEFAULT` (its type is `column.type`) | Code where stated, else `default` |
| `column.maxLength` | `HasMaxLength(n)`, `[MaxLength(n)]`, `[StringLength(n)]` or `varchar(n)`: `Text Constraints` → `MaxLength`. Fixed-length text: none | Code where stated, else `default` |
| `column.required` | `?` on the property, or `IsRequired(false)`: nullable. `IsRequired()` or `[Required]`: not nullable | Code for every nullable one and every one that says so; `default` for the rest |
| `column.precision`, `column.scale` | `HasPrecision(p, s)`, `[Precision(p, s)]` or `decimal(p,s)`: `Decimal Constraints` | Code where stated, else `default` |
| `column.default` | `HasDefaultValueSql("…")` or `HasDefaultValue(…)`: `Default Constraint` → `Value`, as the code writes it | Code where stated, else `default` |
| `relationship.multiplicity` | `HasOne(…).WithMany(…)` → `*->1`, `HasMany(…).WithOne(…)` → `1->*`, `HasMany(…).WithMany(…)` → `*->*`, `HasOne(…).WithOne(…)` → `1->1`, `OwnsOne` → `1->1`, `OwnsMany` → `1->*`. `IsRequired(false)`, a nullable `{TargetEnd}Id` or a `{Target}?` navigation makes the one end `0..1` | Code for every relationship. `default` (`*->1`) only for a required many-to-one nothing marks optional |
| `relationship.foreignKey` | `HasForeignKey(…)`, `[ForeignKey]`, or a `{TargetEnd}Id` property: `Foreign Key` on that attribute | Code. `default` only for a shadow foreign key |
| `index.columns`, `index.unique` | A named index, `HasIndex(…, "IX_…")`, `HasDatabaseName("IX_…")` or `[Index(…, Name = "IX_…")]`: `Index` with a `UniqueKey`, or an `Index` element. `IsUnique()`: unique | Code |

## Adapting to house style

| You see | Change |
| --- | --- |
| An entity configured over several statements (`modelBuilder.Entity<Order>().ToTable(…);` and `modelBuilder.Entity<Order>().HasKey(…);`) | The entity-level finds match several places. For `table.*`, find `statement with Entity<{Entity}>\\(\\)\\.To(?:Table|View)\\(` instead of `statement:Entity<{Entity}>`; for `key`, `statement with Entity<{Entity}>\\(\\)\\.HasKey\\(` |
| A base configuration class (`OrderConfiguration : AuditableConfiguration<Order>`, calling `base.Configure(builder)`) | Add a column find path for its chains, before the entity class's: `class:AuditableConfiguration > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)` |
| An extension method that states a fact (`builder.Property(x => x.Name).IsRequiredString(50)`) | A read case for each: `IsRequiredString\\((\\d+)\\)` for `column.maxLength`, value `true` for `column.required`. One whose value can't be read from its call gets an `unreadable` case |
| A value from a constant (`ToTable(TableNames.Orders)`, `HasMaxLength(Lengths.Name)`) | `table.name` reports it as can't be verified already. Give any other fact an `unreadable` case for it |
| An owned type configured in its owner (`OwnsOne(o => o.Address, a => { a.Property(x => x.Street).HasMaxLength(100); })`), where the navigation is named after the owned class | Add a column find path before the entity class's: `statement with Owns(?:One|Many)\\(\\w+=>\\w+\\.{Entity}\\b > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)` |
| `ApplyConfigurationsFromAssembly`, or a naming-convention package (`UseSnakeCaseNamingConvention()`) | Nothing. Configuration classes are found by their interface, and names are read as the code states them |

## Ownership

The `own` entries report every `Entity<T>` block, every `IEntityTypeConfiguration<T>` class and every named index the model doesn't describe. An entity mapped by annotations alone has no configuration to find: add a convention for the entity classes (configure-drift-check, "New types of each element type"), such as every class in the entities' folder.

## What it can't read

- A many-to-many relationship EF Core infers from a collection navigation on each side, with no `HasMany(…).WithMany(…)`, reads as `1->*` from the source's navigation.
- A one-to-one relationship, or a principal-side `HasMany(…).WithOne()` with no navigation on the dependent, reads the dependent end as required unless the chain says `IsRequired(false)`.
- A value built at run time, such as a loop over `GetEntityTypes()` that names tables, isn't per entity. Where it is a convention, use it as the `default` (above); otherwise give the fact an `unreadable` case for it and tell the developer.
"""";

        public const string MongoDb = """"
# MongoDB: the Domain recipe file

Start `guardrails/domain.drift.json` from this guide when the code stores its documents with the MongoDB C# driver. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, set `files` to the projects that hold the document classes and their class maps, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

A `MongoDB.Driver` package, with `IMongoCollection<T>`, `BsonClassMap.RegisterClassMap<T>` or `[BsonId]` in the code. A document class is mapped in up to two styles, often mixed:

- **Attributes** on the class: `[BsonId]`, `[BsonElement("name")]`, `[BsonRequired]`, `[BsonDefaultValue(…)]`.
- **Class maps:** `BsonClassMap.RegisterClassMap<Order>(map => { map.AutoMap(); map.MapIdMember(o => o.Id); map.MapMember(o => o.Name).SetElementName("name"); })`.

The recipes read a member's class map first, then its attributes, then the driver's conventions.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Are nullable reference types on for the document classes? | `<Nullable>enable</Nullable>` in their csproj, or `#nullable enable` in their files | On: nothing. Off: set `column.required`'s `default` to `false`, and its `unstated` to say a string is nullable |
| A convention pack that names elements (`CamelCaseElementNameConvention`) | `ConventionRegistry.Register` | Nothing: the names the code states are read, and the rest stay empty in the model |

## The recipe file

A document database has no SQL types, lengths or defaults of its own, so the recipes for those facts read only what the code states, and otherwise the value the model gives a document's attribute (`DEFAULT` or empty).

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "key": {
      "find": ["statement:RegisterClassMap<{Entity}>", "class:{Entity}"],
      "read": [
        { "match": "\\.Map(?:IdMember|IdProperty|IdField)\\(\\w+=>\\w+\\.(\\w+)\\)" },
        { "match": "\\[BsonId\\b[^\\]]*\\][^{;]*?(\\w+)(?:\\{|=>)" },
        { "match": "\\b((?i:id|_id))(?:\\{|=>)" }
      ],
      "inherit": ["class:{Entity}"],
      "default": "",
      "unstated": "no `[BsonId]`, `MapIdMember` or `Id` property on `{Entity}` itself"
    },
    "column.name": {
      "find": [
        "statement:RegisterClassMap<{Entity}> > chain with Map(?:Member|Property|Field)\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.SetElementName\\(\"([^\"]+)\"\\)" },
        { "part": "declaration", "match": "\\[BsonElement\\(\"([^\"]+)\"\\)" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no `[BsonElement]` or `SetElementName` for `{Attribute}`, so the element is named after the property, or by a convention pack"
    },
    "column.type": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "MongoDB has no column types"
    },
    "column.textType": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "DEFAULT",
      "absent": "DEFAULT",
      "unstated": "MongoDB stores a string as a BSON string, with no SQL text type"
    },
    "column.maxLength": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "MongoDB has no maximum length"
    },
    "column.precision": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "MongoDB has no decimal precision"
    },
    "column.scale": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "MongoDB has no decimal scale"
    },
    "column.required": {
      "find": "class:{Entity} > property:{Attribute}",
      "read": [
        { "part": "declaration", "match": "\\[BsonRequired\\]", "value": "true" },
        { "part": "declaration", "match": "\\?{Attribute}\\b", "value": "false" }
      ],
      "default": "true",
      "unstated": "`{Attribute}` isn't nullable, and nullable reference types are on"
    },
    "column.default": {
      "find": [
        "statement:RegisterClassMap<{Entity}> > chain with Map(?:Member|Property|Field)\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.SetDefaultValue\\(((?:[^()]|\\([^()]*\\))*)\\)" },
        { "part": "declaration", "match": "\\[BsonDefaultValue\\(((?:[^()]|\\([^()]*\\))*)\\)\\]" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no `[BsonDefaultValue]` or `SetDefaultValue` for `{Attribute}`"
    },
    "relationship.multiplicity": {
      "find": ["class:{Source} > property:{TargetEnd}", "class:{Source} > property:{TargetEnd}Id"],
      "read": [
        { "part": "declaration", "match": "<{Target}>{TargetEnd}\\b", "value": "1->*" },
        { "part": "declaration", "match": "\\b{Target}\\?{TargetEnd}\\b", "value": "1->0..1" },
        { "part": "declaration", "match": "\\b{Target} {TargetEnd}\\b", "value": "1->1" },
        { "part": "declaration", "match": "\\?{TargetEnd}(?<=\\w)Id\\b", "value": "*->0..1" },
        { "part": "declaration", "match": "\\b{TargetEnd}(?<=\\w)Id\\b", "value": "*->1" }
      ]
    },
    "relationship.foreignKey": {
      "find": "class:{Source} > property:{TargetEnd}Id",
      "read": [{ "part": "declaration", "match": "\\b({TargetEnd}(?<=\\w)Id)\\b" }],
      "absent": "",
      "unstated": "no `{TargetEnd}Id` property, so `{TargetEnd}` is embedded, with no foreign key"
    },
    "index.columns": {
      "find": "statement with Name=\"{Index}\"",
      "read": [{ "match": "IndexKeys\\.((?:(?:Ascending|Descending)\\(\\w+=>\\w+\\.\\w+\\)\\.)*(?:Ascending|Descending)\\(\\w+=>\\w+\\.\\w+)\\)" }],
      "replace": [
        { "match": "(?:Ascending|Descending)\\(\\w+=>\\w+\\.(\\w+)\\)?\\.?", "with": "$1," },
        { "match": ",$", "with": "" }
      ],
      "absent": "",
      "unstated": "no index named `{Index}` is created"
    },
    "index.unique": {
      "find": "statement with Name=\"{Index}\"",
      "read": [{ "match": "\\bUnique=true\\b", "value": "true" }],
      "default": "false",
      "absent": "",
      "unstated": "the index named `{Index}` isn't created with `Unique = true`"
    }
  },
  "own": [
    { "kind": "class map", "find": "statement with RegisterClassMap<(\\w+)>", "name": "$1" },
    { "kind": "index", "find": "statement with IndexKeys[^;]*\\bName=\"(\\w+)\"", "name": "$1" }
  ]
}
```

## How each form is modelled

Model each form as this table says. The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` or `absent` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `key` | `[BsonId]`, `MapIdMember`, or an `Id` property: `Primary Key` on that attribute | Code. `default` only where the id is on a base class |
| `column.name` | `[BsonElement("…")]` or `SetElementName("…")`: `Field Setting` → `Name` | Code where the element is renamed, else `default` |
| `column.required` | `?` on the property: nullable. `[BsonRequired]`: not nullable | Code for every nullable one, `default` for the rest |
| `column.default` | `[BsonDefaultValue(…)]` or `SetDefaultValue(…)`: `Default Constraint` → `Value`, as the code writes it | Code where stated, else `default` |
| `column.type`, `column.textType`, `column.maxLength`, `column.precision`, `column.scale` | None: the model leaves them unset | `default` |
| `relationship.multiplicity` | An embedded document (`Address Address`, `List<OrderLine> Lines`): a composition, `1->1` or `1->*`. A reference to another document (`string CustomerId`): an aggregation, `*->1` (`*->0..1` when nullable) | Code |
| `relationship.foreignKey` | The reference property (`CustomerId`): `Foreign Key` on that attribute | Code for a reference, `absent` for an embedded document |
| `index.columns`, `index.unique` | An index created with a name (`new CreateIndexOptions { Name = "IX_…", Unique = true }`): `Index` with a `UniqueKey`, or an `Index` element | Code |

## Adapting to house style

| You see | Change |
| --- | --- |
| A reference collection (`List<string> TagIds`) | A read case before the others for `relationship.multiplicity`: `\\bList<\\w+>{TargetEnd|trimEnd:s}Ids\\b`, value `*->*`, and a find path `class:{Source} > property:{TargetEnd|trimEnd:s}Ids` |
| Class maps registered in a helper (`RegisterMap<Order>(map => …)`) | Replace `RegisterClassMap` in the finds with the helper's name |
| An index created with `Builders<T>.IndexKeys.Combine(…)` | A second read case for `index.columns` that reads each key in the call |

## Ownership

The `own` entries report every class map and every named index the model doesn't describe. A document class mapped by attributes alone has no class map to find: add a convention for the document classes (configure-drift-check, "New types of each element type"), such as every class in the documents' folder.

## What it can't read

- An element name or index built at run time (from a constant or a loop): give the fact an `unreadable` case for it and tell the developer.
"""";

        public const string CosmosDb = """"
# Azure Cosmos DB: the Domain recipe file

Start `guardrails/domain.drift.json` from this guide when the code stores its documents in Azure Cosmos DB. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, set `files` to the projects that hold the document classes, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

Two kinds of code reach Cosmos DB:

- **The Cosmos DB SDK:** a `Microsoft.Azure.Cosmos` package, with `CosmosClient` and `Container`. Document classes are plain classes serialized to JSON: `[JsonProperty("id")]` (Newtonsoft.Json, the SDK's default) or `[JsonPropertyName("id")]` (System.Text.Json, with a custom serializer). The recipe file below is for this.
- **EF Core's Cosmos provider:** `UseCosmos`, with `ToContainer("Orders")` and `HasPartitionKey(…)` in the model configuration. Start from the EF Core guide (`ef-core.md`) instead, with the changes under "EF Core's Cosmos provider" below.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Are nullable reference types on for the document classes? | `<Nullable>enable</Nullable>` in their csproj, or `#nullable enable` in their files | On: nothing. Off: set `column.required`'s `default` to `false`, and its `unstated` to say a string is nullable |
| A serializer that names properties (`CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }`, or `JsonNamingPolicy.CamelCase`) | Where the `CosmosClient` is built | Nothing: the names the code states are read, and the rest stay empty in the model |

## The recipe file

A document database has no SQL types, lengths or defaults of its own, so the recipes for those facts read only what the code states, and otherwise the value the model gives a document's attribute (`DEFAULT` or empty).

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "key": {
      "find": "class:{Entity}",
      "read": [
        { "match": "\\[Json(?:Property|PropertyName)\\((?:PropertyName=)?\"id\"[^\\]]*\\](?:\\[[^\\]]*\\])*[^{;\\[]*?(\\w+)(?:\\{|=>)" },
        { "match": "\\b((?i:id))(?:\\{|=>)" }
      ],
      "default": "",
      "unstated": "no property of `{Entity}` itself is serialized as `id`"
    },
    "column.name": {
      "find": "class:{Entity} > property:{Attribute}",
      "read": [
        { "part": "declaration", "match": "\\[JsonPropertyName\\(\"([^\"]+)\"\\)" },
        { "part": "declaration", "match": "\\[JsonProperty\\((?:PropertyName=)?\"([^\"]+)\"" }
      ],
      "default": "",
      "absent": "",
      "unstated": "no `[JsonProperty]` or `[JsonPropertyName]` name for `{Attribute}`, so it is named after the property, or by the serializer's naming policy"
    },
    "column.type": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "Cosmos DB has no column types"
    },
    "column.textType": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "DEFAULT",
      "absent": "DEFAULT",
      "unstated": "Cosmos DB stores a string as a JSON string, with no SQL text type"
    },
    "column.maxLength": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "Cosmos DB has no maximum length"
    },
    "column.precision": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "Cosmos DB has no decimal precision"
    },
    "column.scale": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "Cosmos DB has no decimal scale"
    },
    "column.required": {
      "find": "class:{Entity} > property:{Attribute}",
      "read": [
        { "part": "declaration", "match": "\\[JsonRequired\\]|Required=Required\\.(?:Always|DisallowNull)|\\brequired\\b", "value": "true" },
        { "part": "declaration", "match": "\\?{Attribute}\\b", "value": "false" }
      ],
      "default": "true",
      "unstated": "`{Attribute}` isn't nullable, and nullable reference types are on"
    },
    "column.default": {
      "find": "class:{Entity} > property:{Attribute}",
      "read": [{ "part": "declaration", "match": "\\[DefaultValue\\(((?:[^()]|\\([^()]*\\))*)\\)\\]" }],
      "default": "",
      "absent": "",
      "unstated": "no `[DefaultValue]` for `{Attribute}`"
    },
    "relationship.multiplicity": {
      "find": ["class:{Source} > property:{TargetEnd}", "class:{Source} > property:{TargetEnd}Id"],
      "read": [
        { "part": "declaration", "match": "<{Target}>{TargetEnd}\\b", "value": "1->*" },
        { "part": "declaration", "match": "\\b{Target}\\?{TargetEnd}\\b", "value": "1->0..1" },
        { "part": "declaration", "match": "\\b{Target} {TargetEnd}\\b", "value": "1->1" },
        { "part": "declaration", "match": "\\?{TargetEnd}(?<=\\w)Id\\b", "value": "*->0..1" },
        { "part": "declaration", "match": "\\b{TargetEnd}(?<=\\w)Id\\b", "value": "*->1" }
      ]
    },
    "relationship.foreignKey": {
      "find": "class:{Source} > property:{TargetEnd}Id",
      "read": [{ "part": "declaration", "match": "\\b({TargetEnd}(?<=\\w)Id)\\b" }],
      "absent": "",
      "unstated": "no `{TargetEnd}Id` property, so `{TargetEnd}` is embedded, with no foreign key"
    }
  },
  "own": []
}
```

Cosmos DB has no named indexes, only a container's indexing policy, so the recipe file has no `index.columns` or `index.unique`.

## How each form is modelled

Model each form as this table says. The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` or `absent` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `key` | The property serialized as `id` (`[JsonProperty("id")]`, `[JsonPropertyName("id")]`, or one named `Id`): `Primary Key` on that attribute. The partition key isn't a fact | Code. `default` only where the id is on a base class |
| `column.name` | `[JsonProperty("…")]` or `[JsonPropertyName("…")]`: `Field Setting` → `Name` | Code where the property is renamed, the id included (`id`); else `default` |
| `column.required` | `?` on the property: nullable. `[JsonRequired]`, `Required = Required.Always` or `required`: not nullable | Code for every nullable one, `default` for the rest |
| `column.default` | `[DefaultValue(…)]`: `Default Constraint` → `Value`, as the code writes it | Code where stated, else `default` |
| `column.type`, `column.textType`, `column.maxLength`, `column.precision`, `column.scale` | None: the model leaves them unset | `default` |
| `relationship.multiplicity` | An embedded document (`Address Address`, `List<OrderLine> Lines`): a composition, `1->1` or `1->*`. A reference to another document (`string CustomerId`): an aggregation, `*->1` (`*->0..1` when nullable) | Code |
| `relationship.foreignKey` | The reference property (`CustomerId`): `Foreign Key` on that attribute | Code for a reference, `absent` for an embedded document |

## EF Core's Cosmos provider

Start from `ef-core.md`'s recipe file, and:

- `table.name`, `table.schema`, `table.kind`, `index.columns` and `index.unique`: delete them. A document model has no tables or named indexes; `ToContainer` names a container, which isn't a fact.
- `column.type`, `column.textType`, `column.maxLength`, `column.precision` and `column.scale`: keep them. EF Core reads `HasMaxLength` and the rest from the model even on Cosmos DB, and the model states what the code says.
- `column.name`: replace it with this one, which reads `ToJsonProperty`:

```json
"column.name": {
  "find": [
    "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
    "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
    "class:{Entity} > property:{Attribute}"
  ],
  "read": [
    { "match": "\\.ToJsonProperty\\(\"([^\"]+)\"\\)" },
    { "part": "declaration", "match": "\\[JsonPropertyName\\(\"([^\"]+)\"\\)" }
  ],
  "inherit": ["class:{Entity} > property:{Attribute}"],
  "default": "",
  "absent": "",
  "unstated": "no `ToJsonProperty` or `[JsonPropertyName]` for `{Attribute}`, so it is named after the property"
}
```

## Adapting to house style

| You see | Change |
| --- | --- |
| A base document class that declares the id (`class Document { [JsonProperty("id")] public string Id … }`) | Nothing: `key` reads `default`, since the model doesn't state a key on a base class |
| A reference collection (`List<string> TagIds`) | A read case before the others for `relationship.multiplicity`: `\\bList<\\w+>{TargetEnd|trimEnd:s}Ids\\b`, value `*->*`, and a find path `class:{Source} > property:{TargetEnd|trimEnd:s}Ids` |

## Ownership

The SDK has no registration per document type for an `own` entry to find. Add a convention for the document classes (configure-drift-check, "New types of each element type"), such as every class in the documents' folder.
"""";
    }
}
