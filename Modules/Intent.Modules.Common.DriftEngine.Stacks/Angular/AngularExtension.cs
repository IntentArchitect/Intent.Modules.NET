using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Markup;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

namespace Intent.Modules.Common.DriftEngine.Stacks.Angular;

/// <summary>
/// Reads the User Interface claims (the <c>intent.ui</c> pack's kinds) of an Angular front end: its components (classes an
/// <c>@Component</c> decorator declares, which the TypeScript language reads as components), their templates (the files a
/// <c>templateUrl</c> names, claimed for Angular file by file, or an inline <c>template</c>), and the router's route tables. It
/// reads the claims, never the designer, and answers only for Angular components; stores, services and proxies, which any
/// TypeScript front end writes alike, are <c>intent.typescript.ui</c>'s. Which route guards secure a route is the house's to
/// say: Angular has no standard one, so a house reader reads <c>security</c> from <see cref="AngularRoutes"/>' guards.
/// </summary>
public sealed class AngularExtension : IDriftExtension
{
    public string Id => "intent.angular";

    public IEnumerable<ILanguage> Languages { get; } = [new AngularTemplates()];

    public IEnumerable<ICodeReader> Readers { get; } = [new AngularInlineTemplates(), new AngularMembers()];

    // Asked from the end: dialogs before the shared links, which read a Show Dialog as a tag.
    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new AngularLinks(), new AngularDialogs(), new AngularComponents(), new AngularInputs(), new AngularRegions()];
}

/// <summary>A route guard as a route lists it: its name, and a guard factory's arguments (<c>roleGuard(ROLES.ADMIN)</c>).</summary>
public sealed record AngularGuard(string Name, IReadOnlyList<ValueNode> Arguments, Span At);

/// <summary>
/// A route that shows a component, with what it inherits from the routes above it.
/// </summary>
/// <param name="Path">The full path, its parents' joined, as the router writes it: <c>/pizzerias/:id</c>.</param>
/// <param name="Component">The component it shows, when the code names one in scope.</param>
/// <param name="Title">Its <c>title</c>, when a string; <see cref="UiCode.Hole"/> when the code computes it (a resolver).</param>
/// <param name="Guards">Its guards and its parents', <c>canMatch</c>, <c>canActivate</c> and <c>canActivateChild</c>.</param>
/// <param name="Data">The keys of its <c>data</c>.</param>
/// <param name="Parent">It has child routes (<c>children</c>, <c>loadChildren</c>), so what it shows hosts theirs.</param>
public sealed record AngularRoute(string Path, TypeNode? Component, string? Title, IReadOnlyList<AngularGuard> Guards, IReadOnlyList<string> Data, bool Parent, Span At)
{
    /// <summary>The routes above it whose paths its path joins, outermost first: a <c>loadChildren</c> entry in another file among them.</summary>
    public IReadOnlyList<Span> Parents { get; init; } = [];
}

/// <summary>
/// An Angular application's routes as its code states them, read once per run: the route tables <c>provideRouter</c> and
/// <c>RouterModule.forRoot</c> are given, following <c>children</c> and <c>loadChildren</c> into the files they load, and a
/// route table nothing reaches from them as a root of its own. Public, so a house reader can read the guards it names.
/// </summary>
public static class AngularRoutes
{
    /// <summary>Every route that shows a component.</summary>
    public static IReadOnlyList<AngularRoute> Of(CodeGraph code) => Site(code).Routes;

    /// <summary>The routes that show the component.</summary>
    public static IReadOnlyList<AngularRoute> Of(CodeGraph code, TypeNode component) => Site(code).Routes.Where(x => x.Component == component).ToList();

    /// <summary>The static redirects: a full path to the full path it redirects to.</summary>
    public static IReadOnlyDictionary<string, string> Redirects(CodeGraph code) => Site(code).Redirects;

    internal static AngularSite Site(CodeGraph code)
    {
        if (code.Get<AngularSite>() is { } cached)
        {
            return cached;
        }

        var site = AngularSite.Read(code);
        code.Set(site);
        return site;
    }
}

/// <summary>What the Angular readers share, read once per run.</summary>
internal sealed class AngularSite
{
    private static readonly Regex Lazy = new(@"import\(\s*['""`]([^'""`]+)['""`]\s*\)(?:\s*\.then\(\s*\(?\s*(\w+)\s*\)?\s*=>\s*\2\s*\.\s*(\w+))?", RegexOptions.Compiled);

    public List<AngularRoute> Routes { get; } = [];

    public Dictionary<string, string> Redirects { get; } = new(StringComparer.Ordinal);

    /// <summary>The components <c>bootstrapApplication</c> or an NgModule's <c>bootstrap</c> starts the app with.</summary>
    public HashSet<TypeNode> Bootstrapped { get; } = [];

    /// <summary>Each component a dialog service opens (<c>dialog.open(ConfirmDialog, ...)</c>), with the calls that open it.</summary>
    public Dictionary<TypeNode, List<CallNode>> Opened { get; } = [];

    /// <summary>The router binds route parameters, query parameters and data to a routed component's inputs (<c>withComponentInputBinding()</c>).</summary>
    public bool BindsInputs { get; private set; }

