using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Messaging;

namespace Intent.Modules.Common.DriftEngine.Stacks.Vue;

/// <summary>
/// Reads the User Interface claims (the <c>intent.ui</c> pack's kinds) from a Vue front end: single-file components,
/// vue-router's route records, Pinia stores, and the TypeScript modules and request helpers its elements are. It reads the
/// claims, never the designer: an element is the code its <c>type.exists</c> claim located, and the vocabulary's bindings say
/// what kind of code that is (<c>Store</c>, <c>Module</c>, <c>Proxy</c>, an Implementation <c>Path</c>). So the standard
/// designer and a house designer whose pack claims the same kinds with the same bindings are read alike. It answers only for
/// TypeScript code, leaving Blazor's to <c>intent.blazor</c>.
/// </summary>
public sealed class VueExtension : IDriftExtension
{
    public string Id => "intent.vue";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new Modules(), new Components(), new Members(), new Links(), new Requests()];
}

/// <summary>What the Vue readers share: bindings, names, files and the router's records.</summary>
internal static class VueCode
{
    /// <summary>A binding the claim's vocabulary gives, when it isn't empty.</summary>
    public static string? Bound(ClaimContext context, string name) =>
        context.Claim.Bindings?.GetValueOrDefault(name) is { Length: > 0 } value ? value.Trim() : null;

    public static bool IsTypeScript(string path) => path.EndsWith(".ts", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".tsx", StringComparison.OrdinalIgnoreCase)
                                                    || path.EndsWith(".mts", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".vue", StringComparison.OrdinalIgnoreCase);

    /// <summary>A Vue single-file component.</summary>
    public static bool IsVue(TypeNode? type) => type is { Kind: "component" } && type.Files.Any(x => x.EndsWith(".vue", StringComparison.OrdinalIgnoreCase));

    /// <summary>Code a TypeScript file declares.</summary>
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

    /// <summary>Every element of the file's template, at any depth.</summary>
    public static IEnumerable<MarkupNode> Markup(FileNode file) => file.Markup.SelectMany(x => x.DescendantsAndSelf());

    /// <summary>The code's own methods a call runs: the function it reaches (a module's <c>request</c> helper), else the implementations the engine finds for it.</summary>
    public static IEnumerable<(TypeNode Type, MemberNode Member)> Reached(CodeGraph code, TypeNode from, CallNode call) =>
        call.Target is { Member.Body: not null } target ? [(target.Type, target.Member)] : MessageCode.Bodies(code, from, call).Select(x => (x.Type, x.Member));

    /// <summary>
    /// A route record: an object with a <c>path</c> and a <c>component</c>, wherever the code writes one (an exported
    /// <c>routes</c> array, <c>createRouter({ routes: [...] })</c>), its path joined to its parents' for a record in another's
    /// <c>children</c>. Its component is the type the name resolves to, or a lazily imported file's.
    /// </summary>
    public sealed record RouteRecord(string Path, TypeNode? Component, ValueNode Value, string? Props);

