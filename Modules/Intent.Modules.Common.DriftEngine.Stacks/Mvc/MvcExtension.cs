using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Blazor;
using Intent.Modules.Common.DriftEngine.Stacks.Interactions;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;
using Intent.Modules.Common.DriftEngine.Stacks.Ui;

namespace Intent.Modules.Common.DriftEngine.Stacks.Mvc;

/// <summary>
/// Reads the User Interface designer's claims (the <c>intent.ui</c> pack's) from ASP.NET Core MVC's server-rendered views,
/// as <c>intent.blazor</c> reads them from Razor components: what a view is (a layout a view or <c>_ViewStart</c> names, a
/// page an action renders or a Razor Page, else a partial), the URLs a GET shows a page at, its title, its placement points
/// (<c>RenderBody()</c>, <c>RenderSection</c>), and what its markup and code render (partials, view components), link to,
/// post forms to and inject. The controllers' routing and which view an action renders are read through
/// <see cref="MvcSite"/>, with ASP.NET Core's own routing (<see cref="AspNetCore.Binding"/>). It answers only for a view a
/// <c>.cshtml</c> file declares, so Blazor's, Vue's and React's readers keep theirs.
/// </summary>
public sealed class MvcExtension : IDriftExtension
{
    public string Id => "intent.mvc";

    public IEnumerable<ICodeReader> Readers { get; } = [new ViewMembers()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new Views(), new ViewLinks(), new ViewRegions()];

    /// <summary>
    /// What a view's members are to the designer, so <c>unmodelled.member</c> asks for none it can't state: an injected service
    /// (<c>@inject</c>) is the view's <c>Uses</c> of it (<see cref="CodeGraph.StatedByLink"/>); any other property or field is
    /// written in code (<see cref="CodeGraph.Unmodellable"/>), since a view takes no parameters (its model and its route values
    /// are its action's); and so is a method of its <c>@functions</c>, a helper that renders it, since what a user does on a
    /// server-rendered page is a form or link to a controller's action, which a Call Service Operation or Navigation states.
    /// </summary>
    private sealed class ViewMembers : ICodeReader
    {
        public void Read(CodeGraph graph)
        {
            foreach (var member in graph.Types.Where(MvcSite.IsView).SelectMany(x => x.Members).Where(x => x.Kind is "property" or "field" or "method"))
            {
                if (member.Attributes.Any(a => a.Name == "Inject"))
                {
                    graph.StatedByLink[member] = "Uses";
                }
                else
                {
                    graph.Unmodellable[member] = member.Kind == "method"
                        ? "an MVC view's code only renders it: what a user does there is a form or link to a controller's action"
                        : "an MVC view takes no parameters: what it shows comes from its model and its own code";
                }
            }
        }
    }

    /// <summary>A view's file name without its folder or extension, as an element not located is named.</summary>
    internal static string Stem(string path)
    {
        var name = path.Split('/')[^1];
        return name.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? name[..^".cshtml".Length] : name;
    }

    internal static string Qualified(TypeNode type) => type.Namespace.Length > 0 ? $"{type.Namespace}.{type.Name}" : type.Name;
}