    public static AngularSite Read(CodeGraph code)
    {
        var site = new AngularSite();
        var calls = AngularCode.Calls(code).ToList();
        site.BindsInputs = calls.Any(x => x.Name == "withComponentInputBinding")
                           || calls.Any(x => x.Name == "forRoot" && x.Arguments.ElementAtOrDefault(1)?.Members.Any(m => m.Member == "bindToComponentInputs" && m.Value.Literal?.Value == "true") == true);

        foreach (var call in calls.Where(x => x is { Name: "bootstrapApplication", Arguments.Count: > 0 }))
        {
            if (UiCode.ComponentNamed(code, call.At.File, call.Arguments[0].Text) is { } component)
            {
                site.Bootstrapped.Add(component);
            }
        }

        foreach (var module in code.Types.Where(x => UiCode.IsTypeScript(x) && x.Attributes.Any(a => a.Name == "NgModule")))
        {
            var bootstrap = module.Attributes.First(a => a.Name == "NgModule").Values.FirstOrDefault(x => x.Name == "bootstrap");
            foreach (var item in bootstrap?.Items ?? [])
            {
                if (UiCode.ComponentNamed(code, module.At.File, item.Text) is { } component)
                {
                    site.Bootstrapped.Add(component);
                }
            }
        }

        foreach (var call in calls.Where(x => x is { Name: "open", Arguments.Count: > 0 } && IsDialogService(x)))
        {
            if (UiCode.ComponentNamed(code, call.At.File, call.Arguments[0].Text) is { } dialog)
            {
                (site.Opened.TryGetValue(dialog, out var list) ? list : site.Opened[dialog] = []).Add(call);
            }
        }

        site.ReadRoutes(code, calls);
        return site;
    }

    /// <summary>A call made on Angular CDK's <c>Dialog</c> or Material's <c>MatDialog</c>: typed as one, or injected as one.</summary>
    private static bool IsDialogService(CallNode call) => call.ReceiverType?.Name is "Dialog" or "MatDialog";

    private void ReadRoutes(CodeGraph code, IReadOnlyList<CallNode> calls)
    {
        var reached = new HashSet<ValueNode>(ReferenceEqualityComparer.Instance);
        var roots = calls.Where(x => x.Name is "provideRouter" or "forRoot" or "forChild" && x.Arguments.Count > 0)
            .Select(x => Table(code, x.At.File, x.Arguments[0]))
            .OfType<ValueNode>()
            .ToList();
        foreach (var root in roots)
        {
            Walk(code, root, "", [], [], reached, 0);
        }

        // A route table nothing reaches from the router's: a root of its own, as the code may provide its router out of scope.
        foreach (var table in code.Types.Where(UiCode.IsTypeScript).SelectMany(x => x.Members).Select(x => x.Value).OfType<ValueNode>().Where(IsTable).ToList())
        {
            if (!reached.Contains(table))
            {
                Walk(code, table, "", [], [], reached, 0);
            }
        }
    }

    /// <summary>An array of route objects: an item with a <c>path</c> and what it shows, redirects to or loads.</summary>
    private static bool IsTable(ValueNode value) =>
        value.Items.Any(x => x.Members.Any(m => m.Member == "path") && x.Members.Any(m => m.Member is "component" or "loadComponent" or "redirectTo" or "children" or "loadChildren"));

    /// <summary>The route table a value is: an array written in place, or the constant it names, in its file or imported.</summary>
    private static ValueNode? Table(CodeGraph code, string file, ValueNode value) =>
        value.Items.Count > 0 ? value : value.Field?.Member.Value is { } field && IsTable(field) ? field : AngularCode.Constant(code, file, value.Text) is { } named && IsTable(named) ? named : null;

    private void Walk(CodeGraph code, ValueNode table, string prefix, IReadOnlyList<AngularGuard> inherited, IReadOnlyList<Span> parents, HashSet<ValueNode> reached, int depth)
    {
        if (depth > 12 || !reached.Add(table))
        {
            return;
        }

        foreach (var route in table.Items.Where(x => x.Members.Count > 0))
        {
            ValueNode? Member(string name) => route.Members.FirstOrDefault(x => x.Member == name)?.Value;

            var path = Member("path") is { } written ? UiCode.Text(code.StringOf(written)) ?? UiCode.Hole.ToString() : null;
            var full = Join(prefix, path ?? "");
            var guards = inherited.Concat(new[] { "canMatch", "canActivate", "canActivateChild" }.SelectMany(x => Guards(Member(x)))).ToList();
            if (Member("redirectTo") is { } redirect && UiCode.Text(code.StringOf(redirect)) is { } to && !to.Contains(UiCode.Hole))
            {
                Redirects[full] = to.StartsWith('/') ? Join("", to) : Join(prefix, to);
            }

            var children = Member("children") is { } nested ? Table(code, route.At.File, nested) : null;
            var loaded = Member("loadChildren") is { } load ? Loaded(code, route.At.File, load.Text) : null;
            var file = route.At.File;
            var component = Member("component") is { } shown ? UiCode.ComponentNamed(code, file, shown.Text)
                : Member("loadComponent") is { } lazy ? LoadedComponent(code, file, lazy.Text) : null;
            if (component is not null || Member("component") is not null || Member("loadComponent") is not null)
            {
                var title = Member("title") is { } titled ? UiCode.Text(code.StringOf(titled)) ?? UiCode.Hole.ToString() : null;
                var data = Member("data")?.Members.Select(x => x.Member).ToList() ?? [];
                Routes.Add(new AngularRoute(full, component, title, guards, data, children is not null || loaded is not null, route.At) { Parents = parents });
            }

            if (children is not null)
            {
                Walk(code, children, full, guards, [.. parents, route.At], reached, depth + 1);
            }

            if (loaded is not null)
            {
                Walk(code, loaded, full, guards, [.. parents, route.At], reached, depth + 1);
            }
        }
    }

    /// <summary>A route's guards: each function named, or a guard factory called (<c>roleGuard(ROLES.ADMIN)</c>) with its arguments.</summary>
    private static IEnumerable<AngularGuard> Guards(ValueNode? list) =>
        list?.Items.Select(x => x.Call is { } call ? new AngularGuard(call.Name, call.Arguments, x.At) : new AngularGuard(x.Text[(x.Text.LastIndexOf('.') + 1)..], [], x.At)) ?? [];