    public static IReadOnlyList<RouteRecord> Records(CodeGraph code)
    {
        if (code.Get<VueRoutes>() is { } cached)
        {
            return cached.Records;
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

        code.Set(new VueRoutes(records));
        return records;

        void Walk(ValueNode value, string parent)
        {
            if (!seen.Add(value))
            {
                return;
            }

            var path = value.Members.FirstOrDefault(x => x.Member == "path")?.Value.Literal is { Kind: "string", Value: { } written } ? written : null;
            var full = path is null ? parent : path.StartsWith('/') ? path : $"{parent.TrimEnd('/')}/{path}";
            if (path is not null && value.Members.FirstOrDefault(x => x.Member == "component")?.Value is { } component)
            {
                records.Add(new RouteRecord(full, ComponentOf(code, component), value, value.Members.FirstOrDefault(x => x.Member == "props")?.Value.Text));
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

    private sealed record VueRoutes(IReadOnlyList<RouteRecord> Records);

    /// <summary>The component a record's <c>component</c> names: an imported name, a type of that name, or <c>() =&gt; import('./X.vue')</c>.</summary>
    private static TypeNode? ComponentOf(CodeGraph code, ValueNode component)
    {
        var file = FileOf(code, component.At.File);
        if (Regex.Match(component.Text, @"import\(\s*['""`]([^'""`]+)['""`]\s*\)") is { Success: true } lazy && file is not null)
        {
            var target = Resolve(file.Path, lazy.Groups[1].Value);
            return code.Types.FirstOrDefault(x => x.Kind == "component" && x.Files.Any(f => f == target || ModuleOf(f) == ModuleOf(target)));
        }

        if (!Regex.IsMatch(component.Text, @"^\w+$"))
        {
            return null;
        }

        if (file?.Imports.FirstOrDefault(x => x.Alias == component.Text || x.Target.EndsWith("." + component.Text, StringComparison.Ordinal)) is { File: { } imported })
        {
            return code.Types.FirstOrDefault(x => x.Kind == "component" && x.Files.Contains(imported));
        }

        return code.Types.Where(x => x.Kind == "component" && x.Name == component.Text).ToList() is [var only] ? only : null;
    }

    private static string ModuleOf(string path) => Regex.Replace(path, @"\.(vue|ts|tsx|js)$", "");

    /// <summary>A relative module path from a file: <c>./pages/Cart.vue</c> from <c>src/router.ts</c> is <c>src/pages/Cart.vue</c>.</summary>
    private static string Resolve(string from, string module)
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

    /// <summary>vue-router's path syntax as the model writes routes: <c>/products/:id</c> is <c>/products/{id}</c>, <c>:id?</c> <c>{id?}</c>, a custom regex dropped.</summary>
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
        return actual.Count == expected.Count && actual.Zip(expected, (a, e) => IsParameter(e) || IsParameter(a) || string.Equals(a, e, StringComparison.OrdinalIgnoreCase)).All(x => x);
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

/// <summary>
/// Where a store, module or proxy is: the file its Implementation Path names, else a TypeScript file named for it in a
/// folder its designer folders end with, and in that file the type of its name, else the file's own code. A Pinia store is
/// also found by its <c>defineStore</c> id or its <c>use{Name}Store</c> function. Components and contracts are left to the
/// engine's own rule, which finds a type by its name.
/// </summary>
internal sealed class Modules : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["type.exists"];

    public ClaimReading? Read(ClaimContext context)
    {
        var code = context.Code;
        var element = context.Element;
        var path = VueCode.Bound(context, "Path");
        var store = VueCode.Bound(context, "Store") is not null;
        var proxy = VueCode.Bound(context, "Proxy") is not null;
        if (path is null && !store && !proxy && VueCode.Bound(context, "Module") is null || !code.Files.Any(x => VueCode.IsTypeScript(x.Path)))
        {
            return null;
        }

        var name = VueCode.Words(element.Name);
        FileNode? file;
        if (path is not null)
        {
            var named = VueCode.Normalized(path);
            file = code.Files.Where(x => VueCode.Names(x.Path, named)).MinBy(x => x.Path.Length);
            if (file is null)
            {
                return context.Scope.Files.FirstOrDefault(x => VueCode.Names(x, named)) is { } excluded && !context.Scope.Includes(excluded)
                    ? ClaimReading.OutOfScope(excluded)
                    : ClaimReading.Missing("the code", $"`{named}`, which its Implementation Path names");
            }
        }
        else
        {
            var folders = VueCode.Folders(element);
            var candidates = code.Files.Where(x => VueCode.IsTypeScript(x.Path) && !x.Path.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase)
                                                   && (VueCode.Words(VueCode.Stem(x.Path)) == name || store && VueCode.Words(VueCode.Stem(x.Path)) == name + "store"))
                .ToList();
            file = candidates.Where(x => VueCode.EndsWith(VueCode.Directory(x.Path), folders)).MinBy(x => x.Path.Length)
                   ?? (candidates.Count == 1 ? candidates[0] : null);
            if (file is null && store && Pinia(code, element.Name) is { } defined)
            {
                return ClaimReading.Found(defined.Module, defined.At);
            }

            if (file is null)
            {
                // A store, service or proxy a C# front end declares is a class of its name, which the engine's own rule finds.
                if (code.Types.Any(x => !VueCode.IsTypeScript(x) && VueCode.Words(x.Name) == name))
                {
                    return null;
                }

                return ClaimReading.Missing(folders.Count > 0 ? $"`{string.Join("/", folders)}/`" : "the code", $"a TypeScript file named for `{element.Name}`");
            }
        }

        var types = code.Types.Where(x => x.Files.Contains(file.Path)).ToList();
        var module = types.FirstOrDefault(x => x.Kind is "module" or "component");
        if (store && Pinia(code, element.Name, file.Path) is { } pinia)
        {
            return ClaimReading.Found(pinia.Module, pinia.At);
        }

        // The type of its name (a proxy's api object, a class), else the only object or class a proxy's file declares, else the file's own code.
        var type = types.FirstOrDefault(x => x.Kind is not ("module" or "interface" or "typeAlias") && VueCode.Words(x.Name) == name)
                   ?? (proxy && types.Where(x => x.Kind is "object" or "class" && x.Access == "public").ToList() is [var only] ? only : null)
                   ?? module;
        return type is null ? ClaimReading.Missing($"`{file.Path}`", "code") : ClaimReading.Found(type, type.At);
    }

    /// <summary>A Pinia store: <c>defineStore('cart', ...)</c> whose id is the store's name, or held by <c>use{Name}Store</c>.</summary>
    private static (TypeNode Module, Span At)? Pinia(CodeGraph code, string name, string? file = null)
    {
        var words = VueCode.Words(name);
        foreach (var source in code.Files.Where(x => file is null || x.Path == file))
        {
            var module = code.Types.FirstOrDefault(x => x.Kind == "module" && x.Files.Contains(source.Path));
            if (module is null)
            {
                continue;
            }

            foreach (var call in (source.TopLevel?.Calls ?? []).Where(x => x.Name == "defineStore" && x.Arguments.Count > 0))
            {
                var id = call.Arguments[0].Literal?.Value ?? call.Arguments[0].Members.FirstOrDefault(x => x.Member == "id")?.Value.Literal?.Value;
                var holder = module.Members.FirstOrDefault(x => x.At.Line == call.At.Line && x.Kind == "field")?.Name;
                if (id is not null && VueCode.Words(id) == words || holder is not null && VueCode.Words(holder) == $"use{words}store")
                {
                    return (module, call.At);
                }
            }
        }

        return null;
    }
}

/// <summary>What a Vue component is (a page its route record mounts, a layout hosting a <c>&lt;RouterView&gt;</c>), a page's routes and title, and its security.</summary>
internal sealed class Components : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["component.kind", "page.route", "page.title", "security", "security.roles", "security.policy"];

    public ClaimReading? Read(ClaimContext context)
    {
        var type = context.Type ?? context.TypeOf(context.Element);
        if (!VueCode.IsVue(type))
        {
            return null;
        }

        var code = context.Code;
        var records = VueCode.Records(code).Where(x => x.Component == type).ToList();
        var file = VueCode.FileOf(code, type!.At.File);
        switch (context.Kind)
        {
            case "component.kind":
            {
                // A Dialog is a component the code shows as one; nothing in its own code says so.
                if (context.Expected == "Dialog")
                {
                    return ClaimReading.CantVerify("noRule", "A Vue component's own code doesn't say whether it is a dialog.", type.At);
                }

                if (records.Count > 0)
                {
                    return ClaimReading.Compared("Page", records[0].Value.At, context.Expected == "Page");
                }

                if (file is not null && VueCode.Markup(file).FirstOrDefault(x => x.Tag is "RouterView" or "router-view") is { } view)
                {
                    return ClaimReading.Compared("Layout", view.At, context.Expected == "Layout");
                }

                return ClaimReading.Absent(type.At, "Component");
            }
            case "page.route":
                return records.Count == 0
                    ? ClaimReading.Absent(type.At)
                    : ClaimReading.Stated(string.Join(", ", records.Select(x => VueCode.ModelRoute(x.Path)).Order(StringComparer.Ordinal)), records[0].Value.At);
            case "page.title":
                return Title(code, type, file, records);
            case "security":
                // Vue has no standard way to secure a component: code that guards none states None, by saying nothing.
                return context.Expected == "None"
                    ? ClaimReading.Absent(type.At, "None")
                    : ClaimReading.CantVerify("noRule", "Vue has no standard way to secure a component; a house reader says what secures one.", type.At);
            default:
                return ClaimReading.CantVerify("noRule", "Vue has no standard way to secure a component; a house reader says what secures one.", type.At);
        }
    }

    /// <summary>A title its route record's <c>meta</c> gives, or its code sets with <c>document.title =</c>, <c>useTitle</c> or <c>useHead({ title })</c>.</summary>
    private static ClaimReading Title(CodeGraph code, TypeNode type, FileNode? file, IReadOnlyList<VueCode.RouteRecord> records)
    {
        foreach (var record in records)
        {
            if (record.Value.Members.FirstOrDefault(x => x.Member == "meta")?.Value.Members.FirstOrDefault(x => x.Member == "title")?.Value is { } meta)
            {
                return Titled(code.StringOf(meta), meta.At);
            }
        }

        foreach (var (_, _, unit) in file is null ? [] : VueCode.Units(code, file))
        {
            if (unit.Assignments.FirstOrDefault(x => x.Target is "document.title") is { } assigned)
            {
                return Titled(code.StringOf(assigned.Value), assigned.Value.At);
            }

            if (unit.Calls.FirstOrDefault(x => x.Name is "useTitle" or "useHead" && x.Arguments.Count > 0) is { } call)
            {
                var value = call.Name == "useHead" ? call.Arguments[0].Members.FirstOrDefault(x => x.Member == "title")?.Value : call.Arguments[0];
                if (value is not null)
                {
                    return Titled(code.StringOf(value), value.At);
                }
            }
        }

        return ClaimReading.Absent(type.At);

        static ClaimReading Titled(LiteralNode literal, Span at) => literal is { Kind: "string", Value: { } text }
            ? ClaimReading.Stated(text, at)
            : ClaimReading.CantVerify("unreadable", "The page's title is computed.", at);
    }
}

/// <summary>
/// A Vue component's props, emits and functions, and a Pinia store's state, getters and actions: the code a Property, Event
/// Emitter or operation is, and what a Property is to its page (bindable, a route or query parameter).
/// </summary>
internal sealed class Members : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.exists", "property.query", "property.route", "property.bindable"];

    public ClaimReading? Read(ClaimContext context)
    {
        var element = context.Element;
        var owner = context.Type ?? (element.ParentElement is { } parent ? context.TypeOf(parent) : null);
        if (owner is null || !VueCode.IsTypeScript(owner))
        {
            return null;
        }

        var code = context.Code;
        if (owner.Kind == "module")
        {
            // A store's state, getters and actions: what its setup function returns, or its options' state, getters and actions.
            return context.Kind == "member.exists" && Store(code, owner, element.Name) is { } stated ? ClaimReading.Found(stated.Value, stated.At) : null;
        }

        if (!VueCode.IsVue(owner))
        {
            return null;
        }

        // Vue's props and emits are camel case in the script and kebab case in templates; the model may name them either way.
        var kind = VueCode.Bound(context, "Event") is not null ? "event" : VueCode.Bound(context, "Operation") is not null ? "method" : "property";
        var words = VueCode.Words(element.Name);
        var member = context.Member ?? owner.Members.Where(x => VueCode.Words(x.Name) == words)
            .OrderByDescending(x => x.Kind == kind).ThenByDescending(x => x.Name == element.Name).FirstOrDefault();
        switch (context.Kind)
        {
            case "member.exists":
            {
                if (member is null)
                {
                    // A page's Property can be what its route gives it: useRoute().params.id.
                    var what = kind switch { "event" => "an emit", "method" => "a function", _ => "a prop or state" };
                    return kind == "property" && Parameter(code, owner, element.Name) is { } read
                        ? ClaimReading.Found(read, read)
                        : ClaimReading.Missing($"`{owner.At.File}`", $"{what} named `{VueCode.Camel(element.Name)}`");
                }

                // A Property is a prop or the component's own state, an operation a function (or a const holding one), an Event Emitter an emit.
                var fits = kind switch
                {
                    "event" => member.Kind == "event",
                    "method" => member.Kind is "method" or "field",
                    _ => member.Kind is "property" or "field"
                };
                return fits
                    ? ClaimReading.Found(member)
                    : ClaimReading.Compared($"a {member.Kind}", member.At, false, $"`{member.Name}` is a {member.Kind}, not {kind switch { "event" => "an emit (defineEmits)", "method" => "a function", _ => "a prop or state" }}.");
            }
            case "property.bindable":
            {
                // v-model: defineModel, or a prop with an update:prop emit. What the route gives a page isn't bindable.
                if (member is null)
                {
                    return ClaimReading.Absent(owner.At, "false");
                }

                var model = member.Modifiers.Contains("model")
                            || owner.Members.Any(x => x.Kind == "event" && (x.Name == $"update:{member.Name}" || x.Name == $"update:{VueCode.Kebab(member.Name)}"));
                return model ? ClaimReading.Compared("true", member.At, context.Expected == "true") : ClaimReading.Absent(member.At, "false");
            }
            default:
            {
                // A route or query parameter: its record's props give it from the route, or the component reads it from the route.
                var from = context.Kind == "property.route" ? "params" : "query";
                var at = member?.At ?? owner.At;
                var records = VueCode.Records(code).Where(x => x.Component == owner).ToList();
                var given = from == "params"
                    ? records.Any(x => x.Props is "true" && Regex.IsMatch(x.Path, $@":{Regex.Escape(VueCode.Camel(element.Name))}\b", RegexOptions.IgnoreCase))
                    : records.Any(x => x.Props is { } props && Regex.IsMatch(props, $@"query\s*\.\s*{Regex.Escape(VueCode.Camel(element.Name))}\b", RegexOptions.IgnoreCase));
                if (given || Reads(code, owner, from, element.Name) is not null)
                {
                    return ClaimReading.Compared("true", at, context.Expected == "true");
                }

                return ClaimReading.Absent(at, "false");
            }
        }
    }

    /// <summary>Where the component reads a route's parameter or query value of the name: <c>route.params.id</c>, <c>$route.query.q</c>.</summary>
    private static Span? Reads(CodeGraph code, TypeNode component, string from, string name)
    {
        var pattern = new Regex($@"\b{from}\s*(\.\s*{Regex.Escape(VueCode.Camel(name))}\b|\[\s*['""]{Regex.Escape(VueCode.Camel(name))}['""]\s*\])", RegexOptions.IgnoreCase);
        if (VueCode.FileOf(code, component.At.File) is not { } file)
        {
            return null;
        }

        foreach (var (_, _, unit) in VueCode.Units(code, file))
        {
            if (unit.Calls.SelectMany(x => x.Arguments).Concat(unit.Assignments.Select(x => x.Value)).FirstOrDefault(x => pattern.IsMatch(x.Text)) is { } value)
            {
                return value.At;
            }
        }

        return component.Members.FirstOrDefault(x => x.Value is { } value && pattern.IsMatch(value.Text))?.At
               ?? VueCode.Markup(file).SelectMany(x => x.Attributes).FirstOrDefault(x => x.Value is { } value && pattern.IsMatch(value))?.At;
    }

    private static Span? Parameter(CodeGraph code, TypeNode component, string name) => Reads(code, component, "params", name) ?? Reads(code, component, "query", name);

    /// <summary>What a Pinia store gives of the name: a member of the object its setup function returns, or of its options' state, getters or actions.</summary>
    private static (InitializerNode Value, Span At)? Store(CodeGraph code, TypeNode module, string name)
    {
        var words = VueCode.Words(name);
        var file = VueCode.FileOf(code, module.At.File);
        foreach (var call in (file?.TopLevel?.Calls ?? []).Where(x => x.Name == "defineStore"))
        {
            var given = call.Arguments.Skip(1).FirstOrDefault() ?? call.Arguments[0];
            var members = given.Function is { } setup
                ? setup.Returns.SelectMany(x => x.Members)
                : given.Members.Where(x => x.Member is "state" or "getters" or "actions")
                    .SelectMany(x => x.Value.Function?.Returns.SelectMany(r => r.Members) ?? x.Value.Members);
            if (members.FirstOrDefault(x => VueCode.Words(x.Member) == words) is { } found)
            {
                return (found, found.Value.At);
            }
        }

        return null;
    }
}

/// <summary>
/// What a Vue component or TypeScript module uses, navigates to, opens and calls, read from its template and code: a child
/// component's tag, an import of a store, service or utility, a <c>&lt;RouterLink&gt;</c> or <c>router.push</c> to a page's
/// route, a dialog rendered in its template, and a call of a proxy's method, itself or through the code's own functions.
/// </summary>
internal sealed class Links : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.uses", "intent.navigates", "intent.opens", "intent.invokes", "intent.target"];

