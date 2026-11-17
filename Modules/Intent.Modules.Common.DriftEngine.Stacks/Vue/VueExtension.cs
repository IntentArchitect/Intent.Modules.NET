using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

namespace Intent.Modules.Common.DriftEngine.Stacks.Vue;

/// <summary>
/// Reads the User Interface claims (the <c>intent.ui</c> pack's kinds) of a Vue front end's components: single-file
/// components and vue-router's route records. It reads the claims, never the designer: an element is the code its
/// <c>type.exists</c> claim located, and the vocabulary's bindings say what kind of code that is. So the standard designer and a
/// house designer whose pack claims the same kinds with the same bindings are read alike. It answers only for <c>.vue</c>
/// components; stores, modules and proxies, which any TypeScript front end writes alike, are <c>intent.typescript.ui</c>'s.
/// </summary>
public sealed class VueExtension : IDriftExtension
{
    public string Id => "intent.vue";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new Components(), new Members(), new VueLinks()];
}

/// <summary>What the Vue readers share: what a Vue component is, and vue-router's records.</summary>
internal static class VueCode
{
    /// <summary>A Vue single-file component.</summary>
    public static bool IsVue(TypeNode? type) => type is { Kind: "component" } && type.Files.Any(x => x.EndsWith(".vue", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// vue-router's route records: an object with a <c>path</c> and a <c>component</c>, wherever the code writes one. Its
    /// component is the type the name resolves to, or a lazily imported file's.
    /// </summary>
    public static IReadOnlyList<UiCode.RouteRecord> Records(CodeGraph code) =>
        UiCode.Records(code, "vue-router", record =>
            record.Members.FirstOrDefault(x => x.Member == "component")?.Value is { } component
                ? (UiCode.ComponentNamed(code, component.At.File, component.Text), true)
                : (null, false));
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
        var file = UiCode.FileOf(code, type!.At.File);
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
                    return ClaimReading.Compared("Page", records[0].At, context.Expected == "Page");
                }

                if (file is not null && UiCode.Markup(file).FirstOrDefault(x => x.Tag is "RouterView" or "router-view") is { } view)
                {
                    return ClaimReading.Compared("Layout", view.At, context.Expected == "Layout");
                }

                return ClaimReading.Absent(type.At, "Component");
            }
            case "page.route":
                if (records.FirstOrDefault(x => x.Path.Contains(UiCode.Hole)) is { } computed)
                {
                    return ClaimReading.CantVerify("unreadable", "A route's path is computed.", computed.At);
                }

