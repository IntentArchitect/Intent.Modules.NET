using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Blazor;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.Ui;
using Intent.Modules.Common.DriftEngine.Stacks.Xaml;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.Maui;

/// <summary>
/// Reads the User Interface designer's claims (the <c>intent.ui</c> pack's) from .NET MAUI's views, as <c>intent.blazor</c>
/// reads them from Razor components: what a view is (a layout for a Shell or a page host, a page, a dialog, a component), the
/// Shell routes and registered routes a page is shown at, its title, its bindable and query properties, the Shell's items as
/// its Regions and the pages they show with their tab entries, and what a view's markup composes, what it uses (its binding
/// context first), where it and its binding context navigate, and whether a Store saves its state on the device. The XAML is
/// the <c>intent.xaml</c> language's; MAUI's meaning of it is read through <see cref="MauiSite"/>. It answers only for a view
/// of a MAUI app, so Blazor's, MVC's and the TypeScript front ends' readers keep theirs.
/// </summary>
public sealed class MauiExtension : IDriftExtension
{
    public string Id => "intent.maui";

    public IEnumerable<ICodeReader> Readers { get; } = [new ViewMembers()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new Views(), new ViewProperties(), new ShellRegions(), new ViewLinks(), new PersistedStores()];

    /// <summary>
    /// What a view's members are to the designer, so <c>unmodelled.member</c> asks for none it can't state: the static
    /// <c>BindableProperty</c> behind a property is that property's, which is what a Property states; a page's or layout's
    /// property is written in code unless a <c>[QueryProperty]</c> binds it from the navigation's query (the designer models a
    /// Page's Properties only for its route and query parameters, and Shell has no route parameters); and a component's or
    /// dialog's is written in code unless a <c>BindableProperty</c> backs it, as only then can a parent supply it.
    /// </summary>
    private sealed class ViewMembers : ICodeReader
    {
        public void Read(CodeGraph graph)
        {
            var site = MauiSite.Of(graph);
            if (!site.HasApp)
            {
                return;
            }

            foreach (var type in graph.Types.Where(site.IsView).ToList())
            {
                var kind = site.KindOf(type)!.Kind;
                foreach (var member in type.Members.Where(x => x.Kind is "property" or "field"))
                {
                    if (member.IsStatic && IsBindableProperty(member))
                    {
                        graph.Unmodellable[member] = "a BindableProperty backs the property it is named for, which is what the designer states";
                    }
                    else if (member is { IsStatic: false, Kind: "property" } && kind is "Page" or "Layout" && QueryProperty(site, type, member.Name) is null)
                    {
                        graph.Unmodellable[member] = "a MAUI page's Properties are its query parameters, which a [QueryProperty] binds; other state is written in code";
                    }
                    else if (member is { IsStatic: false, Kind: "property" } && kind is "Component" or "Dialog" && Backing(site.Graph, type, member.Name) is null)
                    {
                        graph.Unmodellable[member] = "no BindableProperty backs it, so a parent can't supply it";
                    }
                }
            }
        }
    }

    /// <summary>A static field or property holding a <c>BindableProperty</c>.</summary>
    internal static bool IsBindableProperty(MemberNode member) =>
        member.Type?.Name is "BindableProperty" || (member.Value?.Text ?? "").StartsWith("BindableProperty.Create", StringComparison.Ordinal);

    /// <summary>
    /// What backs a bindable property: the static <c>{Name}Property</c> <c>BindableProperty</c> MAUI binds it through, or the
    /// property itself where CommunityToolkit.Maui's <c>[BindableProperty]</c> generates one.
    /// </summary>
    internal static MemberNode? Backing(CodeGraph graph, TypeNode type, string name) =>
        graph.MembersOf(type).FirstOrDefault(x => x.IsStatic && x.Name == name + "Property" && IsBindableProperty(x))
        ?? graph.MembersOf(type).FirstOrDefault(x => x.Name == name && x.Attributes.Any(a => a.Name is "BindableProperty" or "BindablePropertyAttribute"));

