using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Drift;

namespace Intent.Modules.Domain.Guardrails.Contracts.Drift
{
    /// <summary>
    /// The Domain designer's drift check, as this module contributes it to the drift module (Intent.Common.Drift): every
    /// fact the Domain model states, in its own vocabulary, how they are read (<see cref="DriftModel"/>), and the Domain
    /// designer's text in the drift module's rule and skills. This is the model side of the check, fixed by the module;
    /// recipes supply the code side.
    /// </summary>
    internal static class DomainDrift
    {
        public const string Name = "Domain";

        // The Entity and Member descriptions are the Services contribution's too: the configure-drift-check skill used to
        // describe a placeholder once for both designers, and its text is kept as it was.
        private static readonly FactPlaceholder Entity = new("Entity", "the class's name: an entity action's entity, or the class whose constructor or operation raises a domain event");
        private static readonly FactPlaceholder Member = new("Member", "the constructor's or operation's name, the class's name for a constructor; for an entity action, the one it targets, else empty");
        private static readonly FactPlaceholder DomainEvent = new("DomainEvent", "the domain event's name");
        private static readonly FactPlaceholder Attribute = new("Attribute", "the attribute's name");
        private static readonly FactPlaceholder Source = new("Source", "the source class's name");
        private static readonly FactPlaceholder Target = new("Target", "the target class's name");
        private static readonly FactPlaceholder SourceEnd = new("SourceEnd", "the source end's name: the navigation on the target class (may be empty)");
        private static readonly FactPlaceholder TargetEnd = new("TargetEnd", "the target end's name: the navigation on the source class (may be empty)");
        private static readonly FactPlaceholder Index = new("Index", "the index's name: its attributes' `UniqueKey`, or the `Index` element's name");

        private static readonly FactPlaceholder[] ClassPlaceholders = { Entity };
        private static readonly FactPlaceholder[] AttributePlaceholders = { Entity, Attribute };
        private static readonly FactPlaceholder[] RelationshipPlaceholders = { Source, Target, SourceEnd, TargetEnd };
        private static readonly FactPlaceholder[] IndexPlaceholders = { Entity, Index };
        private static readonly FactPlaceholder[] RaisesPlaceholders = { Entity, Member, DomainEvent };

        private static readonly string[] OwnClass = { "Entity" };
        private static readonly string[] OwnAttribute = { "Attribute" };
        private static readonly string[] OwnRelationship = { "Target", "SourceEnd", "TargetEnd" };
        private static readonly string[] OwnIndex = { "Index" };

        public static readonly FactKind TableName = new("table.name", Name, "table name", ClassPlaceholders,
            "the name exactly as modelled, or empty",
            "`Table` → `Name`, or `View` → `Name`, for every class with one of them.",
            OwnClass);

        public static readonly FactKind TableSchema = new("table.schema", Name, "schema", ClassPlaceholders,
            "the schema name, or empty",
            "`Table` or `View` → `Schema`, else the nearest folder's or the package's `Schema` → `Name`, else empty. For every class with `Table` or `View`.",
            OwnClass);

        public static readonly FactKind TableKind = new("table.kind", Name, "table or view", ClassPlaceholders,
            "`Table` or `View`",
            "Which of the two stereotypes the class has.",
            OwnClass);

        public static readonly FactKind Key = new("key", Name, "primary key", ClassPlaceholders,
            "the primary key attributes' names separated by commas, in model order; empty when there are none",
            "The attributes with `Primary Key`, for every persisted class. A persisted class has `Table` or `View`, or a database stereotype on itself or an attribute.",
            OwnClass);

        public static readonly FactKind ColumnName = new("column.name", Name, "column name", AttributePlaceholders,
            "the name exactly as modelled, or empty",
            "`Column` → `Name`, or Document DB `Field Setting` → `Name`, for every attribute of a persisted class.",
            OwnAttribute);

        public static readonly FactKind ColumnType = new("column.type", Name, "column type", AttributePlaceholders,
            "the type exactly as modelled, or empty",
            "`Column` → `Type`, for every attribute of a persisted class.",
            OwnAttribute);

        public static readonly FactKind ColumnTextType = new("column.textType", Name, "text type", AttributePlaceholders,
            "`VARCHAR`, `NVARCHAR`, `TEXT`, `NTEXT` or `DEFAULT`",
            "`Text Constraints` → `SQL Data Type`, `DEFAULT` when not modelled, for every `string` attribute of a persisted class.",
            OwnAttribute);

        public static readonly FactKind ColumnMaxLength = new("column.maxLength", Name, "max length", AttributePlaceholders,
            "a number, or empty",
            "`Text Constraints` → `MaxLength`, for every `string` attribute of a persisted class.",
            OwnAttribute);

        public static readonly FactKind ColumnRequired = new("column.required", Name, "required", AttributePlaceholders,
            "`true` or `false`",
            "`true` when the attribute isn't nullable, for every `string` and `binary` attribute of a persisted class that isn't a primary key.",
            OwnAttribute);

