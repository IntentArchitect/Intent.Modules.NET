using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.UI.Guardrails.Contracts.Drift
{
    /// <summary>
    /// Everything the drift check knows about today's User Interface designer (Intent.Modelers.UI 1.x, with Show Dialog from
    /// Intent.Modelers.UI.Core): how its elements are read into the neutral <see cref="UiModel"/>, where each fact comes
    /// from, and the designer's terms in the drift module's rule and skills. The designer is being overhauled; the next
    /// version gets a reader of its own beside this one, and <see cref="UiDrift"/>, the fact keys and the recipe guides
    /// stay as they are.
    /// </summary>
    /// <remarks>
    /// Elements are read raw, by type id, and stereotypes by name, so the module compiles against no designer API the
    /// overhaul will replace. The ids are those of Intent.Modelers.UI 1.1.6 and Intent.Modelers.UI.Core 1.0.4. Only
    /// Components and Layouts are read, and only their own associations: the Component Operation shares its type id with
    /// the Services designer's Operation.
    /// </remarks>
    internal static class UserInterfaceDesigner1
    {
        public const string DesignerId = "f492faed-0665-4513-9853-5a230721786f";

        private const string ComponentTypeId = "b1c481e1-e91e-4c29-9817-00ab9cad4b6b";
        private const string LayoutTypeId = "776a9393-6b23-4a8c-8937-fd7e833fa0ef";
        private const string DisplayComponentTypeId = "866a90f7-4044-43b9-bb05-7270c7889796";
        private const string CallServiceOperationActionTypeId = "fe5a5cd8-aabd-472f-8d42-f5c233e658dc";
        private const string NavigationTypeId = "6d2b2070-c1cb-4cd2-88b4-4e5f8414bd9e";
        private const string CompositionTypeId = "503b9ea9-4e8b-41d7-bbc6-92c97666c476";
        private const string ShowDialogTypeId = "2a309fb2-c487-4c00-9462-16dac9824731";

        /// <summary>Reads the application's pages, dialogs, components and layouts.</summary>
        public static UiModel Read(IDesigner designer, string applicationId)
        {
            var owners = Elements(designer, applicationId)
                .Where(x => x.SpecializationTypeId is ComponentTypeId or LayoutTypeId)
                .ToList();

            // A component a Show Dialog opens is a dialog, whether or not it has the Dialog stereotype, which models made
            // before Intent.Modelers.UI 1.1.3 don't have.
            var opened = new HashSet<string>(owners.SelectMany(x => Targets(x, ShowDialogTypeId)).Select(x => x.Id), StringComparer.Ordinal);
            return new UiModel { Components = owners.Select(x => Component(x, opened)).ToList() };
        }

        /// <summary>The application's elements in this designer, by id, for the report's links and the run's error.</summary>
        public static IReadOnlyDictionary<string, IElement> ElementsById(IDesigner designer, string applicationId)
        {
            var result = new Dictionary<string, IElement>(StringComparer.Ordinal);
            foreach (var element in Elements(designer, applicationId))
            {
                result.TryAdd(element.Id, element);
            }

            return result;
        }

        private static UiComponent Component(IElement element, IReadOnlySet<string> opened)
        {
            var page = Stereotype(element, "Page");
            var secured = Stereotype(element, "Secured");
            var kind = element.SpecializationTypeId == LayoutTypeId ? UiKind.Layout
                : page != null ? UiKind.Page
                : Stereotype(element, "Dialog") != null || opened.Contains(element.Id) ? UiKind.Dialog
                : UiKind.Component;

            // What the component renders: its Compositions, and the Display Components in its view (or a Layout's header,
            // body, sider or footer), which is how Intent's own samples model it.
            var displayed = Descendants(element.ChildElements)
                .Where(x => x.SpecializationTypeId == DisplayComponentTypeId)
                .Select(x => x.TypeReference?.Element as IElement)
                .Where(x => x?.SpecializationTypeId == ComponentTypeId);

            return new UiComponent
            {
                Id = element.Id,
                Name = element.Name,
                Folder = element.ParentElement is { SpecializationType: "Folder" } folder ? folder.Name : string.Empty,
                Kind = kind,
                Route = page != null ? Route(page) : null,
                Secured = secured != null,
                Roles = Names(secured, "Roles"),
                Policies = Names(secured, "Policy"),
                Calls = Targets(element, CallServiceOperationActionTypeId).Select(x => new UiCall(x.Name, OwnerOf(x))).Distinct().ToList(),
                Navigations = Targets(element, NavigationTypeId).Select(x => new UiLink(x.Name, Stereotype(x, "Page") is { } target ? Route(target) : string.Empty)).Distinct().ToList(),
                Dialogs = Targets(element, ShowDialogTypeId).Select(x => new UiLink(x.Name)).Distinct().ToList(),
                Children = Targets(element, CompositionTypeId).Concat(displayed).Select(x => new UiLink(x.Name)).Distinct().ToList()
            };
        }

        /// <summary>
        /// What the element's associations of this type lead to, wherever in it they are modelled: on the Component or
        /// Layout itself, or on something inside it, such as a Component Operation or a Layout Header. A two-way
        /// Navigation leads both ways. In association order, so the facts are in the same order on every run.
        /// </summary>
        private static IEnumerable<IElement> Targets(IElement element, string associationTypeId)
        {
            return new[] { element }.Concat(Descendants(element.ChildElements))
                .SelectMany(x => x.AssociatedElements ?? Enumerable.Empty<IAssociationEnd>())
                .Where(x => x.Association?.SpecializationTypeId == associationTypeId &&
                            (x.IsTargetEnd() || associationTypeId == NavigationTypeId && x.IsNavigable))
                .OrderBy(x => x.Association.Id, StringComparer.Ordinal)
                .Select(x => x.TypeReference?.Element as IElement)
                .Where(x => x != null);
        }

        /// <summary>The Service a called operation belongs to; empty for a Command or Query.</summary>
        private static string OwnerOf(IElement target)
        {
            return target.SpecializationType == "Operation" && target.ParentElement is { } parent && parent.SpecializationType != "Folder" ? parent.Name : string.Empty;
        }

        private static string Route(IStereotype page) => Property(page, "Route").TrimStart('/');

        /// <summary>A text property's comma-separated names, in model order.</summary>
        private static IReadOnlyList<string> Names(IStereotype stereotype, string property)
        {
            return stereotype == null
                ? Array.Empty<string>()
                : Property(stereotype, property).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.Ordinal).ToList();
        }

        private static IEnumerable<IElement> Elements(IDesigner designer, string applicationId)
        {
            return designer.Packages.Where(x => x.ApplicationId == applicationId).SelectMany(x => Descendants(x.ChildElements));
        }

        private static IEnumerable<IElement> Descendants(IEnumerable<IElement> elements)
        {
            foreach (var element in elements ?? Enumerable.Empty<IElement>())
            {
                yield return element;
                foreach (var descendant in Descendants(element.ChildElements))
                {
                    yield return descendant;
                }
            }
        }

        private static IStereotype Stereotype(IHasStereotypes element, string name)
        {
            return element?.Stereotypes.FirstOrDefault(x => x.Name == name);
        }

        private static string Property(IStereotype stereotype, string name)
        {
            return stereotype != null && stereotype.TryGetProperty(name, out var property) ? property.Value?.Trim() ?? string.Empty : string.Empty;
        }

        /// <summary>Where each kind of fact comes from in this designer, as the configure-drift-check skill lists it.</summary>
        public static class Sources
        {
            public const string Kind = "Every Component: `Page` with the `Page` stereotype, `Dialog` with the `Dialog` stereotype or when a `Show Dialog` opens it, else `Component`. Every Layout, as `Layout`.";
            public const string Route = "A page's `Page` → `Route`, without its leading `/`.";
            public const string Security = "`Secured` on the Component. Stated for every page, and for any other Component with `Secured`.";
            public const string Roles = "`Secured` → `Roles`, where any are modelled.";
            public const string Policy = "`Secured` → `Policy`, where any are modelled.";
            public const string Invokes = "Each `Call Service Operation Action` from the Component or Layout, or from one of its Component Operations, to a Command, Query or Service operation, in this application or one whose Services package it references: one fact per target.";
            public const string Navigates = "Each `Navigation` to a page from the Component or Layout, or from anything in it (a Component Operation, a Layout Header), either way along a two-way one: one fact per page.";
            public const string Opens = "Each `Show Dialog` from the Component, or one of its Component Operations, to a dialog: one fact per dialog.";
            public const string Uses = "Each `Composition` from the Component, and each `Display Component` in its view or in a Layout's header, body, sider or footer: one fact per component rendered.";
        }

        /// <summary>The rule's rows for this designer: what code changes need which model change.</summary>
        public const string RuleRows = """
| A page, dialog, component or layout: added, removed or renamed | That Component (with `Page` or `Dialog`) or Layout in the User Interface designer |
| A page's route, or a page's or component's authorization | Its `Page` stereotype's `Route`, and `Secured` with its roles and policy |
| What a page, dialog, component or layout calls on the backend, the pages it navigates to, the dialogs it opens and the components it renders | Its `Call Service Operation Action`, `Navigation`, `Show Dialog` and `Composition` associations |

""";

        /// <summary>This designer's interaction associations, for the intent drift reference.</summary>
        public const string IntentLinks = "`Call Service Operation Action`, `Navigation`, `Show Dialog` or `Composition`, from a Component or Layout to the Command, Query or Service operation it calls, the page it navigates to, the dialog it opens or the component it renders";

        /// <summary>An interaction the code can gain, which the check never reports.</summary>
        public const string NewIntent = "something a page or component starts doing (a backend call, a navigation, a dialog it opens or a component it renders)";

        /// <summary>What the intent drift reference adds for work that moved to another element.</summary>
        public const string MovedIntent = " If a page now renders the component that does it, add a `Composition` from the page to that component too.";
    }
}
