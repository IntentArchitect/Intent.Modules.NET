using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.Proxies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.Mvc;

/// <summary>A name a view's code states and where: its layout, a partial, a view component, a section. Null where it is computed.</summary>
internal sealed record MvcNamed(string? Name, Span At);

/// <summary>
/// An MVC view or Razor Page (a <c>.cshtml</c> file the C# language read as a component type), with what its own code states
/// for MVC: its <c>Layout</c>, where it renders the page it lays out (<c>RenderBody()</c>) and the sections pages fill
/// (<c>RenderSection("scripts")</c>), the partials and view components its code renders (<c>Html.PartialAsync("x")</c>,
/// <c>Component.InvokeAsync("X")</c>), its <c>ViewData["Title"]</c>, the sections it fills (<c>@section scripts</c>), and a
/// Razor Page's <c>@page</c>.
/// </summary>
internal sealed class MvcView
{
    public required string Path { get; init; }

    public required TypeNode Type { get; init; }

    /// <summary>Its project's folder, relative to the output folder; empty at the root, or when no project governs it.</summary>
    public required string Project { get; init; }

    /// <summary>Its own <c>Layout = "..."</c>; a name of null for <c>Layout = null</c> or a computed one. Null when it sets none.</summary>
    public MvcNamed? Layout { get; set; }

    /// <summary>Where it renders the page it lays out: <c>RenderBody()</c>.</summary>
    public Span? Body { get; set; }

    /// <summary>The sections it renders (<c>RenderSection</c>, <c>RenderSectionAsync</c>): its named placement points.</summary>
    public List<MvcNamed> Sections { get; } = [];

    /// <summary>The partials its code renders: <c>Html.Partial</c>, <c>PartialAsync</c>, <c>RenderPartial</c>, <c>RenderPartialAsync</c>.</summary>
    public List<MvcNamed> PartialCalls { get; } = [];

    /// <summary>The view components its code renders: <c>Component.InvokeAsync("Cart")</c>, <c>InvokeAsync&lt;CartViewComponent&gt;()</c>.</summary>
    public List<MvcNamed> ComponentCalls { get; } = [];

    /// <summary>Its title, <c>ViewData["Title"]</c> or <c>ViewBag.Title</c>; a name of null when computed.</summary>
    public MvcNamed? Title { get; set; }

    /// <summary>The sections it fills for its layout: <c>@section scripts { ... }</c>.</summary>
    public List<MvcNamed> Defines { get; } = [];

    /// <summary>The forms, links and URLs its code builds with the HTML and URL helpers.</summary>
    public List<MvcHelperCall> Helpers { get; } = [];

    /// <summary>A Razor Page's <c>@page</c>, with its route template when it gives one.</summary>
    public MvcNamed? Page { get; set; }

    /// <summary>Its <c>@model</c>, as written.</summary>
    public string? Model { get; set; }
}

/// <summary>An action of an MVC controller: the verbs it takes (null for any), the URLs that reach it and the names of its routes.</summary>
internal sealed record MvcAction(TypeNode Controller, MemberNode Method, string ControllerName, string Name, IReadOnlyList<string>? Verbs, IReadOnlyList<string> Urls, IReadOnlyList<string> RouteNames)
{
    public bool Takes(string verb) => Verbs is null || Verbs.Contains(verb, StringComparer.OrdinalIgnoreCase);

    public string Display => $"{Controller.Name}.{Method.Name}";
}

/// <summary>A view an action renders, where, and whether as a partial view (<c>PartialView(...)</c>).</summary>
internal sealed record MvcRender(MvcAction Action, string ViewPath, Span At, bool Partial);

/// <summary>
/// A form or link of a view (its markup element, or the HTML helper's call), where it is, the verb it sends, how it is
/// written, the actions it reaches and the Razor Pages it shows; computed when the code doesn't give where it goes.
/// </summary>
internal sealed record MvcRequest(object Node, Span At, string Verb, string Written, IReadOnlyList<MvcAction> Actions, IReadOnlyList<string> Pages, bool Computed);

/// <summary>
/// Where a request goes, as the code writes it: the controller, action, route or Razor Page (or page handler) it names, or its
/// path (<see cref="HasPath"/> with no <see cref="Path"/> for another site's URL); computed when a value it names isn't text.
/// </summary>
internal sealed record MvcTarget(string? Controller, string? Action, string? Route, string? Page, bool Handler, LiteralNode? Path, bool HasPath, bool Computed);

/// <summary>
/// An HTML or URL helper's call in a view's code: a form (<c>Html.BeginForm</c>, <c>BeginRouteForm</c>), a link
/// (<c>Html.ActionLink</c>, <c>RouteLink</c>) or a URL (<c>Url.Action</c>, <c>RouteUrl</c>, <c>Page</c>, <c>Content</c>), with
/// where it goes and, for a form, the verb its <c>FormMethod</c> gives.
/// </summary>
internal sealed record MvcHelperCall(string Kind, string Text, Span At, MvcTarget Target, string? Verb);

/// <summary>
/// ASP.NET Core MVC as the code states it, read once per graph from the C# language's reading: the views (<c>.cshtml</c>
/// components) and what their code states, each <c>_ViewStart.cshtml</c>'s layout, the controllers' actions with their verbs
/// and URLs (attribute routes, or the conventional routes that serve them, <see cref="Binding.ConventionalUrls"/>), the views
/// each action renders, and where a view's forms post and its links go. View lookup is MVC's: <c>Views/{Controller}/{Name}</c>,
/// then <c>Views/Shared/{Name}</c> (an area's first); a partial's and layout's from the view's folder, then its <c>Shared</c>
/// folder; a Razor Page's from its folder up to <c>Pages</c>, then <c>Pages/Shared</c> and <c>Views/Shared</c>.
/// </summary>
internal sealed class MvcSite
{
    private static readonly HashSet<string> PartialCalls = new(StringComparer.Ordinal) { "Partial", "PartialAsync", "RenderPartial", "RenderPartialAsync" };

    private static readonly HashSet<string> SectionCalls = new(StringComparer.Ordinal) { "RenderSection", "RenderSectionAsync" };

    private static readonly HashSet<string> VerbAttributes = new(StringComparer.Ordinal) { "HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch", "HttpHead", "HttpOptions" };

