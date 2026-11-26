using System.Xml.Linq;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Messaging;
using Intent.Modules.Common.DriftEngine.Stacks.Xaml;

namespace Intent.Modules.Common.DriftEngine.Stacks.Maui;

/// <summary>
/// .NET MAUI as the code states it, read once per graph from the XAML views and the C# code: which projects are MAUI apps, what
/// each view type is (its framework base), the Shell's items and their contents, the routes registered with
/// <c>Routing.RegisterRoute</c>, each view's binding context, the types its markup uses, and the navigations a type's code
/// makes. MAUI is in a project when a view's XAML is in its namespace, a file imports one of its namespaces, or the code builds
/// a <c>MauiApp</c>; a view of another XAML framework (WPF, WinUI, Avalonia) is never MAUI's.
/// </summary>
public sealed class MauiSite
{
    /// <summary>The XAML namespaces of .NET MAUI and of Xamarin.Forms, which it continues.</summary>
    private static readonly HashSet<string> XamlNamespaces = new(StringComparer.Ordinal) { "http://schemas.microsoft.com/dotnet/2021/maui", "http://xamarin.com/schemas/2014/forms" };

    /// <summary>Framework bases that make a type a Layout: an app shell, or a host of other pages (a bar and a page stack, tabs, a flyout).</summary>
    private static readonly HashSet<string> LayoutBases = new(StringComparer.Ordinal) { "Shell", "NavigationPage", "TabbedPage", "FlyoutPage" };

    /// <summary>Framework bases that make a type a Page: a screen.</summary>
    private static readonly HashSet<string> PageBases = new(StringComparer.Ordinal) { "ContentPage", "TemplatedPage", "Page" };

    /// <summary>Framework bases that make a type a Dialog: CommunityToolkit.Maui's popup, shown over a page.</summary>
    private static readonly HashSet<string> DialogBases = new(StringComparer.Ordinal) { "Popup" };

    /// <summary>Framework bases that make a type a Component: a view placed inside others.</summary>
    private static readonly HashSet<string> ViewBases = new(StringComparer.Ordinal)
    {
        "ContentView", "TemplatedView", "View", "Layout", "Grid", "StackLayout", "VerticalStackLayout", "HorizontalStackLayout", "AbsoluteLayout",
        "FlexLayout", "SwipeView", "Frame", "Border", "ScrollView", "RefreshView", "ContentPresenter", "Label", "Button", "ImageButton", "Image",
        "Entry", "Editor", "SearchBar", "CheckBox", "RadioButton", "Switch", "Slider", "Stepper", "ProgressBar", "ActivityIndicator", "DatePicker",
        "TimePicker", "Picker", "BoxView", "GraphicsView", "WebView", "CollectionView", "CarouselView", "IndicatorView", "ListView", "TableView",
        "ItemsView", "StructuredItemsView", "SelectableItemsView", "BlazorWebView", "Map"
    };

    private readonly CodeGraph _graph;
    private readonly Dictionary<string, FileNode> _files;
    private readonly HashSet<string> _projects;
    private readonly Dictionary<TypeNode, XamlDocument> _documents;
    private readonly Dictionary<TypeNode, MauiKind?> _kinds = [];
    private List<ShellItem>? _items;
    private List<RegisteredRoute>? _registered;
    private Dictionary<MemberNode, int>? _wrappers;

