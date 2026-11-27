using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Messaging;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

/// <summary>
/// What every TypeScript front-end reader shares, whatever its framework: bindings, names, files, the code a link starts in,
/// its units and what their calls reach, route records, and paths and routes. Nothing here knows a framework: Vue, React and a
/// later Angular reader each read their own components and routers with it.
/// </summary>
internal static class UiCode
{
    /// <summary>A binding the claim's vocabulary gives, when it isn't empty.</summary>
    public static string? Bound(ClaimContext context, string name) =>
        context.Claim.Bindings?.GetValueOrDefault(name) is { Length: > 0 } value ? value.Trim() : null;

    private static readonly string[] Extensions = [".ts", ".tsx", ".mts", ".cts", ".vue", ".js", ".jsx", ".mjs", ".cjs"];

    /// <summary>A file the TypeScript language reads: TypeScript, JavaScript or a Vue single-file component.</summary>
    public static bool IsTypeScript(string path) => Extensions.Any(x => path.EndsWith(x, StringComparison.OrdinalIgnoreCase));

    /// <summary>Code a TypeScript or JavaScript file declares.</summary>
    public static bool IsTypeScript(TypeNode? type) => type is not null && type.Files.Any(IsTypeScript);

    /// <summary>A name without case or separators, so <c>FilterSort</c>, <c>filterSort</c>, <c>filter-sort</c> and <c>filter_sort</c> are one.</summary>
    public static string Words(string name) => Regex.Replace(name, "[^A-Za-z0-9]", "").ToLowerInvariant();

    public static string Camel(string name) => name.Length == 0 ? name : char.ToLowerInvariant(name[0]) + name[1..];

    public static string Kebab(string name) => Regex.Replace(name, "(?<=[a-z0-9])([A-Z])", "-$1").ToLowerInvariant();

    /// <summary>A path the model gives, relative to any root: slashes forward, no leading <c>./</c> or <c>/</c>.</summary>
    public static string Normalized(string path) => Regex.Replace(path.Trim().Replace('\\', '/'), @"^(\./)+|^/+", "");

    /// <summary>The file is the one a path from some root names: the same path, or one ending with it.</summary>
    public static bool Names(string file, string path) => file == path || file.EndsWith("/" + path, StringComparison.Ordinal);

    public static string Directory(string file) => file.Contains('/') ? file[..file.LastIndexOf('/')] : "";

    /// <summary>A file's name without its extensions: <c>cart</c> for <c>stores/cart.ts</c>, <c>client</c> for <c>client.d.ts</c>.</summary>
    public static string Stem(string file)
    {
        var name = file[(file.LastIndexOf('/') + 1)..];
        return name.Contains('.') ? name[..name.IndexOf('.')] : name;
    }

    /// <summary>The element's designer folders, outermost first: the elements it sits in.</summary>
    public static IReadOnlyList<string> Folders(IElement element)
    {
        var folders = new List<string>();
        for (var parent = element.ParentElement; parent is not null; parent = parent.ParentElement)
        {
            folders.Insert(0, parent.Name);
        }

        return folders;
    }

    /// <summary>The directory ends with the folders, each compared without case or separators.</summary>
    public static bool EndsWith(string directory, IReadOnlyList<string> folders)
    {
        var parts = directory.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return folders.Count <= parts.Length && folders.Select((x, i) => Words(x) == Words(parts[parts.Length - folders.Count + i])).All(x => x);
    }

    public static FileNode? FileOf(CodeGraph code, string path) => code.Files.FirstOrDefault(x => x.Path == path);

    /// <summary>
    /// The element in whose code a link starts, and the code: the element itself, or the nearest element holding it that a
    /// <c>type.exists</c> claim located (a component operation's or a region's component).
    /// </summary>
    public static (IElement Element, TypeNode Type)? CodeOf(ClaimContext context, IElement? element)
    {
        for (var current = element; current is not null; current = current.ParentElement)
        {
            if (context.TypeOf(current) is { } type)
            {
                return (current, type);
            }
        }

        return null;
    }

    /// <summary>Every unit of code in a file: its types' members, and what it runs when it loads (a component's <c>&lt;script setup&gt;</c>).</summary>
    public static IEnumerable<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> Units(CodeGraph code, FileNode file)
    {
        var owner = code.Types.FirstOrDefault(x => x.Files.Contains(file.Path) && x.Kind is "component" or "module");
        foreach (var type in code.Types.Where(x => x.Files.Contains(file.Path)))
        {
            foreach (var member in type.Members.Where(x => x.Body is not null))
            {
                yield return (type, member, member.Body!);
            }
        }

        if (file.TopLevel is { } top && owner is not null)
        {
            yield return (owner, null, top);
        }
    }