    /// <summary>The <c>[QueryProperty(nameof(X), "x")]</c> naming a property, on the page or its binding context, which Shell sets too.</summary>
    internal static AttributeNode? QueryProperty(MauiSite site, TypeNode page, string name) =>
        new[] { page }.Concat(site.BindingContexts(page).Select(x => x.Type).OfType<TypeNode>())
            .SelectMany(x => new[] { x }.Concat(site.Graph.Bases(x)))
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(x => x.Name is "QueryProperty" or "QueryPropertyAttribute" && x.Arguments.Count > 0 && Named(x.Arguments[0]) == name);

    /// <summary>A name an attribute's argument gives: <c>nameof(Product)</c> or <c>"Product"</c>.</summary>
    private static string Named(string argument)
    {
        var text = argument.Trim();
        return text.StartsWith("nameof(", StringComparison.Ordinal) && text.EndsWith(')') ? text["nameof(".Length..^1].Split('.')[^1].Trim() : text.Trim('"');
    }
}

/// <summary>What a view is, a page's routes and title, and its security.</summary>
internal sealed class Views : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["component.kind", "page.route", "page.title", "security", "security.roles", "security.policy"];

    public ClaimReading? Read(ClaimContext context)
    {
        var type = context.TypeOf(context.Element) ?? context.Type;
        var site = MauiSite.Of(context.Code);
        if (site.KindOf(type) is not { } kind)
        {
            return null;
        }

        var document = site.DocumentOf(type);
        var at = site.At(type!);
        var name = context.Element.Name;
        switch (context.Kind)
        {
            case "component.kind":
            {
                if (context.Expected == "Dialog" && kind.Kind == "Page")
                {
                    return ClaimReading.CantVerify("noRule", "A MAUI page's own code doesn't say whether it is shown as a dialog (pushed modally).", at);
                }

                var derives = kind.Declaring == type ? $"`{type!.Name}` derives from `{kind.Base}`" : $"`{type!.Name}` derives from `{kind.Declaring.Name}`, a `{kind.Base}`";
                return ClaimReading.Compared(kind.Kind, at, context.Expected == kind.Kind,
                    $"`{name}` component kind: model `{context.Expected}`, code `{kind.Kind}`. {derives}{(document is null ? "" : $" (`{document.Path}`)")}. Model it as a {kind.Kind}.");
            }
            case "page.route":
            {
                var routes = site.PageRoutes(type!);
                return routes.Count == 0 ? ClaimReading.Absent(at) : ClaimReading.Stated(string.Join(", ", routes.Select(x => x.Route).Distinct()), routes[0].At);
            }
            case "page.title":
                return Title(context, site, type!, document, at);
            case "security":
                return ClaimReading.Compared("None", at, context.Expected == "None",
                    $"`{name}` security: model `{context.Expected}`, code `None`. A MAUI view carries no authorization of its own: the app decides what to show, and the API it calls secures its operations. Leave the element's security unset.");
            default:
                return ClaimReading.Absent(at);
        }
    }

    /// <summary>A page's title: its XAML root's <c>Title</c>, else a <c>Title = ...</c> its constructor sets.</summary>
    private static ClaimReading Title(ClaimContext context, MauiSite site, TypeNode type, Xaml.XamlDocument? document, Span at)
    {
        if (document?.Root.Attributes().FirstOrDefault(x => x.Name.NamespaceName.Length == 0 && x.Name.LocalName == "Title") is { } attribute)
        {
            return Xaml.XamlMarkupExtension.Parse(attribute.Value) is null
                ? ClaimReading.Stated(Xaml.XamlMarkupExtension.Unescaped(attribute.Value), document.At(attribute))
                : ClaimReading.CantVerify("unreadable", "The page's Title is bound or computed.", document.At(attribute));
        }

        var assignment = type.Members.Where(x => x.Kind == "constructor").SelectMany(x => x.Body?.Assignments ?? []).FirstOrDefault(x => x.Target is "Title" or "this.Title");
        if (assignment is null)
        {
            return ClaimReading.Absent(at);
        }

        return site.Graph.StringOf(assignment.Value) is { Kind: "string", Value: { } title }
            ? ClaimReading.Stated(title, assignment.At)
            : ClaimReading.CantVerify("unreadable", "The page's Title is computed.", assignment.At);
    }
}