    public ClaimReading? Read(ClaimContext context)
    {
        // A link is an association, from the element at its source end, or an element its source holds and types by the target
        // (a house designer's Rendered Component).
        var (source, target) = context.Element is IAssociationEnd end
            ? (context.Owner(end), end.TypeReference?.Element as IElement)
            : (context.Element.ParentElement, context.Element.TypeReference?.Element as IElement);
        if (source is null || target is null
            || VueCode.CodeOf(context, source) is not { } located || !VueCode.IsTypeScript(located.Type) || VueCode.FileOf(context.Code, located.Type.At.File) is not { } file)
        {
            return null;
        }

        var (holder, type) = located;
        var code = context.Code;
        var start = new Span(file.Path, 1);
        // From one of a component's operations, only that function's code is read.
        var units = VueCode.Units(code, file).ToList();
        if (holder != source && units.FirstOrDefault(x => x.Member is { } member && VueCode.Words(member.Name) == VueCode.Words(source.Name)) is { Member: not null } operation)
        {
            units = [operation];
        }

        switch (context.Kind)
        {
            case "intent.uses" or "intent.opens":
            {
                if (context.TypeOf(target) is not { } used)
                {
                    // A target whose own claim found no code is that claim's finding; one nothing claims is looked for by name.
                    if (context.ClaimOn(target, "type.exists") is not null)
                    {
                        return ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located.");
                    }

                    return VueCode.Markup(file).FirstOrDefault(x => x.Tag == target.Name || x.Tag == VueCode.Kebab(target.Name)) is { } named
                        ? ClaimReading.Found(named, named.At)
                        : file.Imports.FirstOrDefault(x => !x.TypeOnly && x.File is { } imported && VueCode.Words(VueCode.Stem(imported)) == VueCode.Words(target.Name)) is { } byName
                            ? ClaimReading.Found(byName, byName.At)
                            : ClaimReading.Absent(start);
                }

                if (used.Kind == "component")
                {
                    return Rendered(code, file, type, used) is { } tag ? ClaimReading.Found(tag, tag.At) : ClaimReading.Absent(start);
                }

                return file.Imports.FirstOrDefault(x => !x.TypeOnly && x.File is { } imported && used.Files.Contains(imported)) is { } import
                    ? ClaimReading.Found(import, import.At)
                    : ClaimReading.Absent(start);
            }
            case "intent.navigates":
                return Navigates(context, code, file, units, target, start);
            case "intent.target":
                return Target(context, code, file, type, units, target);
            default:
                return Invocation(context, code, file, units, target) is { } call ? ClaimReading.Found(call.Value, call.At) : ClaimReading.Absent(start);
        }
    }

