namespace Intent.Modules.Common.DriftEngine.Code;

/// <summary>
/// The string a value is, as far as the code says without running it (code-reader.md, String values), read from the facts
/// every language gives a value, so it is the same for all of them: literals and interpolation, <c>+</c>, what a local is
/// given, fields and constants, conversions that keep the text, trims, and what a method of the code returns, its
/// parameters bound to the call's arguments. Anything else is a hole.
/// </summary>
internal static class StringValues
{
    // How deep it follows names and calls; a URI is built a few helpers deep at most.
    private const int MaxDepth = 16;

    private static readonly IReadOnlyDictionary<ParameterNode, List<Part>> Unbound = new Dictionary<ParameterNode, List<Part>>();

    public static LiteralNode Of(ValueNode value) => Literal(Evaluate(value, Unbound, 0));

    /// <summary>
    /// A value written in a method of the code, as the calls into it give it: <paramref name="through"/> runs from the outermost
    /// call to the one into the method holding the value, and each binds its method's parameters to its arguments, read with the
    /// bindings of the calls before it.
    /// </summary>
    public static LiteralNode Of(ValueNode value, IReadOnlyList<CallNode> through) =>
        through.All(x => x.Target is not null) ? Of(value, through.Select(x => (x, x.Target!.Member)).ToList()) : Literal([Hole(value)]);

    /// <summary>
    /// As <see cref="Of(ValueNode, IReadOnlyList{CallNode})"/>, each call with the method it runs, where the call doesn't say:
    /// one made on an interface runs an implementation's method.
    /// </summary>
    public static LiteralNode Of(ValueNode value, IReadOnlyList<(CallNode Call, MemberNode Into)> through)
    {
        var bindings = Unbound;
        foreach (var (call, into) in through)
        {
            if (into.Body is not { } body)
            {
                return Literal([Hole(value)]);
            }

            var outer = bindings;
            bindings = Bind(body.Parameters, call.Arguments, x => Evaluate(x, outer, 1));
        }

        return Literal(Evaluate(value, bindings, 0));
    }

    private static LiteralNode Literal(List<Part> parts)
    {
        return parts.All(x => x.Hole is null)
            ? new LiteralNode { Kind = "string", Value = string.Concat(parts.Select(x => x.Text)) }
            : new LiteralNode
            {
                Kind = "interpolated",
                Parts = parts.Where(x => x.Hole is not null || x.Text!.Length > 0)
                    .Select(x => x.Hole ?? new ValueNode { Text = x.Text!, Literal = new LiteralNode { Kind = "string", Value = x.Text }, At = x.At })
                    .ToList()
            };
    }

    private static List<Part> Evaluate(ValueNode value, IReadOnlyDictionary<ParameterNode, List<Part>> bindings, int depth)
    {
        if (depth > MaxDepth)
        {
            return [Hole(value)];
        }

        if (value.Literal is { } literal)
        {
            // A number, boolean or type is text only as a language prints it, so it is a hole.
            return literal.Kind switch
            {
                "string" or "char" => [new Part(literal.Value ?? "", null, value.At)],
                "interpolated" => Merge(literal.Parts.SelectMany(x => Evaluate(x, bindings, depth + 1))),
                _ => [Hole(value)]
            };
        }

        if (value.Joined.Count > 0)
        {
            return Merge(value.Joined.SelectMany(x => Evaluate(x, bindings, depth + 1)));
        }

        if (value.Trimmed is { } trim)
        {
            return Trim(Evaluate(trim.Value, bindings, depth + 1), trim);
        }

        if (value.Parameter is { } parameter)
        {
            return bindings.TryGetValue(parameter, out var bound) ? bound : [Hole(value)];
        }

        if (value.Field is { } field)
        {
            return FieldValue(field, depth) ?? [Hole(value)];
        }

        if (value.Call is { } call && Returned(call, bindings, depth) is { } returned)
        {
            return returned;
        }

        if (value.Given.Count > 0 && Agreed(value.Given.Select(x => Evaluate(x, bindings, depth + 1)).ToList()) is { } given)
        {
            return given;
        }

        return [Hole(value)];
    }