/// <summary>What a view is, a page's URLs and title, and its security.</summary>
internal sealed class Views : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["component.kind", "page.route", "page.title", "security", "security.roles", "security.policy"];

    public ClaimReading? Read(ClaimContext context)
    {
        var type = context.TypeOf(context.Element);
        if (type is not null && MvcSite.IsViewComponent(context.Code, type))
        {
            return ViewComponent(context, type);
        }

        if (!MvcSite.IsView(type) || MvcSite.Of(context.Code) is not { } site || site.ViewOf(type) is not { } view)
        {
            return null;
        }

        var name = context.Element.Name;
        switch (context.Kind)
        {
            case "component.kind":
                return Kind(context, site, view);
            case "page.route":
            {
                var urls = site.PageUrls(view);
                var at = view.Page?.At ?? site.RendersOf(view).FirstOrDefault(x => x.Action.Takes("GET"))?.At;
                return urls.Count == 0 || at is null ? ClaimReading.Absent(view.Type.At) : ClaimReading.Stated(string.Join(", ", urls), at);
            }
            case "page.title":
                return view.Title switch
                {
                    { Name: { } title, At: var at } => ClaimReading.Stated(title, at),
                    { At: var at } => ClaimReading.CantVerify("unreadable", "The view's title (ViewData[\"Title\"] or ViewBag.Title) is computed.", at),
                    null => ClaimReading.Absent(view.Type.At)
                };
            case "security" when view.Page is null:
                return ClaimReading.Compared("None", view.Type.At, context.Expected == "None",
                    $"`{name}` security: model `{context.Expected}`, code `None`. An MVC view carries no authorization of its own: the controller action that renders it does, and the Services model states it on that operation. Leave the view's security unset.");
            case "security":
            {
                // A Razor Page is its own endpoint: its @attribute [Authorize] or its page model's.
                var authorize = RazorPageAttributes(context.Code, view).FirstOrDefault(x => x.Name is "Authorize" or "AllowAnonymous");
                if (authorize is null)
                {
                    return ClaimReading.Absent(view.Page!.At, "None");
                }

                var value = authorize.Name == "Authorize" ? "Secured" : "Unsecured";
                return ClaimReading.Compared(value, authorize.At, value == context.Expected);
            }
            default:
            {
                var authorize = view.Page is null ? null : RazorPageAttributes(context.Code, view).FirstOrDefault(x => x.Name == "Authorize");
                var value = context.Kind == "security.roles" ? authorize?.Named.GetValueOrDefault("Roles") : authorize?.Named.GetValueOrDefault("Policy") ?? authorize?.Arguments.FirstOrDefault();
                return value is { Length: > 0 }
                    ? ClaimReading.Stated(string.Join(", ", value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)), authorize!.At)
                    : ClaimReading.Absent(authorize?.At ?? view.Type.At);
            }
        }
    }

    /// <summary>
    /// A view's kind: a layout when a view or a <c>_ViewStart.cshtml</c> names it as its layout; a page when it is a Razor Page or
    /// an action renders it (<c>View(...)</c>); else a component, which a view rendering it as a partial, or an action as a
    /// partial view, states, and silence otherwise. A dialog can't be told from a view's own code.
    /// </summary>
    private static ClaimReading Kind(ClaimContext context, MvcSite site, MvcView view)
    {
        var name = context.Element.Name;
        if (context.Expected == "Dialog")
        {
            return ClaimReading.CantVerify("noRule", "An MVC view's own code doesn't say whether it is a dialog.", view.Type.At);
        }

        if (site.LayoutUsers(view) is [var user, ..])
        {
            return ClaimReading.Compared("Layout", user.At, context.Expected == "Layout",
                $"`{name}` component kind: model `{context.Expected}`, code `Layout`. `{user.From}` names it as its layout. Model it as a Layout.");
        }

        if (view.Page is { } page)
        {
            return ClaimReading.Compared("Page", page.At, context.Expected == "Page",
                $"`{name}` component kind: model `{context.Expected}`, code `Page`. `{view.Path}` is a Razor Page (`@page`). Model it as a Page.");
        }

        if (site.RendersOf(view) is [var render, ..])
        {
            return ClaimReading.Compared("Page", render.At, context.Expected == "Page",
                $"`{name}` component kind: model `{context.Expected}`, code `Page`. `{render.Action.Display}` renders it as a view. Model it as a Page.");
        }

        if (site.PartialRendersOf(view) is [var fragment, ..])
        {
            return ClaimReading.Compared("Component", fragment.At, context.Expected == "Component",
                $"`{name}` component kind: model `{context.Expected}`, code `Component`. `{fragment.Action.Display}` renders it as a partial view, which no layout shows. Model it as a Component.");
        }

        if (site.PartialUsers(view) is [var partial, ..])
        {
            return ClaimReading.Compared("Component", partial.At, context.Expected == "Component",
                $"`{name}` component kind: model `{context.Expected}`, code `Component`. No controller action renders it; `{partial.From.Type.Name}` renders it as a partial. Model it as a Component.");
        }

        return ClaimReading.Absent(view.Type.At, "Component");
    }

    /// <summary>
    /// A view component's class (<c>CartSummary : ViewComponent</c>): a component a view renders (<c>&lt;vc:cart-summary /&gt;</c>),
    /// never a page or a layout, carrying no authorization of its own, as a partial doesn't.
    /// </summary>
    private static ClaimReading? ViewComponent(ClaimContext context, TypeNode type)
    {
        var name = context.Element.Name;
        return context.Kind switch
        {
            "component.kind" when context.Expected == "Dialog" => ClaimReading.CantVerify("noRule", "A view component's own code doesn't say whether it is a dialog.", type.At),
            "component.kind" => ClaimReading.Compared("Component", type.At, context.Expected == "Component",
                $"`{name}` component kind: model `{context.Expected}`, code `Component`. `{type.Name}` is a view component, which a view renders inside itself. Model it as a Component."),
            "security" => ClaimReading.Compared("None", type.At, context.Expected == "None",
                $"`{name}` security: model `{context.Expected}`, code `None`. A view component carries no authorization of its own: the action that renders its page does. Leave its security unset."),
            "security.roles" or "security.policy" => ClaimReading.Absent(type.At),
            _ => null
        };
    }

    /// <summary>A Razor Page's attributes: its own (<c>@attribute [Authorize]</c>), then its page model's (<c>@model</c>).</summary>
    private static IEnumerable<AttributeNode> RazorPageAttributes(CodeGraph code, MvcView view)
    {
        var model = view.Model is { } written
            ? code.TypeFor(new TypeRefNode { Text = written, Name = written.Split('.')[^1].Split('<')[0], QualifiedName = written.Split('<')[0] }, view.Type)
            : null;
        return view.Type.Attributes.Concat(model is null ? [] : new[] { model }.Concat(code.Bases(model)).SelectMany(x => x.Attributes));
    }
}