    /// <summary>
    /// What a link reaches is the element the model names, not another of its name: a child's tag resolves to the component
    /// the child is located as, and a proxy operation's call is made on the proxy the operation's owner is located as.
    /// </summary>
    private static ClaimReading? Target(ClaimContext context, CodeGraph code, FileNode file, TypeNode type, IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> units, IElement target)
    {
        var expected = context.ExpectedValue as IElement ?? target;
        var located = context.TypeOf(expected);
        if (located is { Kind: "component" })
        {
            if (VueCode.Markup(file).FirstOrDefault(x => x.Tag == located.Name || x.Tag == VueCode.Kebab(located.Name) || x.Component?.Name == located.Name) is not { } tag)
            {
                return null;
            }

            return tag.Component is { } named && code.TypeFor(named, type) is { } resolved
                ? ClaimReading.Compared(resolved.Name, tag.At, resolved == located)
                : ClaimReading.CantVerify("unresolved", "The tag doesn't resolve to a component in scope.", tag.At);
        }

        if (Invocation(context, code, file, units, target) is not { Value: CallNode call })
        {
            return null;
        }

        var on = call.Target?.Type;
        return ClaimReading.Compared(on?.Name ?? call.Receiver ?? call.Text, call.At, located is null || on == located || on is null && call.Receiver == located.Name);
    }