    /// <summary>The route table <c>loadChildren: () =&gt; import('./admin.routes').then((m) =&gt; m.ADMIN_ROUTES)</c> loads: the named constant, else the file's default export.</summary>
    private static ValueNode? Loaded(CodeGraph code, string from, string text)
    {
        if (Lazy.Match(text) is not { Success: true } match || UiCode.FileLoaded(code, from, match.Groups[1].Value) is not { } file)
        {
            return null;
        }

        var members = code.Types.Where(x => x.Files.Contains(file)).SelectMany(x => x.Members).Where(x => x.Value is not null).ToList();
        var named = match.Groups[3].Success ? members.FirstOrDefault(x => x.Name == match.Groups[3].Value) : members.FirstOrDefault(x => x.Name == "default") ?? members.FirstOrDefault(x => IsTable(x.Value!));
        return named?.Value is { } value && IsTable(value) ? value : null;
    }

    /// <summary>The component <c>loadComponent: () =&gt; import('./cart-page').then((m) =&gt; m.CartPage)</c> loads.</summary>
    private static TypeNode? LoadedComponent(CodeGraph code, string from, string text)
    {
        if (Lazy.Match(text) is not { Success: true } match || UiCode.FileLoaded(code, from, match.Groups[1].Value) is not { } file)
        {
            return null;
        }

        var components = code.Types.Where(x => x.Kind == "component" && x.Files.Contains(file)).ToList();
        return match.Groups[3].Success ? components.FirstOrDefault(x => x.Name == match.Groups[3].Value) : components.Count == 1 ? components[0] : null;
    }

    /// <summary>A child path under its parent's, as the router joins them: <c>/pizzerias</c> and <c>:id</c> are <c>/pizzerias/:id</c>.</summary>
    internal static string Join(string prefix, string path)
    {
        var segments = prefix.Split('/', StringSplitOptions.RemoveEmptyEntries).Concat(path.Split('/', StringSplitOptions.RemoveEmptyEntries));
        return "/" + string.Join("/", segments);
    }
}

/// <summary>What Angular's readers share: what an Angular component is, its template's markup and units.</summary>
internal static class AngularCode
{
    /// <summary>An Angular component: a class the TypeScript language read whose <c>@Component</c> decorator declares one.</summary>
    public static bool IsAngular(TypeNode? type) => type is { Kind: "component" } && UiCode.IsTypeScript(type) && type.Attributes.Any(x => x.Name == "Component");

    /// <summary>
    /// A member a parent binds: an <c>input()</c>, <c>output()</c> or <c>model()</c> (<c>input.required()</c> and
    /// <c>model.required()</c> included), an <c>outputFromObservable()</c>, or an <c>@Input</c> or <c>@Output</c>.
    /// </summary>
    public static bool IsBinding(MemberNode member) =>
        member.Attributes.Any(x => x.Name is "Input" or "Output")
        || member.Value?.Call is { } call
        && (call.Name == "required" ? call.Receiver : string.IsNullOrEmpty(call.Receiver) ? call.Name : null) is "input" or "output" or "model" or "outputFromObservable";

    /// <summary>The component's decorator argument, as written: its <c>selector</c>, <c>templateUrl</c> or <c>template</c>.</summary>
    public static string? Decorated(TypeNode type, string name) =>
        type.Attributes.FirstOrDefault(x => x.Name == "Component")?.Named.GetValueOrDefault(name) is { Length: > 0 } value ? value : null;

    /// <summary>The template file a component's <c>templateUrl</c> names, from its TypeScript file.</summary>
    public static string? TemplateOf(TypeNode type) =>
        Decorated(type, "templateUrl") is { } url && type.Files.FirstOrDefault(UiCode.IsTypeScript) is { } file ? UiCode.Resolve(file, url) : null;

    /// <summary>The component's selectors, a comma-separated list read one by one: <c>rw-header</c>, <c>tr[rw-admin-order-row]</c>.</summary>
    public static IEnumerable<CssSelector> Selectors(TypeNode type) => Decorated(type, "selector") is { } selector ? CssSelector.ParseList(selector) : [];

    /// <summary>
    /// The markup the component renders, every element at any depth: its template file's, which joined its files
    /// (<see cref="AngularTemplates"/>), or its inline template's (<see cref="AngularInlineTemplates"/>).
    /// </summary>
    public static IReadOnlyList<MarkupNode> Markup(CodeGraph code, TypeNode component) => code.MarkupOf(component).ToList();

    /// <summary>The component's own code: its members' bodies and its fields' initializers (<see cref="UiCode.MemberUnits"/>).</summary>
    public static IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> Units(TypeNode component) => UiCode.MemberUnits(component).ToList();

    /// <summary>The value of a constant a file names: its own module's, else the one it imports of that name.</summary>
    public static ValueNode? Constant(CodeGraph code, string file, string name)
    {
        if (!Regex.IsMatch(name, @"^\w+$"))
        {
            return null;
        }

        if (code.Types.Where(x => x.Files.Contains(file)).SelectMany(x => x.Members).FirstOrDefault(x => x.Name == name && x.Value is not null) is { } own)
        {
            return own.Value;
        }

        var import = UiCode.FileOf(code, file)?.Imports.FirstOrDefault(x => x.Alias == name || x.Alias is null && x.Target.EndsWith("." + name, StringComparison.Ordinal));
        var exported = import?.Target[(import.Target.LastIndexOf('.') + 1)..];
        return import?.File is { } imported
            ? code.Types.Where(x => x.Files.Contains(imported)).SelectMany(x => x.Members).FirstOrDefault(x => x.Name == exported && x.Value is not null)?.Value
            : null;
    }