    /// <summary>What a controller's factory is called on: nothing (its own), or <c>this</c>.</summary>
    private static readonly HashSet<string> Receivers = new(StringComparer.Ordinal) { "this", "base" };

    private const int MaxDepth = 3;

    private readonly CodeGraph _code;

    private MvcSite(CodeGraph code)
    {
        _code = code;
        if (CSharpSyntax.Of(code) is { } csharp)
        {
            foreach (var (file, tree) in csharp.Trees.Where(x => RazorFile.IsCshtml(x.Key)))
            {
                if (RazorFile.Of(tree) is not { } razor)
                {
                    continue;
                }

                if (RazorFile.IsViewConfiguration(file))
                {
                    if (LayoutSet(tree, razor, file) is { } layout)
                    {
                        ViewStarts.Add((file, layout));
                    }

                    continue;
                }

                if (code.Types.FirstOrDefault(x => x.Kind == "component" && x.Files.Contains(file)) is not { } type)
                {
                    continue;
                }

                var view = new MvcView { Path = file, Type = type, Project = ProjectFolder(file) };
                Read(view, tree, razor, file);
                Views[file] = view;
            }
        }

        Actions = Views.Count == 0 ? [] : ReadActions();
        Renders = Actions.SelectMany(RendersOf).ToList();
    }

