using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Drift;

namespace Intent.Modules.UI.Guardrails.Contracts.Drift
{
    /// <summary>
    /// The User Interface designer's drift check, as this module contributes it to the drift module (Intent.Common.Drift):
    /// every fact the UI model states, built from the neutral <see cref="UiModel"/>, and the designer's text in the drift
    /// module's rule and skills. The fact keys, placeholders and values are a contract with consumers' committed recipe
    /// files, so they don't depend on the designer's version; what does (where each fact comes from, the designer's terms)
    /// is in <see cref="UserInterfaceDesigner1"/>.
    /// </summary>
    internal static class UiDrift
    {
        public const string Name = "UI";

        private static readonly FactPlaceholder Folder = new("Folder", "the name of the folder the page, dialog, component or layout is in (empty at the package root)");
        private static readonly FactPlaceholder Component = new("Component", "the page's, dialog's, component's or layout's name");
        private static readonly FactPlaceholder Invoked = new("Invoked", "the called Command's, Query's or Service operation's name");
        private static readonly FactPlaceholder InvokedOwner = new("InvokedOwner", "the name of the Service the called operation belongs to (empty for a Command or Query)");
        private static readonly FactPlaceholder Page = new("Page", "the name of the page navigated to");
        private static readonly FactPlaceholder Route = new("Route", "that page's route as modelled, without a leading `/`");
        private static readonly FactPlaceholder RouteStart = new("RouteStart", "that page's route up to its first parameter, without the `/` before it (`customer/edit` for `customer/edit/{id}`), or the whole route when it has none");
        private static readonly FactPlaceholder Dialog = new("Dialog", "the name of the dialog opened");
        private static readonly FactPlaceholder Child = new("Child", "the name of the component rendered");

        private static readonly FactPlaceholder[] ComponentPlaceholders = { Folder, Component };
        private static readonly string[] OwnComponent = { "Component" };

        public static readonly FactKind ComponentKind = new("component.kind", Name, "kind", ComponentPlaceholders,
            "`Page`, `Dialog`, `Component` or `Layout`",
            UserInterfaceDesigner1.Sources.Kind,
            OwnComponent);

        public static readonly FactKind PageRoute = new("page.route", Name, "route", ComponentPlaceholders,
            "the route as modelled, without a leading `/`, or empty",
            UserInterfaceDesigner1.Sources.Route,
            OwnComponent);

        public static readonly FactKind Security = new("security", Name, "security", ComponentPlaceholders,
            "`Secured` or `None`",
            UserInterfaceDesigner1.Sources.Security,
            OwnComponent);

        public static readonly FactKind SecurityRoles = new("security.roles", Name, "roles", ComponentPlaceholders,
            "role names separated by commas, with no spaces, in model order",
            UserInterfaceDesigner1.Sources.Roles,
            OwnComponent);

        public static readonly FactKind SecurityPolicy = new("security.policy", Name, "policy", ComponentPlaceholders,
            "policy names separated by commas, with no spaces, in model order",
            UserInterfaceDesigner1.Sources.Policy,
            OwnComponent);

        public static readonly FactKind IntentInvokes = new("intent.invokes", Name, "invokes", new[] { Folder, Component, Invoked, InvokedOwner },
            "the called element's name",
            UserInterfaceDesigner1.Sources.Invokes,
            new[] { "Invoked", "InvokedOwner" });

        public static readonly FactKind IntentNavigates = new("intent.navigates", Name, "navigates to", new[] { Folder, Component, Page, Route, RouteStart },
            "the page's name",
            UserInterfaceDesigner1.Sources.Navigates,
            new[] { "Page", "Route", "RouteStart" });

        public static readonly FactKind IntentOpens = new("intent.opens", Name, "opens", new[] { Folder, Component, Dialog },
            "the dialog's name",
            UserInterfaceDesigner1.Sources.Opens,
            new[] { "Dialog" });

        public static readonly FactKind IntentUses = new("intent.uses", Name, "uses", new[] { Folder, Component, Child },
            "the rendered component's name",
            UserInterfaceDesigner1.Sources.Uses,
            new[] { "Child" });

        public static readonly IReadOnlyList<FactKind> Kinds = new[]
        {
            ComponentKind, PageRoute, Security, SecurityRoles, SecurityPolicy, IntentInvokes, IntentNavigates, IntentOpens, IntentUses
        };