    private MauiSite(CodeGraph graph)
    {
        _graph = graph;
        _files = graph.Files.GroupBy(x => x.Path, StringComparer.Ordinal).ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
        var views = XamlSyntax.Of(graph)?.Documents.Values.ToList() ?? [];
        _documents = views.Where(x => x.Type is not null).GroupBy(x => x.Type!).ToDictionary(x => x.Key, x => x.First());
        _projects = views.Where(x => XamlNamespaces.Contains(x.Root.Name.NamespaceName)).Select(x => x.Type is { } type ? ProjectOf(type.At.File) : ProjectOf(x.Path))
            .Concat(graph.Files.Where(x => x.Imports.Any(i => IsMauiNamespace(i.Target))).Select(x => x.Project ?? ""))
            .Concat(graph.Types.Where(x => x.Members.Any(m => m.Calls.Any(c => c.Name == "CreateBuilder" && (c.Receiver == "MauiApp" || c.Receiver?.EndsWith(".MauiApp", StringComparison.Ordinal) == true))))
                .Select(x => ProjectOf(x.At.File)))
            .ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>The graph's MAUI reading, read on first use.</summary>
    public static MauiSite Of(CodeGraph graph)
    {
        if (graph.Get<MauiSite>() is not { } site)
        {
            graph.Set(site = new MauiSite(graph));
        }

        return site;
    }

    public CodeGraph Graph => _graph;

    /// <summary>Whether .NET MAUI is in the code of the type's project (the whole code, when no project file is in scope).</summary>
    public bool InApp(TypeNode type) => _projects.Contains(ProjectOf(type.At.File));

    /// <summary>Whether .NET MAUI is in the code at all.</summary>
    public bool HasApp => _projects.Count > 0;

    /// <summary>The view the type's code-behind is the markup half of; null for a view written in C# alone, or no view.</summary>
    public XamlDocument? DocumentOf(TypeNode? type) => type is not null && _documents.TryGetValue(type, out var document) ? document : null;

    /// <summary>Where a view is said to be: its XAML's root element, else its type.</summary>
    public Span At(TypeNode type) => DocumentOf(type) is { } document ? document.At(document.Root) : type.At;

    /// <summary>
    /// What the type is to MAUI, from the first framework base on its base chain: <c>Layout</c> (<c>Shell</c>,
    /// <c>NavigationPage</c>, <c>TabbedPage</c>, <c>FlyoutPage</c>), <c>Page</c> (<c>ContentPage</c>), <c>Dialog</c> (a
    /// <c>Popup</c>) or <c>Component</c> (a view: <c>ContentView</c>, <c>Grid</c>, ...); null for a type that isn't a MAUI
    /// view, or isn't in a MAUI app.
    /// </summary>
    public MauiKind? KindOf(TypeNode? type)
    {
        if (type is null || type.Kind is not ("class" or "record"))
        {
            return null;
        }

        if (_kinds.TryGetValue(type, out var known))
        {
            return known;
        }

        MauiKind? kind = null;
        if (InApp(type))
        {
            foreach (var declaring in new[] { type }.Concat(_graph.Bases(type)))
            {
                var reference = declaring.BaseTypes.FirstOrDefault(x => _graph.TypeFor(x, declaring) is null && Kind(x.Name) is not null);
                if (reference is not null)
                {
                    kind = new MauiKind(Kind(reference.Name)!, reference.Name, declaring);
                    break;
                }
            }
        }

        return _kinds[type] = kind;
    }

    private static string? Kind(string name) =>
        LayoutBases.Contains(name) ? "Layout" : PageBases.Contains(name) ? "Page" : DialogBases.Contains(name) ? "Dialog" : ViewBases.Contains(name) ? "Component" : null;

    /// <summary>A MAUI view: a page, layout, dialog or component of a MAUI app.</summary>
    public bool IsView(TypeNode? type) => KindOf(type) is not null;

    // ------------------------------------------------------------------ Shell

    /// <summary>
    /// Every Shell's items (<c>FlyoutItem</c>, <c>TabBar</c>, <c>ShellItem</c>, a <c>Tab</c> or a bare <c>ShellContent</c>,
    /// which MAUI wraps in an item of its own), with the pages they show.
    /// </summary>
    public IReadOnlyList<ShellItem> ShellItems => _items ??= ReadShellItems();

    /// <summary>A Shell's items.</summary>
    public IReadOnlyList<ShellItem> ItemsOf(TypeNode shell) => ShellItems.Where(x => x.Shell == shell).ToList();

    /// <summary>The Shell item a name names: the one whose Route it is, or, for one with no Route, whose only content's Route is.</summary>
    public ShellItem? ItemNamed(TypeNode shell, string name) => ShellItems.FirstOrDefault(x => x.Shell == shell && x.Name == name);

    private List<ShellItem> ReadShellItems()
    {
        var items = new List<ShellItem>();
        foreach (var (type, document) in _documents)
        {
            if (KindOf(type)?.Base != "Shell")
            {
                continue;
            }

            foreach (var child in document.Root.Elements().Where(x => XamlDocument.TypeOf(x) is not null))
            {
                var tag = child.Name.LocalName;
                if (tag is not ("FlyoutItem" or "TabBar" or "ShellItem" or "Tab" or "ShellContent"))
                {
                    continue;
                }

                var route = Attribute(child, "Route");
                var contents = tag == "ShellContent"
                    ? [Content(document, child, [], null)]
                    : Contents(document, child, route is null ? [] : [route], Single(child)).ToList();
                items.Add(new ShellItem(type, document, child, tag, route ?? (contents.Count == 1 ? contents[0].Route : null), route, contents, document.At(child)));
            }
        }

        return items;
    }

    /// <summary>A Shell item's or tab's contents, through its tabs, each with the routes above it.</summary>
    /// <param name="holder">The item or tab above, when it holds only one content, which then presents it.</param>
    private IEnumerable<ShellContent> Contents(XamlDocument document, XElement parent, IReadOnlyList<string> routes, XElement? holder)
    {
        foreach (var child in parent.Elements().Where(x => XamlDocument.TypeOf(x) is not null))
        {
            if (child.Name.LocalName == "ShellContent")
            {
                yield return Content(document, child, routes, holder);
            }
            else if (child.Name.LocalName == "Tab")
            {
                var route = Attribute(child, "Route");
                foreach (var content in Contents(document, child, route is null ? routes : [.. routes, route], Single(child) ?? holder))
                {
                    yield return content;
                }
            }
        }
    }

    /// <summary>The element when it holds exactly one content, through its tabs.</summary>
    private static XElement? Single(XElement element) =>
        element.Descendants().Count(x => x.Name.LocalName == "ShellContent") == 1 ? element : null;

    private ShellContent Content(XamlDocument document, XElement element, IReadOnlyList<string> routes, XElement? holder)
    {
        var route = Attribute(element, "Route");
        var path = string.Join("/", route is null ? routes : [.. routes, route]);
        var shown = Template(element);
        var icon = Attribute(element, "Icon") ?? (holder is null ? null : Attribute(holder, "Icon"));
        if (icon is not null && XamlMarkupExtension.Parse(icon) is { Name: "StaticResource" or "DynamicResource", Argument: { } key })
        {
            icon = key;
        }

        return new ShellContent(document, element, route, path, Attribute(element, "Title") ?? (holder is null ? null : Attribute(holder, "Title")), icon,
            route ?? (holder is null ? null : Attribute(holder, "Route")), shown, TypeOf(shown), document.At(element));
    }

    /// <summary>
    /// The page a content shows: its <c>ContentTemplate="{DataTemplate views:X}"</c>, else the view in its
    /// <c>ShellContent.ContentTemplate</c> or <c>ShellContent.Content</c>, else the element written inside it.
    /// </summary>
    private static XamlTypeName? Template(XElement content)
    {
        if (Attribute(content, "ContentTemplate") is { } written)
        {
            return XamlMarkupExtension.Parse(written) is { Name: "DataTemplate", Argument: { } argument } ? XamlDocument.TypeOf(argument, content) : null;
        }

        return content.Elements().Where(x => x.Name.LocalName is "ShellContent.ContentTemplate" or "ShellContent.Content")
                   .SelectMany(x => x.Descendants()).Select(XamlDocument.TypeOf).FirstOrDefault(x => x is not null && x.Name != "DataTemplate")
               ?? content.Elements().Select(XamlDocument.TypeOf).FirstOrDefault(x => x is not null);
    }

    // ------------------------------------------------------------------ routes

    /// <summary>Every <c>Routing.RegisterRoute("route", typeof(View))</c> in the code.</summary>
    public IReadOnlyList<RegisteredRoute> RegisteredRoutes => _registered ??= ReadRegisteredRoutes();

    private List<RegisteredRoute> ReadRegisteredRoutes()
    {
        var routes = new List<RegisteredRoute>();
        foreach (var type in _graph.Types)
        {
            foreach (var call in type.Members.SelectMany(x => x.Calls))
            {
                if (call.Name != "RegisterRoute" || call.Receiver is not ("Routing" or "Microsoft.Maui.Controls.Routing" or "Xamarin.Forms.Routing") || call.Arguments.Count < 2
                    || _graph.StringOf(call.Arguments[0]) is not { Kind: "string", Value: { } route } || TypeOf(call.Arguments[1].Text) is not { } written)
                {
                    continue;
                }

                var view = _graph.TypeFor(new TypeRefNode { Text = written, Name = written.Split('.')[^1], QualifiedName = written }, type);
                routes.Add(new RegisteredRoute(route, written.Split('.')[^1], view, call.At));
            }
        }

        return routes;
    }

    /// <summary>The type <c>typeof(X)</c> names.</summary>
    private static string? TypeOf(string written)
    {
        var text = written.Trim();
        return text.StartsWith("typeof(", StringComparison.Ordinal) && text.EndsWith(')') ? text["typeof(".Length..^1].Trim() : null;
    }

    /// <summary>The routes the code shows a page at, each with a leading <c>/</c>: its Shell contents' paths (<c>/Main/Catalog</c>), and the routes registered for it.</summary>
    public IReadOnlyList<(string Route, Span At)> PageRoutes(TypeNode page) =>
        ShellItems.SelectMany(x => x.Contents).Where(x => x.Type == page && x.Path.Length > 0).Select(x => ("/" + x.Path, x.At))
            .Concat(RegisteredRoutes.Where(x => x.Type == page).Select(x => ("/" + x.Route.TrimStart('/'), x.At)))
            .ToList();

    /// <summary>
    /// The page a Shell navigation's route reaches: an absolute one (<c>//Main/Catalog</c>) against the Shell contents' paths
    /// and item names (an item's first content), a relative one (<c>Basket</c>) against the registered routes too. Its query
    /// is no part of it; going back (<c>..</c>) reaches no page of its own.
    /// </summary>
    public TypeNode? Resolve(string route)
    {
        var path = route.Trim().Split('?')[0];
        if (path.StartsWith("..", StringComparison.Ordinal))
        {
            return null;
        }

        path = path.TrimStart('/').TrimEnd('/');
        var content = ShellItems.SelectMany(x => x.Contents).FirstOrDefault(x => x.Path == path) ?? ShellItems.FirstOrDefault(x => x.Name == path)?.Contents.FirstOrDefault();
        return content?.Type ?? RegisteredRoutes.FirstOrDefault(x => x.Route.Trim('/') == path)?.Type;
    }

    // ------------------------------------------------------------------ binding context and markup

    /// <summary>
    /// The binding contexts a view sets: <c>BindingContext = viewModel</c> in its code (a parameter, field or created object,
    /// through a chained assignment), or an object in a <c>&lt;ContentPage.BindingContext&gt;</c> element of its XAML. Not its
    /// <c>x:DataType</c>, which only declares the type its compiled bindings are checked against.
    /// </summary>
    public IReadOnlyList<BindingContext> BindingContexts(TypeNode view)
    {
        var found = new List<BindingContext>();
        foreach (var member in view.Members)
        {
            foreach (var assignment in member.Body?.Assignments ?? [])
            {
                if (assignment.Target is "BindingContext" or "this.BindingContext" or "base.BindingContext" && TypeOfValue(view, member, assignment.Value) is { } reference)
                {
                    found.Add(new BindingContext(_graph.TypeFor(reference, view), reference.Name, assignment.At));
                }
            }
        }

        if (DocumentOf(view) is { } document)
        {
            foreach (var element in document.Root.Elements().Where(x => x.Name.LocalName.EndsWith(".BindingContext", StringComparison.Ordinal)).SelectMany(x => x.Elements()))
            {
                if (XamlDocument.TypeOf(element) is { } name)
                {
                    found.Add(new BindingContext(TypeOf(name), name.Name, document.At(element)));
                }
            }
        }

        return found;
    }

    /// <summary>The type a value assigned to the binding context is: as the code types it, else the parameter, field or creation it is.</summary>
    private TypeRefNode? TypeOfValue(TypeNode view, MemberNode member, ValueNode value)
    {
        if (value.Types.FirstOrDefault(x => _graph.TypeFor(x, view) is not null) is { } typed)
        {
            return typed;
        }

        if ((value.Parameter?.Type ?? value.Creations.FirstOrDefault()?.Type) is { } direct)
        {
            return direct;
        }

        // A chained assignment, BindingContext = _viewModel = viewModel: what is assigned last.
        var name = value.Text.Split('=')[^1].Trim();
        name = name.StartsWith("this.", StringComparison.Ordinal) ? name["this.".Length..] : name;
        return member.Parameters.FirstOrDefault(x => x.Name == name)?.Type
               ?? _graph.MembersOf(view).FirstOrDefault(x => x.Kind is "field" or "property" && x.Name == name)?.Type
               ?? value.Types.FirstOrDefault();
    }

    /// <summary>
    /// The types a view's XAML uses below its root: the objects it creates (<c>&lt;templates:ProductTemplate /&gt;</c>), the
    /// views its data templates make (<c>{DataTemplate views:LoginView}</c>) and the types it names (<c>{x:Type local:Routes}</c>,
    /// a Blazor hybrid app's root component), each of a CLR namespace, in document order. A framework's own type isn't the app's.
    /// </summary>
    public IReadOnlyList<MarkupUse> MarkupUses(TypeNode view)
    {
        var used = new List<MarkupUse>();
        if (DocumentOf(view) is not { } document)
        {
            return used;
        }

        foreach (var element in document.Root.Descendants())
        {
            if (XamlDocument.TypeOf(element) is { Namespace: not null } name)
            {
                used.Add(new MarkupUse(name, TypeOf(name), document.At(element), true));
            }

            foreach (var attribute in element.Attributes().Where(x => !x.IsNamespaceDeclaration))
            {
                foreach (var extension in XamlMarkupExtension.Parse(attribute.Value)?.DescendantsAndSelf() ?? [])
                {
                    if ((IsTypeExtension(extension.Name, element) || extension.Name == "DataTemplate") && extension.Argument is { } argument
                        && XamlDocument.TypeOf(argument, element) is { Namespace: not null } named)
                    {
                        used.Add(new MarkupUse(named, TypeOf(named), document.At(attribute), false));
                    }
                }
            }
        }

        return used;
    }

    /// <summary>The XAML language's <c>{x:Type}</c>, by its prefix's namespace.</summary>
    private static bool IsTypeExtension(string name, XElement scope)
    {
        var colon = name.IndexOf(':');
        return colon > 0 && name[(colon + 1)..] is "Type" or "TypeExtension" && scope.GetNamespaceOfPrefix(name[..colon]) is { } ns && XamlDocument.LanguageNamespaces.Contains(ns.NamespaceName);
    }

    /// <summary>The in-scope type a XAML name of a CLR namespace stands for, by its full name; null for a framework's.</summary>
    public TypeNode? TypeOf(XamlTypeName? name) =>
        name?.Namespace is null ? null : _graph.TypesNamed(name.Name).FirstOrDefault(x => Qualified(x) == name.Qualified);

    // ------------------------------------------------------------------ navigation

    /// <summary>
    /// The navigations a type's code makes: a Shell navigation (<c>Shell.Current.GoToAsync(route)</c>, or a call of the code's
    /// own method that takes the route as a parameter and makes one, such as <c>INavigationService.NavigateToAsync(route)</c>,
    /// matched through what the call runs, whatever it is named), or a page pushed on the navigation stack
    /// (<c>Navigation.PushAsync(new BasketView())</c>).
    /// </summary>
    public IReadOnlyList<Navigation> NavigationsFrom(TypeNode type)
    {
        var navigations = new List<Navigation>();
        var wrappers = Wrappers;
        foreach (var call in _graph.MembersOf(type).SelectMany(x => x.Calls))
        {
            var index = IsShellNavigation(call, type) ? 0 : Wrapped(type, call, wrappers);
            if (index >= 0 && index < call.Arguments.Count)
            {
                var argument = call.Arguments[index];
                var route = _graph.StringOf(argument) is { Kind: "string", Value: { } text } ? text : null;
                navigations.Add(new Navigation(route, argument.Text, route is null ? null : Resolve(route), call.At));
            }
            else if (call.Name is "PushAsync" or "PushModalAsync" && call.Arguments.Count > 0)
            {
                var pushed = MessageCode.Created(_graph, type, call.Arguments[0]).Select(x => _graph.TypeFor(x.Type, type)).FirstOrDefault(IsView);
                navigations.Add(new Navigation(null, call.Arguments[0].Text, pushed, call.At) { Pushed = true });
            }
        }

        return navigations;
    }

    /// <summary>The index of the route parameter of the navigation wrapper the call runs; -1 when it runs none.</summary>
    private int Wrapped(TypeNode type, CallNode call, Dictionary<MemberNode, int> wrappers)
    {
        if (wrappers.Count == 0 || !wrappers.Keys.Any(x => x.Name == call.Name))
        {
            return -1;
        }

        return MessageCode.Bodies(_graph, type, call).Select(x => wrappers.TryGetValue(x.Member, out var index) ? index : -1).FirstOrDefault(x => x >= 0, -1);
    }

    /// <summary>
    /// <c>GoToAsync</c> on the Shell: <c>Shell.Current</c> (or a Shell of the code's, <c>AppShell.Current</c>), something typed as
    /// one, or the Shell's own code.
    /// </summary>
    private bool IsShellNavigation(CallNode call, TypeNode from)
    {
        if (call.Name != "GoToAsync")
        {
            return false;
        }

        if (call.Receiver is null or "this" or "base")
        {
            return KindOf(from)?.Base == "Shell";
        }

        var segments = call.Receiver.Split('.');
        return segments.Any(x => x == "Shell")
               || segments is [.., var owner, "Current"] && _graph.TypesNamed(owner).Any(x => KindOf(x)?.Base == "Shell")
               || call.ReceiverType is { } receiverType && (receiverType.Name == "Shell" || _graph.TypeFor(receiverType, from) is { } typed && KindOf(typed)?.Base == "Shell");
    }

    /// <summary>
    /// The code's own methods that navigate the Shell to a route they are given, each with that parameter's index: one making a
    /// Shell navigation to it (through a local or a <c>ShellNavigationState</c> made from it), or calling another such method
    /// with it. No wrapper is known by name.
    /// </summary>
    private Dictionary<MemberNode, int> Wrappers => _wrappers ??= ReadWrappers();

    private Dictionary<MemberNode, int> ReadWrappers()
    {
        var wrappers = new Dictionary<MemberNode, int>();
        var methods = _graph.Types.Where(x => x.Kind is "class" or "record" or "struct").SelectMany(x => x.Members.Where(m => m.Kind == "method" && m.Body is not null).Select(m => (Type: x, Method: m))).ToList();
        foreach (var (type, method) in methods)
        {
            foreach (var call in method.Calls.Where(x => x.Arguments.Count > 0 && IsShellNavigation(x, type)))
            {
                if (ParameterIndex(method, call.Arguments[0], 0) is var index and >= 0)
                {
                    wrappers[method] = index;
                    break;
                }
            }
        }

        // A method handing its route on to a wrapper is one too.
        for (var round = 0; round < 4; round++)
        {
            var added = false;
            foreach (var (type, method) in methods.Where(x => !wrappers.ContainsKey(x.Method)))
            {
                foreach (var call in method.Calls)
                {
                    if (Wrapped(type, call, wrappers) is var at and >= 0 && at < call.Arguments.Count && ParameterIndex(method, call.Arguments[at], 0) is var index and >= 0)
                    {
                        wrappers[method] = index;
                        added = true;
                        break;
                    }
                }
            }

            if (!added)
            {
                break;
            }
        }

        return wrappers;
    }

    /// <summary>The index of the method's parameter a value comes from: the parameter, a local or a value made from it.</summary>
    private static int ParameterIndex(MemberNode method, ValueNode value, int depth)
    {
        var parameters = method.Body?.Parameters is { Count: > 0 } own ? own : method.Parameters;
        if (value.Parameter is { } parameter)
        {
            for (var i = 0; i < parameters.Count; i++)
            {
                if (ReferenceEquals(parameters[i], parameter) || parameters[i].Name == parameter.Name)
                {
                    return i;
                }
            }
        }

        if (depth >= 3)
        {
            return -1;
        }

        return value.Creations.SelectMany(x => x.Arguments).Concat(value.Given).Concat(value.Joined)
            .Select(x => ParameterIndex(method, x, depth + 1)).FirstOrDefault(x => x >= 0, -1);
    }

    // ------------------------------------------------------------------ helpers

    private string ProjectOf(string file) => _files.GetValueOrDefault(file)?.Project ?? "";

    private static bool IsMauiNamespace(string ns) =>
        ns == "Microsoft.Maui" || ns.StartsWith("Microsoft.Maui.", StringComparison.Ordinal) || ns == "Xamarin.Forms" || ns.StartsWith("Xamarin.Forms.", StringComparison.Ordinal);

    internal static string Qualified(TypeNode type) => type.Namespace.Length > 0 ? $"{type.Namespace}.{type.Name}" : type.Name;

    /// <summary>An attribute of no namespace, by its name.</summary>
    internal static string? Attribute(XElement element, string name) =>
        element.Attributes().FirstOrDefault(x => x.Name.NamespaceName.Length == 0 && x.Name.LocalName == name)?.Value;
}

/// <summary>What a type is to MAUI: <c>Layout</c>, <c>Page</c>, <c>Dialog</c> or <c>Component</c>, from the framework base <see cref="Base"/> that <see cref="Declaring"/> derives from.</summary>
public sealed record MauiKind(string Kind, string Base, TypeNode Declaring);

/// <summary>A Shell item (<c>FlyoutItem</c>, <c>TabBar</c>, ...): its name (its Route, or its only content's) and the pages it shows.</summary>
public sealed record ShellItem(TypeNode Shell, XamlDocument Document, XElement Element, string Tag, string? Name, string? Route, IReadOnlyList<ShellContent> Contents, Span At);

/// <summary>
/// A <c>ShellContent</c>: its route, its path below the Shell (<c>Main/Catalog</c>), how its tab or flyout entry presents it
/// (its title, icon and key: its own, else those of the tab or item holding it alone) and the page it shows.
/// </summary>
public sealed record ShellContent(XamlDocument Document, XElement Element, string? Route, string Path, string? Title, string? Icon, string? Key, XamlTypeName? Content, TypeNode? Type, Span At);

/// <summary>A <c>Routing.RegisterRoute(route, typeof(View))</c>.</summary>
public sealed record RegisteredRoute(string Route, string Written, TypeNode? Type, Span At);

/// <summary>A view's binding context: its type when the code declares it, the name the code writes, and where it is set.</summary>
public sealed record BindingContext(TypeNode? Type, string Name, Span At);

/// <summary>A type a view's XAML uses: as named, the type it is, where, and whether the markup creates an object of it.</summary>
public sealed record MarkupUse(XamlTypeName Name, TypeNode? Type, Span At, bool Instance);

/// <summary>A navigation: its route (null when the code doesn't state it as text), its argument as written, and the page it reaches.</summary>
public sealed record Navigation(string? Route, string Written, TypeNode? Target, Span At)
{
    /// <summary>A page pushed on the navigation stack, not a route.</summary>
    public bool Pushed { get; init; }
}