/// <summary>What a view renders, links to, posts its forms to and injects, read from its markup and code.</summary>
internal sealed class ViewLinks : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.uses", "intent.navigates", "intent.target", "intent.invokes"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element is not IAssociationEnd end || end.TypeReference?.Element is not IElement target
            || BlazorExtension.CodeOf(context, context.Owner(end)) is not { } located || !MvcSite.IsView(located.Type)
            || MvcSite.Of(context.Code) is not { } site || site.ViewOf(located.Type) is not { } view)
        {
            return null;
        }

        var source = context.Owner(end)!;
        var owner = located.Element;
        return end.Association?.SpecializationType switch
        {
            "Composition" when context.Kind is "intent.uses" or "intent.target" => Composes(context, site, view, source, owner, target),
            "Navigation" when context.Kind == "intent.navigates" => Navigates(context, site, view, owner, target),
            "Call Service Operation Action" when context.Kind is "intent.invokes" or "intent.target" => Invokes(context, site, view, owner, target),
            "Uses" when context.Kind == "intent.uses" => Uses(context, view, owner, target),
            _ => null
        };
    }

    /// <summary>
    /// A Composition: a Region's places a page in the layout's placement point (the page is shown in the layout, or fills its
    /// section); any other renders a partial (<c>&lt;partial name&gt;</c>, <c>Html.PartialAsync</c>) or a view component
    /// (<c>&lt;vc:x&gt;</c>, <c>Component.InvokeAsync</c>).
    /// </summary>
    private static ClaimReading? Composes(ClaimContext context, MvcSite site, MvcView view, IElement source, IElement owner, IElement target)
    {
        var targetType = context.TypeOf(target);
        var targetView = site.ViewOf(targetType);
        if (source != owner && context.ClaimOn(source, "region.outlet") is not null)
        {
            if (targetType is null)
            {
                return ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located.");
            }

            // A page the Region places; a partial or view component it shows is rendered as any other.
            if (targetView is not null && (targetView.Page is not null || site.RendersOf(targetView).Count > 0))
            {
                return Placed(context, site, view, source, owner, target, targetView);
            }
        }

        var partials = site.PartialsOf(view);
        var components = site.ViewComponentsOf(view);
        var hit = partials.Where(x => targetView is not null ? site.ResolveFrom(view.Path, x.Name) == targetView.Path : MvcExtension.Stem(x.Name) == target.Name)
            .Concat(components.Where(x => Names(x.Name, target, targetType)))
            .Select(x => (Span?)x.At)
            .FirstOrDefault();
        if (hit is { } at)
        {
            return context.Kind == "intent.target"
                ? ClaimReading.Compared(targetType is null ? target.Name : MvcExtension.Qualified(targetType), at, true)
                : ClaimReading.Found(target, at);
        }

        // What the target is waits on whether it is rendered, which says so.
        if (context.Kind == "intent.target")
        {
            return null;
        }

        var rendered = partials.Select(x => $"`{x.Name}`").Concat(components.Select(x => $"`{x.Name}` (view component)")).ToList();
        return ClaimReading.Compared(rendered.Count == 0 ? "none" : string.Join(", ", partials.Select(x => x.Name).Concat(components.Select(x => x.Name))), view.Type.At, false,
            $"`{owner.Name}` renders `{target.Name}`: in the model, not in the code. `{view.Path}` renders {(rendered.Count == 0 ? "no partial or view component" : string.Join(", ", rendered))}. The model states it in its Composition. Render it with `<partial name=\"{target.Name}\" />`, or remove the Composition.");
    }

    /// <summary>A view component's name names the element: its class's (<c>Cart</c>, <c>CartViewComponent</c>), or the name its <c>[ViewComponent]</c> gives.</summary>
    private static bool Names(string component, IElement target, TypeNode? located) =>
        located is not null
            ? located.Name == component || located.Name == component + "ViewComponent" || located.Attributes.Any(x => x.Name == "ViewComponent" && x.Named.GetValueOrDefault("Name") == component)
            : target.Name == component || target.Name == component + "ViewComponent";

    /// <summary>
    /// A page a Region of a layout places: in the Region the layout's <c>RenderBody()</c> is (one no section names), the page is
    /// shown in the layout, its own <c>Layout</c> or its <c>_ViewStart</c>'s; in a Region a <c>RenderSection</c> names, the page
    /// fills that section (<c>@section name</c>).
    /// </summary>
    private static ClaimReading? Placed(ClaimContext context, MvcSite site, MvcView layout, IElement region, IElement owner, IElement target, MvcView page)
    {
        Span? at;
        string shown;
        if (layout.Sections.Any(x => x.Name == region.Name))
        {
            at = page.Defines.FirstOrDefault(x => x.Name == region.Name)?.At;
            shown = page.Defines.Count == 0 ? "fills no section" : $"fills the sections {string.Join(", ", page.Defines.Select(x => $"`{x.Name}`"))}";
        }
        else
        {
            var (path, set) = site.LayoutOf(page);
            at = path == layout.Path ? set : null;
            shown = path is null ? "is shown in no layout" : $"is shown in `{MvcExtension.Stem(path)}`";
        }

        if (at is { } found)
        {
            return context.Kind == "intent.target" ? ClaimReading.Compared(MvcExtension.Qualified(page.Type), found, true) : ClaimReading.Found(target, found);
        }

        return context.Kind == "intent.target"
            ? null
            : ClaimReading.Compared("none", page.Type.At, false,
                $"`{owner.Name}` places `{target.Name}` in its region `{region.Name}`: in the model, not in the code. `{page.Path}` {shown}. The model states it in the Region's Composition. Show the page in that layout, or move the Composition.");
    }

    /// <summary>
    /// A Navigation: a link of the view (<c>&lt;a href&gt;</c>, or <c>asp-controller</c>/<c>asp-action</c>, <c>asp-route</c>,
    /// <c>asp-page</c>) reaching an action that takes a GET and renders the target page, or the target Razor Page.
    /// </summary>
    private static ClaimReading Navigates(ClaimContext context, MvcSite site, MvcView view, IElement owner, IElement target)
    {
        var targetView = site.ViewOf(context.TypeOf(target));
        bool Is(string path) => targetView is not null ? path == targetView.Path : MvcExtension.Stem(path) == target.Name;
        var links = site.Links(view);
        if (links.FirstOrDefault(x => x.Pages.Any(Is) || x.Actions.Any(a => site.ViewsOf(a).Any(Is))) is { } hit)
        {
            return ClaimReading.Found(hit.Node, hit.At);
        }

        if (links.FirstOrDefault(x => x.Computed) is { } computed)
        {
            return ClaimReading.CantVerify("unreadable", "A link's URL is computed.", computed.At);
        }

        var urls = targetView is null ? [] : site.PageUrls(targetView);
        return ClaimReading.Compared(links.Count == 0 ? "none" : string.Join(", ", links.Select(x => x.Written)), view.Type.At, false,
            $"`{owner.Name}` navigates to `{target.Name}`: in the model, not in the code. {(urls.Count == 0 ? $"No GET shows `{target.Name}`" : $"`{target.Name}` is at {string.Join(", ", urls.Select(x => $"`{x}`"))}")}, and `{view.Path}` links to {(links.Count == 0 ? "nothing" : string.Join(", ", links.Select(x => $"`{x.Written}`")))}. The model states it in its Navigation. Link to the page, or remove the Navigation.");
    }

    /// <summary>
    /// A Call Service Operation Action: a form of the view reaching the operation's action (the controller method the Services
    /// model locates it as, else the one its Service and its name give) with the verb its Http Settings give.
    /// </summary>
    private static ClaimReading? Invokes(ClaimContext context, MvcSite site, MvcView view, IElement owner, IElement operation)
    {
        if (operation.ParentElement is not { } service)
        {
            return null;
        }

        var verb = context.Model.Stereotype(operation, "Http Settings", "Verb");
        var method = Reach.Located<MemberNode>(context, operation, "member.exists");
        bool Same(MvcAction action) => method is not null
            ? action.Method == method
            : (action.Name == operation.Name || action.Method.Name == operation.Name) && Bare(action.Controller.Name) == Bare(service.Name);
        var forms = site.Forms(view);
        if (forms.FirstOrDefault(x => x.Actions.Any(Same) && (verb is null || string.Equals(verb, x.Verb, StringComparison.OrdinalIgnoreCase))) is { } hit)
        {
            return context.Kind == "intent.target" ? ClaimReading.Compared(service.Name, hit.At, true) : ClaimReading.Found(hit.Node, hit.At);
        }

        // What the target is waits on whether it is invoked, which says so.
        if (context.Kind == "intent.target")
        {
            return null;
        }

        if (forms.FirstOrDefault(x => x.Computed) is { } computed)
        {
            return ClaimReading.CantVerify("unreadable", "A form's action is computed.", computed.At);
        }

        var posted = forms.Select(x => $"`{x.Written}` ({x.Verb} {(x.Actions.Count == 0 ? "no action" : string.Join(", ", x.Actions.Select(a => $"`{a.Display}`")))})").ToList();
        return ClaimReading.Compared(forms.Count == 0 ? "none" : string.Join(", ", forms.SelectMany(x => x.Actions).Select(x => x.Display).Distinct()), forms.Count == 0 ? view.Type.At : forms[0].At, false,
            $"`{owner.Name}` invokes `{service.Name}.{operation.Name}`{(verb is null ? "" : $" ({verb})")}: in the model, not in the code. `{view.Path}` {(forms.Count == 0 ? "has no form" : "posts " + string.Join(", ", posted))}. The model states it in its Call Service Operation Action. Post a form to the operation's action, or link the operation it posts to.");
    }

    /// <summary>A controller's or service's name without its <c>Controller</c> or <c>Service</c> suffix.</summary>
    private static string Bare(string name) =>
        name.EndsWith("Controller", StringComparison.Ordinal) ? name[..^"Controller".Length] : name.EndsWith("Service", StringComparison.Ordinal) ? name[..^"Service".Length] : name;

    /// <summary>A Uses: what the view injects (<c>@inject</c>) or otherwise holds, as any class's dependency is read. A Behaviour isn't read here.</summary>
    private static ClaimReading? Uses(ClaimContext context, MvcView view, IElement owner, IElement target)
    {
        if (UiCode.Bound(context, "Behaviour") is not null)
        {
            return null;
        }

        if (context.TypeOf(target) is not { } used)
        {
            return ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located.");
        }

        return Dependencies.Find(context.Code, view.Type, used) is { } dependency
            ? ClaimReading.Found(dependency.Node, dependency.At)
            : ClaimReading.Compared("none", view.Type.At, false,
                $"`{owner.Name}` uses `{target.Name}`: in the model, not in the code. `{view.Path}` injects no `{used.Name}` or interface of it. The model states it in its Uses. Inject it with `@inject`, or remove the Uses.");
    }
}