/// <summary>Whether a view's Property is bindable, a query parameter or a route parameter.</summary>
internal sealed class ViewProperties : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["property.bindable", "property.query", "property.route"];

    public ClaimReading? Read(ClaimContext context)
    {
        var site = MauiSite.Of(context.Code);
        if (context.Element.ParentElement is not { } parent || context.TypeOf(parent) is not { } type || !site.IsView(type))
        {
            return null;
        }

        var name = context.Element.Name;
        var at = context.Member?.At ?? site.At(type);
        switch (context.Kind)
        {
            case "property.bindable":
            {
                var backing = MauiExtension.Backing(site.Graph, type, name);
                var bindable = backing is null ? "false" : "true";
                return ClaimReading.Compared(bindable, backing?.At ?? at, context.Expected == bindable,
                    backing is null
                        ? $"`{parent.Name}.{name}` bindable: model `{context.Expected}`, code `false`. No `{name}Property` BindableProperty backs it, so a parent can't bind it. Remove [Bindable], or back it with a BindableProperty."
                        : $"`{parent.Name}.{name}` bindable: model `{context.Expected}`, code `true`. `{backing.Name}` is a BindableProperty, so a parent binds it. Mark it [Bindable].");
            }
            case "property.query":
            {
                if (MauiExtension.QueryProperty(site, type, name) is { } query)
                {
                    return ClaimReading.Compared("true", query.At, context.Expected == "true",
                        $"`{parent.Name}.{name}` query parameter: model `{context.Expected}`, code `true`. `{type.Name}` binds it from the navigation's query with [QueryProperty]. Mark it [Query Parameter].");
                }

                // IQueryAttributable takes the whole query at once, so which property it sets isn't the code's to say.
                if (context.Expected == "true" && new[] { type }.Concat(site.BindingContexts(type).Select(x => x.Type).OfType<TypeNode>())
                        .Any(x => context.Code.Supertypes(x).Any(s => s is "IQueryAttributable" || s.EndsWith(".IQueryAttributable", StringComparison.Ordinal))))
                {
                    return ClaimReading.CantVerify("unreadable", $"`{type.Name}` takes its query in ApplyQueryAttributes, which sets what it likes.", at);
                }

                return ClaimReading.Compared("false", at, context.Expected == "false",
                    $"`{parent.Name}.{name}` query parameter: model `{context.Expected}`, code `false`. No [QueryProperty] on `{type.Name}` or its binding context names it. Remove [Query Parameter], or bind it with [QueryProperty].");
            }
            default:
                return ClaimReading.Compared("false", at, context.Expected == "false",
                    $"`{parent.Name}.{name}` route parameter: model `{context.Expected}`, code `false`. MAUI Shell has no route parameters: a page receives navigation values as query parameters. Remove [Route Parameter].");
        }
    }
}

