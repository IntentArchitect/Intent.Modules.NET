using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

namespace Intent.Modules.Common.DriftEngine.Stacks.React;

/// <summary>
/// Reads the User Interface claims (the <c>intent.ui</c> pack's kinds) of a React front end's components: function and class
/// components in TypeScript or JavaScript, their JSX, and react-router's routes, written as route objects
/// (<c>createBrowserRouter</c>, <c>useRoutes</c>) or as <c>&lt;Route&gt;</c> elements. It reads the claims, never the designer:
/// an element is the code its <c>type.exists</c> claim located, and the vocabulary's bindings say what kind of code that is. It
/// answers only for React components; stores, modules and proxies, which any TypeScript front end writes alike, are
/// <c>intent.typescript.ui</c>'s. A route a file's path gives (Next.js, Remix) isn't read.
/// </summary>
public sealed class ReactExtension : IDriftExtension
{
    public string Id => "intent.react";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new Components(), new Members(), new ReactLinks()];
}

/// <summary>What the React readers share: what a React component is, its markup and units, and react-router's routes.</summary>
internal static class ReactCode
{
    /// <summary>A React component: a component the TypeScript language read from a TypeScript or JavaScript file, not a Vue one.</summary>
    public static bool IsReact(TypeNode? type) =>
        type is { Kind: "component" } && UiCode.IsTypeScript(type) && !type.Files.Any(x => x.EndsWith(".vue", StringComparison.OrdinalIgnoreCase));

    /// <summary>The markup the component writes: every element of its file's JSX inside its own declaration, at any depth.</summary>
    public static IReadOnlyList<MarkupNode> Markup(CodeGraph code, TypeNode component) =>
        UiCode.FileOf(code, component.At.File) is { } file ? UiCode.Markup(file).Where(x => Within(x.At, component.At)).ToList() : [];

    private static bool Within(Span at, Span declared) =>
        at.File == declared.File && at.Line >= declared.Line && at.Line <= (declared.EndLine ?? declared.Line);

    /// <summary>
    /// The component's own code: its render body and the functions it declares, and its route module's data functions
    /// (react-router's <c>loader</c> and <c>action</c>, exported beside it), which run for its route.
    /// </summary>
    public static IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> Units(CodeGraph code, TypeNode component) =>
        component.Members.Where(x => x.Body is not null).Select(x => (component, (MemberNode?)x, x.Body!))
            .Concat(code.Types.Where(x => x.Kind == "module" && x.Files.Contains(component.At.File))
                .SelectMany(module => module.Members.Where(x => x is { Kind: "method", Body: not null, Name: "loader" or "action" or "clientLoader" or "clientAction" })
                    .Select(x => (module, (MemberNode?)x, x.Body!))))
            .ToList();

    /// <summary>
    /// A page whose route its file's path gives, which isn't read: no react-router route is in the code, and the file is a
    /// Next.js <c>app/**/page</c> or <c>pages/</c> file, or a Remix or React Router framework <c>app/routes/</c> file.
    /// </summary>
    public static bool FileRouted(CodeGraph code, TypeNode component) =>
        Records(code).Count == 0 && Regex.IsMatch(component.At.File, @"(^|/)(app/(.+/)?page\.[jt]sx?$|pages/|app/routes/)", RegexOptions.IgnoreCase);