    /// <summary>Every call in the TypeScript code: in bodies, top-level code and the values its members are given, at any depth.</summary>
    public static IEnumerable<CallNode> Calls(CodeGraph code)
    {
        var seen = new HashSet<CallNode>(ReferenceEqualityComparer.Instance);
        var types = code.Types.Where(UiCode.IsTypeScript).ToList();
        var units = types.SelectMany(x => x.Members).Select(x => x.Body).OfType<FunctionNode>()
            .Concat(code.Files.Where(x => UiCode.IsTypeScript(x.Path)).Select(x => x.TopLevel).OfType<FunctionNode>());
        foreach (var call in units.SelectMany(x => x.Calls))
        {
            if (seen.Add(call))
            {
                yield return call;
            }
        }

        foreach (var value in types.SelectMany(x => x.Members).Select(x => x.Value).OfType<ValueNode>())
        {
            foreach (var call in UiCode.CallsIn(value))
            {
                if (seen.Add(call))
                {
                    yield return call;
                }
            }
        }
    }
}

/// <summary>
/// Angular's template syntax: a tag or attribute a component's <c>selector</c> matches is that component
/// (<c>&lt;rw-header /&gt;</c>, <c>&lt;tr rw-admin-order-row&gt;</c>); <c>[x]</c> binds, <c>(x)</c> listens, <c>[(x)]</c> binds both
/// ways, <c>*x</c> is a structural directive and <c>#x</c> a reference, as are their <c>bind-</c>, <c>on-</c>, <c>bindon-</c> and
/// <c>ref-</c> forms; and a plain attribute's value interpolates <c>{{ }}</c>.
/// </summary>
internal sealed class AngularDialect(CodeGraph code) : VueTemplate.MarkupDialect
{
    private readonly List<(CssSelector Selector, TypeNode Type)> _selectors =
        code.Types.Where(AngularCode.IsAngular).SelectMany(x => AngularCode.Selectors(x).Select(s => (s, x))).ToList();

    public override bool Interpolates => true;

    public override TypeRefNode? ComponentOf(MarkupNode element) =>
        _selectors.FirstOrDefault(x => x.Selector.Matches(element)).Type is { } type
            ? new TypeRefNode { Text = element.Tag, Name = type.Name, QualifiedName = type.Namespace.Length == 0 ? type.Name : $"{type.Namespace}.{type.Name}", Language = TypeScriptLanguage.LanguageName }
            : null;

    public override (string? Directive, string? Argument, List<string> Modifiers) Split(string name) => name switch
    {
        _ when name.StartsWith("[(", StringComparison.Ordinal) && name.EndsWith(")]", StringComparison.Ordinal) => ("model", name[2..^2], []),
        _ when name.StartsWith('[') && name.EndsWith(']') => ("bind", name[1..^1], []),
        _ when name.StartsWith('(') && name.EndsWith(')') => ("on", name[1..^1], []),
        _ when name.StartsWith('*') => ("structural", name[1..], []),
        _ when name.StartsWith('#') => ("ref", name[1..], []),
        _ when name.StartsWith("bindon-", StringComparison.Ordinal) => ("model", name[7..], []),
        _ when name.StartsWith("bind-", StringComparison.Ordinal) => ("bind", name[5..], []),
        _ when name.StartsWith("on-", StringComparison.Ordinal) => ("on", name[3..], []),
        _ when name.StartsWith("ref-", StringComparison.Ordinal) => ("ref", name[4..], []),
        _ => (null, null, [])
    };
}

/// <summary>
/// Angular component templates: an <c>.html</c> file is Angular's when a component's <c>templateUrl</c> names it (code-reader.md,
/// Claimed files). Each is read as markup in Angular's dialect and joins its component's files, as a view joins its code-behind,
/// so the template is the located component's code.
/// </summary>
internal sealed class AngularTemplates : IClaimingLanguage
{
    private static readonly Regex Syntax = new(@"@(if|for|switch|defer|let)\b|\*ng[A-Z]\w*=|\[\(?[\w.\-]+\)?\]=|\(\w[\w.]*\)=", RegexOptions.Compiled);

    public string Name => "angular";

    public string Title => "Angular";

    public bool Reads(string path) => false;

    public bool MayClaim(string path) => path.EndsWith(".html", StringComparison.OrdinalIgnoreCase);

    public bool WrittenIn(string text) => Syntax.IsMatch(text);

    public IEnumerable<FileClaim> Claim(string root, IReadOnlyList<string> files, CodeGraph graph)
    {
        var offered = files.ToHashSet(StringComparer.Ordinal);
        foreach (var component in graph.Types.Where(AngularCode.IsAngular))
        {
            if (AngularCode.TemplateOf(component) is { } template && offered.Contains(template))
            {
                yield return new FileClaim(template, ClaimEvidence.Named, $"the templateUrl of `{component.Name}` ({component.At.File}:{component.At.Line})");
            }
        }
    }

    public void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken)
    {
        var dialect = new AngularDialect(graph);
        var owners = graph.Types.Where(AngularCode.IsAngular).Select(x => (Type: x, Template: AngularCode.TemplateOf(x))).Where(x => x.Template is not null).ToList();
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var text = File.ReadAllText(Path.Combine(root, file));
            graph.Files.Add(new FileNode { Path = file, Language = Name, Markup = VueTemplate.ReadDocument(file, text, dialect) });
            foreach (var (component, _) in owners.Where(x => x.Template == file))
            {
                component.Files.Add(file);
            }
        }
    }
}

/// <summary>
/// Each component's inline <c>template</c>, read as markup in Angular's dialect where the template is written (from the
/// character after its opening quote) and kept on the component (<see cref="TypeNode.Markup"/>), so every reader of its markup,
/// a house module's included, reads it with <see cref="CodeGraph.MarkupOf"/>, as it reads a <c>templateUrl</c> file's.
/// </summary>
internal sealed class AngularInlineTemplates : ICodeReader
{
    public void Read(CodeGraph graph)
    {
        var dialect = new AngularDialect(graph);
        foreach (var component in graph.Types.Where(AngularCode.IsAngular))
        {
            var decorator = component.Attributes.First(x => x.Name == "Component");
            if (AngularCode.TemplateOf(component) is null && AngularCode.Decorated(component, "template") is { } text)
            {
                var at = decorator.Values.FirstOrDefault(x => x.Name == "template")?.At;
                component.Markup.AddRange(at is null
                    ? VueTemplate.ReadDocument(component.At.File, text, dialect, decorator.At.Line, decorator.At.Column ?? 1)
                    : VueTemplate.ReadDocument(component.At.File, text, dialect, at.Line, (at.Column ?? 0) + 1));
            }
        }
    }
}