        public static readonly DriftDesigner Designer = new(
            Name: Name,
            DesignerId: UserInterfaceDesigner1.DesignerId,
            Order: 30,
            Kinds: Kinds,
            ReadFacts: (designer, applicationId) => Facts(UserInterfaceDesigner1.Read(designer, applicationId)),
            ReadElements: UserInterfaceDesigner1.ElementsById,
            ValidateTaskId: "Intent.UI.Guardrails.Contracts.ValidateDriftRecipes",
            Guidance: new DriftGuidance(
                FactsText: "pages, dialogs, components and layouts, each page's route and security, and what each one calls on the backend, navigates to, opens and renders",
                RuleRows: UserInterfaceDesigner1.RuleRows,
                GuideRows: """
| UI | Blazor | `.razor` files with `@page` directives, `@code` blocks or `.razor.cs` code-behind | [references/blazor.md](references/blazor.md) |
| UI | Angular | `@Component` classes in `.component.ts` files, with routes in `app.routes.ts` or a routing module | [references/angular.md](references/angular.md) |
| UI | React | Function components in `.jsx` or `.tsx` files, with React Router routes | [references/react.md](references/react.md) |

""",
                IntentLinks: UserInterfaceDesigner1.IntentLinks,
                NewIntent: UserInterfaceDesigner1.NewIntent,
                MovedIntent: UserInterfaceDesigner1.MovedIntent));

        /// <summary>
        /// Every fact the UI model states. Each one belongs to its page, dialog, component or layout, which the report
        /// names and links; an interaction's placeholders name what it leads to as well.
        /// </summary>
        public static IReadOnlyList<ModelFact> Facts(UiModel model)
        {
            var facts = new List<ModelFact>();
            foreach (var component in model.Components)
            {
                var owner = new Dictionary<string, string> { ["Folder"] = component.Folder, ["Component"] = component.Name };
                facts.Add(Fact(ComponentKind, component, component.Kind, owner));
                if (component.Kind == UiKind.Page)
                {
                    facts.Add(Fact(PageRoute, component, component.Route ?? string.Empty, owner));
                }

                if (component.Kind == UiKind.Page || component.Secured)
                {
                    facts.Add(Fact(Security, component, component.Secured ? "Secured" : "None", owner));
                    if (component.Roles.Count > 0)
                    {
                        facts.Add(Fact(SecurityRoles, component, string.Join(",", component.Roles), owner));
                    }

                    if (component.Policies.Count > 0)
                    {
                        facts.Add(Fact(SecurityPolicy, component, string.Join(",", component.Policies), owner));
                    }
                }

                foreach (var call in component.Calls)
                {
                    facts.Add(Fact(IntentInvokes, component, call.Name, owner, ("Invoked", call.Name), ("InvokedOwner", call.Owner)));
                }

                foreach (var page in component.Navigations)
                {
                    facts.Add(Fact(IntentNavigates, component, page.Name, owner, ("Page", page.Name), ("Route", page.Route ?? string.Empty), ("RouteStart", StartOf(page.Route))));
                }

                foreach (var dialog in component.Dialogs)
                {
                    facts.Add(Fact(IntentOpens, component, dialog.Name, owner, ("Dialog", dialog.Name)));
                }

                foreach (var child in component.Children)
                {
                    facts.Add(Fact(IntentUses, component, child.Name, owner, ("Child", child.Name)));
                }
            }

            return facts;
        }

        /// <summary>
        /// A route up to its first parameter (a segment with <c>{</c>, or starting <c>:</c> or <c>*</c>), without the
        /// <c>/</c> before it, so a recipe can match a navigation that builds the rest: <c>customer/edit</c> for
        /// <c>customer/edit/{id}</c>.
        /// </summary>
        internal static string StartOf(string route)
        {
            var segments = (route ?? string.Empty).Split('/');
            return string.Join("/", segments.TakeWhile(x => !x.Contains('{') && !x.StartsWith(':') && !x.StartsWith('*')));
        }

        private static ModelFact Fact(FactKind kind, UiComponent component, string value, IReadOnlyDictionary<string, string> owner, params (string Name, string Value)[] placeholders)
        {
            var values = new Dictionary<string, string>(owner);
            foreach (var (name, placeholderValue) in placeholders)
            {
                values[name] = placeholderValue ?? string.Empty;
            }

            return new ModelFact { Kind = kind, Element = component.Name, ElementIds = new[] { component.Id }, Value = value?.Trim() ?? string.Empty, Placeholders = values };
        }
    }
}