    /// <summary>
    /// react-router's routes: route objects (a <c>path</c> or <c>index</c> with an <c>element</c>, <c>Component</c> or
    /// <c>lazy</c>), and <c>&lt;Route&gt;</c> elements (<c>path</c> or <c>index</c>, with <c>element</c>, <c>Component</c>,
    /// react-router 5's <c>component</c>, or a child element), each nested route's path joined to its parents'.
    /// </summary>
    public static IReadOnlyList<UiCode.RouteRecord> Records(CodeGraph code)
    {
        if (code.Get<ReactRoutes>() is { } cached)
        {
            return cached.Records;
        }

        var records = UiCode.Records(code, "react-router", record =>
            record.Members.FirstOrDefault(x => x.Member is "element" or "Component" or "lazy") is { } mounted
                ? (Mounted(code, mounted.Value.At.File, mounted.Value.Text, mounted.Value.At), true)
                : (null, false)).ToList();
        foreach (var file in code.Files)
        {
            foreach (var root in file.Markup)
            {
                Elements(root, "");
            }

            void Elements(MarkupNode element, string parent)
            {
                if (element.Tag != "Route")
                {
                    foreach (var child in element.Children)
                    {
                        Elements(child, parent);
                    }

                    return;
                }

                var path = element.Attributes.FirstOrDefault(x => x.Name == "path")?.Literal switch
                {
                    null => null,
                    { Kind: "interpolated", Parts: [{ Literal: null } computed] } => UiCode.Text(code.StringOf(computed)) ?? UiCode.Hole.ToString(),
                    var written => UiCode.Text(written) ?? UiCode.Hole.ToString()
                };
                var index = element.Attributes.Any(x => x.Name == "index" && (x.Literal is null || x.Literal.Value == "true"));
                var full = path is null ? parent : UiCode.Joined(parent, path);
                var routes = element.Children.Where(x => x.Tag == "Route").ToList();
                if (path is not null || index)
                {
                    // element={<Page />}, Component={Page}, component={Page}, or <Route path="/x"><Page /></Route>.
                    var given = element.Attributes.FirstOrDefault(x => x.Name is "element" or "Component" or "component" or "render");
                    var component = given?.Literal?.Parts is [var value]
                        ? Mounted(code, file.Path, value.Text, value.At)
                        : element.Children.FirstOrDefault(x => x.Tag != "Route") is { } child ? Chain(code, file.Path, child) : null;
                    if (given is not null || component is not null)
                    {
                        // An index route under a pathless layout route is the root.
                        records.Add(new UiCode.RouteRecord(full.Length == 0 ? "/" : full, component, null, null, element.At, routes.Count > 0));
                    }
                }

                foreach (var route in routes)
                {
                    Elements(route, full);
                }
            }
        }

        code.Set(new ReactRoutes(records));
        return records;
    }

    private sealed record ReactRoutes(IReadOnlyList<UiCode.RouteRecord> Records);

    /// <summary>
    /// The component a route mounts: an element's (<c>&lt;Page /&gt;</c>, through a wrapper of one element such as
    /// <c>&lt;RequireAuth&gt;&lt;Page /&gt;&lt;/RequireAuth&gt;</c>), a name's, or a lazy import's file's.
    /// </summary>
    private static TypeNode? Mounted(CodeGraph code, string file, string text, Span at)
    {
        // The element written as the value, also inside parentheses.
        var roots = UiCode.FileOf(code, file)?.Markup ?? [];
        if ((roots.FirstOrDefault(x => x.At == at) ?? roots.FirstOrDefault(x => Within(x.At, at))) is { } element)
        {
            return Chain(code, file, element);
        }

        return Named(code, file, text);
    }

    /// <summary>An element's component, else, while it wraps one element, the innermost in-scope component it wraps.</summary>
    private static TypeNode? Chain(CodeGraph code, string file, MarkupNode element)
    {
        var found = Named(code, file, element.Tag);
        for (var current = element; current.Children is [var only]; current = only)
        {
            found = Named(code, file, only.Tag) ?? found;
        }

        return found;
    }

    /// <summary>A component a name or lazy import stands for, also through a <c>lazy(() =&gt; import('./Page'))</c> variable the file holds.</summary>
    private static TypeNode? Named(CodeGraph code, string file, string text)
    {
        if (UiCode.ComponentNamed(code, file, text) is { } component)
        {
            return component;
        }

        return Regex.IsMatch(text, @"^\w+$")
               && code.Types.Where(x => x.Files.Contains(file)).SelectMany(x => x.Members).FirstOrDefault(x => x.Kind == "field" && x.Name == text)?.Value is { } held
               && held.Text.Contains("import(", StringComparison.Ordinal)
            ? UiCode.ComponentNamed(code, file, held.Text)
            : null;
    }
}