        public static readonly FactKind ColumnPrecision = new("column.precision", Name, "precision", AttributePlaceholders,
            "a number, or empty",
            "`Decimal Constraints` → `Precision`, for every `decimal` attribute of a persisted class.",
            OwnAttribute);

        public static readonly FactKind ColumnScale = new("column.scale", Name, "scale", AttributePlaceholders,
            "a number, or empty",
            "`Decimal Constraints` → `Scale`, for every `decimal` attribute of a persisted class.",
            OwnAttribute);

        public static readonly FactKind ColumnDefault = new("column.default", Name, "default", AttributePlaceholders,
            "the value exactly as modelled, or empty",
            "`Default Constraint` → `Value`, for every attribute of a persisted class.",
            OwnAttribute);

        public static readonly FactKind RelationshipMultiplicity = new("relationship.multiplicity", Name, "multiplicity", RelationshipPlaceholders,
            "`<source end>-><target end>`, each `1`, `0..1` or `*`, e.g. `*->1` for many orders to one customer",
            "Each end of an association between two classes, at least one of them persisted. `*` is a collection end, `0..1` a nullable one and `1` any other.",
            OwnRelationship);

        public static readonly FactKind RelationshipForeignKey = new("relationship.foreignKey", Name, "foreign key", RelationshipPlaceholders,
            "the foreign key attributes' names separated by commas, in model order; empty when there are none",
            "The attributes whose `Foreign Key` → `Association` is this association.",
            OwnRelationship);

        public static readonly FactKind IndexColumns = new("index.columns", Name, "columns", IndexPlaceholders,
            "the key attributes' names separated by commas, in the index's order",
            "Every named index of a persisted class: `Index` on its attributes, one index per `UniqueKey`, the attributes ordered by `Order`; or an `Index` element in the class with `Use Default Name` off, and its `Key` columns. An index with no name (`Index` without a `UniqueKey`, or `Use Default Name` on) isn't stated.",
            OwnIndex);

        public static readonly FactKind IndexUnique = new("index.unique", Name, "unique", IndexPlaceholders,
            "`true` or `false`",
            "`Index` → `IsUnique` on the index's first attribute, or the `Index` element's `Unique`, for every named index.",
            OwnIndex);

        public static readonly FactKind IntentRaises = new("intent.raises", Name, "raises", RaisesPlaceholders,
            "the domain event's name",
            "Each `Domain Event Origin` from a class's constructor or operation to a Domain Event: one fact per link. An operation's parameter names choose between overloads.",
            new[] { "DomainEvent" });

        public static readonly IReadOnlyList<FactKind> Kinds = new[]
        {
            TableName, TableSchema, TableKind, Key, ColumnName, ColumnType, ColumnTextType, ColumnMaxLength, ColumnRequired,
            ColumnPrecision, ColumnScale, ColumnDefault, RelationshipMultiplicity, RelationshipForeignKey, IndexColumns, IndexUnique,
            IntentRaises
        };

        public static readonly DriftDesigner Designer = new(
            Name: Name,
            DesignerId: ApiMetadataDesignerExtensions.DomainDesignerId,
            Order: 10,
            Kinds: Kinds,
            ReadFacts: DriftModel.Read,
            ReadElements: DriftModel.ElementsById,
            ValidateTaskId: "Intent.Domain.Guardrails.Contracts.ValidateDriftRecipes",
            Guidance: new DriftGuidance(
                FactsText: "database tables, keys, columns and relationships, and the domain events constructors and operations raise",
                RuleRows: """
| An entity, value object, data contract, enum, repository, domain service or domain event: the type, a property, a method or its signature | That element in the Domain designer |
| A database table, collection, column, key or relationship | The Domain designer's database stereotypes and associations |
| A domain event a constructor or operation raises | Its `Domain Event Origin` to the Domain Event |

""",
                GuideRows: """
| Domain | EF Core | A `Microsoft.EntityFrameworkCore` package and a class deriving from `DbContext` | [references/ef-core.md](references/ef-core.md) |
| Domain | MongoDB | A `MongoDB.Driver` package: `IMongoCollection<T>`, `BsonClassMap`, `[BsonId]` | [references/mongodb.md](references/mongodb.md) |
| Domain | Azure Cosmos DB | A `Microsoft.Azure.Cosmos` package (`CosmosClient`, `Container`), or EF Core's `UseCosmos` | [references/cosmos-db.md](references/cosmos-db.md) |
| Domain | Domain events | A `Domain Event Origin` in the model, from a constructor or operation to the Domain Event it raises | [references/domain-events.md](references/domain-events.md) |

""",
                IntentLinks: "a `Domain Event Origin`, from a constructor or operation to a Domain Event it raises",
                NewIntent: "a domain event a constructor or operation starts raising"));
    }
}
