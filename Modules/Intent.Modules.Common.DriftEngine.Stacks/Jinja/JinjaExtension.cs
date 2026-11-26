using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.FastApi;
using Intent.Modules.Common.DriftEngine.Stacks.Python;
using Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.Jinja;

/// <summary>
/// Reads the User Interface designer's claims (the <c>intent.ui</c> pack's) from Jinja templates, as <c>intent.mvc</c> reads
/// them from MVC views: a template others <c>{% extends %}</c> is a Layout and its <c>{% block %}</c>s its Regions; a template
/// that extends one, or that a GET path operation renders, is a Page, its route that operation's and its title its
/// <c>title</c> block; an included one is a Component. Its links are its markup's: a page filling a layout's block is that
/// region's Composition, a <c>&lt;script src&gt;</c> Uses the script, an <c>&lt;a href&gt;</c> navigates and a
/// <c>&lt;form action&gt;</c> calls the path operation its <c>url_for(...)</c> resolves to, as Starlette resolves it. Which
/// operation renders a template and what <c>url_for</c> names are FastAPI's (<see cref="FastApiSite"/>).
/// </summary>
public sealed class JinjaExtension : IDriftExtension
{
    public string Id => "intent.jinja";

    public IEnumerable<ILanguage> Languages { get; } = [new JinjaLanguage()];

    public IEnumerable<ICodeReader> Readers { get; } = [new JinjaPageParameters()];

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new JinjaPages(), new JinjaLinks()];
}

/// <summary>The templates the run read, by their types, and the members a page's route parameters became.</summary>
internal sealed class JinjaSite
{
    public Dictionary<TypeNode, JinjaTemplate> Templates { get; } = [];

    public HashSet<MemberNode> RouteParameters { get; } = [];

    public static JinjaSite? Of(CodeGraph graph) => graph.Get<JinjaSite>();

    public JinjaTemplate? TemplateOf(TypeNode? type) => type is not null && Templates.TryGetValue(type, out var template) ? template : null;

    /// <summary>The template a name relative to a templates folder is: <c>base.html</c>, <c>projects/list.html</c>.</summary>
    public JinjaTemplate? Named(string name) => Templates.Values.FirstOrDefault(x => x.Is(name));

    public TypeNode? TypeOf(JinjaTemplate template) => Templates.FirstOrDefault(x => x.Value == template).Key;

    /// <summary>The GET path operations that render a template, in the app's route order.</summary>
    public static IReadOnlyList<FastApiEndpoint> PagesOf(CodeGraph graph, JinjaTemplate template) =>
        FastApiSite.Of(graph)?.Endpoints.Where(x => x.Verbs.Contains("GET") && FastApiSite.Renders(x.Router.Module, x.Function).Any(r => template.Is(r.Template))).ToList() ?? [];
}

/// <summary>
/// Jinja templates: each is a component type named by its file without the extension, as the User Interface designer names
/// Pages and Layouts. An HTML file is a template only because the app's Python renders it with Jinja: a file Jinja's own
/// extension names (<c>.jinja</c>, <c>.j2</c>) is one, and an <c>.html</c> file is one when it is in a templates folder, the folder a
/// template the Python code renders by name (<c>templates.TemplateResponse(request, "index.html", ...)</c>, Flask's
/// <c>render_template</c>, <c>env.get_template</c>) is found in. Other HTML, a Vue app's <c>index.html</c> or a static page, is
/// no template. The Python language reads first (its extension comes first), so its syntax is on the graph.
/// </summary>
internal sealed class JinjaLanguage : ILanguage
{
    /// <summary>The calls that render a template by its name, in Starlette, FastAPI, Flask and Jinja's own environment.</summary>
    private static readonly HashSet<string> Renderers = ["TemplateResponse", "render_template", "render_template_string", "stream_template", "get_template", "select_template"];

    public string Name => "jinja";

    public string Title => "Jinja";

    public bool Reads(string path) =>
        path.EndsWith(".html", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) || JinjaTemplate.IsJinjaFile(path);