    /// <summary>The views, by path.</summary>
    public Dictionary<string, MvcView> Views { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Each <c>_ViewStart.cshtml</c> that sets a layout, with the layout it sets.</summary>
    public List<(string Path, MvcNamed Layout)> ViewStarts { get; } = [];

    public IReadOnlyList<MvcAction> Actions { get; }

    public IReadOnlyList<MvcRender> Renders { get; }

    /// <summary>The site, read on first use; null when the code has no view.</summary>
    public static MvcSite? Of(CodeGraph code)
    {
        if (code.Get<MvcSite>() is not { } site)
        {
            code.Set(site = new MvcSite(code));
        }

        return site.Views.Count == 0 ? null : site;
    }

    /// <summary>A type an MVC view or Razor Page is: a component a <c>.cshtml</c> file declares.</summary>
    public static bool IsView(TypeNode? type) => type is { Kind: "component" } && type.Files.Any(RazorFile.IsCshtml);

    /// <summary>A view component's class, as MVC discovers one: deriving <c>ViewComponent</c>, marked <c>[ViewComponent]</c>, or named <c>...ViewComponent</c>.</summary>
    public static bool IsViewComponent(CodeGraph code, TypeNode type) =>
        type is { Kind: "class", IsAbstract: false }
        && (type.Name.EndsWith("ViewComponent", StringComparison.Ordinal) || type.Attributes.Any(x => x.Name == "ViewComponent")
            || code.Supertypes(type).Any(x => x is "ViewComponent" or "Microsoft.AspNetCore.Mvc.ViewComponent"));

    /// <summary>The view a type is; null for any other type.</summary>
    public MvcView? ViewOf(TypeNode? type) => IsView(type) ? type!.Files.Select(x => Views.GetValueOrDefault(x)).FirstOrDefault(x => x is not null) : null;

    /// <summary>The views a view's code shows the user: <c>View(...)</c>s of actions, not partial views.</summary>
    public IReadOnlyList<MvcRender> RendersOf(MvcView view) => Renders.Where(x => x.ViewPath == view.Path && !x.Partial).ToList();

    /// <summary>The actions rendering it as a partial view (<c>PartialView(...)</c>): a fragment a page's script fetches.</summary>
    public IReadOnlyList<MvcRender> PartialRendersOf(MvcView view) => Renders.Where(x => x.ViewPath == view.Path && x.Partial).ToList();

    /// <summary>
    /// The URLs a GET shows it at: those of the actions that take a GET and render it, and a Razor Page's own (its path under
    /// <c>Pages</c>, an <c>Index</c> its folder's too, with its <c>@page</c> template).
    /// </summary>
    public IReadOnlyList<string> PageUrls(MvcView view) =>
        RendersOf(view).Where(x => x.Action.Takes("GET")).SelectMany(x => x.Action.Urls).Concat(RazorPageUrls(view))
            .Distinct(StringComparer.OrdinalIgnoreCase).Order(StringComparer.Ordinal).ToList();

    /// <summary>The URLs a Razor Page is at; none for an MVC view.</summary>
    public IReadOnlyList<string> RazorPageUrls(MvcView view)
    {
        if (view.Page is not { } page || PagesRoot(view.Path) is not { } root)
        {
            return [];
        }

        var template = page.Name?.Trim();
        if (template is not null && (template.StartsWith('/') || template.StartsWith("~/", StringComparison.Ordinal)))
        {
            return ["/" + template.TrimStart('~', '/')];
        }

        var relative = view.Path[(root.Length + 1)..^".cshtml".Length];
        var paths = new List<string> { relative };
        if (relative.Split('/')[^1].Equals("Index", StringComparison.OrdinalIgnoreCase))
        {
            paths.Add(relative.Contains('/') ? relative[..relative.LastIndexOf('/')] : "");
        }

        var urls = new List<string>();
        foreach (var path in paths)
        {
            var url = "/" + string.Join("/", new[] { path, template ?? "" }.Where(x => x.Length > 0));
            urls.Add(url);

            // A template whose every segment is optional may be left out.
            if (template is { Length: > 0 } && template.Split('/').All(x => x.EndsWith("?}", StringComparison.Ordinal)))
            {
                urls.Add("/" + path);
            }
        }

        return urls.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>
    /// What names a view as a layout: a view's own <c>Layout = "..."</c>, or a <c>_ViewStart.cshtml</c>'s, each resolved from
    /// the file that writes it.
    /// </summary>
    public IReadOnlyList<(string From, Span At)> LayoutUsers(MvcView layout)
    {
        var users = Views.Values.Where(x => x != layout && x.Layout is { Name: { } name } && ResolveFrom(x.Path, name) == layout.Path).Select(x => (x.Path, x.Layout!.At));
        var starts = ViewStarts.Where(x => x.Layout.Name is { } name && ResolveFrom(x.Path, name) == layout.Path).Select(x => (x.Path, x.Layout.At));
        return users.Concat(starts).ToList();
    }

    /// <summary>
    /// The layout a view is shown in, and what says so: its own <c>Layout = "..."</c>, else that of the nearest
    /// <c>_ViewStart.cshtml</c> in its folder or above that sets one, its name resolved from the view as MVC does.
    /// </summary>
    public (string? Path, Span? At) LayoutOf(MvcView view)
    {
        if (view.Layout is { } own)
        {
            return (own.Name is null ? null : ResolveFrom(view.Path, own.Name), own.At);
        }

        var folder = Folder(view.Path);
        var start = ViewStarts.Where(x => Folder(x.Path) is var above && (above.Length == 0 || folder.Equals(above, StringComparison.OrdinalIgnoreCase)
                                                                          || folder.StartsWith(above + "/", StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(x => x.Path.Length)
            .Select(x => ((string Path, MvcNamed Layout)?)x)
            .FirstOrDefault();
        return start is { Layout: { Name: { } name } layout } ? (ResolveFrom(view.Path, name), layout.At) : (null, start?.Layout.At);
    }

    /// <summary>The partials a view renders, each by name: <c>&lt;partial name="x" /&gt;</c> and the HTML helpers' calls.</summary>
    public IReadOnlyList<(string Name, Span At)> PartialsOf(MvcView view) =>
        Markup(view).Where(x => string.Equals(x.Tag, "partial", StringComparison.OrdinalIgnoreCase))
            .Select(x => (Name: Blazor.BlazorMarkup.Literal(x, "name"), x.At))
            .Where(x => x.Name is not null)
            .Select(x => (x.Name!, x.At))
            .Concat(view.PartialCalls.Where(x => x.Name is not null).Select(x => (x.Name!, x.At)))
            .ToList();

    private Dictionary<string, List<(MvcView From, Span At)>>? _partialUsers;

    /// <summary>The views rendering a view as a partial, each with where, read once for the site.</summary>
    public IReadOnlyList<(MvcView From, Span At)> PartialUsers(MvcView partial)
    {
        _partialUsers ??= Views.Values
            .SelectMany(view => PartialsOf(view).Select(x => (Path: ResolveFrom(view.Path, x.Name), From: view, x.At)))
            .Where(x => x.Path is not null)
            .GroupBy(x => x.Path!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.Select(u => (u.From, u.At)).ToList(), StringComparer.OrdinalIgnoreCase);
        return _partialUsers.GetValueOrDefault(partial.Path) ?? [];
    }

    /// <summary>The view components a view renders, each by name: <c>&lt;vc:shopping-cart /&gt;</c> is <c>ShoppingCart</c>, and <c>Component.InvokeAsync</c>'s.</summary>
    public IReadOnlyList<(string Name, Span At)> ViewComponentsOf(MvcView view) =>
        Markup(view).Where(x => x.Tag.StartsWith("vc:", StringComparison.OrdinalIgnoreCase))
            .Select(x => (Pascal(x.Tag[3..]), x.At))
            .Concat(view.ComponentCalls.Where(x => x.Name is not null).Select(x => (x.Name!, x.At)))
            .ToList();

    public IReadOnlyList<MarkupNode> Markup(MvcView view) => _code.MarkupOf(view.Type).Distinct().ToList();

    /// <summary>
    /// The view a name resolves to from a view (or a <c>_ViewStart.cshtml</c>): an app-relative path (<c>~/Views/Shared/_X.cshtml</c>)
    /// from its project; a relative path with its extension from its folder; else its folder's, then the <c>Shared</c> folder of
    /// its <c>Views</c> (an area's, then the project's), or for a Razor Page each folder up to <c>Pages</c>, then
    /// <c>Pages/Shared</c> and <c>Views/Shared</c>.
    /// </summary>
    public string? ResolveFrom(string from, string name)
    {
        var project = ProjectFolder(from);
        if (name.StartsWith("~/", StringComparison.Ordinal) || name.StartsWith('/'))
        {
            return Find(Join(project, WithExtension(name.TrimStart('~', '/'))));
        }

        var folder = Folder(from);
        if (name.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
        {
            return Find(Join(folder, name));
        }

        var file = name + ".cshtml";
        var candidates = new List<string> { Join(folder, file) };
        if (PagesRoot(from) is { } pages)
        {
            for (var up = folder; up.Length > pages.Length; up = Folder(up))
            {
                candidates.Add(Join(Folder(up), file));
            }

            candidates.Add(Join(pages, "Shared/" + file));
        }
        else if (ViewsRoot(from) is { } views)
        {
            candidates.Add(Join(views, "Shared/" + file));
        }

        candidates.Add(Join(project, "Views/Shared/" + file));
        return candidates.Select(Find).FirstOrDefault(x => x is not null);
    }

    /// <summary>
    /// Where a view's forms post: a form the tag helper builds (any <c>asp-</c> attribute) to <c>asp-controller</c> and
    /// <c>asp-action</c>, each defaulting to the controller and action that rendered the view, or to the route
    /// <c>asp-route</c> names (with no route of that name, the URL the view was rendered at, as the tag helper writes no
    /// action); a plain form to its <c>action</c>'s path (or the URL helper's, <c>action="@Url.Action("Login")"</c>); either
    /// with neither, to the URL the view was rendered at. A form the tag helper builds posts unless its <c>method</c> says
    /// otherwise; a plain one GETs. An HTML helper's form (<c>Html.BeginForm("Login", "Account")</c>, <c>BeginRouteForm</c>)
    /// posts to the action or route it names, unless its <c>FormMethod</c> says otherwise. A Razor Page handler (<c>asp-page</c>,
    /// <c>asp-page-handler</c>) is no controller's action.
    /// </summary>
    public IReadOnlyList<MvcRequest> Forms(MvcView view)
    {
        var requests = new List<MvcRequest>();
        foreach (var form in Markup(view).Where(x => string.Equals(x.Tag, "form", StringComparison.OrdinalIgnoreCase)))
        {
            var tagHelper = form.Attributes.Any(x => x.Name.StartsWith("asp-", StringComparison.OrdinalIgnoreCase));
            var verb = (Blazor.BlazorMarkup.Literal(form, "method") ?? (tagHelper ? "post" : "get")).ToUpperInvariant();
            var written = string.Join(" ", form.Attributes.Where(x => IsRouting(x.Name) || x.Name.Equals("action", StringComparison.OrdinalIgnoreCase)).Select(Written));
            requests.Add(Resolve(view, form, form.At, verb, written.Length == 0 ? "no action" : written, tagHelper ? TagHelper(form) : Plain(view, form, "action")));
        }

        foreach (var form in view.Helpers.Where(x => x.Kind == "form"))
        {
            requests.Add(Resolve(view, form, form.At, form.Verb ?? "POST", form.Text, form.Target));
        }

        return requests;
    }

    /// <summary>
    /// Where a view's links go, each a GET: an <c>&lt;a&gt;</c> the tag helper builds to <c>asp-controller</c> and
    /// <c>asp-action</c>, <c>asp-route</c> or a Razor Page's <c>asp-page</c>; a plain one to its <c>href</c>'s path, or the URL
    /// helper's (<c>href="@Url.Action("Index", "Home")"</c>); and an HTML helper's link (<c>Html.ActionLink</c>, <c>RouteLink</c>).
    /// </summary>
    public IReadOnlyList<MvcRequest> Links(MvcView view)
    {
        var requests = new List<MvcRequest>();
        foreach (var link in Markup(view).Where(x => string.Equals(x.Tag, "a", StringComparison.OrdinalIgnoreCase)))
        {
            var tagHelper = link.Attributes.Any(x => IsRouting(x.Name));
            if (!tagHelper && !link.Attributes.Any(x => x.Name.Equals("href", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var written = tagHelper
                ? string.Join(" ", link.Attributes.Where(x => IsRouting(x.Name)).Select(Written))
                : string.Join(" ", link.Attributes.Where(x => x.Name.Equals("href", StringComparison.OrdinalIgnoreCase)).Select(Written));
            requests.Add(Resolve(view, link, link.At, "GET", written, tagHelper ? TagHelper(link) : Plain(view, link, "href")));
        }

        foreach (var link in view.Helpers.Where(x => x.Kind == "link"))
        {
            requests.Add(Resolve(view, link, link.At, "GET", link.Text, link.Target));
        }

        return requests;
    }

    /// <summary>Where a form or link the tag helper builds goes, as its <c>asp-</c> attributes write it.</summary>
    private static MvcTarget TagHelper(MarkupNode node) => new(
        Blazor.BlazorMarkup.Literal(node, "asp-controller"), Blazor.BlazorMarkup.Literal(node, "asp-action"), Blazor.BlazorMarkup.Literal(node, "asp-route"),
        Blazor.BlazorMarkup.Literal(node, "asp-page"), node.Attributes.Any(x => x.Name.Equals("asp-page-handler", StringComparison.OrdinalIgnoreCase)), null, false,
        node.Attributes.Any(x => IsRouting(x.Name) && x.Literal is not { Kind: "string" }));

    /// <summary>
    /// Where a plain form or link goes: its path attribute's value, or the URL helper's call that is all of it
    /// (<c>@Url.Action("Index", "Home")</c>); none at all for a form with no action, which posts to the URL it was shown at.
    /// </summary>
    private static MvcTarget Plain(MvcView view, MarkupNode node, string attribute)
    {
        if (node.Attributes.FirstOrDefault(x => x.Directive is null && x.Name.Equals(attribute, StringComparison.OrdinalIgnoreCase)) is not { } value)
        {
            return new MvcTarget(null, null, null, null, false, null, false, false);
        }

        if (value.Literal is { Kind: "interpolated", Parts: [{ Literal: null }] } && view.Helpers.FirstOrDefault(x => x.Kind == "url" && value.At.Contains(x.At)) is { } url)
        {
            return url.Target;
        }

        if (Local(value.Literal) is not { } local)
        {
            // Another site's URL, a fragment or a script reaches no page of this one; a computed one can't be read.
            return new MvcTarget(null, null, null, null, false, null, true, value.Literal is not { Kind: "string" } || value.Value?.Contains('@') == true);
        }

        return new MvcTarget(null, null, null, null, false, local, true, false);
    }

    /// <summary>The views an action renders through <c>View(...)</c>, not as partial views.</summary>
    public IReadOnlyList<string> ViewsOf(MvcAction action) => Renders.Where(x => x.Action == action && !x.Partial).Select(x => x.ViewPath).Distinct().ToList();

    private static bool IsRouting(string attribute) =>
        attribute.Equals("asp-controller", StringComparison.OrdinalIgnoreCase) || attribute.Equals("asp-action", StringComparison.OrdinalIgnoreCase)
        || attribute.Equals("asp-route", StringComparison.OrdinalIgnoreCase) || attribute.Equals("asp-page", StringComparison.OrdinalIgnoreCase)
        || attribute.Equals("asp-page-handler", StringComparison.OrdinalIgnoreCase) || attribute.Equals("asp-area", StringComparison.OrdinalIgnoreCase);

    private static string Written(MarkupAttributeNode attribute) => $"{attribute.Name}=\"{attribute.Literal?.Value ?? attribute.Value}\"";

    /// <summary>
    /// The actions taking <paramref name="verb"/> and the Razor Pages a request reaches, from a view: a Razor Page it names; the
    /// actions of the route it names (with none of that name and no controller or action named, the URL the view was shown
    /// at); the action and controller it names, each defaulting to the view's rendering action's; the actions and pages served
    /// at its path, compared as ASP.NET Core matches routes (<see cref="ProxyRequests.RouteMatches"/>); or with nothing named,
    /// those at the URL the view was shown at.
    /// </summary>
    private MvcRequest Resolve(MvcView view, object node, Span at, string verb, string written, MvcTarget target)
    {
        var current = RendersOf(view).Select(x => x.Action).Distinct().ToList();
        if (target.Computed)
        {
            return new MvcRequest(node, at, verb, written, [], [], true);
        }

        if (target.Page is not null || target.Handler)
        {
            var page = target.Page is null ? (view.Page is null ? null : view.Path) : RazorPage(view, target.Page);
            return new MvcRequest(node, at, verb, written, [], page is null ? [] : [page], false);
        }

        var (controller, action) = (target.Controller, target.Action);
        if (target.Route is { } route)
        {
            var named = Actions.Where(x => x.RouteNames.Contains(route, StringComparer.Ordinal) && x.Takes(verb)).ToList();
            if (named.Count > 0 || controller is null && action is null)
            {
                // No action's route has the name: with no other routing value, the helper writes no URL of its own.
                return new MvcRequest(node, at, verb, written, named.Count > 0 ? named : AtUrlOf(current, verb), [], false);
            }
        }

        if (controller is not null || action is not null)
        {
            var pairs = controller is null
                ? current.Select(x => (Controller: x.ControllerName, Action: action ?? x.Name))
                : action is null ? current.Select(x => (Controller: controller, Action: x.Name)) : [(Controller: controller, Action: action)];
            var reached = pairs.Distinct()
                .SelectMany(p => Actions.Where(x => string.Equals(x.ControllerName, p.Controller, StringComparison.OrdinalIgnoreCase)
                                                    && string.Equals(x.Name, p.Action, StringComparison.OrdinalIgnoreCase) && x.Takes(verb)))
                .Distinct()
                .ToList();
            return new MvcRequest(node, at, verb, written, reached, [], false);
        }

        if (!target.HasPath)
        {
            return new MvcRequest(node, at, verb, written, AtUrlOf(current, verb), [], false);
        }

        if (target.Path is null)
        {
            return new MvcRequest(node, at, verb, written, [], [], false);
        }

        if (ProxyRequests.Route(target.Path) is not { } path)
        {
            return new MvcRequest(node, at, verb, written, [], [], true);
        }

        var actions = Actions.Where(x => x.Takes(verb) && x.Urls.Any(u => ProxyRequests.RouteMatches(path.Segments, u))).ToList();
        var pages = verb == "GET" ? Views.Values.Where(x => RazorPageUrls(x).Any(u => ProxyRequests.RouteMatches(path.Segments, u))).Select(x => x.Path).ToList() : [];
        return new MvcRequest(node, at, verb, written, actions, pages, false);
    }

    /// <summary>The Razor Page an <c>asp-page</c> names: from the <c>Pages</c> folder when it starts with <c>/</c>, else from the view's folder.</summary>
    private string? RazorPage(MvcView view, string page)
    {
        var pages = PagesRoot(view.Path) ?? Join(view.Project, "Pages");
        var path = page.StartsWith('/') ? Join(pages, page.TrimStart('/')) : Join(Folder(view.Path), page.TrimStart('.', '/'));
        return Find(WithExtension(path)) is { } found && Views.TryGetValue(found, out var target) && target.Page is not null ? found : null;
    }

    /// <summary>The actions that take <paramref name="verb"/> at a URL the <paramref name="current"/> actions are reached at.</summary>
    private IReadOnlyList<MvcAction> AtUrlOf(IReadOnlyList<MvcAction> current, string verb)
    {
        var urls = current.SelectMany(x => x.Urls).ToList();
        return Actions.Where(x => x.Takes(verb) && x.Urls.Any(u => urls.Any(c => SameRoute(c, u)))).ToList();
    }

    /// <summary>A link's path within the application, its <c>~</c> dropped; null for another site's URL, a fragment, mail or script.</summary>
    private static LiteralNode? Local(LiteralNode? url)
    {
        var text = url switch
        {
            { Kind: "string", Value: { } value } => value,
            { Kind: "interpolated", Parts: [{ Literal: { Kind: "string", Value: { } first } }, ..] } => first,
            _ => null
        };
        if (text is null || text.StartsWith("//", StringComparison.Ordinal) || text.Contains("://", StringComparison.Ordinal) || text.StartsWith('#')
            || text.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) || text.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) || text.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (!text.StartsWith('~'))
        {
            return url;
        }

        return url!.Kind == "string"
            ? new LiteralNode { Kind = "string", Value = url.Value![1..] }
            : new LiteralNode
            {
                Kind = "interpolated",
                Parts = url.Parts.Select((x, i) => i == 0 ? new ValueNode { Text = x.Text, Literal = new LiteralNode { Kind = "string", Value = x.Literal!.Value![1..] }, At = x.At } : x).ToList()
            };
    }

    /// <summary>Two routes the code gives are the same URL path: literals without case, a <c>{parameter}</c> any one segment.</summary>
    private static bool SameRoute(string a, string b)
    {
        var x = a.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var y = b.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return x.Length == y.Length && x.Zip(y).All(p => p.First.StartsWith('{') || p.Second.StartsWith('{') || string.Equals(p.First, p.Second, StringComparison.OrdinalIgnoreCase));
    }

    // ------------------------------------------------------------------ what a view's code states

    /// <summary>Reads what a view's code states for MVC from the C# the Razor compiler generated for it, each node where it is written.</summary>
    private static void Read(MvcView view, SyntaxTree tree, RazorFile razor, string file)
    {
        view.Layout = LayoutSet(tree, razor, file);
        foreach (var node in tree.GetRoot().DescendantNodes())
        {
            switch (node)
            {
                case AssignmentExpressionSyntax assignment when razor.IsWritten(assignment) && IsTitle(assignment.Left, file):
                    view.Title = new MvcNamed(Text(assignment.Right, file), CSharpSyntax.SpanOf(assignment, file));
                    break;
                case InvocationExpressionSyntax invocation when razor.IsWritten(invocation):
                {
                    var first = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
                    switch (invocation.Expression)
                    {
                        case IdentifierNameSyntax { Identifier.Text: "RenderBody" }:
                            view.Body ??= CSharpSyntax.SpanOf(invocation, file);
                            break;
                        case IdentifierNameSyntax { Identifier.Text: var name } when SectionCalls.Contains(name):
                            view.Sections.Add(new MvcNamed(first is null ? null : Text(first, file), CSharpSyntax.SpanOf(invocation, file)));
                            break;
                        case MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "Html" }, Name.Identifier.Text: var name } when PartialCalls.Contains(name):
                            view.PartialCalls.Add(new MvcNamed(first is null ? null : Text(first, file), CSharpSyntax.SpanOf(invocation, file)));
                            break;
                        case MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "Component" }, Name: var name } when name.Identifier.Text == "InvokeAsync":
                        {
                            var component = name is GenericNameSyntax { TypeArgumentList.Arguments: [var generic] } ? generic.ToString()
                                : first is TypeOfExpressionSyntax typeOf ? typeOf.Type.ToString()
                                : first is null ? null : Text(first, file);
                            view.ComponentCalls.Add(new MvcNamed(component is null ? null : ViewComponentName(component), CSharpSyntax.SpanOf(invocation, file)));
                            break;
                        }
                        case MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "Html" or "Url" } } when Helper(invocation, file) is { } helper:
                            view.Helpers.Add(helper);
                            break;
                    }

                    break;
                }
            }
        }

        foreach (var directive in razor.Directives.Where(x => x.At.File == file))
        {
            switch (directive.Name)
            {
                case "section" when directive.Tokens is [var section, ..]:
                    view.Defines.Add(new MvcNamed(section, directive.At));
                    break;
                case "page":
                    view.Page = new MvcNamed(directive.Tokens is [var template, ..] ? template.Trim('"') : null, directive.At);
                    break;
                case "model" when directive.Tokens is [var model, ..]:
                    view.Model = model;
                    break;
            }
        }
    }

    /// <summary>
    /// An HTML or URL helper's call, by the parameters ASP.NET Core (and ASP.NET MVC) give it, positional or named: a form
    /// (<c>Html.BeginForm(actionName, controllerName, FormMethod)</c>, <c>BeginRouteForm(routeName)</c>), a link
    /// (<c>Html.ActionLink(linkText, actionName, controllerName)</c>, <c>RouteLink(linkText, routeName)</c>) or a URL
    /// (<c>Url.Action(action, controller)</c>, <c>RouteUrl(routeName)</c>, <c>Page(pageName)</c>, <c>Content(contentPath)</c>).
    /// An argument that is route values (<c>new { id }</c>) or a <c>FormMethod</c> is no name; a name the code computes makes the
    /// call computed. Null for any other call.
    /// </summary>
    private static MvcHelperCall? Helper(InvocationExpressionSyntax invocation, string file)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: var receiver }, Name.Identifier.Text: var method })
        {
            return null;
        }

        var arguments = invocation.ArgumentList.Arguments;
        var computed = false;
        ExpressionSyntax? Argument(int position, string name) =>
            arguments.FirstOrDefault(x => x.NameColon?.Name.Identifier.Text == name)?.Expression
            ?? (position < arguments.Count && arguments[position].NameColon is null ? arguments[position].Expression : null);
        string? Name(int position, string name)
        {
            if (Argument(position, name) is not { } argument || argument is AnonymousObjectCreationExpressionSyntax or ObjectCreationExpressionSyntax || IsFormMethod(argument))
            {
                return null;
            }

            var text = Text(argument, file);
            computed |= text is null && !argument.IsKind(SyntaxKind.NullLiteralExpression);
            return text;
        }

        var verb = arguments.Select(x => x.Expression).FirstOrDefault(IsFormMethod) is MemberAccessExpressionSyntax formMethod ? formMethod.Name.Identifier.Text.ToUpperInvariant() : null;
        (string Kind, MvcTarget Target)? found = (receiver, method) switch
        {
            ("Html", "BeginForm") => ("form", new MvcTarget(Name(1, "controllerName"), Name(0, "actionName"), null, null, false, null, false, false)),
            ("Html", "BeginRouteForm") => ("form", new MvcTarget(null, null, Name(0, "routeName"), null, false, null, false, false)),
            ("Html", "ActionLink") => ("link", new MvcTarget(Name(2, "controllerName"), Name(1, "actionName"), null, null, false, null, false, false)),
            ("Html", "RouteLink") => ("link", new MvcTarget(null, null, Name(1, "routeName"), null, false, null, false, false)),
            ("Url", "Action") => ("url", new MvcTarget(Name(1, "controller"), Name(0, "action"), null, null, false, null, false, false)),
            ("Url", "RouteUrl") => ("url", new MvcTarget(null, null, Name(0, "routeName"), null, false, null, false, false)),
            ("Url", "Page") => ("url", new MvcTarget(null, null, null, Name(0, "pageName"), false, null, false, false)),
            ("Url", "Content") => ("url", Name(0, "contentPath") is { } path
                ? new MvcTarget(null, null, null, null, false, Local(new LiteralNode { Kind = "string", Value = path }), true, false)
                : new MvcTarget(null, null, null, null, false, null, true, false)),
            _ => null
        };
        return found is { } call
            ? new MvcHelperCall(call.Kind, Regex.Replace(invocation.ToString(), @"\s+", " "), CSharpSyntax.SpanOf(invocation, file), call.Target with { Computed = computed }, verb)
            : null;
    }

    private static bool IsFormMethod(ExpressionSyntax expression) => expression is MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "FormMethod" } };

    /// <summary>The layout a view's code (or a <c>_ViewStart.cshtml</c>'s) sets, the last <c>Layout = ...</c> it writes; null when none.</summary>
    private static MvcNamed? LayoutSet(SyntaxTree tree, RazorFile razor, string file) =>
        tree.GetRoot().DescendantNodes().OfType<AssignmentExpressionSyntax>()
            .Where(x => x.Left is IdentifierNameSyntax { Identifier.Text: "Layout" } && razor.IsWritten(x))
            .Select(x => new MvcNamed(Text(x.Right, file), CSharpSyntax.SpanOf(x, file)))
            .LastOrDefault();

    /// <summary><c>ViewData["Title"]</c> or <c>ViewBag.Title</c>.</summary>
    private static bool IsTitle(ExpressionSyntax target, string file) => target switch
    {
        ElementAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "ViewData" }, ArgumentList.Arguments: [{ Expression: var key }] } => Text(key, file) == "Title",
        MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "ViewBag" }, Name.Identifier.Text: "Title" } => true,
        _ => false
    };

    /// <summary>The string a value of a view's code is, when the code gives all of it; null for a computed one.</summary>
    private static string? Text(ExpressionSyntax expression, string file) =>
        CSharpReader.LiteralOf(expression, file, x => CSharpReader.ValueOf(x, file)) is { Kind: "string", Value: var value } ? value : null;

    /// <summary>A view component's name: its class's without <c>ViewComponent</c>, as MVC names it.</summary>
    private static string ViewComponentName(string written)
    {
        var name = written.Split('.')[^1];
        return name.EndsWith("ViewComponent", StringComparison.Ordinal) && name.Length > "ViewComponent".Length ? name[..^"ViewComponent".Length] : name;
    }

    // ------------------------------------------------------------------ controllers, their URLs and the views they render

    private IReadOnlyList<MvcAction> ReadActions()
    {
        var actions = new List<MvcAction>();
        foreach (var type in _code.Types.Where(x => x is { Kind: "class", IsAbstract: false, IsStatic: false } && AspNetCoreClaims.IsController(_code, x)
                                                    && !Binding.WebApi2(_code, x) && !Binding.OData(_code, x)))
        {
            var controllerName = type.Name.EndsWith("Controller", StringComparison.Ordinal) && type.Name.Length > "Controller".Length ? type.Name[..^"Controller".Length] : type.Name;
            var prefixes = new[] { type }.Concat(_code.Bases(type))
                .SelectMany(x => x.Attributes.Where(a => a.Name is "Route" or "RoutePrefix" && a.Arguments.Count > 0).Select(a => a.Arguments[0]))
                .ToList();
            var names = type.Attributes.Where(x => x.Name == "Route").Select(x => x.Named.GetValueOrDefault("Name")).OfType<string>().ToList();
            foreach (var method in _code.MembersOf(type).Where(x => x is { Kind: "method", Access: "public", IsStatic: false } && x.Attributes.All(a => a.Name != "NonAction")))
            {
                var action = method.Attributes.FirstOrDefault(x => x.Name == "ActionName" && x.Arguments.Count > 0)?.Arguments[0].Trim('"') ?? method.Name;
                var own = method.Attributes.Where(x => (x.Name == "Route" || VerbAttributes.Contains(x.Name)) && x.Arguments.Count > 0).Select(x => x.Arguments[0]).ToList();
                var urls = own.Count > 0 || prefixes.Count > 0 ? AttributeUrls(prefixes, own, type.Name, action) : Binding.ConventionalUrls(_code, type, method);
                var routeNames = method.Attributes.Where(x => x.Name == "Route" || VerbAttributes.Contains(x.Name)).Select(x => x.Named.GetValueOrDefault("Name")).OfType<string>()
                    .Concat(names).Distinct(StringComparer.Ordinal).ToList();
                actions.Add(new MvcAction(type, method, controllerName, action, AspNetCoreClaims.Verbs(_code, type, method)?.Verbs, urls, routeNames));
            }
        }

        return actions;
    }

    /// <summary>
    /// An attribute-routed action's URLs: each of its own templates under each of its controller's (or alone, when it starts
    /// with <c>/</c> or <c>~/</c>), or its controller's alone when it has none, with <c>[controller]</c> and <c>[action]</c> filled in.
    /// </summary>
    private static IReadOnlyList<string> AttributeUrls(IReadOnlyList<string> prefixes, IReadOnlyList<string> own, string controller, string action)
    {
        var urls = new List<string>();
        foreach (var template in own.Count == 0 ? [""] : own)
        {
            if (template.StartsWith('/') || template.StartsWith("~/", StringComparison.Ordinal))
            {
                urls.Add("/" + AspNetCoreClaims.Tokens(template.TrimStart('~', '/'), controller, action));
                continue;
            }

            foreach (var prefix in prefixes.Count == 0 ? [""] : prefixes)
            {
                var combined = string.Join("/", new[] { prefix.Trim('/'), template.Trim('/') }.Where(x => x.Length > 0));
                urls.Add("/" + AspNetCoreClaims.Tokens(combined, controller, action));
            }
        }

        return urls.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private IEnumerable<MvcRender> RendersOf(MvcAction action)
    {
        if (action.Method.Body is not { } body)
        {
            yield break;
        }

        foreach (var (name, at, partial) in Rendered(body, [], Receivers, 0, new HashSet<FunctionNode>()))
        {
            if (Resolve(action, name ?? action.Name) is { } path)
            {
                yield return new MvcRender(action, path, at, partial);
            }
        }
    }

    /// <summary>
    /// The views a body's returns render (a null name for the action's own), following locals and conditionals, the code's own
    /// methods it returns, and its controller extension methods (<c>this.LoadingPage("Redirect", url)</c>), with their
    /// parameters bound to the call's arguments (<see cref="CodeGraph.StringOf(ValueNode, IReadOnlyList{ValueTuple{CallNode, MemberNode}})"/>).
    /// A helper called twice with other arguments is read for each; one calling itself is read once on the way.
    /// </summary>
    private IEnumerable<(string? Name, Span At, bool Partial)> Rendered(FunctionNode body, IReadOnlyList<(CallNode Call, MemberNode Into)> through, IReadOnlySet<string> controllers,
        int depth, IReadOnlySet<FunctionNode> path)
    {
        if (depth > MaxDepth || path.Contains(body))
        {
            yield break;
        }

        var inner = new HashSet<FunctionNode>(path) { body };
        foreach (var value in body.Returns)
        {
            foreach (var found in Rendered(body, value, through, controllers, depth, inner))
            {
                yield return found;
            }
        }
    }

    private IEnumerable<(string? Name, Span At, bool Partial)> Rendered(FunctionNode body, ValueNode value, IReadOnlyList<(CallNode Call, MemberNode Into)> through,
        IReadOnlySet<string> controllers, int depth, IReadOnlySet<FunctionNode> seen)
    {
        if (value.Call is null && value.Creations.Count == 0 && value.Given.Count > 0)
        {
            foreach (var found in value.Given.Take(16).SelectMany(x => Rendered(body, x, through, controllers, depth, seen)))
            {
                yield return found;
            }

            yield break;
        }

        if ((value.Call ?? MinimalApiClaims.Outermost(body, value.At)) is not { } call)
        {
            yield break;
        }

        if (Http.SuccessStatus.FromResult(call) is { } wrapped)
        {
            foreach (var found in Rendered(body, wrapped, through, controllers, depth, seen))
            {
                yield return found;
            }

            yield break;
        }

        var onController = call.Receiver is null || controllers.Contains(call.Receiver);
        if (onController && call.Target is null && call.Name is "View" or "PartialView")
        {
            var arguments = call.Arguments;
            var named = arguments.FirstOrDefault(x => x.Name == "viewName") ?? (arguments.Count > 1 || arguments is [var only] && IsViewName(only) ? arguments[0] : null);
            if (named is null)
            {
                yield return (null, call.At, call.Name == "PartialView");
            }
            else if (_code.StringOf(named, through) is { Kind: "string", Value: { } name })
            {
                yield return (name, call.At, call.Name == "PartialView");
            }

            yield break;
        }

        if (onController && call.Target is null && ActionResults.Extension(_code, call) is { Body: { } extension, Parameters: [var extended, ..] } helper)
        {
            // An extension call x.M(a) runs M(x, a).
            var asStatic = new CallNode { Name = call.Name, Arguments = [new ValueNode { Text = call.Receiver ?? "this", At = call.At }, .. call.Arguments], At = call.At, Text = call.Text };
            var controllersThere = new HashSet<string>(Receivers, StringComparer.Ordinal) { extended.Name };
            foreach (var found in Rendered(extension, [.. through, (asStatic, helper)], controllersThere, depth + 1, seen))
            {
                yield return (found.Name, call.At, found.Partial);
            }

            yield break;
        }

        if (call.Target is { Member: { Body: { } own, Type: { } type } member } && IsResult(type))
        {
            foreach (var found in Rendered(own, [.. through, (call, member)], Receivers, depth + 1, seen))
            {
                yield return (found.Name, call.At, found.Partial);
            }
        }
    }

    /// <summary><c>View(viewName)</c>'s argument is a name: text, not the model the view renders.</summary>
    private static bool IsViewName(ValueNode value) =>
        value.Literal is { Kind: "string" or "interpolated" } || value is { IsKnown: true, Types: [{ Name: "string" or "String" }] } || value.Parameter?.Type?.Name is "string" or "String";

    /// <summary>A method's result type an action could return: <c>IActionResult</c>, <c>ViewResult</c>, under <c>Task</c>.</summary>
    private static bool IsResult(TypeRefNode type) =>
        ActionResults.IsUntyped(type) || (type is { Name: "Task" or "ValueTask", Arguments: [var awaited] } ? awaited : type).Name.EndsWith("Result", StringComparison.Ordinal);

    /// <summary>
    /// The view a name an action renders resolves to: an app-relative path from the controller's project; else
    /// <c>Views/{Controller}/{Name}</c>, then <c>Views/Shared/{Name}</c> (an area's first, for an <c>[Area]</c> controller), in
    /// the controller's project, then in any other (a Razor class library's).
    /// </summary>
    private string? Resolve(MvcAction action, string name)
    {
        var project = ProjectOf(action.Controller);
        if (name.StartsWith("~/", StringComparison.Ordinal) || name.StartsWith('/') || name.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
        {
            var relative = WithExtension(name.TrimStart('~', '/'));
            return Find(Join(project ?? "", relative)) ?? Views.Keys.FirstOrDefault(x => x.EndsWith("/" + relative, StringComparison.OrdinalIgnoreCase));
        }

        var area = new[] { action.Controller }.Concat(_code.Bases(action.Controller)).SelectMany(x => x.Attributes).FirstOrDefault(x => x.Name == "Area" && x.Arguments.Count > 0)?.Arguments[0];
        var relatives = (area is null ? [] : new[] { $"Areas/{area}/Views/{action.ControllerName}/{name}.cshtml", $"Areas/{area}/Views/Shared/{name}.cshtml" })
            .Concat([$"Views/{action.ControllerName}/{name}.cshtml", $"Views/Shared/{name}.cshtml"])
            .ToList();
        foreach (var relative in relatives)
        {
            if (project is not null && Find(Join(project, relative)) is { } own)
            {
                return own;
            }
        }

        return relatives.Select(r => Views.Keys.FirstOrDefault(x => x.Equals(r, StringComparison.OrdinalIgnoreCase) || x.EndsWith("/" + r, StringComparison.OrdinalIgnoreCase)))
            .FirstOrDefault(x => x is not null);
    }

    /// <summary>The folder of a type's project, from the project file the C# language names for its file.</summary>
    private string? ProjectOf(TypeNode type) =>
        _code.Files.FirstOrDefault(x => type.Files.Contains(x.Path) && x.Project is not null)?.Project is { } project ? Folder(project.Replace('\\', '/')) : null;

    /// <summary>The folder of a file's project; empty when none governs it.</summary>
    private string ProjectFolder(string file) =>
        _code.Files.FirstOrDefault(x => x.Path == file)?.Project is { } project ? Folder(project.Replace('\\', '/'))
        : _code.ProjectReferences.Keys.Select(x => Folder(x.Replace('\\', '/'))).Where(x => x.Length == 0 || file.StartsWith(x + "/", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Length).FirstOrDefault() ?? "";

    private string? Find(string path) => Views.ContainsKey(path) ? Views.Keys.First(x => x.Equals(path, StringComparison.OrdinalIgnoreCase)) : null;

    private static string WithExtension(string path) => path.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? path : path + ".cshtml";

    private static string Join(string folder, string path) => folder.Length == 0 ? path : $"{folder}/{path}";

    private static string Folder(string path) => path.LastIndexOf('/') is var slash and >= 0 ? path[..slash] : "";

    /// <summary>The <c>Views</c> folder of its project a view is under (an area's, <c>Areas/X/Views</c>, for an area's view); null for none.</summary>
    private string? ViewsRoot(string path) => Root(path, "Views");

    /// <summary>The <c>Pages</c> folder of its project a Razor Page is under (an area's, <c>Areas/X/Pages</c>); null for none.</summary>
    private string? PagesRoot(string path) => Root(path, "Pages");

    /// <summary>
    /// The project's <c>Views</c> or <c>Pages</c> folder a file is under, or an area's (<c>Areas/X/Views</c>); with no project
    /// known, the nearest folder of that name above it.
    /// </summary>
    private string? Root(string path, string folder)
    {
        var project = ProjectFolder(path);
        var relative = project.Length == 0 ? path : path[(project.Length + 1)..];
        var segments = relative.Split('/');
        var root = segments.Length > 1 && segments[0].Equals(folder, StringComparison.OrdinalIgnoreCase) ? segments[0]
            : segments.Length > 3 && segments[0].Equals("Areas", StringComparison.OrdinalIgnoreCase) && segments[2].Equals(folder, StringComparison.OrdinalIgnoreCase) ? string.Join("/", segments[..3])
            : null;
        if (root is not null)
        {
            return Join(project, root);
        }

        // The folders above the file, nearest first.
        var index = project.Length == 0 && segments.Length > 1 ? Array.FindLastIndex(segments, segments.Length - 2, segments.Length - 1, x => x.Equals(folder, StringComparison.OrdinalIgnoreCase)) : -1;
        return index < 0 ? null : string.Join("/", segments[..(index + 1)]);
    }

    private static string Pascal(string kebab) =>
        string.Concat(kebab.Split('-', StringSplitOptions.RemoveEmptyEntries).Select(x => char.ToUpperInvariant(x[0]) + x[1..]));
}