/// <summary>
/// What a component's members are to the designer, so <c>unmodelled.member</c> asks only for those it can state: Angular's
/// lifecycle hooks are Angular's (<see cref="CodeGraph.Unmodellable"/>), a field an injector gives
/// (<c>private readonly auth = inject(Auth)</c>) is its <c>Uses</c> of that service (<see cref="CodeGraph.StatedByLink"/>), and
/// a component's property that is no binding a parent sets or handles (an <c>input()</c>, <c>output()</c> or <c>model()</c>, or
/// an <c>@Input</c> or <c>@Output</c>) is its own state, written in code: a signal, a computed value, a form, a field its template
/// reads. The designer states a component's Properties and Event Emitters only for its bindings, and a Page's only for its
/// route and query values, as it does a Blazor component's only for its parameters.
/// </summary>
internal sealed class AngularMembers : ICodeReader
{
    private static readonly HashSet<string> Lifecycle =
        ["ngOnChanges", "ngOnInit", "ngDoCheck", "ngAfterContentInit", "ngAfterContentChecked", "ngAfterViewInit", "ngAfterViewChecked", "ngOnDestroy"];

    public void Read(CodeGraph graph)
    {
        foreach (var type in graph.Types.Where(x => UiCode.IsTypeScript(x) && x.Attributes.Any(a => a.Name is "Component" or "Directive" or "Injectable" or "Pipe")))
        {
            var component = AngularCode.IsAngular(type);
            foreach (var member in type.Members)
            {
                if (member.Kind == "method" && Lifecycle.Contains(member.Name))
                {
                    graph.Unmodellable[member] = "Angular calls it for every component: no designer states it";
                }
                else if (member.Kind is "property" or "field" && member.Value?.Call?.Name is "inject" or "injectAsync")
                {
                    graph.StatedByLink[member] = "Uses";
                }
                else if (component && member.Kind is "property" or "field" && !member.IsStatic && !AngularCode.IsBinding(member) && !graph.StatedByLink.ContainsKey(member))
                {
                    graph.Unmodellable[member] = "it is no input, output or model(), so a parent can't bind it";
                }
            }
        }
    }
}

/// <summary>
/// What an Angular component is (a page a route shows, a layout hosting a <c>&lt;router-outlet&gt;</c>, a dialog a dialog
/// service opens), a page's routes and title, and its security as far as Angular states it.
/// </summary>
internal sealed class AngularComponents : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["component.kind", "page.route", "page.title", "security", "security.roles", "security.policy"];

    private const string Unsecured = "Angular has no standard way to say which route guards secure a route; a house reader names them (AngularRoutes gives each route's guards).";

    public ClaimReading? Read(ClaimContext context)
    {
        var type = context.Type ?? context.TypeOf(context.Element);
        if (!AngularCode.IsAngular(type))
        {
            return null;
        }

        var code = context.Code;
        var site = AngularRoutes.Site(code);
        var records = site.Routes.Where(x => x.Component == type).ToList();
        // A route with child routes is theirs as much as its own, so a route without any says best what the component is.
        var pages = records.Where(x => !x.Parent).ToList() is { Count: > 0 } leaves ? leaves : records;
        switch (context.Kind)
        {
            case "component.kind":
            {
                var outlet = AngularCode.Markup(code, type!).FirstOrDefault(x => x.Tag == "router-outlet");
                if (site.Bootstrapped.Contains(type!))
                {
                    return ClaimReading.Compared("Layout", outlet?.At ?? type!.At, context.Expected == "Layout");
                }

                if (records.Any(x => !x.Parent) || records.Count > 0 && outlet is null)
                {
                    return ClaimReading.Compared("Page", pages[0].At, context.Expected == "Page");
                }

                if (records.Count > 0 || outlet is not null)
                {
                    return ClaimReading.Compared("Layout", outlet?.At ?? records[0].At, context.Expected == "Layout");
                }

                if (site.Opened.TryGetValue(type!, out var opened))
                {
                    return ClaimReading.Compared("Dialog", opened[0].At, context.Expected == "Dialog");
                }

                // A dialog shown otherwise, such as rendered behind a flag (<app-modal [isOpen]="...">), says so nowhere in its own
                // code, as a React or Vue one doesn't.
                return context.Expected == "Dialog"
                    ? ClaimReading.CantVerify("noRule", "An Angular component's own code doesn't say whether it is a dialog: only one a dialog service opens (CDK's Dialog, Material's MatDialog) is read as one.", type!.At)
                    : ClaimReading.Absent(type!.At, "Component");
            }
            case "page.route":
                if (pages.FirstOrDefault(x => x.Path.Contains(UiCode.Hole)) is { } computed)
                {
                    return ClaimReading.CantVerify("unreadable", "A route's path is computed.", computed.At);
                }

                return pages.Count == 0
                    ? ClaimReading.Absent(type!.At)
                    : ClaimReading.Stated(string.Join(", ", pages.Select(x => UiCode.ModelRoute(x.Path)).Distinct().Order(StringComparer.Ordinal)), pages[0].At)
                        // The parent routes a path joins are where its start is written, a lazy loadChildren in app.routes.ts among them.
                        .Through(pages.SelectMany(x => x.Parents).Distinct().ToList());
            case "page.title":
                return Title(context, code, type!, pages);
            case "security":
                // A component no guard is on states None by saying nothing; which guards secure a route is the house's to say.
                return context.Expected == "None" && records.All(x => x.Guards.Count == 0)
                    ? ClaimReading.Absent(records.FirstOrDefault()?.At ?? type!.At, "None")
                    : ClaimReading.CantVerify("noRule", Unsecured, records.FirstOrDefault()?.At ?? type!.At);
            default:
                return ClaimReading.CantVerify("noRule", Unsecured, records.FirstOrDefault()?.At ?? type!.At);
        }
    }

    /// <summary>A page's title: its route's <c>title</c> (the one the model names, when its routes differ), else one its code sets with the <c>Title</c> service.</summary>
    private static ClaimReading Title(ClaimContext context, CodeGraph code, TypeNode type, IReadOnlyList<AngularRoute> pages)
    {
        var titled = pages.Where(x => x.Title is not null).ToList();
        if ((titled.FirstOrDefault(x => x.Title == context.Expected) ?? titled.FirstOrDefault()) is { } route)
        {
            return route.Title!.Contains(UiCode.Hole) ? ClaimReading.CantVerify("unreadable", "The page's title is computed (a title resolver).", route.At) : ClaimReading.Stated(route.Title, route.At);
        }

        foreach (var (_, _, unit) in AngularCode.Units(type))
        {
            if (unit.Calls.FirstOrDefault(x => x is { Name: "setTitle", Arguments.Count: > 0 } && (x.ReceiverType?.Name == "Title" || x.Receiver?.EndsWith("title", StringComparison.OrdinalIgnoreCase) == true)) is { } call)
            {
                return code.StringOf(call.Arguments[0]) is { Kind: "string", Value: { } text }
                    ? ClaimReading.Stated(text, call.At)
                    : ClaimReading.CantVerify("unreadable", "The page's title is computed.", call.At);
            }
        }

        return ClaimReading.Absent(pages.FirstOrDefault()?.At ?? type.At);
    }
}