/// <summary>
/// What a React component is (a page a route mounts, a layout hosting an <c>&lt;Outlet /&gt;</c>), a page's routes and title,
/// and its security.
/// </summary>
internal sealed class Components : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["component.kind", "page.route", "page.title", "security", "security.roles", "security.policy"];

    private const string Unsecured = "React has no standard way to secure a component; a house reader says what secures one.";

    public ClaimReading? Read(ClaimContext context)
    {
        var type = context.Type ?? context.TypeOf(context.Element);
        if (!ReactCode.IsReact(type))
        {
            return null;
        }

        var code = context.Code;
        var records = ReactCode.Records(code).Where(x => x.Component == type).ToList();
        // A route with child routes is theirs as much as its own, so a route without any says best what the component is.
        var pages = records.Where(x => !x.Parent).ToList() is { Count: > 0 } leaves ? leaves : records;
        var markup = ReactCode.Markup(code, type!);
        if (context.Kind is "component.kind" or "page.route" && records.Count == 0 && ReactCode.FileRouted(code, type!))
        {
            return ClaimReading.CantVerify("noRule", $"`{type!.At.File}`'s route is its file's path (Next.js, Remix), which isn't read; only react-router's routes are.", type.At);
        }

        switch (context.Kind)
        {
            case "component.kind":
            {
                // A Dialog is a component the code shows as one; nothing in its own code says so.
                if (context.Expected == "Dialog")
                {
                    return ClaimReading.CantVerify("noRule", "A React component's own code doesn't say whether it is a dialog.", type!.At);
                }

                var outlet = markup.FirstOrDefault(x => x.Tag == "Outlet");
                if (records.Any(x => !x.Parent) || records.Count > 0 && outlet is null)
                {
                    return ClaimReading.Compared("Page", pages[0].At, context.Expected == "Page");
                }

                return outlet is not null
                    ? ClaimReading.Compared("Layout", outlet.At, context.Expected == "Layout")
                    : ClaimReading.Absent(type!.At, "Component");
            }
            case "page.route":
                if (pages.FirstOrDefault(x => x.Path.Contains(UiCode.Hole)) is { } computed)
                {
                    return ClaimReading.CantVerify("unreadable", "A route's path is computed.", computed.At);
                }

                return pages.Count == 0
                    ? ClaimReading.Absent(type!.At)
                    : ClaimReading.Stated(string.Join(", ", pages.Select(x => UiCode.ModelRoute(x.Path)).Distinct().Order(StringComparer.Ordinal)), pages[0].At);
            case "page.title":
                return Title(code, type!, markup);
            case "security":
                // Code that guards nothing states None, by saying nothing.
                return context.Expected == "None" ? ClaimReading.Absent(type!.At, "None") : ClaimReading.CantVerify("noRule", Unsecured, type!.At);
            default:
                return ClaimReading.CantVerify("noRule", Unsecured, type!.At);
        }
    }

    /// <summary>
    /// A title the component renders (React 19's <c>&lt;title&gt;</c>, also inside react-helmet's <c>&lt;Helmet&gt;</c>), or its
    /// code sets with <c>document.title =</c>, <c>useTitle(...)</c> or <c>useDocumentTitle(...)</c>.
    /// </summary>
    private static ClaimReading Title(CodeGraph code, TypeNode type, IReadOnlyList<MarkupNode> markup)
    {
        if (markup.FirstOrDefault(x => x.Tag == "title") is { } element)
        {
            return element.Content is { } content ? Titled(Literal(code, content), element.At) : ClaimReading.CantVerify("unreadable", "The page's title is computed.", element.At);
        }

        foreach (var (_, _, unit) in ReactCode.Units(code, type))
        {
            if (unit.Assignments.FirstOrDefault(x => x.Target is "document.title") is { } assigned)
            {
                return Titled(code.StringOf(assigned.Value), assigned.Value.At);
            }

            if (unit.Calls.FirstOrDefault(x => x.Name is "useTitle" or "useDocumentTitle" && x.Arguments.Count > 0) is { } call)
            {
                return Titled(code.StringOf(call.Arguments[0]), call.Arguments[0].At);
            }
        }

        return ClaimReading.Absent(type.At);

        static ClaimReading Titled(LiteralNode literal, Span at) => literal is { Kind: "string", Value: { } text }
            ? ClaimReading.Stated(text, at)
            : ClaimReading.CantVerify("unreadable", "The page's title is computed.", at);
    }

    /// <summary>Markup content as a string: its text, or the one value it holds, evaluated.</summary>
    private static LiteralNode Literal(CodeGraph code, LiteralNode content) =>
        content is { Kind: "interpolated", Parts: [var only] } ? code.StringOf(only) : content;
}

