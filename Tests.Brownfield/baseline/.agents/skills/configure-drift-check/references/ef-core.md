---
contentHash: 4E1560E42BD8F8D12C6CA935D8FC7F7060AD3BE1E7E3F93326F4F113FD067504
---
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