/// <summary>
/// What an Angular component's Property is to its page: <c>[Bindable]</c> when two-way (<c>model()</c>, or an input with an
/// <c>xChange</c> output); a route parameter when the router binds it from its route (<c>withComponentInputBinding()</c>, an input
/// named like a <c>:param</c>) or the code reads it from the <c>ActivatedRoute</c>; a query parameter when the router binds it
/// from the query string (an input of a routed component that is no route parameter and no route <c>data</c>) or the code reads
/// it from the route's query parameters. A page's Property the code reads from the <c>ActivatedRoute</c> without a member of its
/// own (<c>userId = signal(this.route.snapshot.paramMap.get('id'))</c> for the model's <c>id</c>) is where it is read, and its type
/// is the <c>string</c> the router gives (<see cref="UiCode.RouterGivenType"/>), as a React page's <c>useParams()</c> value is.
/// </summary>
internal sealed class AngularInputs : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.exists", "member.type", "member.nullable", "property.query", "property.route", "property.bindable"];

    public ClaimReading? Read(ClaimContext context)
    {
        var owner = context.Type ?? (context.Element.ParentElement is { } parent ? context.TypeOf(parent) : null);
        if (!AngularCode.IsAngular(owner))
        {
            return null;
        }

        var code = context.Code;
        var member = context.Member ?? owner!.Members.FirstOrDefault(x => x.Kind is "property" or "field" && UiCode.Words(x.Name) == UiCode.Words(context.Element.Name));
        var name = member?.Name ?? UiCode.Camel(context.Element.Name);
        var function = member?.Value?.Call is { } call ? call.Name is "required" ? call.Receiver : call.Name : null;
        var decorated = member?.Attributes.Any(x => x.Name == "Input") == true;
        var isInput = function is "input" or "model" || decorated;
        var site = AngularRoutes.Site(code);
        var routes = site.Routes.Where(x => x.Component == owner).ToList();
        var bound = site.BindsInputs && routes.Count > 0 && isInput;
        switch (context.Kind)
        {
            case "member.exists":
                // A member of the Property's name is the engine's own rule to find; what it can't is a value the code only reads.
                return member is null && UiCode.Bound(context, "Property") is not null && Given(owner!, name) is { } read ? ClaimReading.Found(read, read) : null;
            case "member.type":
                return member is null && Given(owner!, name) is { } given ? UiCode.RouterGivenType(context, owner!.Name, given) : null;
            case "member.nullable":
            {
                if (member is not null)
                {
                    return null;
                }

                var paths = routes.Select(x => x.Path).ToList();
                return Read(owner!, name, "paramMap", "params") is { } parameter ? UiCode.RouterGivenNullable(context, owner!.Name, parameter, false, paths)
                    : Read(owner!, name, "queryParamMap", "queryParams") is { } value ? UiCode.RouterGivenNullable(context, owner!.Name, value, true, paths)
                    : null;
            }
            case "property.bindable":
            {
                var change = owner!.Members.Any(x => x.Name == name + "Change" && (x.Value?.Call?.Name == "output" || x.Attributes.Any(a => a.Name == "Output")));
                return function == "model" || decorated && change
                    ? ClaimReading.Compared("true", member!.At, context.Expected == "true")
                    : ClaimReading.Absent(member?.At ?? owner.At, "false");
            }
            case "property.route":
            {
                var at = bound && routes.Any(r => Parameters(r.Path).Contains(name)) ? member!.At : Read(owner!, name, "paramMap", "params");
                return at is { } found ? ClaimReading.Compared("true", found, context.Expected == "true") : ClaimReading.Absent(member?.At ?? owner!.At, "false");
            }
            default:
            {
                var at = bound && !routes.Any(r => Parameters(r.Path).Contains(name) || r.Data.Contains(name)) ? member!.At : Read(owner!, name, "queryParamMap", "queryParams");
                return at is { } found ? ClaimReading.Compared("true", found, context.Expected == "true") : ClaimReading.Absent(member?.At ?? owner!.At, "false");
            }
        }
    }

    private static IEnumerable<string> Parameters(string path) =>
        path.Split('/', StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith(':')).Select(x => x[1..].TrimEnd('?'));

    /// <summary>Where the component reads a route or query value of the name from its <c>ActivatedRoute</c>.</summary>
    private static Span? Given(TypeNode component, string name) => Read(component, name, "paramMap", "params") ?? Read(component, name, "queryParamMap", "queryParams");

    /// <summary>
    /// Where the component reads a value of the name from its <c>ActivatedRoute</c>: <c>paramMap.get('id')</c> (or the query's
    /// <c>queryParamMap</c>), <c>params['id']</c> or <c>params.id</c>, also through its <c>snapshot</c>.
    /// </summary>
    private static Span? Read(TypeNode component, string name, string map, string record)
    {
        var escaped = Regex.Escape(name);
        foreach (var (_, _, unit) in AngularCode.Units(component))
        {
            if (unit.Calls.FirstOrDefault(x => x is { Name: "get", Arguments.Count: > 0 } && x.Receiver?.EndsWith(map, StringComparison.Ordinal) == true
                                               && string.Equals(x.Arguments[0].Literal?.Value, name, StringComparison.Ordinal)) is { } call)
            {
                return call.At;
            }

            if (Regex.IsMatch(unit.Text, $@"\b{record}\s*(?:\[\s*['""]{escaped}['""]\s*\]|\.\s*{escaped}\b)"))
            {
                return unit.At;
            }
        }

        return null;
    }
}

