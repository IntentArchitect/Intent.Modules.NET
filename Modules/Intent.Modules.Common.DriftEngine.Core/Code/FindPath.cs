using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Code;

/// <summary>A place a find path reached: the type it's in, the member, call or annotation, and the groups its regular expressions captured.</summary>
public sealed record FindHit(TypeNode Type, MemberNode? Member, object Node, Span At, IReadOnlyList<string> Groups)
{
    /// <summary>
    /// A convention's <c>name</c> for the place: <c>{Class}</c> and <c>{Type}</c>, <c>{Method}</c> and <c>{Member}</c>,
    /// <c>{File}</c>, and <c>$1</c>… for what the last step's expression captured.
    /// </summary>
    public string Name(string? template)
    {
        if (template is null)
        {
            return Member is null ? Type.Name : $"{Type.Name}.{Member.Name}";
        }

        var own = Node switch { ParameterNode x => x.Name, MemberNode x => x.Name, CallNode x => x.Name, AttributeNode x => x.Name, MarkupNode x => x.Tag, _ => Type.Name };
        var name = template.Replace("{Class}", Type.Name).Replace("{Type}", Type.Name).Replace("{File}", At.File)
            .Replace("{Method}", Member?.Name ?? "").Replace("{Member}", Member?.Name ?? "").Replace("{Name}", own);
        for (var i = Groups.Count - 1; i >= 1; i--)
        {
            name = name.Replace($"${i}", Groups[i]);
        }

        return name;
    }
}

/// <summary>
/// A find path over the graph (rules.md, Recipes): steps joined by <c> &gt; </c>, each a kind of node, optionally
/// <c>with</c> a .NET regular expression its canonical text must match somewhere. <c>type</c> (or <c>class</c>) starts it;
/// <c>method</c>, <c>property</c>, <c>field</c>, <c>constructor</c> and <c>member</c> go into a type's members, <c>parameter</c>
/// into a member's parameters, <c>call</c> into a member's calls (every member's, after a type), <c>annotation</c> onto the
/// node before, and <c>markup</c> into the markup elements of a type's files, or those inside the element before. An element's
/// text holds the elements inside it, so <c>markup with</c> keeps only the innermost elements that match (rules.md, Recipes).
/// </summary>
public static class FindPath
{
    private static readonly string[] MemberKinds = ["method", "property", "field", "constructor", "member"];

    /// <param name="path">The find path.</param>
    /// <param name="types">The types it starts at.</param>
    /// <param name="code">The graph the types are in, for their files' markup.</param>
    /// <exception cref="ArgumentException">A step the path language doesn't have.</exception>
    public static IEnumerable<FindHit> Run(string path, IEnumerable<TypeNode> types, CodeGraph code)
    {
        // A regular expression may hold '>' itself (Property<T>), so steps are split on the spaced form only.
        var steps = path.Split(" > ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(Step).ToList();
        if (steps.Count == 0 || steps[0].Kind is not ("type" or "class"))
        {
            throw new ArgumentException($"A find path starts at a type: '{path}'.");
        }

        IEnumerable<FindHit> hits = types.Select(x => new FindHit(x, null, x, x.At, []));
        foreach (var (kind, regex) in steps)
        {
            var step = kind;
            var with = regex;
            hits = step == "markup"
                ? hits.SelectMany(hit => Markup(hit, with, code))
                : hits.SelectMany(hit => Next(hit, step)).SelectMany(hit => Match(hit, with));
        }

        return hits;
    }

    private static IEnumerable<FindHit> Markup(FindHit hit, Regex? regex, CodeGraph code)
    {
        var elements = hit.Node switch
        {
            TypeNode type when hit.Member is null => code.MarkupOf(type).ToList(),
            MarkupNode element => element.DescendantsAndSelf().Skip(1).ToList(),
            _ => []
        };
        var matches = elements.Select(x => (Element: x, Match: regex?.Match(x.Text))).Where(x => x.Match is null or { Success: true }).ToList();
        if (regex is not null)
        {
            // An element whose text matches only because an element inside it does isn't the place.
            var inner = matches.Select(x => x.Element).ToHashSet();
            matches = matches.Where(x => !x.Element.DescendantsAndSelf().Skip(1).Any(inner.Contains)).ToList();
        }

        return matches.Select(x => hit with { Node = x.Element, At = x.Element.At, Groups = x.Match?.Groups.Cast<Group>().Select(g => g.Value).ToList() ?? [] });
    }

    private static (string Kind, Regex? With) Step(string step)
    {
        var at = step.IndexOf(" with ", StringComparison.Ordinal);
        var kind = (at < 0 ? step : step[..at]).Trim();
        if (kind is not ("type" or "class" or "call" or "annotation" or "parameter" or "markup") && !MemberKinds.Contains(kind))
        {
            throw new ArgumentException($"A find path has a step this engine doesn't read: '{step}'.");
        }

        return (kind, at < 0 ? null : new Regex(step[(at + " with ".Length)..], RegexOptions.CultureInvariant));
    }

    private static IEnumerable<FindHit> Next(FindHit hit, string kind) => kind switch
    {
        "type" or "class" => hit.Member is null && hit.Node is TypeNode ? [hit] : [],
        "call" => (hit.Member is { } member ? [member] : hit.Type.Members).SelectMany(m => m.Calls.Select(c => hit with { Member = m, Node = c, At = c.At })),
        "annotation" => (hit.Node switch { TypeNode t => t.Attributes, MemberNode m => m.Attributes, ParameterNode p => p.Attributes, _ => [] }).Select(a => hit with { Node = a, At = a.At }),
        "parameter" => hit.Node is MemberNode owner ? owner.Parameters.Select(p => hit with { Node = p, At = p.At }) : [],
        _ => hit.Node is TypeNode type
            ? type.Members.Where(m => kind == "member" || m.Kind == kind).Select(m => hit with { Member = m, Node = m, At = m.At })
            : []
    };

    private static IEnumerable<FindHit> Match(FindHit hit, Regex? regex)
    {
        if (regex is null)
        {
            return [hit];
        }

        var match = regex.Match(TextOf(hit.Node));
        return match.Success ? [hit with { Groups = match.Groups.Cast<Group>().Select(x => x.Value).ToList() }] : [];
    }

    private static string TextOf(object node) => node switch
    {
        TypeNode type => type.Text,
        MemberNode member => member.Text,
        ParameterNode parameter => parameter.Text,
        CallNode call => call.Text,
        MarkupNode element => element.Text,
        AttributeNode attribute => attribute.Arguments.Count == 0 ? attribute.Name : $"{attribute.Name}({string.Join(",", attribute.Arguments)})",
        _ => ""
    };
}