    /// <summary>A tag in the template that renders the component: one resolving to its type, or of its name in Pascal or kebab case.</summary>
    private static MarkupNode? Rendered(CodeGraph code, FileNode file, TypeNode from, TypeNode component) =>
        VueCode.Markup(file).FirstOrDefault(x => x.Component is { } named && code.TypeFor(named, from) == component)
        ?? VueCode.Markup(file).FirstOrDefault(x => x.Tag == component.Name || x.Tag == VueCode.Kebab(component.Name));

    /// <summary>
    /// A <c>to</c> on a <c>&lt;RouterLink&gt;</c> (or any element), or a <c>push</c> or <c>replace</c> on the router, to one
    /// of the target page's routes: its <c>page.route</c> claim's, else the vocabulary's <c>Route</c>. A computed part of a link
    /// stands for a parameter; a link the code computes whole, where none matches, can't be verified.
    /// </summary>
    private static ClaimReading Navigates(ClaimContext context, CodeGraph code, FileNode file, IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> units, IElement target, Span start)
    {
        // A route is shown without its slashes, so the root route is the empty one.
        var claimed = context.ClaimOn(target, "page.route")?.Expected ?? context.Claim.Bindings?.GetValueOrDefault("Route");
        if (claimed is null)
        {
            return ClaimReading.CantVerify("unresolved", $"`{target.Name}` has no route to link to.");
        }

        var routes = claimed.Split(',', StringSplitOptions.TrimEntries) is var split && split.All(x => x.Length > 0) ? split : [.. split.Where(x => x.Length > 0), ""];

        var links = new List<(string? Path, Span At)>();
        foreach (var attribute in VueCode.Markup(file).SelectMany(x => x.Attributes))
        {
            if (attribute is { Directive: null, Name: "to" or "href" } && attribute.Literal is { Kind: "string", Value: { } to })
            {
                links.Add((to, attribute.At));
            }
            else if (attribute is { Directive: "bind", Argument: "to" } && attribute.Value is { } value)
            {
                links.Add((VueCode.Written(value) ?? Path(value), attribute.At));
            }
        }

        foreach (var call in units.SelectMany(x => x.Unit.Calls).Where(x => x.Name is "push" or "replace" && x.Arguments.Count > 0 && x.Receiver is { } receiver && Regex.IsMatch(receiver, @"(?i)router$")))
        {
            var argument = call.Arguments[0];
            var path = argument.Members.FirstOrDefault(x => x.Member == "path")?.Value ?? argument;
            links.Add((VueCode.Text(code.StringOf(path)), call.At));
        }

        Span? computed = null;
        foreach (var (path, at) in links)
        {
            // A link the code computes whole (:to="target") can't be read; one with values in it (`/orders/${id}`) can.
            if (path is null || path.Contains(VueCode.Hole) && path.Replace("/", "").All(x => x == VueCode.Hole))
            {
                computed ??= at;
                continue;
            }

            if (routes.Any(route => VueCode.Matches(path, route)))
            {
                return ClaimReading.Found(target, at);
            }
        }

        return computed is { } unreadable ? ClaimReading.CantVerify("unreadable", "A link's path is computed.", unreadable) : ClaimReading.Absent(start);

        // :to="{ path: '/cart' }"
        static string? Path(string value) => Regex.Match(value, @"path\s*:\s*(['""`][^'""`]*['""`])") is { Success: true } path ? VueCode.Written(path.Groups[1].Value) : null;
    }