/// <summary>
/// <c>region.outlet</c> for a Shell: a Region of a Shell is the Shell item it names, by the item's Route, or, for an item with
/// no Route, its only content's (<c>//Login</c> reaches a <c>FlyoutItem</c> with no Route that way). An item no Region names,
/// when only one is, is what the Region was renamed from.
/// </summary>
internal sealed class ShellRegions : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["region.outlet"];

    public ClaimReading? Read(ClaimContext context)
    {
        var site = MauiSite.Of(context.Code);
        if (context.Element.ParentElement is not { } parent || context.TypeOf(parent) is not { } shell || site.KindOf(shell)?.Base != "Shell")
        {
            return null;
        }

        var name = context.Element.Name;
        var (item, named) = ItemOf(context, site, shell, context.Element);
        if (named)
        {
            return ClaimReading.Stated(name, item!.At);
        }

        var items = site.ItemsOf(shell);
        var actual = item?.Name ?? "none";
        var has = items.Count == 0 ? "has no Shell items" : $"has the items {string.Join(", ", items.Select(x => $"`{x.Name ?? x.Tag}`"))}";
        return ClaimReading.Compared(actual, item?.At ?? site.At(shell), false,
            $"`{parent.Name}` region `{name}`: model `{name}`, code {(actual == "none" ? "none" : $"`{actual}`")}. `{parent.Name}` {has}, and none is named `{name}` (by its Route, or its only content's). The model states it in the Region's name. Rename the Region, or the Shell item's Route.");
    }

    /// <summary>
    /// The Shell item a Region is: the one it names, else the only item no Region of the Shell names, which the Region was
    /// renamed from (<paramref name="Named"/> false), so a renamed item is one finding, the Region's, and the pages it shows are
    /// still read through it.
    /// </summary>
    internal static (ShellItem? Item, bool Named) ItemOf(ClaimContext context, MauiSite site, TypeNode shell, IElement region)
    {
        if (site.ItemNamed(shell, region.Name) is { } item)
        {
            return (item, true);
        }

        var regions = (region.ParentElement?.ChildElements ?? []).Where(x => x == region || context.ClaimOn(x, "region.outlet") is not null)
            .Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
        return (site.ItemsOf(shell).Where(x => x.Name is null || !regions.Contains(x.Name)).ToList() is [var spare] ? spare : null, false);
    }
}

