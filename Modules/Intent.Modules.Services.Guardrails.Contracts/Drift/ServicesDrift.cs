using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Drift;

namespace Intent.Modules.Services.Guardrails.Contracts.Drift
{
    /// <summary>
    /// The Services designer's drift check, as this module contributes it to the drift module (Intent.Common.Drift): every
    /// fact the Services model states, in its own vocabulary, how they are read (<see cref="DriftModel"/>), and the Services
    /// designer's text in the drift module's rule and skills. This is the model side of the check, fixed by the module;
    /// recipes supply the code side.
    /// </summary>
    internal static class ServicesDrift
    {
        public const string Name = "Services";

        private const string Handlers = "a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event";

        private static readonly FactPlaceholder Service = new("Service", "the Service's name (empty for a Command or Query), or an event handler's name for its subscription");
        private static readonly FactPlaceholder Operation = new("Operation", "the operation's name, or the Command's or Query's, or the message or domain event an event handler's subscription handles");
        private static readonly FactPlaceholder Folder = new("Folder", "the name of the folder the endpoint, Service or handler is in (empty at the package root)");
        private static readonly FactPlaceholder Parameter = new("Parameter", "the parameter's or field's name");

        // The Entity and Member descriptions are the Domain contribution's too: the configure-drift-check skill used to
        // describe a placeholder once for both designers, and its text is kept as it was.
        private static readonly FactPlaceholder Entity = new("Entity", "the class's name: an entity action's entity, or the class whose constructor or operation raises a domain event");
        private static readonly FactPlaceholder Member = new("Member", "the constructor's or operation's name, the class's name for a constructor; for an entity action, the one it targets, else empty");
        private static readonly FactPlaceholder Invoked = new("Invoked", "the invoked operation's, Command's, Query's or stored procedure's name");
        private static readonly FactPlaceholder InvokedOwner = new("InvokedOwner", "the name of the Service, Domain Service, Repository or Service Proxy the invoked operation belongs to (empty for a Command, Query or stored procedure)");
        private static readonly FactPlaceholder Message = new("Message", "the integration event's or integration command's name");

        private static readonly FactPlaceholder[] EndpointPlaceholders = { Service, Operation, Folder };
        private static readonly FactPlaceholder[] ParameterPlaceholders = { Service, Operation, Folder, Parameter };
        private static readonly FactPlaceholder[] EntityActionPlaceholders = { Service, Operation, Folder, Entity, Member };
        private static readonly FactPlaceholder[] EntityPlaceholders = { Service, Operation, Folder, Entity };
        private static readonly FactPlaceholder[] InvocationPlaceholders = { Service, Operation, Folder, Invoked, InvokedOwner };
        private static readonly FactPlaceholder[] MessagePlaceholders = { Service, Operation, Folder, Message };

        private static readonly string[] OwnEndpoint = { "Operation" };

        public static readonly FactKind ServiceRoute = new("service.route", Name, "route", new[] { Service },
            "the route exactly as modelled, or empty",
            "A Service's `Http Service Settings` → `Route`, for every Service with that stereotype.",
            new[] { "Service" });

        public static readonly FactKind OperationVerb = new("operation.verb", Name, "verb", EndpointPlaceholders,
            "`GET`, `POST`, `PUT`, `PATCH` or `DELETE`",
            "An endpoint's `Http Settings` → `Verb`. An endpoint is a Service operation, Command or Query with `Http Settings`.",
            OwnEndpoint);

        public static readonly FactKind OperationRoute = new("operation.route", Name, "route", EndpointPlaceholders,
            "the route exactly as modelled, or empty",
            "An endpoint's `Http Settings` → `Route`. A Service operation's route is relative to its Service's route, unless it starts with `~/` or `/`.",
            OwnEndpoint);

        public static readonly FactKind Security = new("security", Name, "security", EndpointPlaceholders,
            "`Secured`, `Unsecured` or `None`",
            "An endpoint's effective security: `Unsecured` or `Secured` on the element itself, else on its Service, its folders or its package, else `None`. Stated for every endpoint once the designer uses `Secured` or `Unsecured` anywhere.",
            OwnEndpoint);

        public static readonly FactKind SecurityRoles = new("security.roles", Name, "roles", EndpointPlaceholders,
            "role names separated by commas, with no spaces, in model order",
            "`Secured` → `Roles` and `Security Roles`, from the stereotype that makes the endpoint secured, where any are modelled.",
            OwnEndpoint);

        public static readonly FactKind SecurityPolicy = new("security.policy", Name, "policy", EndpointPlaceholders,
            "policy names separated by commas, with no spaces, in model order",
            "`Secured` → `Policy` and `Security Policies`, from the stereotype that makes the endpoint secured, where any are modelled.",
            OwnEndpoint);

        public static readonly FactKind ParameterSource = new("parameter.source", Name, "binding", ParameterPlaceholders,
            "`Default`, `From Body`, `From Form`, `From Header`, `From Query` or `From Route`",
            "`Parameter Settings` → `Source` on an endpoint's parameter, or on a field of a Command or Query endpoint.",
            new[] { "Parameter" });