    /// <summary>Every element of the file's markup, at any depth.</summary>
    public static IEnumerable<MarkupNode> Markup(FileNode file) => file.Markup.SelectMany(x => x.DescendantsAndSelf());

    /// <summary>The code's own methods a call runs: the function it reaches (a module's <c>request</c> helper), else the implementations the engine finds for it.</summary>
    public static IEnumerable<(TypeNode Type, MemberNode Member)> Reached(CodeGraph code, TypeNode from, CallNode call) =>
        call.Target is { Member.Body: not null } target ? [(target.Type, target.Member)] : MessageCode.Bodies(code, from, call).Select(x => (x.Type, x.Member));

    /// <summary>
    /// A route record: an object with a <c>path</c> and what it mounts, wherever the code writes one (an exported <c>routes</c>
    /// array, <c>createRouter({ routes: [...] })</c>, <c>createBrowserRouter([...])</c>), its path joined to its parents' for a
    /// record in another's <c>children</c>. An index record (<c>index: true</c>) has its parent's path.
    /// </summary>
    /// <param name="Component">The component it mounts, when the code names one in scope.</param>
    /// <param name="Value">The record.</param>
    /// <param name="Props">What a record's <c>props</c> gives, as written.</param>
    /// <param name="At">Where the record is.</param>
    /// <param name="Parent">It has child routes, so what it mounts hosts theirs: a layout's route.</param>
    public sealed record RouteRecord(string Path, TypeNode? Component, ValueNode? Value, string? Props, Span At, bool Parent = false);

    /// <summary>
    /// Every route record of objects in the code, read once per router and kept on the graph.
    /// </summary>
    /// <param name="router">The router's name, which the records are kept by.</param>
    /// <param name="mounts">What a record mounts: the value of the member that names its component, if it has one.</param>
    public static IReadOnlyList<RouteRecord> Records(CodeGraph code, string router, Func<ValueNode, (TypeNode? Component, bool Mounts)> mounts)
    {
        if (code.Get<RouteRecords>() is not { } cache)
        {
            code.Set(cache = new RouteRecords());
        }

        if (cache.ByRouter.TryGetValue(router, out var cached))
        {
            return cached;
        }

        var records = new List<RouteRecord>();
        var seen = new HashSet<ValueNode>();
        var roots = code.Types.SelectMany(x => x.Members)
            .SelectMany(x => (x.Value is { } value ? [value] : Array.Empty<ValueNode>()).Concat(x.Calls.SelectMany(c => c.Arguments)))
            .Concat(code.Files.SelectMany(x => x.TopLevel?.Calls ?? []).SelectMany(x => x.Arguments))
            .ToList();
        foreach (var root in roots)
        {
            Walk(root, "");
        }

        cache.ByRouter[router] = records;
        return records;

        void Walk(ValueNode value, string parent)
        {
            if (!seen.Add(value))
            {
                return;
            }

            // A path the code computes (ROUTES.cart) is the string it evaluates to, each part it can't a hole.
            var path = value.Members.FirstOrDefault(x => x.Member == "path")?.Value is { } given ? Text(code.StringOf(given)) ?? Hole.ToString() : null;
            var index = value.Members.Any(x => x.Member == "index" && x.Value.Literal is { Kind: "boolean", Value: "true" });
            var full = path is null ? parent : Joined(parent, path);
            if (path is not null || index)
            {
                var (component, mounted) = mounts(value);
                if (mounted)
                {
                    records.Add(new RouteRecord(full.Length == 0 ? "/" : full, component, value, value.Members.FirstOrDefault(x => x.Member == "props")?.Value.Text, value.At,
                        value.Members.Any(x => x.Member == "children" && x.Value.Items.Count > 0)));
                }
            }

            foreach (var item in value.Items)
            {
                Walk(item, parent);
            }

            foreach (var member in value.Members)
            {
                Walk(member.Value, member.Member == "children" ? full : parent);
            }
        }
    }

    private sealed class RouteRecords
    {
        public Dictionary<string, IReadOnlyList<RouteRecord>> ByRouter { get; } = new(StringComparer.Ordinal);
    }

    /// <summary>A child route's path under its parent's: an absolute child path is its own, a relative one is joined.</summary>
    public static string Joined(string parent, string path) => path.StartsWith('/') ? path : path.Length == 0 ? parent : $"{parent.TrimEnd('/')}/{path}";