/// <summary>What a view composes, uses and navigates to, and how a Shell presents the pages its Regions show.</summary>
internal sealed class ViewLinks : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.uses", "intent.target", "intent.navigates", "entry.label", "entry.icon", "entry.key", "entry.shortcut"];

    public ClaimReading? Read(ClaimContext context)
    {
        var site = MauiSite.Of(context.Code);
        if (context.Element is not IAssociationEnd end || end.TypeReference?.Element is not IElement target || context.Owner(end) is not { } source
            || BlazorExtension.CodeOf(context, source) is not { } located || site.KindOf(located.Type) is not { } kind)
        {
            return null;
        }

        var (owner, type) = located;
        return end.Association?.SpecializationType switch
        {
            "Composition" when source != owner && context.ClaimOn(source, "region.outlet") is not null =>
                kind.Base == "Shell" ? Shown(context, site, source, owner, type, target) : null,
            "Composition" when context.Kind is "intent.uses" or "intent.target" => Composes(context, site, owner, type, target),
            "Uses" when context.Kind == "intent.uses" => Uses(context, site, owner, type, target),
            "Navigation" when context.Kind == "intent.navigates" => Navigates(context, site, owner, type, target),
            _ => null
        };
    }

    /// <summary>
    /// A Region's Composition of a page: a <c>ShellContent</c> of the Shell item the Region names showing it, and the entry
    /// (tab or flyout item) presenting it: Label is its Title, Icon its icon (a resource's key), Key its Route, each its own or
    /// that of the tab or item holding it alone. MAUI Shell has no keyboard shortcut for an entry.
    /// </summary>
    private static ClaimReading? Shown(ClaimContext context, MauiSite site, IElement region, IElement shellElement, TypeNode shell, IElement target)
    {
        if (context.TypeOf(target) is not { } page)
        {
            return context.Kind is "intent.uses" ? ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located.") : null;
        }

        var (item, _) = ShellRegions.ItemOf(context, site, shell, region);
        var content = item?.Contents.FirstOrDefault(x => x.Type == page);
        switch (context.Kind)
        {
            case "intent.uses":
            {
                if (content is not null)
                {
                    return ClaimReading.Found(content, content.At);
                }

                var shown = item?.Contents.Select(x => x.Content?.Name).OfType<string>().ToList() ?? [];
                return ClaimReading.Compared(shown.Count == 0 ? "none" : string.Join(", ", shown), item?.At ?? site.At(shell), false,
                    $"`{region.Name}` shows `{target.Name}`: in the model, not in the code. {(item is null ? $"`{shellElement.Name}` has no Shell item named `{region.Name}`" : $"The Shell item `{region.Name}` shows {(shown.Count == 0 ? "no page" : string.Join(", ", shown.Select(x => $"`{x}`")))}")}. The model states it in the Region's Composition. Add a `ShellContent` for the page, or remove the Composition.");
            }
            case "intent.target":
                return content is null ? null : ClaimReading.Compared(MauiSite.Qualified(page), content.At, true);
            case "entry.label":
                return content is null ? null : Entry(context, region, target, content, content.Title, "Label", "Title");
            case "entry.icon":
                return content is null ? null : Entry(context, region, target, content, content.Icon, "Icon", "Icon");
            case "entry.key":
                return content is null ? null : Entry(context, region, target, content, content.Key, "Key", "Route");
            default:
                return content is null ? null : Entry(context, region, target, content, null, "Shortcut", null);
        }
    }

    /// <summary>
    /// An Entry field against the Shell's: the model's, when it sets one; for a probe (none set), what the code states that the
    /// designer would have the Entry state. A label that is the page's own name needs no Entry ("whose label differs from its name").
    /// </summary>
    private static ClaimReading Entry(ClaimContext context, IElement region, IElement target, ShellContent content, string? value, string property, string? attribute)
    {
        if (string.IsNullOrEmpty(context.Expected))
        {
            return value is null || property == "Label" && string.Equals(value, target.Name, StringComparison.OrdinalIgnoreCase)
                ? ClaimReading.Absent(content.At)
                : ClaimReading.Stated(value, content.At);
        }

        var states = attribute is null
            ? "MAUI Shell has no keyboard shortcut for a tab or flyout item"
            : value is null ? $"The `ShellContent` showing it has no `{attribute}`" : $"The `ShellContent` showing it has `{attribute}=\"{value}\"`";
        return ClaimReading.Compared(value ?? "none", content.At, value == context.Expected,
            $"`{region.Name}`'s entry for `{target.Name}` {property}: model `{context.Expected}`, code {(value is null ? "none" : $"`{value}`")}. {states}. The model states it in the Composition's Entry {property}.");
    }

    /// <summary>
    /// A view's Composition of a component: its XAML creates one (<c>&lt;templates:ProductTemplate /&gt;</c>), a data template
    /// of it makes one (<c>{DataTemplate views:X}</c>), or it names it (<c>{x:Type local:Routes}</c>, a Blazor hybrid app's root).
    /// </summary>
    private static ClaimReading? Composes(ClaimContext context, MauiSite site, IElement owner, TypeNode type, IElement target)
    {
        var targetType = context.TypeOf(target);
        var uses = site.MarkupUses(type);
        var hit = uses.FirstOrDefault(x => targetType is not null ? x.Type == targetType : x.Name.Name == target.Name);
        if (hit is not null)
        {
            return context.Kind == "intent.target"
                ? ClaimReading.Compared(targetType is null ? target.Name : MauiSite.Qualified(targetType), hit.At, true)
                : ClaimReading.Found(hit, hit.At);
        }

        // What the target is waits on whether it is composed, which says so; a target nothing located is its own finding.
        if (context.Kind == "intent.target")
        {
            return null;
        }

        if (targetType is null)
        {
            return ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located.");
        }

        var document = site.DocumentOf(type);
        var own = uses.Where(x => x.Type is not null).Select(x => x.Name.Name).Distinct().ToList();
        return ClaimReading.Compared(own.Count == 0 ? "none" : string.Join(", ", own), site.At(type), false,
            $"`{owner.Name}` uses `{target.Name}`: in the model, not in the code. {(document is null ? $"`{type.Name}` has no XAML" : $"`{document.Path}` uses {(own.Count == 0 ? "none of the app's views" : string.Join(", ", own.Select(x => $"`{x}`")))}")}, and no element, `DataTemplate` or `x:Type` of it is a `{target.Name}`. The model states it in its Composition. Place the component in the view's XAML, or remove the Composition.");
    }

    /// <summary>
    /// A view's Uses: its binding context is one (a view model, in MVVM), its XAML creates one (a behaviour it attaches), or it
    /// holds or creates one as any class's dependency is read (<see cref="Dependencies"/>).
    /// </summary>
    private static ClaimReading Uses(ClaimContext context, MauiSite site, IElement owner, TypeNode type, IElement target)
    {
        if (context.TypeOf(target) is not { } used)
        {
            return ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located.");
        }

        var contexts = site.BindingContexts(type);
        if (contexts.FirstOrDefault(x => x.Type is { } bound && (bound == used || site.Graph.Bases(used).Contains(bound))) is { } binding)
        {
            return ClaimReading.Found(binding, binding.At);
        }

        if (site.MarkupUses(type).FirstOrDefault(x => x.Instance && x.Type == used) is { } created)
        {
            return ClaimReading.Found(created, created.At);
        }

        if (Dependencies.Find(site.Graph, type, used) is { } dependency)
        {
            return ClaimReading.Found(dependency.Node, dependency.At);
        }

        var bound = contexts.Count == 0 ? "is never set" : $"is {string.Join(", ", contexts.Select(x => $"`{x.Name}`"))}";
        return ClaimReading.Compared(contexts.Count == 0 ? "none" : string.Join(", ", contexts.Select(x => x.Name)), contexts.Count == 0 ? site.At(type) : contexts[0].At, false,
            $"`{owner.Name}` uses `{target.Name}`: in the model, not in the code. `{type.Name}`'s binding context {bound}, and no field, property or constructor parameter of it is a `{used.Name}` or an interface of it, and it creates none. The model states it in its Uses. Set the view's BindingContext to a `{used.Name}`, give it the dependency, or remove the Uses.");
    }

    /// <summary>
    /// A Navigation: a navigation in the view's code or its binding context's (its view model navigates, in MVVM) reaching the
    /// page; from a Shell, also a Shell content showing it, the menu entry a Layout's navigation is.
    /// </summary>
    private static ClaimReading Navigates(ClaimContext context, MauiSite site, IElement owner, TypeNode type, IElement target)
    {
        if (context.TypeOf(target) is not { } page)
        {
            return ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located.");
        }

        if (site.KindOf(type)?.Base == "Shell" && site.ItemsOf(type).SelectMany(x => x.Contents).FirstOrDefault(x => x.Type == page) is { } entry)
        {
            return ClaimReading.Found(entry, entry.At);
        }

        var holders = new List<TypeNode> { type };
        holders.AddRange(site.BindingContexts(type).Select(x => x.Type).OfType<TypeNode>().Where(x => !holders.Contains(x)));
        var navigations = holders.SelectMany(site.NavigationsFrom).ToList();
        if (navigations.FirstOrDefault(x => x.Target == page) is { } hit)
        {
            return ClaimReading.Found(hit, hit.At);
        }

        if (navigations.FirstOrDefault(x => x is { Route: null, Pushed: false }) is { } computed)
        {
            return ClaimReading.CantVerify("unreadable", "A navigation's route is computed.", computed.At);
        }

        var routes = site.PageRoutes(page).Select(x => $"`{x.Route}`").Distinct().ToList();
        var through = holders.Count == 1 ? $"`{type.Name}` navigates" : $"`{type.Name}` and its binding context {string.Join(", ", holders.Skip(1).Select(x => $"`{x.Name}`"))} navigate";
        return ClaimReading.Compared(navigations.Count == 0 ? "none" : string.Join(", ", navigations.Select(x => x.Route ?? x.Written)), navigations.Count == 0 ? site.At(type) : navigations[0].At, false,
            $"`{owner.Name}` navigates to `{target.Name}`: in the model, not in the code. {(routes.Count == 0 ? $"No Shell route shows `{target.Name}`" : $"`{target.Name}` is at {string.Join(", ", routes)}")}, and {through} to {(navigations.Count == 0 ? "nothing" : string.Join(", ", navigations.Select(x => $"`{x.Written}`").Distinct()))}. The model states it in its Navigation. Navigate to the page's route, or remove the Navigation.");
    }
}