    /// <summary>
    /// A field's or property's value: its initializer, or what its type's code assigns it, when there is one, or when all of
    /// them give the same string; none when its code changes it any other way (<c>+=</c>). It is the type's, so a call's
    /// bindings don't reach it.
    /// </summary>
    private static List<Part>? FieldValue(FieldRef field, int depth)
    {
        var name = field.Member.Name;
        if (field.Type.Members.Select(x => x.Body).OfType<FunctionNode>().SelectMany(x => x.Changes).Any(x => x == name || x == $"this.{name}"))
        {
            return null;
        }

        var values = new List<ValueNode>();
        if (field.Member.Value is { } initializer)
        {
            values.Add(initializer);
        }

        values.AddRange(field.Type.Members.Select(x => x.Body).OfType<FunctionNode>().SelectMany(x => x.Assignments)
            .Where(x => x.Target == name || x.Target == $"this.{name}")
            .Select(x => x.Value));
        return Agreed(values.Select(x => Evaluate(x, Unbound, depth + 1)).ToList());
    }

    /// <summary>What a method of the code returns, with its parameters bound to the call's arguments: its every return, when they agree.</summary>
    private static List<Part>? Returned(CallNode call, IReadOnlyDictionary<ParameterNode, List<Part>> bindings, int depth)
    {
        if (call.Target?.Member.Body is not { Returns.Count: > 0 } body)
        {
            return null;
        }

        var bound = Bind(body.Parameters, call.Arguments, x => Evaluate(x, bindings, depth + 1));
        return Agreed(body.Returns.Select(x => Evaluate(x, bound, depth + 1)).ToList());
    }

    /// <summary>
    /// A call's arguments bound to its method's parameters: a positional argument by its place, a named one by its name
    /// (C#'s <c>status: "Paid"</c>, Python's <c>status="Paid"</c>), wherever it is written.
    /// </summary>
    private static Dictionary<ParameterNode, List<Part>> Bind(IReadOnlyList<ParameterNode> parameters, IReadOnlyList<ValueNode> arguments, Func<ValueNode, List<Part>> evaluate)
    {
        var bound = new Dictionary<ParameterNode, List<Part>>();
        for (var i = 0; i < arguments.Count; i++)
        {
            var parameter = arguments[i].Name is { } name ? parameters.FirstOrDefault(x => x.Name == name) : i < parameters.Count ? parameters[i] : null;
            if (parameter is not null)
            {
                bound[parameter] = evaluate(arguments[i]);
            }
        }

        return bound;
    }

    /// <summary>The one string all the ways give; null when they differ, or there are none.</summary>
    private static List<Part>? Agreed(IReadOnlyList<List<Part>> values) =>
        values.Count > 0 && values.Skip(1).All(x => Same(x, values[0])) ? values[0] : null;

    private static bool Same(List<Part> a, List<Part> b) =>
        a.Count == b.Count && a.Zip(b).All(x => x.First.Hole is null ? x.Second.Hole is null && x.First.Text == x.Second.Text : x.Second.Hole?.Text == x.First.Hole.Text);

    /// <summary>A trim takes characters from the text at its start or end; a hole there may hold them, so it is left as it is.</summary>
    private static List<Part> Trim(List<Part> parts, TrimNode trim)
    {
        var characters = trim.Characters.Count == 0 ? null : trim.Characters.ToArray();
        var trimmed = parts.ToList();
        if (trim.End && trimmed.Count > 0 && trimmed[^1].Hole is null)
        {
            trimmed[^1] = trimmed[^1] with { Text = characters is null ? trimmed[^1].Text!.TrimEnd() : trimmed[^1].Text!.TrimEnd(characters) };
        }

        if (trim.Start && trimmed.Count > 0 && trimmed[0].Hole is null)
        {
            trimmed[0] = trimmed[0] with { Text = characters is null ? trimmed[0].Text!.TrimStart() : trimmed[0].Text!.TrimStart(characters) };
        }

        return trimmed;
    }

    /// <summary>Adjacent text joined into one part.</summary>
    private static List<Part> Merge(IEnumerable<Part> parts)
    {
        var merged = new List<Part>();
        foreach (var part in parts)
        {
            if (part.Hole is null && merged.Count > 0 && merged[^1].Hole is null)
            {
                merged[^1] = merged[^1] with { Text = merged[^1].Text + part.Text };
            }
            else
            {
                merged.Add(part);
            }
        }

        return merged;
    }

    private static Part Hole(ValueNode value) => new(null, value, value.At);

    /// <param name="Text">Known text; null for a hole.</param>
    /// <param name="Hole">The value the code doesn't say the text of.</param>
    /// <param name="At">Where the text is written, or the hole.</param>
    private readonly record struct Part(string? Text, ValueNode? Hole, Span At);
}