    public void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken)
    {
        var folders = TemplateFolders(graph, files);
        var site = new JinjaSite();
        foreach (var file in files.Where(x => JinjaTemplate.IsJinjaFile(x) || folders.Any(f => x.StartsWith(f, StringComparison.Ordinal))))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var text = File.ReadAllText(Path.Combine(root, file));
            var template = JinjaTemplate.Parse(file, text);
            var folder = file.Contains('/') ? file[..file.LastIndexOf('/')] : "";
            graph.Files.Add(new FileNode { Path = file, Language = Name, Namespaces = [folder] });
            var type = new TypeNode
            {
                Name = template.Name,
                Namespace = folder,
                Kind = "component",
                Access = "public",
                At = new Span(file, template.Extends is null ? 1 : template.ExtendsLine)
            };
            type.Files.Add(file);
            graph.Types.Add(type);
            site.Templates[type] = template;
        }

        if (site.Templates.Count > 0)
        {
            graph.Set(site);
        }
    }

    /// <summary>
    /// The folders, with a trailing <c>/</c>, the Python code renders templates from: for each template name a renderer is given
    /// as a string (<c>"index.html"</c>, <c>"projects/list.html"</c>), the folder of the in-scope file at that path.
    /// </summary>
    private static HashSet<string> TemplateFolders(CodeGraph graph, IReadOnlyList<string> files)
    {
        var folders = new HashSet<string>(StringComparer.Ordinal);
        if (PythonSyntax.Of(graph) is not { } syntax)
        {
            return folders;
        }

        foreach (var call in syntax.Trees.Values.SelectMany(x => x.Descendants()).OfType<PythonCall>())
        {
            var name = call.Function switch
            {
                PythonName named => named.Id,
                PythonAttribute attribute => attribute.Name,
                _ => null
            };
            if (name is null || !Renderers.Contains(name))
            {
                continue;
            }

            foreach (var rendered in call.Arguments.Where(x => x.Name is null or "name" or "template_name" or "template_name_or_list").Select(x => x.Value).OfType<PythonString>().Select(x => x.Value).OfType<string>())
            {
                var relative = rendered.TrimStart('/');
                foreach (var file in files.Where(x => x == relative || x.EndsWith("/" + relative, StringComparison.Ordinal)))
                {
                    folders.Add(file[..^relative.Length]);
                }
            }
        }

        return folders;
    }
}

/// <summary>
/// A page's route parameters (<c>project_id</c> of the <c>GET /projects/{project_id}</c> that renders <c>project.html</c>) are its
/// properties, typed as the operation's parameter is, as a routed component's are.
/// </summary>
internal sealed class JinjaPageParameters : ICodeReader
{
    public void Read(CodeGraph graph)
    {
        if (JinjaSite.Of(graph) is not { } site)
        {
            return;
        }

        foreach (var (type, template) in site.Templates)
        {
            foreach (var endpoint in JinjaSite.PagesOf(graph, template))
            {
                foreach (var name in endpoint.PathParameters.Where(x => type.Members.All(m => m.Name != x)))
                {
                    var parameter = endpoint.Function.Parameters.FirstOrDefault(x => x.Name == name);
                    var member = new MemberNode { Name = name, Kind = "property", Access = "public", Type = parameter?.Type, At = parameter?.At ?? endpoint.Decorator.At };
                    type.Members.Add(member);
                    site.RouteParameters.Add(member);
                }
            }
        }
    }
}