/// <summary>
/// A React component's props, state and functions: the code a Property, Event Emitter or operation is, and what a Property is
/// to its page (bindable, a route or query parameter).
/// </summary>
internal sealed class Members : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.exists", "property.query", "property.route", "property.bindable"];

    public ClaimReading? Read(ClaimContext context)
    {
        var element = context.Element;
        var owner = context.Type ?? (element.ParentElement is { } parent ? context.TypeOf(parent) : null);
        if (!ReactCode.IsReact(owner))
        {
            return null;
        }

        var code = context.Code;
        var kind = UiCode.Bound(context, "Event") is not null ? "event" : UiCode.Bound(context, "Operation") is not null ? "method" : "property";
        var words = UiCode.Words(element.Name);
        // An Event Emitter is a callback prop: Selected or OnSelected is onSelected.
        var member = context.Member ?? owner!.Members.Where(x => x.Kind != "constructor" && (UiCode.Words(x.Name) == words || kind == "event" && UiCode.Words(x.Name) == "on" + words))
            .OrderByDescending(x => Fits(x, kind)).ThenByDescending(x => x.Name == element.Name).FirstOrDefault();
        switch (context.Kind)
        {
            case "member.exists":
            {
                if (member is null)
                {
                    // A page's Property can be what its route gives it: useParams().id, useSearchParams()'s get('q').
                    var what = kind switch { "event" => "an on... callback prop", "method" => "a function", _ => "a prop or state" };
                    return kind == "property" && (Route(owner!, element.Name) ?? Query(owner!, element.Name)) is { } read
                        ? ClaimReading.Found(read, read)
                        : ClaimReading.Missing($"`{owner!.At.File}`", $"{what} of `{owner.Name}` named `{UiCode.Camel(element.Name)}`");
                }

                return Fits(member, kind)
                    ? ClaimReading.Found(member)
                    : ClaimReading.Compared($"a {member.Kind}", member.At, false,
                        $"`{member.Name}` is a {(member.Modifiers.Contains("state") ? "state value" : member.Kind)}, not {kind switch { "event" => "an on... callback prop", "method" => "a function", _ => "a prop or state" }}.");
            }
            case "property.bindable":
            {
                // A controlled prop: value with onChange, or name with onNameChange. What the route gives a page isn't bindable.
                if (member is null)
                {
                    return ClaimReading.Absent(owner!.At, "false");
                }

                var change = owner!.Members.FirstOrDefault(x => x.Kind == "property" && x.Access == "public"
                                                                && (UiCode.Words(x.Name) == $"on{UiCode.Words(member.Name)}change" || member.Name == "value" && x.Name == "onChange"));
                return change is not null && member.Access == "public"
                    ? ClaimReading.Compared("true", member.At, context.Expected == "true")
                    : ClaimReading.Absent(member.At, "false");
            }
            default:
            {
                var at = (context.Kind == "property.route" ? Route(owner!, element.Name) : Query(owner!, element.Name)) is { } read ? read : (Span?)null;
                return at is { } found ? ClaimReading.Compared("true", found, context.Expected == "true") : ClaimReading.Absent(member?.At ?? owner!.At, "false");
            }
        }
    }

    /// <summary>A Property is a prop or state, an operation a function the component declares, an Event Emitter an <c>on...</c> callback prop.</summary>
    private static bool Fits(MemberNode member, string kind) => kind switch
    {
        "event" => member.Kind == "property" && member.Access == "public" && Regex.IsMatch(member.Name, "^on[A-Z]"),
        "method" => member.Kind is "method",
        _ => member.Kind is "property" or "field"
    };

    /// <summary>Where the component reads a route parameter of the name: <c>const { id } = useParams()</c>, <c>useParams().id</c>, <c>params.id</c>.</summary>
    private static Span? Route(TypeNode component, string name)
    {
        var camel = Regex.Escape(UiCode.Camel(name));
        foreach (var unit in component.Members.Select(x => x.Body).OfType<FunctionNode>())
        {
            if (unit.Calls.FirstOrDefault(x => x is { Name: "useParams", Receiver: null }) is not { } call)
            {
                continue;
            }

            var held = Regex.Match(unit.Text, @"(?:const|let|var)\s+(\w+)\s*(?::[^=]+)?=\s*useParams\b") is { Success: true } variable ? Regex.Escape(variable.Groups[1].Value) : null;
            if (Regex.IsMatch(unit.Text, $@"(?:const|let|var)\s*\{{[^}}]*\b{camel}\b[^}}]*\}}\s*(?::[^=]+)?=\s*useParams\b", RegexOptions.IgnoreCase)
                || Regex.IsMatch(unit.Text, $@"useParams\s*(?:<[^>]*>)?\s*\(\s*\)\s*\.\s*{camel}\b", RegexOptions.IgnoreCase)
                || held is not null && Regex.IsMatch(unit.Text, $@"\b{held}\s*(?:\.\s*{camel}\b|\[\s*['""]{camel}['""]\s*\])", RegexOptions.IgnoreCase))
            {
                return call.At;
            }
        }

        return null;
    }

    /// <summary>Where the component reads a query value of the name: <c>useSearchParams()</c>'s, or a <c>URLSearchParams</c>'s, <c>get('q')</c>.</summary>
    private static Span? Query(TypeNode component, string name)
    {
        foreach (var unit in component.Members.Select(x => x.Body).OfType<FunctionNode>())
        {
            var holders = Regex.Matches(unit.Text, @"(?:\[\s*(\w+)[^\]]*\]\s*=\s*useSearchParams\b|(\w+)\s*=\s*new\s+URLSearchParams\b)")
                .Select(x => x.Groups[1].Success ? x.Groups[1].Value : x.Groups[2].Value)
                .ToHashSet(StringComparer.Ordinal);
            if (unit.Calls.FirstOrDefault(x => x is { Name: "get" or "getAll" or "has", Arguments.Count: > 0 } && x.Receiver is { } receiver && holders.Contains(receiver)
                                               && string.Equals(x.Arguments[0].Literal?.Value, name, StringComparison.OrdinalIgnoreCase)) is { } read)
            {
                return read.At;
            }
        }

        return null;
    }
}