    /// <summary>
    /// The call of the target operation the code makes, nearest first: in its own code, then in the code's own functions it
    /// calls, on the operation's located method or one of its name on the proxy's located type; or a call of it written in the
    /// template, with the proxy's file imported.
    /// </summary>
    private static (object Value, Span At)? Invocation(ClaimContext context, CodeGraph code, FileNode file, IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> units, IElement operation)
    {
        var method = context.MemberOf(operation);
        var proxy = operation.ParentElement is { } parent ? context.TypeOf(parent) : null;
        var name = method?.Name ?? VueCode.Camel(operation.Name);
        bool IsOperation(CallNode call) => call.Target?.Member is { } reached && method is not null
            ? reached == method
            : call.Name == name && (proxy is null ? call.Receiver is not null : call.Target?.Type == proxy || call.Receiver == proxy.Name || call.ReceiverType?.Name == proxy.Name);

        var seen = new HashSet<FunctionNode>();
        var queue = new Queue<(TypeNode Type, FunctionNode Unit)>(units.Select(x => (x.Type, x.Unit)).Where(x => seen.Add(x.Unit)));
        while (queue.TryDequeue(out var current))
        {
            foreach (var call in current.Unit.Calls)
            {
                if (IsOperation(call))
                {
                    return (call, call.At);
                }

                foreach (var (type, member) in VueCode.Reached(code, current.Type, call))
                {
                    if (member.Body is { } body && seen.Add(body))
                    {
                        queue.Enqueue((type, body));
                    }
                }
            }
        }

        if (proxy is not null && file.Imports.Any(x => !x.TypeOnly && x.File is { } imported && proxy.Files.Contains(imported)))
        {
            var written = new Regex($@"\.{Regex.Escape(name)}\s*\(");
            if (VueCode.Markup(file).SelectMany(x => x.Attributes).FirstOrDefault(x => x.Directive is not null && x.Value is { } value && written.IsMatch(value)) is { } attribute)
            {
                return (attribute, attribute.At);
            }
        }

        return null;
    }
}