/// <summary>
/// <c>region.outlet</c> for a view: a Region is a section of its name the view renders (<c>RenderSection("scripts")</c>),
/// which pages fill with <c>@section</c>; a layout's <c>RenderBody()</c> is its one placement point the code doesn't name, so
/// it is the Region no section names, when only one is (or only one of those places a Page, as the designer places pages in
/// a layout), as Blazor reads <c>@Body</c>.
/// </summary>
internal sealed class ViewRegions : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["region.outlet"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element.ParentElement is not { } parent || context.TypeOf(parent) is not { } type || !MvcSite.IsView(type)
            || MvcSite.Of(context.Code) is not { } site || site.ViewOf(type) is not { } view)
        {
            return null;
        }

        var name = context.Element.Name;
        if (view.Sections.FirstOrDefault(x => x.Name == name) is { } section)
        {
            return ClaimReading.Stated(name, section.At);
        }

        // A Region named for a section pages fill (@section) is that section, renamed in the layout or not, never its body.
        var regions = parent.ChildElements.Where(x => x == context.Element || context.ClaimOn(x, "region.outlet") is not null).ToList();
        var filled = site.Views.Values.SelectMany(x => x.Defines).Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
        var unnamed = regions.Where(x => view.Sections.All(s => s.Name != x.Name) && !filled.Contains(x.Name)).ToList();
        if (view.Body is { } body && Body(context, unnamed) == context.Element)
        {
            return ClaimReading.Stated(name, body);
        }

        if (view.Sections.FirstOrDefault(x => x.Name is null) is { } computed)
        {
            return ClaimReading.CantVerify("unreadable", "A rendered section's name is computed.", computed.At);
        }

        var spare = view.Sections.Where(x => regions.All(r => r.Name != x.Name)).ToList();
        var actual = spare.Count == 1 ? spare[0].Name! : "none";
        var has = view.Sections.Count == 0 ? "renders no section" : $"renders the sections {string.Join(", ", view.Sections.Select(x => $"`{x.Name}`"))}";
        return ClaimReading.Compared(actual, spare.Count == 1 ? spare[0].At : view.Type.At, false,
            $"`{parent.Name}` region `{name}`: model `{name}`, code {(spare.Count == 1 ? $"`{actual}`" : "none")}. `{view.Path}` {has}, and no `RenderSection(\"{name}\")`{(view.Body is null ? "" : "; its `RenderBody()` is another Region")}. The model states it in the Region's name. Rename the Region, or the section.");
    }

    /// <summary>The Region a layout's <c>RenderBody()</c> is: of the Regions no section names, the only one, else the only one placing a Page.</summary>
    private static IElement? Body(ClaimContext context, IReadOnlyList<IElement> unnamed) =>
        unnamed.Count == 1 ? unnamed[0]
        : unnamed.Where(x => context.Model.Links(x, "Composition").Any(l => l.Target.SpecializationType == "Page")).ToList() is [var only] ? only : null;
}
