using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Domain.Guardrails.Contracts.Drift
{
    /// <summary>
    /// A kind of fact the model states: what it is called in a recipe file, how the report labels it, the
    /// placeholders its recipes can use, and the model's vocabulary for its value. <paramref name="Own"/> names the
    /// placeholders that name the fact's own element, where the rule in <see cref="FactKinds.OwnPlaceholders"/> doesn't.
    /// </summary>
    internal sealed record FactKind(
        string Key,
        string Designer,
        string Label,
        IReadOnlyList<string> Placeholders,
        string Values,
        string Source,
        IReadOnlyList<string> Own = null);

    /// <summary>One fact the model states about one element.</summary>
    internal sealed class ModelFact
    {
        public FactKind Kind { get; init; }

        /// <summary>The element as the report names it, e.g. <c>sapPlants.differential</c>.</summary>
        public string Element { get; init; }

        /// <summary>
        /// The ids of the model element that states the fact and of the elements it belongs to, outermost first (a
        /// class, then its attribute), for the report's links to them.
        /// </summary>
        public IReadOnlyList<string> ElementIds { get; init; } = Array.Empty<string>();

        public string Value { get; init; }

        public IReadOnlyDictionary<string, string> Placeholders { get; init; }

        /// <summary>The operation's parameter names in model order, used to choose between overloads.</summary>
        public IReadOnlyList<string> ParameterNames { get; init; }
    }

    /// <summary>
    /// Every fact the Guardrails designers state, in their own vocabulary. This is the model side of the drift
    /// check, fixed by the module; recipes supply the code side. Identical in both Guardrails modules apart from
    /// the namespace, because the shared configure-drift-check skill documents both designers' facts.
    /// </summary>
    internal static class FactKinds
    {
        public const string Domain = "Domain";
        public const string Services = "Services";

        private static readonly string[] EndpointPlaceholders = { "Service", "Operation", "Folder" };
        private static readonly string[] ParameterPlaceholders = { "Service", "Operation", "Folder", "Parameter" };
        private static readonly string[] ClassPlaceholders = { "Entity" };
        private static readonly string[] AttributePlaceholders = { "Entity", "Attribute" };
        private static readonly string[] RelationshipPlaceholders = { "Source", "Target", "SourceEnd", "TargetEnd" };
        private static readonly string[] IndexPlaceholders = { "Entity", "Index" };
        private static readonly string[] RaisesPlaceholders = { "Entity", "Member", "DomainEvent" };
        private static readonly string[] EntityActionPlaceholders = { "Service", "Operation", "Folder", "Entity", "Member" };
        private static readonly string[] EntityPlaceholders = { "Service", "Operation", "Folder", "Entity" };
        private static readonly string[] InvocationPlaceholders = { "Service", "Operation", "Folder", "Invoked", "InvokedOwner" };
        private static readonly string[] MessagePlaceholders = { "Service", "Operation", "Folder", "Message" };

        private const string Handlers = "a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event";

        public static readonly FactKind ServiceRoute = new("service.route", Services, "route", new[] { "Service" },
            "the route exactly as modelled, or empty",
            "A Service's `Http Service Settings` → `Route`, for every Service with that stereotype.");

        public static readonly FactKind OperationVerb = new("operation.verb", Services, "verb", EndpointPlaceholders,
            "`GET`, `POST`, `PUT`, `PATCH` or `DELETE`",
            "An endpoint's `Http Settings` → `Verb`. An endpoint is a Service operation, Command or Query with `Http Settings`.");

        public static readonly FactKind OperationRoute = new("operation.route", Services, "route", EndpointPlaceholders,
            "the route exactly as modelled, or empty",
            "An endpoint's `Http Settings` → `Route`. A Service operation's route is relative to its Service's route, unless it starts with `~/` or `/`.");

        public static readonly FactKind Security = new("security", Services, "security", EndpointPlaceholders,
            "`Secured`, `Unsecured` or `None`",
            "An endpoint's effective security: `Unsecured` or `Secured` on the element itself, else on its Service, its folders or its package, else `None`. Stated for every endpoint once the designer uses `Secured` or `Unsecured` anywhere.");

        public static readonly FactKind SecurityRoles = new("security.roles", Services, "roles", EndpointPlaceholders,
            "role names separated by commas, with no spaces, in model order",
            "`Secured` → `Roles` and `Security Roles`, from the stereotype that makes the endpoint secured, where any are modelled.");

        public static readonly FactKind SecurityPolicy = new("security.policy", Services, "policy", EndpointPlaceholders,
            "policy names separated by commas, with no spaces, in model order",
            "`Secured` → `Policy` and `Security Policies`, from the stereotype that makes the endpoint secured, where any are modelled.");

        public static readonly FactKind ParameterSource = new("parameter.source", Services, "binding", ParameterPlaceholders,
            "`Default`, `From Body`, `From Form`, `From Header`, `From Query` or `From Route`",
            "`Parameter Settings` → `Source` on an endpoint's parameter, or on a field of a Command or Query endpoint.");

        public static readonly FactKind TableName = new("table.name", Domain, "table name", ClassPlaceholders,
            "the name exactly as modelled, or empty",
            "`Table` → `Name`, or `View` → `Name`, for every class with one of them.");

        public static readonly FactKind TableSchema = new("table.schema", Domain, "schema", ClassPlaceholders,
            "the schema name, or empty",
            "`Table` or `View` → `Schema`, else the nearest folder's or the package's `Schema` → `Name`, else empty. For every class with `Table` or `View`.");

        public static readonly FactKind TableKind = new("table.kind", Domain, "table or view", ClassPlaceholders,
            "`Table` or `View`",
            "Which of the two stereotypes the class has.");

        public static readonly FactKind Key = new("key", Domain, "primary key", ClassPlaceholders,
            "the primary key attributes' names separated by commas, in model order; empty when there are none",
            "The attributes with `Primary Key`, for every persisted class. A persisted class has `Table` or `View`, or a database stereotype on itself or an attribute.");

        public static readonly FactKind ColumnName = new("column.name", Domain, "column name", AttributePlaceholders,
            "the name exactly as modelled, or empty",
            "`Column` → `Name`, or Document DB `Field Setting` → `Name`, for every attribute of a persisted class.");

        public static readonly FactKind ColumnType = new("column.type", Domain, "column type", AttributePlaceholders,
            "the type exactly as modelled, or empty",
            "`Column` → `Type`, for every attribute of a persisted class.");

        public static readonly FactKind ColumnTextType = new("column.textType", Domain, "text type", AttributePlaceholders,
            "`VARCHAR`, `NVARCHAR`, `TEXT`, `NTEXT` or `DEFAULT`",
            "`Text Constraints` → `SQL Data Type`, `DEFAULT` when not modelled, for every `string` attribute of a persisted class.");

        public static readonly FactKind ColumnMaxLength = new("column.maxLength", Domain, "max length", AttributePlaceholders,
            "a number, or empty",
            "`Text Constraints` → `MaxLength`, for every `string` attribute of a persisted class.");

        public static readonly FactKind ColumnRequired = new("column.required", Domain, "required", AttributePlaceholders,
            "`true` or `false`",
            "`true` when the attribute isn't nullable, for every `string` and `binary` attribute of a persisted class that isn't a primary key.");

        public static readonly FactKind ColumnPrecision = new("column.precision", Domain, "precision", AttributePlaceholders,
            "a number, or empty",
            "`Decimal Constraints` → `Precision`, for every `decimal` attribute of a persisted class.");

        public static readonly FactKind ColumnScale = new("column.scale", Domain, "scale", AttributePlaceholders,
            "a number, or empty",
            "`Decimal Constraints` → `Scale`, for every `decimal` attribute of a persisted class.");

        public static readonly FactKind ColumnDefault = new("column.default", Domain, "default", AttributePlaceholders,
            "the value exactly as modelled, or empty",
            "`Default Constraint` → `Value`, for every attribute of a persisted class.");

        public static readonly FactKind RelationshipMultiplicity = new("relationship.multiplicity", Domain, "multiplicity", RelationshipPlaceholders,
            "`<source end>-><target end>`, each `1`, `0..1` or `*`, e.g. `*->1` for many orders to one customer",
            "Each end of an association between two classes, at least one of them persisted. `*` is a collection end, `0..1` a nullable one and `1` any other.");

        public static readonly FactKind RelationshipForeignKey = new("relationship.foreignKey", Domain, "foreign key", RelationshipPlaceholders,
            "the foreign key attributes' names separated by commas, in model order; empty when there are none",
            "The attributes whose `Foreign Key` → `Association` is this association.");

        public static readonly FactKind IndexColumns = new("index.columns", Domain, "columns", IndexPlaceholders,
            "the key attributes' names separated by commas, in the index's order",
            "Every named index of a persisted class: `Index` on its attributes, one index per `UniqueKey`, the attributes ordered by `Order`; or an `Index` element in the class with `Use Default Name` off, and its `Key` columns. An index with no name (`Index` without a `UniqueKey`, or `Use Default Name` on) isn't stated.");

        public static readonly FactKind IndexUnique = new("index.unique", Domain, "unique", IndexPlaceholders,
            "`true` or `false`",
            "`Index` → `IsUnique` on the index's first attribute, or the `Index` element's `Unique`, for every named index.");

        public static readonly FactKind IntentRaises = new("intent.raises", Domain, "raises", RaisesPlaceholders,
            "the domain event's name",
            "Each `Domain Event Origin` from a class's constructor or operation to a Domain Event: one fact per link. An operation's parameter names choose between overloads.",
            Own: new[] { "DomainEvent" });

        public static readonly FactKind IntentCreates = new("intent.creates", Services, "creates", EntityActionPlaceholders,
            "the entity's class name",
            $"Each `Create Entity Action` from {Handlers}: one fact per action. `{{Member}}` is the constructor or factory operation it targets, if any.",
            Own: new[] { "Entity", "Member" });

        public static readonly FactKind IntentUpdates = new("intent.updates", Services, "updates", EntityActionPlaceholders,
            "the entity's class name",
            $"Each `Update Entity Action` from {Handlers}: one fact per action. `{{Member}}` is the operation it targets, if any.",
            Own: new[] { "Entity", "Member" });

        public static readonly FactKind IntentDeletes = new("intent.deletes", Services, "deletes", EntityPlaceholders,
            "the entity's class name",
            $"Each `Delete Entity Action` from {Handlers}: one fact per action.",
            Own: new[] { "Entity" });

        public static readonly FactKind IntentQueries = new("intent.queries", Services, "queries", EntityPlaceholders,
            "the entity's class name",
            $"Each `Query Entity Action` from {Handlers}: one fact per action. Its multiplicity isn't stated.",
            Own: new[] { "Entity" });

        public static readonly FactKind IntentInvokes = new("intent.invokes", Services, "invokes", InvocationPlaceholders,
            "the invoked element's name",
            $"Each `Perform Invocation` (once called `Call Service Operation`) from {Handlers}, to a Service, Domain Service, Repository or Service Proxy operation, a Command, a Query or a stored procedure: one fact per invocation.",
            Own: new[] { "Invoked", "InvokedOwner" });

        public static readonly FactKind IntentPublishes = new("intent.publishes", Services, "publishes", MessagePlaceholders,
            "the integration event's name",
            $"Each `Publish Integration Event` from {Handlers}: one fact per event.",
            Own: new[] { "Message" });

        public static readonly FactKind IntentSends = new("intent.sends", Services, "sends", MessagePlaceholders,
            "the integration command's name",
            $"Each `Send Integration Command` from {Handlers}: one fact per command.",
            Own: new[] { "Message" });

        public static readonly IReadOnlyList<FactKind> All = new[]
        {
            TableName, TableSchema, TableKind, Key, ColumnName, ColumnType, ColumnTextType, ColumnMaxLength, ColumnRequired,
            ColumnPrecision, ColumnScale, ColumnDefault, RelationshipMultiplicity, RelationshipForeignKey, IndexColumns, IndexUnique,
            IntentRaises,
            ServiceRoute, OperationVerb, OperationRoute, Security, SecurityRoles, SecurityPolicy, ParameterSource,
            IntentCreates, IntentUpdates, IntentDeletes, IntentQueries, IntentInvokes, IntentPublishes, IntentSends
        };

        public static IReadOnlyList<FactKind> For(string designer) => All.Where(x => x.Designer == designer).ToList();

        /// <summary>
        /// The placeholders that name the element a fact is about, as opposed to what it belongs to: a column fact's
        /// attribute rather than its class, an endpoint fact's operation rather than its Service, a relationship's target
        /// and ends rather than its source, an intent fact's target rather than the handler or member it is modelled on.
        /// The kind's other placeholders name the element's owner.
        /// </summary>
        public static IReadOnlyList<string> OwnPlaceholders(FactKind kind)
        {
            if (kind.Own != null)
            {
                return kind.Own;
            }

            foreach (var own in new[] { "Parameter", "Operation", "Attribute", "Index" })
            {
                if (kind.Placeholders.Contains(own))
                {
                    return new[] { own };
                }
            }

            return kind.Placeholders.Contains("SourceEnd")
                ? new[] { "Target", "SourceEnd", "TargetEnd" }
                : kind.Placeholders;
        }

        /// <summary>The placeholders an element's own recipe placeholders are documented with.</summary>
        public static string Describe(string placeholder) => placeholder switch
        {
            "Service" => "the Service's name (empty for a Command or Query), or an event handler's name for its subscription",
            "Operation" => "the operation's name, or the Command's or Query's, or the message or domain event an event handler's subscription handles",
            "Folder" => "the name of the folder the endpoint, Service or handler is in (empty at the package root)",
            "Parameter" => "the parameter's or field's name",
            "Entity" => "the class's name: an entity action's entity, or the class whose constructor or operation raises a domain event",
            "Member" => "the constructor's or operation's name, the class's name for a constructor; for an entity action, the one it targets, else empty",
            "Invoked" => "the invoked operation's, Command's, Query's or stored procedure's name",
            "InvokedOwner" => "the name of the Service, Domain Service, Repository or Service Proxy the invoked operation belongs to (empty for a Command, Query or stored procedure)",
            "Message" => "the integration event's or integration command's name",
            "DomainEvent" => "the domain event's name",
            "Attribute" => "the attribute's name",
            "Source" => "the source class's name",
            "Target" => "the target class's name",
            "SourceEnd" => "the source end's name: the navigation on the target class (may be empty)",
            "TargetEnd" => "the target end's name: the navigation on the source class (may be empty)",
            "Index" => "the index's name: its attributes' `UniqueKey`, or the `Index` element's name",
            _ => throw new InvalidOperationException($"Unknown placeholder {placeholder}")
        };
    }
}