/// <summary>
/// An Angular component's placement points (<see cref="MarkupRegions"/>): its template's <c>&lt;ng-content&gt;</c>, named by
/// what its <c>select</c> names (<c>[slot=footer]</c> and <c>.footer</c> are <c>footer</c>, <c>[actions]</c> is <c>actions</c>,
/// <c>app-card-title</c> is <c>app-card-title</c>), and its <c>&lt;router-outlet&gt;</c>, named by its <c>name</c>. A bare
/// <c>&lt;ng-content /&gt;</c> or <c>&lt;router-outlet /&gt;</c> is the one the template doesn't name.
/// </summary>
internal sealed class AngularRegions : MarkupRegions
{
    protected override string Written => "<ng-content select=\"[{0}]\" />";

    protected override bool Answers(TypeNode type) => AngularCode.IsAngular(type);

    protected override IReadOnlyList<Outlet> Outlets(CodeGraph code, TypeNode type)
    {
        var outlets = new List<Outlet>();
        foreach (var node in AngularCode.Markup(code, type))
        {
            if (node.Tag == "ng-content")
            {
                var select = node.Attributes.FirstOrDefault(x => x is { Directive: null, Name: "select" })?.Value;
                if (string.IsNullOrWhiteSpace(select))
                {
                    outlets.Add(new Outlet(null, node.At));
                    continue;
                }

                // A select this doesn't read names its placement point no Region can be compared with.
                var selectors = CssSelector.ParseList(select).ToList();
                outlets.AddRange(selectors.Count == 0
                    ? [new Outlet(UiCode.Hole.ToString(), node.At)]
                    : selectors.Select(x => new Outlet(Named(x) ?? UiCode.Hole.ToString(), node.At)));
            }
            else if (node.Tag == "router-outlet")
            {
                var name = node.Attributes.FirstOrDefault(x => x is { Directive: null, Name: "name" })?.Value;
                outlets.Add(new Outlet(node.Attributes.Any(x => x is { Directive: "bind", Argument: "name" }) ? UiCode.Hole.ToString() : string.IsNullOrWhiteSpace(name) ? null : name, node.At));
            }
        }

        return outlets;
    }

    /// <summary>What a <c>select</c> names: an attribute's value (<c>[slot=footer]</c>), else the attribute (<c>[footer]</c>), a class, an id or the tag.</summary>
    private static string? Named(CssSelector selector) =>
        selector.Attributes.Count > 0
            ? selector.Attributes[0].Value is { Length: > 0 } value ? value : selector.Attributes[0].Name
            : selector.Classes.FirstOrDefault() ?? selector.Id ?? selector.Tag;
}

/// <summary>
/// A Show Dialog from Angular code: a call of a dialog service's <c>open</c> with the dialog's component
/// (<c>this.dialog.open(ConfirmDialog, { data })</c>, CDK's <c>Dialog</c> or Material's <c>MatDialog</c>), in the source's own
/// code or the code's own functions it calls. Where the source opens it with no service, the dialog is shown where the source
/// renders its tag (<c>&lt;app-modal [isOpen]="showDeleteModal()"&gt;</c>), which the shared reading reads
/// (<see cref="AngularLinks"/>), as it does a Vue or React dialog.
/// </summary>
internal sealed class AngularDialogs : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.opens"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element is not IAssociationEnd end || context.Owner(end) is not { } source || end.TypeReference?.Element is not IElement target
            || UiCode.CodeOf(context, source) is not { } located || !UiCode.IsTypeScript(located.Type) || context.TypeOf(target) is not { } dialog
            || !AngularCode.IsAngular(dialog))
        {
            return null;
        }

        var opened = AngularRoutes.Site(context.Code).Opened.GetValueOrDefault(dialog) ?? [];
        var own = UiCode.MemberUnits(located.Type).Select(x => x.Unit).ToList();
        if (opened.FirstOrDefault(x => own.Any(u => u.Calls.Contains(x)) || located.Type.Files.Contains(x.At.File) && located.Type.Kind != "component") is { } call)
        {
            return ClaimReading.Found(call, call.At);
        }

        return null;
    }
}

/// <summary>
/// What an Angular component uses, navigates to and calls: a child component's tag in its template (by the child's
/// <c>selector</c>), an import of a service or store, a <c>routerLink</c> or <c>href</c>, a router's <c>navigate</c> or
/// <c>navigateByUrl</c>, relative links from its own route and static redirects followed, and what its code calls
/// (<see cref="ScriptLinks"/>).
/// </summary>
internal sealed class AngularLinks : ScriptLinks
{
    protected override bool Answers(TypeNode type) => AngularCode.IsAngular(type);

