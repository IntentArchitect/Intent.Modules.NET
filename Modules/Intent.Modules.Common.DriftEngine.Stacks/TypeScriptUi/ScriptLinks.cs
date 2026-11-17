using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

/// <summary>
/// What TypeScript code uses, navigates to, opens and calls, read from its code and, for a component, its markup: a child
/// component's tag, an import of a store, service or utility, a link or a router's navigation to a page's route, a dialog it
/// renders, and a call of a proxy's method, itself or through the code's own functions. What is read from code is the same
/// for every framework; a framework's reader says which code is its own and how its markup renders, links and calls.
/// </summary>
internal abstract class ScriptLinks : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.uses", "intent.navigates", "intent.opens", "intent.invokes", "intent.target"];

    /// <summary>The code a link starts in is this reader's to read.</summary>
    protected abstract bool Answers(TypeNode type);

    /// <summary>The units of code a link from the type reads: by default every unit of its file.</summary>
    protected virtual IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> Units(CodeGraph code, FileNode file, TypeNode type) => UiCode.Units(code, file).ToList();

    /// <summary>The markup the type renders, every element at any depth; none for code that renders nothing.</summary>
    protected virtual IReadOnlyList<MarkupNode> Markup(FileNode file, TypeNode type) => [];

    /// <summary>Where a link the code doesn't make is reported.</summary>
    protected virtual Span Start(FileNode file, TypeNode type) => new(file.Path, 1);

    /// <summary>A tag of the name, for a target nothing located: the name as written, or as the framework also writes it.</summary>
    protected virtual bool Tagged(MarkupNode tag, string name) => tag.Tag == name;

    /// <summary>The paths the markup links to, null for one it computes whole, and where each is.</summary>
    protected virtual IEnumerable<(string? Path, Span At)> MarkupLinks(CodeGraph code, IReadOnlyList<MarkupNode> markup) => [];

    /// <summary>A call of the method written in markup that isn't code the units hold (a Vue template's <c>@click="api.save()"</c>).</summary>
    protected virtual MarkupAttributeNode? MarkupCall(IReadOnlyList<MarkupNode> markup, string name) => null;

    public ClaimReading? Read(ClaimContext context)
    {
        // A link is an association, from the element at its source end, or an element its source holds and types by the target
        // (a house designer's Rendered Component).
        var (source, target) = context.Element is IAssociationEnd end
            ? (context.Owner(end), end.TypeReference?.Element as IElement)
            : (context.Element.ParentElement, context.Element.TypeReference?.Element as IElement);
        if (source is null || target is null
            || UiCode.CodeOf(context, source) is not { } located || !UiCode.IsTypeScript(located.Type) || !Answers(located.Type)
            || UiCode.FileOf(context.Code, located.Type.At.File) is not { } file)
        {
            return null;
        }

        var (holder, type) = located;
        var code = context.Code;
        var start = Start(file, type);
        var markup = Markup(file, type);
        // From one of a component's operations, only that function's code is read.
        var units = Units(code, file, type);
        if (holder != source && units.FirstOrDefault(x => x.Member is { } member && UiCode.Words(member.Name) == UiCode.Words(source.Name)) is { Member: not null } operation)
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

                    return markup.FirstOrDefault(x => Tagged(x, target.Name)) is { } named
                        ? ClaimReading.Found(named, named.At)
                        : file.Imports.FirstOrDefault(x => !x.TypeOnly && x.File is { } imported && UiCode.Words(UiCode.Stem(imported)) == UiCode.Words(target.Name)) is { } byName
                            ? ClaimReading.Found(byName, byName.At)
                            : ClaimReading.Absent(start);
                }

                if (used.Kind == "component" && Rendered(code, markup, type, used) is { } tag)
                {
                    return ClaimReading.Found(tag, tag.At);
                }

                // A Composition renders its child; a Uses (whose vocabulary binds what is Used) can also import it for its value.
                if (used.Kind == "component" && !(UsesByImport && context.Kind == "intent.uses" && UiCode.Bound(context, "Used") is not null))
                {
                    return ClaimReading.Absent(start);
                }

                return file.Imports.FirstOrDefault(x => !x.TypeOnly && x.File is { } imported && used.Files.Contains(imported) && x.File != file.Path) is { } import
                    ? ClaimReading.Found(import, import.At)
                    : ClaimReading.Absent(start);
            }
            case "intent.navigates":
                return Navigates(context, code, markup, units, target, start);
            case "intent.target":
                return Target(context, code, file, type, markup, units, target);
            default:
                return Invocation(context, code, file, markup, units, target) is { } call ? ClaimReading.Found(call.Value, call.At) : ClaimReading.Absent(start);
        }
    }

    /// <summary>A component the code doesn't render is used, though not composed or opened, when its file is imported for its value (a route's element, a prop).</summary>
    protected virtual bool UsesByImport => false;

    /// <summary>A tag in the markup that renders the component: one resolving to its type, else one of its name.</summary>
    private MarkupNode? Rendered(CodeGraph code, IReadOnlyList<MarkupNode> markup, TypeNode from, TypeNode component) =>
        markup.FirstOrDefault(x => x.Component is { } named && code.TypeFor(named, from) == component)
        ?? markup.FirstOrDefault(x => Tagged(x, component.Name));

    /// <summary>
    /// What a link reaches is the element the model names, not another of its name: a child's tag resolves to the component
    /// the child is located as, and a proxy operation's call is made on the proxy the operation's owner is located as.
    /// </summary>
    private ClaimReading? Target(ClaimContext context, CodeGraph code, FileNode file, TypeNode type, IReadOnlyList<MarkupNode> markup,
        IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> units, IElement target)
    {
        var expected = context.ExpectedValue as IElement ?? target;
        var located = context.TypeOf(expected);
        if (located is { Kind: "component" })
        {
            if (markup.FirstOrDefault(x => Tagged(x, located.Name) || x.Component?.Name == located.Name) is not { } tag)
            {
                return null;
            }

            return tag.Component is { } named && code.TypeFor(named, type) is { } resolved
                ? ClaimReading.Compared(resolved.Name, tag.At, resolved == located)
                : ClaimReading.CantVerify("unresolved", "The tag doesn't resolve to a component in scope.", tag.At);
        }

        if (Invocation(context, code, file, markup, units, target) is not { Value: CallNode call })
        {
            return null;
        }

        var on = call.Target?.Type;
        return ClaimReading.Compared(on?.Name ?? call.Receiver ?? call.Text, call.At, located is null || on == located || on is null && call.Receiver == located.Name);
    }

    /// <summary>
    /// A link in the markup, or a navigation in the code, to one of the target page's routes: its <c>page.route</c> claim's,
    /// else the vocabulary's <c>Route</c>. The code navigates with a router's or history's <c>push</c> or <c>replace</c>
    /// (vue-router, react-router 5), <c>navigate(...)</c> (react-router's <c>useNavigate()</c>, a data router's) or
    /// <c>redirect(...)</c>. A computed part of a link stands for a parameter; a link the code computes whole, where none
    /// matches, can't be verified.
    /// </summary>
    private ClaimReading Navigates(ClaimContext context, CodeGraph code, IReadOnlyList<MarkupNode> markup, IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> units, IElement target, Span start)
    {
        // A route is shown without its slashes, so the root route is the empty one.
        var claimed = context.ClaimOn(target, "page.route")?.Expected ?? context.Claim.Bindings?.GetValueOrDefault("Route");
        if (claimed is null)
        {
            return ClaimReading.CantVerify("unresolved", $"`{target.Name}` has no route to link to.");
        }

        var routes = claimed.Split(',', StringSplitOptions.TrimEntries) is var split && split.All(x => x.Length > 0) ? split : [.. split.Where(x => x.Length > 0), ""];

        var links = MarkupLinks(code, markup).ToList();
        foreach (var call in units.SelectMany(x => x.Unit.Calls).Where(IsNavigation))
        {
            var argument = call.Arguments[0];
            var path = argument.Members.FirstOrDefault(x => x.Member is "path" or "pathname")?.Value ?? argument;
            links.Add((UiCode.Text(code.StringOf(path)), call.At));
        }

        Span? computed = null;
        foreach (var (path, at) in links)
        {
            // A link the code computes whole (:to="target") can't be read; one with values in it (`/orders/${id}`) can.
            if (path is null || path.Contains(UiCode.Hole) && path.Replace("/", "").All(x => x == UiCode.Hole))
            {
                computed ??= at;
                continue;
            }

            if (routes.Any(route => UiCode.Matches(path, route)))
            {
                return ClaimReading.Found(target, at);
            }
        }

        return computed is { } unreadable ? ClaimReading.CantVerify("unreadable", "A link's path is computed.", unreadable) : ClaimReading.Absent(start);
    }

    private static readonly Regex Router = new(@"(?i)(router|history)$");

    private static bool IsNavigation(CallNode call) => call.Arguments.Count > 0 && (
        call.Name is "push" or "replace" && call.Receiver is { } receiver && Router.IsMatch(receiver)
        || call is { Name: "navigate" or "redirect", Receiver: null }
        || call is { Name: "navigate", Receiver: { } on } && Router.IsMatch(on));

    /// <summary>
    /// The call of the target operation the code makes, nearest first: in its own code, then in the code's own functions it
    /// calls, on the operation's located method or one of its name on the proxy's located type; or a call of it written in
    /// markup, with the proxy's file imported.
    /// </summary>
    private (object Value, Span At)? Invocation(ClaimContext context, CodeGraph code, FileNode file, IReadOnlyList<MarkupNode> markup,
        IReadOnlyList<(TypeNode Type, MemberNode? Member, FunctionNode Unit)> units, IElement operation)
    {
        var method = context.MemberOf(operation);
        var proxy = operation.ParentElement is { } parent ? context.TypeOf(parent) : null;
        var name = method?.Name ?? UiCode.Camel(operation.Name);
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

                foreach (var (type, member) in UiCode.Reached(code, current.Type, call))
                {
                    if (member.Body is { } body && seen.Add(body))
                    {
                        queue.Enqueue((type, body));
                    }
                }
            }
        }

        if (proxy is not null && file.Imports.Any(x => !x.TypeOnly && x.File is { } imported && proxy.Files.Contains(imported)) && MarkupCall(markup, name) is { } attribute)
        {
            return (attribute, attribute.At);
        }

        return null;
    }
}

/// <summary>
/// The links of TypeScript code that isn't a component: a store, service, utility or proxy, read from its code alone, so any
/// framework's modules are read alike.
/// </summary>
internal sealed class ModuleLinks : ScriptLinks
{
    protected override bool Answers(TypeNode type) => type.Kind != "component";
}