/// <summary>
/// The request a TypeScript proxy operation sends: a <c>fetch</c>, or an HTTP client's verb method (axios, Angular's
/// <c>HttpClient</c>, ky, ofetch), in its method or in the code's own helper it calls, whose URL and verb are read with the
/// helper's parameters bound to what the operation passes (<c>request('GET', `/api/products/${id}`)</c> into
/// <c>fetch(`${baseUrl}${path}`, { method })</c>). A base URL before the path isn't part of the route.
/// </summary>
internal sealed class Requests : IClaimReader
{
    private static readonly Regex Client = new(@"(?i)(^|\.)(axios|\$?http|httpclient|client|ky|instance|\$fetch|ofetch)$");

    private static readonly string[] Verbs = ["get", "post", "put", "patch", "delete", "head", "options"];

    public IReadOnlyCollection<string> Kinds { get; } = ["proxy.request", "proxy.verb", "proxy.route"];

    public ClaimReading? Read(ClaimContext context)
    {
        var operation = context.Element is IAssociationEnd end ? context.Owner(end) : context.Element;
        if (operation?.ParentElement is not { } proxy || context.TypeOf(proxy) is not { } type || !VueCode.IsTypeScript(type))
        {
            return null;
        }

        var code = context.Code;
        var method = context.MemberOf(operation)
                     ?? type.Members.FirstOrDefault(x => x.Kind == "method" && VueCode.Words(x.Name) == VueCode.Words(operation.Name));
        if (method?.Body is null)
        {
            return null;
        }

        if (Request(code, type, method) is not { } request)
        {
            return context.Kind == "proxy.request" ? ClaimReading.Absent(method.At) : ClaimReading.CantVerify("notFound", $"`{method.Name}` sends no request the engine reads (a fetch, or an HTTP client's verb method).", method.At);
        }

        switch (context.Kind)
        {
            case "proxy.request":
                return ClaimReading.Compared("present", request.At, context.Expected == "present");
            case "proxy.verb":
                return request.Verb is { } verb
                    ? ClaimReading.Compared(verb, request.At, string.Equals(verb, context.Expected, StringComparison.OrdinalIgnoreCase))
                    : ClaimReading.CantVerify("unreadable", "The request's method is computed.", request.At);
            default:
            {
                if (request.Url is not { } url || Route(url) is not { } route)
                {
                    return ClaimReading.CantVerify("unreadable", "The request's URL is computed.", request.At);
                }

                return ClaimReading.Compared(route.Shown, request.At, VueCode.Matches(route.Path, context.Expected));
            }
        }
    }