                return records.Count == 0
                    ? ClaimReading.Absent(type.At)
                    : ClaimReading.Stated(string.Join(", ", records.Select(x => UiCode.ModelRoute(x.Path)).Order(StringComparer.Ordinal)), records[0].At);
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
    private static ClaimReading Title(CodeGraph code, TypeNode type, FileNode? file, IReadOnlyList<UiCode.RouteRecord> records)
    {
        foreach (var record in records)
        {
            if (record.Value?.Members.FirstOrDefault(x => x.Member == "meta")?.Value.Members.FirstOrDefault(x => x.Member == "title")?.Value is { } meta)
            {
                return Titled(code.StringOf(meta), meta.At);
            }
        }

        foreach (var (_, _, unit) in file is null ? [] : UiCode.Units(code, file))
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
/// A Vue component's props, emits and functions: the code a Property, Event Emitter or operation is, and what a Property is to
/// its page (bindable, a route or query parameter). A store's members are <c>intent.typescript.ui</c>'s.
/// </summary>
internal sealed class Members : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.exists", "property.query", "property.route", "property.bindable"];

    public ClaimReading? Read(ClaimContext context)
    {
        var element = context.Element;
        var owner = context.Type ?? (element.ParentElement is { } parent ? context.TypeOf(parent) : null);
        if (!VueCode.IsVue(owner))
        {
            return null;
        }

        var code = context.Code;

        // Vue's props and emits are camel case in the script and kebab case in templates; the model may name them either way.
        var kind = UiCode.Bound(context, "Event") is not null ? "event" : UiCode.Bound(context, "Operation") is not null ? "method" : "property";
        var words = UiCode.Words(element.Name);
        var member = context.Member ?? owner!.Members.Where(x => UiCode.Words(x.Name) == words)
            .OrderByDescending(x => x.Kind == kind).ThenByDescending(x => x.Name == element.Name).FirstOrDefault();
        switch (context.Kind)
        {
            case "member.exists":
            {
                if (member is null)
                {
                    // A page's Property can be what its route gives it: useRoute().params.id.
                    var what = kind switch { "event" => "an emit", "method" => "a function", _ => "a prop or state" };
                    return kind == "property" && Parameter(code, owner!, element.Name) is { } read
                        ? ClaimReading.Found(read, read)
                        : ClaimReading.Missing($"`{owner!.At.File}`", $"{what} named `{UiCode.Camel(element.Name)}`");
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
                    return ClaimReading.Absent(owner!.At, "false");
                }

                var model = member.Modifiers.Contains("model")
                            || owner!.Members.Any(x => x.Kind == "event" && (x.Name == $"update:{member.Name}" || x.Name == $"update:{UiCode.Kebab(member.Name)}"));
                return model ? ClaimReading.Compared("true", member.At, context.Expected == "true") : ClaimReading.Absent(member.At, "false");
            }
            default:
            {
                // A route or query parameter: its record's props give it from the route, or the component reads it from the route.
                var from = context.Kind == "property.route" ? "params" : "query";
                var at = member?.At ?? owner!.At;
                var records = VueCode.Records(code).Where(x => x.Component == owner).ToList();
                var camel = Regex.Escape(UiCode.Camel(element.Name));
                var given = from == "params"
                    ? records.Any(x => x.Props is "true" && Regex.IsMatch(x.Path, $@":{camel}\b", RegexOptions.IgnoreCase))
                    : records.Any(x => x.Props is { } props && Regex.IsMatch(props, $@"query\s*\.\s*{camel}\b", RegexOptions.IgnoreCase));
                if (given || Reads(code, owner!, from, element.Name) is not null)
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
        var camel = Regex.Escape(UiCode.Camel(name));
        var pattern = new Regex($@"\b{from}\s*(\.\s*{camel}\b|\[\s*['""]{camel}['""]\s*\])", RegexOptions.IgnoreCase);
        if (UiCode.FileOf(code, component.At.File) is not { } file)
        {
            return null;
        }

        foreach (var (_, _, unit) in UiCode.Units(code, file))
        {
            if (unit.Calls.SelectMany(x => x.Arguments).Concat(unit.Assignments.Select(x => x.Value)).FirstOrDefault(x => pattern.IsMatch(x.Text)) is { } value)
            {
                return value.At;
            }
        }

        return component.Members.FirstOrDefault(x => x.Value is { } value && pattern.IsMatch(value.Text))?.At
               ?? UiCode.Markup(file).SelectMany(x => x.Attributes).FirstOrDefault(x => x.Value is { } value && pattern.IsMatch(value))?.At;
    }

    private static Span? Parameter(CodeGraph code, TypeNode component, string name) => Reads(code, component, "params", name) ?? Reads(code, component, "query", name);
}

/// <summary>
/// What a Vue component uses, navigates to, opens and calls: a child component's tag in its template (Pascal or kebab case), a
/// <c>&lt;RouterLink to&gt;</c> or <c>:to</c>, a call written in a directive, and what its script does (<see cref="ScriptLinks"/>).
/// </summary>
internal sealed class VueLinks : ScriptLinks
{
    protected override bool Answers(TypeNode type) => VueCode.IsVue(type);

    protected override IReadOnlyList<MarkupNode> Markup(FileNode file, TypeNode type) => UiCode.Markup(file).ToList();

    protected override bool Tagged(MarkupNode tag, string name) => tag.Tag == name || tag.Tag == UiCode.Kebab(name);

    protected override IEnumerable<(string? Path, Span At)> MarkupLinks(CodeGraph code, IReadOnlyList<MarkupNode> markup)
    {
        foreach (var attribute in markup.SelectMany(x => x.Attributes))
        {
            if (attribute is { Directive: null, Name: "to" or "href" } && attribute.Literal is { Kind: "string", Value: { } to })
            {
                yield return (to, attribute.At);
            }
            else if (attribute is { Directive: "bind", Argument: "to" } && attribute.Value is { } value)
            {
                yield return (UiCode.Written(value) ?? Path(value), attribute.At);
            }
        }

        // :to="{ path: '/cart' }"
        static string? Path(string value) => Regex.Match(value, @"path\s*:\s*(['""`][^'""`]*['""`])") is { Success: true } path ? UiCode.Written(path.Groups[1].Value) : null;
    }

    protected override MarkupAttributeNode? MarkupCall(IReadOnlyList<MarkupNode> markup, string name)
    {
        var written = new Regex($@"\.{Regex.Escape(name)}\s*\(");
        return markup.SelectMany(x => x.Attributes).FirstOrDefault(x => x.Directive is not null && x.Value is { } value && written.IsMatch(value));
    }
}