/// <summary>What a template is, a page's route, title and route parameters, a layout's regions, and a page's security (its operation's).</summary>
internal sealed class JinjaPages : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } =
        ["component.kind", "page.route", "page.title", "region.outlet", "property.route", "property.query", "property.bindable", "security"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (JinjaSite.Of(context.Code) is not { } site)
        {
            return null;
        }

        if (context.Kind.StartsWith("property.", StringComparison.Ordinal))
        {
            return context.Member is { } member && site.RouteParameters.Contains(member)
                ? ClaimReading.Stated(context.Kind == "property.route" ? "true" : "false", member.At)
                : null;
        }

        if (context.Kind == "region.outlet")
        {
            if (context.Element.ParentElement is not { } owner || site.TemplateOf(context.Type ?? context.TypeOf(owner)) is not { } layout)
            {
                return null;
            }

            return layout.Block(context.Expected) is { } block
                ? ClaimReading.Stated(block.Name, new Span(layout.Path, block.Line))
                : ClaimReading.Missing(layout.Path, $"a `{{% block {context.Expected} %}}`");
        }

        if (site.TemplateOf(context.Type) is not { } template)
        {
            return null;
        }

        var at = context.Type!.At;
        switch (context.Kind)
        {
            case "component.kind":
            {
                var extended = site.Templates.Values.FirstOrDefault(x => x.Extends is { } parent && template.Is(parent));
                var kind = extended is not null ? "Layout"
                    : template.Extends is not null || JinjaSite.PagesOf(context.Code, template).Count > 0 ? "Page"
                    : site.Templates.Values.Any(x => x.Includes.Any(i => template.Is(i.Name))) ? "Component"
                    : "Page";
                return ClaimReading.Stated(kind, extended is not null ? new Span(extended.Path, extended.ExtendsLine) : at);
            }
            case "page.route":
            {
                var pages = JinjaSite.PagesOf(context.Code, template);
                if (pages.Count == 0)
                {
                    return ClaimReading.Absent(at);
                }

                var routes = context.Expected.Split(',', StringSplitOptions.TrimEntries).Select(x => FastApiSite.Route(x)).ToList();
                var match = pages.FirstOrDefault(x => routes.Contains(x.Route, StringComparer.OrdinalIgnoreCase)) ?? pages[0];
                return ClaimReading.Compared(match.Url, match.Decorator.At, routes.Contains(match.Route, StringComparer.OrdinalIgnoreCase),
                    $"`{template.Path}` is rendered by `{match.Verb} {match.Url}` (`{match.Function.Name}`), not `{context.Expected}`. The model states it in the page's Route.");
            }
            case "page.title":
            {
                // A title with values in it (`{{ project.name }} · Taskboard`) shows each value as {...}.
                if (template.Block("title") is not { } title)
                {
                    return ClaimReading.Absent(at);
                }

                return ClaimReading.Stated(JinjaTemplate.Shown(title.Body).Text, new Span(template.Path, title.Line));
            }
            default:
            {
                var page = JinjaSite.PagesOf(context.Code, template).FirstOrDefault();
                return page is null || FastApiSite.Of(context.Code) is not { } fastApi
                    ? ClaimReading.Stated("None", at)
                    : ClaimReading.Stated(fastApi.Secured(context.Code, page) ? "Secured" : "None", page.Decorator.At);
            }
        }
    }
}