        public static readonly FactKind IntentCreates = new("intent.creates", Name, "creates", EntityActionPlaceholders,
            "the entity's class name",
            $"Each `Create Entity Action` from {Handlers}: one fact per action. `{{Member}}` is the constructor or factory operation it targets, if any.",
            new[] { "Entity", "Member" });

        public static readonly FactKind IntentUpdates = new("intent.updates", Name, "updates", EntityActionPlaceholders,
            "the entity's class name",
            $"Each `Update Entity Action` from {Handlers}: one fact per action. `{{Member}}` is the operation it targets, if any.",
            new[] { "Entity", "Member" });

        public static readonly FactKind IntentDeletes = new("intent.deletes", Name, "deletes", EntityPlaceholders,
            "the entity's class name",
            $"Each `Delete Entity Action` from {Handlers}: one fact per action.",
            new[] { "Entity" });

        public static readonly FactKind IntentQueries = new("intent.queries", Name, "queries", EntityPlaceholders,
            "the entity's class name",
            $"Each `Query Entity Action` from {Handlers}: one fact per action. Its multiplicity isn't stated.",
            new[] { "Entity" });

        public static readonly FactKind IntentInvokes = new("intent.invokes", Name, "invokes", InvocationPlaceholders,
            "the invoked element's name",
            $"Each `Perform Invocation` (once called `Call Service Operation`) from {Handlers}, to a Service, Domain Service, Repository or Service Proxy operation, a Command, a Query or a stored procedure: one fact per invocation.",
            new[] { "Invoked", "InvokedOwner" });

        public static readonly FactKind IntentPublishes = new("intent.publishes", Name, "publishes", MessagePlaceholders,
            "the integration event's name",
            $"Each `Publish Integration Event` from {Handlers}: one fact per event.",
            new[] { "Message" });

        public static readonly FactKind IntentSends = new("intent.sends", Name, "sends", MessagePlaceholders,
            "the integration command's name",
            $"Each `Send Integration Command` from {Handlers}: one fact per command.",
            new[] { "Message" });

        public static readonly IReadOnlyList<FactKind> Kinds = new[]
        {
            ServiceRoute, OperationVerb, OperationRoute, Security, SecurityRoles, SecurityPolicy, ParameterSource,
            IntentCreates, IntentUpdates, IntentDeletes, IntentQueries, IntentInvokes, IntentPublishes, IntentSends
        };

        public static readonly DriftDesigner Designer = new(
            Name: Name,
            DesignerId: ApiMetadataDesignerExtensions.ServicesDesignerId,
            Order: 20,
            Kinds: Kinds,
            ReadFacts: DriftModel.Read,
            ReadElements: DriftModel.ElementsById,
            ValidateTaskId: "Intent.Services.Guardrails.Contracts.ValidateDriftRecipes",
            Guidance: new DriftGuidance(
                FactsText: "HTTP routes, verbs and parameter binding, security, and what each handler does: the entities it acts on, what it invokes, and the integration events and commands it publishes and sends",
                RuleRows: """
| A command, query, DTO, service, enum, message, handler or API client: the type, a field, a method or its signature | That element in the Services designer |
| An endpoint's route, verb or parameter binding, or its authorization | Its `Http Settings` and `Parameter Settings`, and `Secured` or `Unsecured` |
| What a command, query, service operation or event handler does: the entities it creates, updates, deletes or reads, what it calls, the events it publishes and the commands it sends | Its entity actions, `Perform Invocation`, `Publish Integration Event` and `Send Integration Command` |

""",
                GuideRows: """
| Services | ASP.NET Core controllers | Classes deriving from `ControllerBase` or `Controller`, with `[ApiController]` or `[HttpGet]` | [references/aspnetcore-controllers.md](references/aspnetcore-controllers.md) |
| Services | Minimal APIs | `app.MapGet(…)`, `MapPost(…)` or `MapGroup(…)` | [references/minimal-apis.md](references/minimal-apis.md) |
| Services | FastEndpoints | A `FastEndpoints` package: classes deriving from `Endpoint<TRequest>` | [references/fastendpoints.md](references/fastendpoints.md) |
| Services | Handler intent | Entity actions, invocations, published integration events or sent integration commands in the model, on MediatR handlers, service classes or event handlers | [references/handler-intent.md](references/handler-intent.md) |

""",
                IntentLinks: "an entity action (`Create`, `Update`, `Delete` or `Query Entity Action`), `Perform Invocation`, `Publish Integration Event` or `Send Integration Command`, from a Command, Query, Service operation or event handler to the entity it acts on, what it calls, or the event or command it publishes or sends",
                NewIntent: "something a handler starts doing (an entity it acts on, a call, an event it publishes or a command it sends)",
                MovedIntent: " If this element now calls that one (sends its Command, calls its Service operation), add a `Perform Invocation` to it too."));
    }
}