    /// <summary>
    /// The component a name or a lazy import stands for: an imported name, a type of that name, or
    /// <c>() =&gt; import('./X.vue')</c>'s file's component (any component its file declares).
    /// </summary>
    public static TypeNode? ComponentNamed(CodeGraph code, string file, string text)
    {
        var from = FileOf(code, file);
        if (Regex.Match(text, @"import\(\s*['""`]([^'""`]+)['""`]\s*\)") is { Success: true } lazy && from is not null)
        {
            var target = Resolve(from.Path, lazy.Groups[1].Value);
            var components = code.Types.Where(x => x.Kind == "component" && x.Files.Any(f => f == target || ModuleOf(f) == ModuleOf(target))).ToList();
            // A file's default export, when it declares several.
            return components.Count > 1 ? components.FirstOrDefault(x => x.Name == Stem(target)) ?? components[0] : components.FirstOrDefault();
        }

        if (!Regex.IsMatch(text, @"^\w+$"))
        {
            return null;
        }

        if (from?.Imports.FirstOrDefault(x => x.Alias == text || x.Alias is null && x.Target.EndsWith("." + text, StringComparison.Ordinal)) is { File: { } imported } import)
        {
            var name = import.Target[(import.Target.LastIndexOf('.') + 1)..];
            var components = code.Types.Where(x => x.Kind == "component" && x.Files.Contains(imported)).ToList();
            return components.FirstOrDefault(x => x.Name == name) ?? (components.Count == 1 ? components[0] : null);
        }

        return code.Types.Where(x => x.Kind == "component" && x.Name == text).ToList() is [var only] ? only : null;
    }

    private static string ModuleOf(string path) => Regex.Replace(path, @"\.(vue|ts|tsx|js|jsx|mjs|cjs)$", "");

    /// <summary>A relative module path from a file: <c>./pages/Cart.vue</c> from <c>src/router.ts</c> is <c>src/pages/Cart.vue</c>.</summary>
    public static string Resolve(string from, string module)
    {
        var parts = Directory(from).Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (var part in module.Split('/'))
        {
            if (part == "..")
            {
                if (parts.Count > 0)
                {
                    parts.RemoveAt(parts.Count - 1);
                }
            }
            else if (part is not ("." or ""))
            {
                parts.Add(part);
            }
        }

        return string.Join("/", parts);
    }

    /// <summary>
    /// A router's path syntax as the model writes routes: <c>/products/:id</c> is <c>/products/{id}</c>, <c>:id?</c>
    /// <c>{id?}</c>, a custom regex dropped. vue-router and react-router write a parameter alike.
    /// </summary>
    public static string ModelRoute(string path) =>
        Regex.Replace(path, @":(\w+)(\([^)]*\))?([?*+])?", m => m.Groups[3].Value == "?" ? $"{{{m.Groups[1].Value}?}}" : $"{{{m.Groups[1].Value}}}");

    /// <summary>
    /// The route's segments as patterns: a parameter (<c>{id}</c>, <c>:id</c>) stands for any one segment, a literal segment
    /// for itself without case.
    /// </summary>
    public static bool Matches(string path, string route)
    {
        var actual = Segments(path);
        var expected = Segments(route);
        return actual.Count == expected.Count && actual.Zip(expected, (a, e) => IsParameter(e) || IsParameter(a) || string.Equals(a, e, StringComparison.OrdinalIgnoreCase)
                                                                              || Http.HttpExtension.TemplateSegmentMatches(e, a)).All(x => x);
    }

    public const char Hole = '\u0001';

    private static bool IsParameter(string segment) => segment.StartsWith('{') || segment.StartsWith(':') || segment.Contains(Hole);

    private static List<string> Segments(string path) => path.Split('?', '#')[0].Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

    /// <summary>A string value as text, each value the code computes a <see cref="Hole"/>; null when it isn't a string.</summary>
    public static string? Text(LiteralNode? literal) => literal switch
    {
        { Kind: "string", Value: { } text } => text,
        { Kind: "interpolated" } => string.Concat(literal.Parts.Select(x => x.Literal is { Kind: "string", Value: { } part } ? part : Hole.ToString())),
        _ => null
    };

    /// <summary>A path with its holes shown as the values they are: <c>/orders/{id}</c> for <c>`/orders/${encodeURIComponent(id)}`</c>.</summary>
    public static string Shown(LiteralNode literal) => literal.Kind == "string"
        ? literal.Value ?? ""
        : string.Concat(literal.Parts.Select(x => x.Literal is { Kind: "string", Value: { } part } ? part : $"{{{HoleName(x.Text)}}}"));

    private static string HoleName(string text) =>
        Regex.Match(text.Trim(), @"(\w+)\s*\)?\s*$") is { Success: true } name ? name.Groups[1].Value : "…";

    /// <summary>
    /// A string written in a template's code: <c>'/cart'</c>, or a template literal with its <c>${...}</c> as holes; null for
    /// anything else.
    /// </summary>
    public static string? Written(string expression)
    {
        var text = expression.Trim();
        if (text.Length < 2 || text[0] != text[^1] || text[0] is not ('\'' or '"' or '`'))
        {
            return null;
        }

        var inner = text[1..^1];
        return text[0] == '`' ? Regex.Replace(inner, @"\$\{[^}]*\}", Hole.ToString()) : inner;
    }
}