/// <summary>
/// <c>store.persisted</c>: a Store's state is saved and restored across restarts when its class reads or writes MAUI's device
/// storage: <c>Preferences</c> or <c>SecureStorage</c> (their <c>Default</c> instances too), or an <c>IPreferences</c> or
/// <c>ISecureStorage</c> it is given, anywhere in its code, its property accessors included. It answers only for a Store of a
/// MAUI app; another app saves its state in other ways, which its own stack reads.
/// </summary>
internal sealed class PersistedStores : IClaimReader
{
    private static readonly HashSet<string> Storage = new(StringComparer.Ordinal) { "Preferences", "SecureStorage" };

    public IReadOnlyCollection<string> Kinds { get; } = ["store.persisted"];

    public ClaimReading? Read(ClaimContext context)
    {
        var type = context.TypeOf(context.Element) ?? context.Type;
        var site = MauiSite.Of(context.Code);
        if (type is null || !site.InApp(type) || CSharpSyntax.Of(context.Code) is not { } csharp)
        {
            return null;
        }

        var given = context.Code.MembersOf(type).Where(x => x.Kind is "field" or "property").Select(x => (x.Name, x.Type))
            .Concat(type.Members.Where(x => x.Kind == "constructor").SelectMany(x => x.Parameters).Select(x => (x.Name, x.Type)))
            .Where(x => x.Type?.Name is "IPreferences" or "ISecureStorage")
            .Select(x => x.Name)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var file in type.Files.Distinct())
        {
            if (!csharp.Trees.TryGetValue(file, out var tree))
            {
                continue;
            }

            foreach (var declaration in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().Where(x => x.Identifier.Text == type.Name))
            {
                foreach (var access in declaration.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
                {
                    if (Accesses(context.Code, type, access.Expression, given))
                    {
                        return ClaimReading.Stated("true", CSharpSyntax.SpanOf(access, file));
                    }
                }
            }
        }

        return context.Expected == "true"
            ? ClaimReading.Compared("false", type.At, false,
                $"`{context.Element.Name}` persisted: model `true`, code `false`. `{type.Name}` reads and writes neither `Preferences` nor `SecureStorage`, so its state isn't kept across restarts. The model states it in the store's Persisted stereotype. Save its state on the device, or remove Persisted.")
            : ClaimReading.Absent(type.At, "false");
    }

    /// <summary>
    /// What a member access is made on is MAUI's device storage: <c>Preferences</c> or <c>Preferences.Default</c>, bare or
    /// qualified by MAUI's or Xamarin's namespace, where no type of the code has the name, or a member typed as its interface.
    /// </summary>
    private static bool Accesses(CodeGraph code, TypeNode type, ExpressionSyntax on, IReadOnlySet<string> given)
    {
        var segments = on.ToString().Replace(" ", "").Split('.');
        if (segments is [.., _, "Default"])
        {
            segments = segments[..^1];
        }

        var qualifier = string.Join(".", segments[..^1]);
        if (Storage.Contains(segments[^1]) && (qualifier.Length == 0 || qualifier is "Microsoft.Maui.Storage" or "Microsoft.Maui.Essentials" or "Xamarin.Essentials" or "global::Microsoft.Maui.Storage")
            && code.TypeFor(new TypeRefNode { Text = segments[^1], Name = segments[^1], QualifiedName = segments[^1] }, type) is null)
        {
            return true;
        }

        return on is IdentifierNameSyntax identifier && given.Contains(identifier.Identifier.Text)
               || on is MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax, Name: IdentifierNameSyntax member } && given.Contains(member.Identifier.Text);
    }
}