/// <summary>
/// A template's links: a page that extends a layout and fills a block is that region's Composition; a
/// <c>&lt;script src&gt;</c> Uses the script it loads; an <c>&lt;a href&gt;</c> navigates to the page the GET operation it names
/// renders; a <c>&lt;form method action&gt;</c> calls the path operation it names, by <c>url_for(...)</c> or by its path.
/// </summary>
internal sealed class JinjaLinks : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.uses", "intent.navigates", "intent.invokes", "intent.target"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (JinjaSite.Of(context.Code) is not { } site || context.Element is not IAssociationEnd end
            || context.Owner(end) is not { } source || end.TypeReference?.Element is not IElement target)
        {
            return null;
        }

        return end.Association?.SpecializationType switch
        {
            "Composition" => Composition(context, site, source, target),
            "Uses" when context.Kind == "intent.uses" => Uses(context, site, source, target),
            "Navigation" when context.Kind == "intent.navigates" => Navigates(context, site, source, target),
            "Call Service Operation Action" => Calls(context, site, source, target),
            _ => null
        };
    }

    /// <summary>The source is a layout's Region, the target a page: the page extends the layout and fills its block.</summary>
    private static ClaimReading? Composition(ClaimContext context, JinjaSite site, IElement region, IElement target)
    {
        if (region.ParentElement is not { } owner || site.TemplateOf(context.TypeOf(owner)) is not { } layout)
        {
            return null;
        }

        var page = site.TemplateOf(context.TypeOf(target));
        if (context.Kind == "intent.target")
        {
            var filling = site.Templates.Values.FirstOrDefault(x => x.Extends is { } parent && layout.Is(parent) && x.Block(region.Name) is not null && (page is null || x == page));
            return filling is null ? null : ClaimReading.Stated(filling.Name, new Span(filling.Path, filling.ExtendsLine));
        }

        if (page is null)
        {
            return null;
        }

        var fills = page.Extends is { } extended && layout.Is(extended) && page.Block(region.Name) is not null;
        return ClaimReading.Compared(fills ? "present" : "absent", new Span(page.Path, page.Extends is null ? 1 : page.ExtendsLine), fills,
            fills ? null : $"`{page.Path}` doesn't extend `{layout.Path}` and fill its `{region.Name}` block. Fill it, or remove the Composition.");
    }

    /// <summary>A <c>&lt;script src&gt;</c> that loads the used script's file: <c>url_for('static', path='board.js')</c> or its path.</summary>
    private static ClaimReading? Uses(ClaimContext context, JinjaSite site, IElement source, IElement target)
    {
        if (site.TemplateOf(context.TypeOf(source)) is not { } template || context.TypeOf(target) is not { } used)
        {
            return null;
        }

        var script = template.Tags.FirstOrDefault(x => x.Name == "script" && x["src"] is { } src && Loads(src, used));
        return script is null ? ClaimReading.Absent(new Span(template.Path, 1)) : ClaimReading.Compared("present", new Span(template.Path, script.Line), true);
    }

    private static bool Loads(HtmlAttribute src, TypeNode used)
    {
        var path = src.Text;
        if (src.UrlFor is { } urlFor)
        {
            var given = urlFor.Values.GetValueOrDefault("path") ?? urlFor.Values.GetValueOrDefault("filename");
            path = given is PythonString { Value: { } file } ? file : null;
        }

        path = path?.Split('?')[0].TrimStart('/');
        return path is { Length: > 0 } && used.Files.Any(x => x.EndsWith("/" + path, StringComparison.Ordinal) || path.EndsWith("/" + x[(x.LastIndexOf('/') + 1)..], StringComparison.Ordinal) || x == path);
    }

    /// <summary>An <c>&lt;a href&gt;</c> to the target page: <c>url_for</c> naming a GET operation that renders it, or a path matching its route.</summary>
    private static ClaimReading? Navigates(ClaimContext context, JinjaSite site, IElement source, IElement target)
    {
        if (site.TemplateOf(context.TypeOf(source)) is not { } template || site.TemplateOf(context.TypeOf(target)) is not { } page
            || FastApiSite.Of(context.Code) is not { } fastApi)
        {
            return null;
        }

        var pages = JinjaSite.PagesOf(context.Code, page);
        var link = template.Tags.FirstOrDefault(x => x.Name == "a" && x["href"] is { } href
            && (href.UrlFor is { } urlFor ? fastApi.Resolve(urlFor.Name, urlFor.Parameters) is { } reached && pages.Contains(reached)
                : Path(href) is { } path && pages.Any(p => Same(path, p.Route))));
        return link is null ? ClaimReading.Absent(new Span(template.Path, 1)) : ClaimReading.Compared("present", new Span(template.Path, link.Line), true);
    }

    /// <summary>
    /// A form that calls the target operation: its <c>action</c>'s <c>url_for(...)</c> resolves to the operation's path operation (the
    /// app's first route of that name and those path parameters), or its path is the operation's route, with its <c>method</c>.
    /// </summary>
    private static ClaimReading? Calls(ClaimContext context, JinjaSite site, IElement source, IElement target)
    {
        if (site.TemplateOf(context.TypeOf(source)) is not { } template || FastApiSite.Of(context.Code) is not { } fastApi
            || fastApi.EndpointFor(context, target) is not { } endpoint)
        {
            return null;
        }

        var forms = template.Tags.Where(x => x.Name == "form").Select(x => (Tag: x, Method: (x["method"]?.Raw ?? "get").ToUpperInvariant(), UrlFor: x["action"]?.UrlFor, Reached: Reached(fastApi, x)))
            .ToList();
        var form = forms.FirstOrDefault(x => x.Reached == endpoint && endpoint.Verbs.Contains(x.Method));
        if (form.Tag is not null)
        {
            var at = new Span(template.Path, form.Tag.Line);
            return context.Kind == "intent.target" ? ClaimReading.Stated(endpoint.Router.Module.Name, at) : ClaimReading.Compared("present", at, true);
        }

        if (context.Kind != "intent.invokes")
        {
            return null;
        }

        // A form that names the operation but reaches another route of its name, or sends a method the route doesn't answer.
        if (forms.FirstOrDefault(x => x.Reached == endpoint) is { Tag: not null } wrongMethod)
        {
            return ClaimReading.Compared($"{wrongMethod.Method} {endpoint.Url}", new Span(template.Path, wrongMethod.Tag.Line), false,
                $"The form at `{template.Path}:{wrongMethod.Tag.Line}` sends {wrongMethod.Method} to `{endpoint.Url}`, but `{endpoint.Router.Module.Name}.{endpoint.Function.Name}` answers {string.Join(", ", endpoint.Verbs)} there: an HTML form sends only GET or POST. Give the operation a route the form can send, or link the form to the operation it reaches.");
        }

        if (forms.FirstOrDefault(x => x.UrlFor?.Name == endpoint.Name && x.Reached is not null) is { Tag: not null } shadowed)
        {
            var reached = shadowed.Reached!;
            return ClaimReading.Compared($"{reached.Router.Module.Name}.{reached.Function.Name}", new Span(template.Path, shadowed.Tag.Line), false,
                $"The form at `{template.Path}:{shadowed.Tag.Line}` posts to `url_for('{endpoint.Name}')`, which resolves to `{reached.Verb} {reached.Url}` (`{reached.Router.Module.Name}.{reached.Function.Name}`), the app's first route named `{endpoint.Name}` with those path parameters, not `{endpoint.Verb} {endpoint.Url}` (`{endpoint.Router.Module.Name}.{endpoint.Function.Name}`). Give the route a unique name, or link the form to the operation it reaches.");
        }

        return ClaimReading.Absent(new Span(template.Path, 1));
    }

    /// <summary>The path operation a form's action reaches: its <c>url_for</c> resolved, or its path matched with a route its method answers.</summary>
    private static FastApiEndpoint? Reached(FastApiSite fastApi, HtmlTag form)
    {
        if (form["action"] is not { } action)
        {
            return null;
        }

        if (action.UrlFor is { } urlFor)
        {
            return fastApi.Resolve(urlFor.Name, urlFor.Parameters);
        }

        var method = (form["method"]?.Raw ?? "get").ToUpperInvariant();
        return Path(action) is { } path ? fastApi.Endpoints.FirstOrDefault(x => x.Verbs.Contains(method) && Same(path, x.Route)) : null;
    }

    /// <summary>An attribute's path, each <c>{{ ... }}</c> in it a parameter: <c>/projects/{{ project.id }}</c> is <c>/projects/{}</c>.</summary>
    private static string? Path(HtmlAttribute attribute)
    {
        if (attribute.Raw is not { } raw || raw.StartsWith("http", StringComparison.OrdinalIgnoreCase) || raw.StartsWith('#'))
        {
            return null;
        }

        return System.Text.RegularExpressions.Regex.Replace(raw, @"\{\{.*?\}\}", "{}").Split('?', '#')[0];
    }

    private static bool Same(string path, string route)
    {
        static string Shape(string text) => string.Join("/", text.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Contains('{') ? "{}" : x.ToLowerInvariant()));
        return Shape(path) == Shape(route);
    }
}