/// <summary>
/// What a React component uses, navigates to, opens and calls: a child component's tag in its JSX, else a value import of its
/// file, a <c>&lt;Link&gt;</c>, <c>&lt;NavLink&gt;</c> or <c>&lt;Navigate&gt;</c> <c>to</c> or an <c>&lt;a href&gt;</c>, and what its
/// code does, its hooks and handlers included (<see cref="ScriptLinks"/>).
/// </summary>
internal sealed class ReactLinks : ScriptLinks
{
    protected override bool Answers(TypeNode type) => ReactCode.IsReact(type);

    protected override IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> Units(CodeGraph code, FileNode file, TypeNode type) => ReactCode.Units(code, type);

    protected override IReadOnlyList<MarkupNode> Markup(FileNode file, TypeNode type) =>
        UiCode.Markup(file).Where(x => x.At.Line >= type.At.Line && x.At.Line <= (type.At.EndLine ?? type.At.Line)).ToList();

    protected override Span Start(FileNode file, TypeNode type) => type.At;

    protected override bool UsesByImport => true;

    protected override IEnumerable<(string? Path, Span At)> MarkupLinks(CodeGraph code, IReadOnlyList<MarkupNode> markup)
    {
        foreach (var attribute in markup.SelectMany(x => x.Attributes).Where(x => x.Name is "to" or "href"))
        {
            // to="/cart", to={`/orders/${id}`}, to={target}, to={{ pathname: '/cart' }}.
            if (attribute.Literal is { Kind: "interpolated", Parts: [{ Literal: null } only] })
            {
                var path = only.Members.FirstOrDefault(x => x.Member is "pathname" or "path")?.Value ?? only;
                yield return (UiCode.Text(code.StringOf(path)), attribute.At);
            }
            else
            {
                yield return (UiCode.Text(attribute.Literal), attribute.At);
            }
        }
    }
}