    private sealed record Sent(string? Verb, LiteralNode? Url, Span At);

    /// <summary>The first request the method sends, nearest first, following the code's own functions with the calls that lead into them.</summary>
    private static Sent? Request(CodeGraph code, TypeNode type, MemberNode method)
    {
        var seen = new HashSet<FunctionNode> { method.Body! };
        var queue = new Queue<(TypeNode Type, FunctionNode Unit, IReadOnlyList<CallNode> Through)>([(type, method.Body!, [])]);
        while (queue.TryDequeue(out var current))
        {
            foreach (var call in current.Unit.Calls)
            {
                if (Read(code, call, current.Through) is { } sent)
                {
                    return sent;
                }

                foreach (var (owner, member) in VueCode.Reached(code, current.Type, call))
                {
                    if (member.Body is { } body && seen.Add(body))
                    {
                        queue.Enqueue((owner, body, [.. current.Through, call]));
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// A call that sends a request: its verb and URL, read through the calls that lead to it, placed at the operation's own
    /// call that leads there (its <c>request(...)</c>), since a shared helper's <c>fetch</c> is every operation's.
    /// </summary>
    private static Sent? Read(CodeGraph code, CallNode call, IReadOnlyList<CallNode> through)
    {
        var sent = Sends(code, call, through);
        return sent is null || through.Count == 0 ? sent : sent with { At = through[0].At };
    }

    private static Sent? Sends(CodeGraph code, CallNode call, IReadOnlyList<CallNode> through)
    {
        string? Literal(ValueNode value) => code.StringOf(value, through) is { Kind: "string", Value: { } text } ? text : null;

        // fetch(url, { method }), $fetch and ofetch alike; GET when no method is given.
        if (call is { Name: "fetch" or "$fetch" or "ofetch", Arguments.Count: > 0 } && call.Target is null)
        {
            var init = call.Arguments.ElementAtOrDefault(1);
            var verb = init?.Members.FirstOrDefault(x => x.Member == "method")?.Value is { } method ? Literal(method) : init is null || init.Members.Count > 0 ? "GET" : null;
            return new Sent(verb?.ToUpperInvariant(), code.StringOf(call.Arguments[0], through), call.At);
        }

        // axios.get(url), http.post<T>(url, body), and axios({ method, url }) or client.request({ method, url }).
        if (call.Target is null && call.Receiver is { } receiver && Client.IsMatch(receiver))
        {
            if (Verbs.Contains(call.Name) && call.Arguments.Count > 0)
            {
                return new Sent(call.Name.ToUpperInvariant(), code.StringOf(call.Arguments[0], through), call.At);
            }

            if (call.Name == "request" && call.Arguments.FirstOrDefault() is { Members.Count: > 0 } config)
            {
                var verb = config.Members.FirstOrDefault(x => x.Member == "method")?.Value is { } method ? Literal(method) : "GET";
                var url = config.Members.FirstOrDefault(x => x.Member == "url")?.Value;
                return new Sent(verb?.ToUpperInvariant(), url is null ? null : code.StringOf(url, through), call.At);
            }
        }

        return null;
    }

    /// <summary>
    /// A URL as a route: from its first <c>/</c> after a base URL (a value before the path, or a scheme and host), without its
    /// query string, its values as parameters.
    /// </summary>
    private static (string Path, string Shown)? Route(LiteralNode url)
    {
        var text = VueCode.Text(url);
        if (text is null)
        {
            return null;
        }

        var shown = VueCode.Shown(url);
        var path = Regex.Replace(text, @"^[a-z]+://[^/]*", "", RegexOptions.IgnoreCase);
        shown = Regex.Replace(shown, @"^[a-z]+://[^/]*", "", RegexOptions.IgnoreCase);
        var slash = path.IndexOf('/');
        if (slash > 0 && path[..slash].All(x => x == VueCode.Hole))
        {
            path = path[slash..];
            shown = shown[shown.IndexOf('/')..];
        }

        if (path.Length == 0 || path.All(x => x == VueCode.Hole))
        {
            return null;
        }

        return (path.Split('?', '#')[0], shown.Split('?', '#')[0].Trim('/'));
    }
}