    protected override IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> Units(CodeGraph code, FileNode file, TypeNode type) => AngularCode.Units(type);

    protected override IReadOnlyList<MarkupNode> Markup(CodeGraph code, FileNode file, TypeNode type) => AngularCode.Markup(code, type);

    protected override Span Start(FileNode file, TypeNode type) => type.At;

    protected override bool Tagged(MarkupNode tag, string name) => tag.Component?.Name == name;

    protected override bool UsesByImport => true;

    protected override string Resolve(CodeGraph code, TypeNode type, string path)
    {
        var redirects = AngularRoutes.Redirects(code);
        if (!path.StartsWith('/'))
        {
            var from = AngularRoutes.Of(code, type).FirstOrDefault()?.Path ?? "/";
            var segments = from.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var part in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                if (part == "..")
                {
                    if (segments.Count > 0)
                    {
                        segments.RemoveAt(segments.Count - 1);
                    }
                }
                else if (part != ".")
                {
                    segments.Add(part);
                }
            }

            path = "/" + string.Join("/", segments);
        }

        for (var hop = 0; hop < 5 && !path.Contains(UiCode.Hole) && redirects.TryGetValue(AngularSite.Join("", path), out var to); hop++)
        {
            path = to;
        }

        return path;
    }

    protected override IEnumerable<(string? Path, Span At)> MarkupLinks(CodeGraph code, TypeNode type, IReadOnlyList<MarkupNode> markup)
    {
        foreach (var attribute in markup.SelectMany(x => x.Attributes))
        {
            if (attribute is { Directive: null, Name: "routerLink" } || attribute is { Directive: null, Name: "href" } && attribute.Value?.StartsWith('/') == true)
            {
                yield return (UiCode.Text(attribute.Literal), attribute.At);
            }
            else if (attribute is { Directive: "bind", Argument: "routerLink" or "href" } && attribute.Value is { } expression)
            {
                foreach (var branch in Branches(expression))
                {
                    yield return (Link(code, type, branch), attribute.At);
                }
            }
        }
    }

    /// <summary>
    /// A template expression's text: a string written in it, else a path of the component's members (<c>authUrls.register</c>,
    /// <c>ROOT_URLS.home</c> held in a field, <c>detailUrl()</c>), read as the code gives it; null for anything else.
    /// </summary>
    private static string? Text(CodeGraph code, TypeNode type, string expression)
    {
        if (UiCode.Written(expression) is { } written)
        {
            return written;
        }

        var path = new List<(string Member, bool Called)>();
        foreach (var step in expression.Trim().Split('.'))
        {
            var name = step.Trim();
            var called = name.EndsWith("()", StringComparison.Ordinal);
            name = called ? name[..^2].TrimEnd() : name;
            if (name.Length == 0 || !name.All(c => char.IsLetterOrDigit(c) || c is '_' or '$'))
            {
                return null;
            }

            path.Add((name, called));
        }

        return UiCode.Text(code.StringOf(type, path));
    }

    /// <summary>Both branches of <c>cond ? a : b</c>, outside quotes and brackets; the expression itself otherwise.</summary>
    private static IEnumerable<string> Branches(string expression)
    {
        var (question, colon) = (-1, -1);
        var depth = 0;
        char? quote = null;
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];
            if (quote is not null)
            {
                quote = c == quote ? null : quote;
            }
            else if (c is '\'' or '"' or '`')
            {
                quote = c;
            }
            else if (c is '[' or '(' or '{')
            {
                depth++;
            }
            else if (c is ']' or ')' or '}')
            {
                depth--;
            }
            else if (depth == 0 && c == '?' && question < 0 && (i + 1 >= expression.Length || expression[i + 1] is not ('.' or '?')))
            {
                question = i;
            }
            else if (depth == 0 && c == ':' && question >= 0)
            {
                colon = i;
                break;
            }
        }

        return question >= 0 && colon > question ? [expression[(question + 1)..colon].Trim(), expression[(colon + 1)..].Trim()] : [expression.Trim()];
    }

    /// <summary>
    /// A bound link as a path: <c>'/cart'</c>, a member of the component the code gives the text of (<c>authUrls.register</c>
    /// over <c>readonly authUrls = AUTH_URLS</c>, <c>detailUrl()</c> over a <c>computed</c>), or a commands array
    /// (<c>['/pizzerias', pizzeria.id]</c>), each segment's text where the code gives it and each other value a hole; null for a
    /// link the code computes whole.
    /// </summary>
    private static string? Link(CodeGraph code, TypeNode type, string expression)
    {
        if (Text(code, type, expression) is { } written)
        {
            return written;
        }

        if (!expression.StartsWith('[') || !expression.EndsWith(']'))
        {
            return null;
        }

        var items = new List<string>();
        var (start, depth) = (1, 0);
        char? quote = null;
        for (var i = 1; i < expression.Length - 1; i++)
        {
            var c = expression[i];
            if (quote is not null)
            {
                quote = c == quote ? null : quote;
            }
            else if (c is '\'' or '"' or '`')
            {
                quote = c;
            }
            else if (c is '[' or '(' or '{')
            {
                depth++;
            }
            else if (c is ']' or ')' or '}')
            {
                depth--;
            }
            else if (c == ',' && depth == 0)
            {
                items.Add(expression[start..i]);
                start = i + 1;
            }
        }

        items.Add(expression[start..^1]);
        var parts = items.Select(x => x.Trim()).Where(x => x.Length > 0).Select(x => Text(code, type, x) is { } text && !text.Contains(UiCode.Hole) ? text : UiCode.Hole.ToString()).ToList();
        if (parts.Count == 0)
        {
            return null;
        }

        var joined = string.Join("/", parts.Select((x, i) => i == 0 ? x.TrimEnd('/') : x.Trim('/')));
        return joined.Length == 0 ? "/" : joined;
    }
}
