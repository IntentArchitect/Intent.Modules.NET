using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Scope;
using Intent.Modules.Common.DriftEngine.Vocabulary;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>A pack's recipe for one designer's claim kind (rules.md, Recipes), and the files it may read.</summary>
internal sealed record RecipeRule(Pack Pack, string Designer, string Kind, JsonObject Data, IReadOnlyList<GlobPattern> Files);

/// <summary>
/// A place a recipe's find reached: a whole file (no node), or a type, member, parameter, annotation, call or markup element,
/// with the type and member it is in.
/// </summary>
internal sealed record RecipeHit(string File, object? Node, TypeNode? Type, MemberNode? Member, Span At);

/// <summary>
/// Recipes (rules.md, Recipes): Common.Drift's format over the code graph. A find path's steps reach a place in the code, a
/// read case's regular expression reads a value from its text, and the outcome follows the outcome order of
/// claims-and-findings.md: nothing found is contradicted for an existence kind or a <c>whenSet</c> one, the recipe's
/// <c>absent</c> where it has one, else can't verify; found with no read case matching is its <c>default</c>, else the same.
/// Text is a node's canonical text, or a file's read as text, so a recipe reads any language and any file.
/// </summary>
public sealed partial class DriftEngine
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(2);

    private static readonly HashSet<string> StepKinds = new(StringComparer.Ordinal)
    {
        "file", "type", "class", "method", "constructor", "property", "field", "member", "parameter", "annotation", "call", "chain", "markup"
    };

    private List<RecipeRule>? _recipes;
    private readonly Dictionary<string, (string Text, IReadOnlyList<int> Lines)?> _sources = new(StringComparer.Ordinal);

    /// <summary>
    /// Existence claims a recipe verified from text no node of the graph holds, such as a file read as text: what waits on
    /// them can't read the code they found.
    /// </summary>
    private readonly HashSet<Claim> _textOnly = [];

    /// <summary>The recipe that reads the claim: of the packs' recipes for its designer and kind whose <c>for</c> selects its element, the last pack's.</summary>
    private RecipeRule? RecipeFor(Claim claim) =>
        Recipes().LastOrDefault(x => x.Designer == claim.Designer && x.Kind == claim.Kind && Selects(x.Data["for"], claim.Source));

    /// <summary>
    /// Every pack's recipes, checked once without the code (rules.md, Plug-ins): one that can't run is a configuration error
    /// and doesn't run. A <c>whenSet</c> kind's recipe with a <c>default</c> or <c>absent</c> is refused (decision 4).
    /// </summary>
    private List<RecipeRule> Recipes()
    {
        if (_recipes is not null)
        {
            return _recipes;
        }

        _recipes = [];
        foreach (var pack in _packs)
        {
            foreach (var (designer, rules) in pack.Data["rules"] as JsonObject ?? [])
            {
                var files = Globs(rules?["files"]);
                foreach (var (kind, data) in rules?["facts"] as JsonObject ?? [])
                {
                    var problem = data is JsonObject recipe ? RecipeProblem(kind, recipe) : ("config/ruleInvalid", "isn't an object with find and read");
                    if (problem is var (rule, text))
                    {
                        _packFindings.Add(new Finding(rule, ScopeArea, "Code", "error", $"Pack `{pack.Id}`'s {designer} recipe for `{kind}` {text}", new Anchor(Text: pack.Id)));
                        continue;
                    }

                    var own = (JsonObject)data!;
                    _recipes.Add(new RecipeRule(pack, designer, kind, own, own["files"] is { } narrower ? Globs(narrower) : files));
                }
            }
        }

        return _recipes;
    }

    private (string Rule, string Text)? RecipeProblem(string kind, JsonObject recipe)
    {
        if (!_kinds.TryGetValue(kind, out var definition))
        {
            return ("config/ruleInvalid", $"reads `{kind}`, which no installed pack or extension defines, so it doesn't run.");
        }

        if (definition.Stated == "whenSet" && (recipe["default"] is not null || recipe["absent"] is not null))
        {
            return ("config/silenceForWhenSet",
                $"has {(recipe["default"] is not null ? "a `default`" : "an `absent`")}, but `{kind}` is stated only when set: the model states it only where the code does, so the code's silence can't be a value of it (decision 4). Remove it; the recipe doesn't run until then.");
        }

        if (recipe["query"] is not null)
        {
            return ("config/ruleInvalid", "is a query (rules.md, Queries), which this engine doesn't run yet, so it doesn't run.");
        }

        var paths = Strings(recipe["find"]);
        if (paths.Count == 0)
        {
            return ("config/ruleInvalid", "has no find, so it doesn't run.");
        }

        try
        {
            // An inherit target that isn't a kind of code around the find's is a find path of its own.
            var inherits = Strings(recipe["inherit"]).Where(x => !InheritKinds.Contains(x));
            foreach (var step in paths.Concat(inherits).SelectMany(Steps).Where(x => x.Pattern is not null))
            {
                _ = Compile(Naming.Fill(step.Pattern!, Samples, Regex.Escape));
            }

            foreach (var readCase in recipe["read"] as JsonArray ?? [])
            {
                if (readCase is not JsonObject item)
                {
                    return ("config/ruleInvalid", $"has a read case that isn't an object ({readCase?.ToJsonString()}), so it doesn't run.");
                }

                if (item["node"]?.GetValue<string>() is { } node && NodePath(node).Select(x => x.Field).FirstOrDefault(x => !NodeFields.Contains(x)) is { } unknown)
                {
                    return ("config/ruleInvalid", $"has a node read case naming `{unknown}`, a field no code node has (code-reader.md, The code graph), so it doesn't run.");
                }

                if (item["match"]?.GetValue<string>() is { } match)
                {
                    _ = Compile(Naming.Fill(match, Samples, Regex.Escape));
                }
            }
        }
        catch (ArgumentException e)
        {
            return ("config/ruleInvalid", $"can't run: {e.Message}");
        }

        return null;
    }

    // Any placeholder, for checking a regular expression before the claims give them values.
    private static readonly IReadOnlyDictionary<string, string> Samples = new SampleValues();

    private sealed class SampleValues : Dictionary<string, string>, IReadOnlyDictionary<string, string>
    {
        bool IReadOnlyDictionary<string, string>.TryGetValue(string key, out string value)
        {
            value = "x";
            return true;
        }
    }

    /// <summary>A recipe's <c>for</c>: the element type, or the association a claim's end belongs to, and its <c>where</c>.</summary>
    private static bool Selects(JsonNode? @for, IElement element)
    {
        if (@for is null)
        {
            return true;
        }

        if (@for["element"]?.GetValue<string>() is { } type && element.SpecializationType != type)
        {
            return false;
        }

        if ((@for["association"] ?? @for["associationEnd"])?.GetValue<string>() is { } association && (element as IAssociationEnd)?.Association?.SpecializationType != association)
        {
            return false;
        }

        try
        {
            return @for["where"]?.GetValue<string>() is not { } where || Expression.Evaluate(where, element) is true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private void Recipe(Claim claim, RecipeRule rule)
    {
        try
        {
            RunRecipe(claim, rule);
        }
        catch (RegexMatchTimeoutException)
        {
            Cant(claim, "ruleFailed", new Reading { Detail = $"A regular expression of pack `{rule.Pack.Id}`'s recipe took too long. Simplify it." });
        }
        catch (Exception e) when (e is ArgumentException or FormatException)
        {
            Cant(claim, "ruleFailed", new Reading { Detail = $"Pack `{rule.Pack.Id}`'s recipe failed: {e.Message}" });
        }
    }

    private void RunRecipe(Claim claim, RecipeRule rule)
    {
        var recipe = rule.Data;
        var values = Placeholders(claim);
        var files = _files.Where(x => x.Decision.In && (rule.Files.Count == 0 || rule.Files.Any(g => g.IsMatch(x.Path)))).Select(x => x.Path).ToHashSet(StringComparer.Ordinal);
        var paths = Strings(recipe["find"]);
        List<RecipeHit> hits = [];
        RecipeHit? near = null;
        foreach (var path in paths)
        {
            hits = Find(claim, Steps(path), values, files, ref near);
            if (hits.Count > 0)
            {
                break;
            }
        }

        var searched = string.Join("` or `", paths.Select(x => Naming.Fill(x, values)));
        if (hits.Count == 0)
        {
            NothingFound(claim, rule, values, paths, searched, near);
            return;
        }

        // Several places are one only when they are one type's parts: files a type is declared across (a Razor component's
        // .razor and its code-behind, R37) are read together. A recipe reading any of several takes the first.
        hits = hits.OrderBy(x => x.At.File, StringComparer.Ordinal).ThenBy(x => x.At.Line).ToList();
        var parts = hits.Count > 1 && hits.All(x => x.Member is null) && hits.Select(x => x.File).Distinct().Count() == hits.Count
                    && (_graph.Types.Any(t => hits.All(h => t.Files.Contains(h.File))) || hits.Select(x => CodeBehindOf(x.File)).Distinct().Count() == 1);
        if (hits.Count > 1 && !parts && recipe["matches"]?.GetValue<string>() != "any")
        {
            Cant(claim, "ambiguous", new Reading { Candidates = hits.Select(x => x.At).ToList() });
            return;
        }

        var unit = hits[0];
        var cases = (recipe["read"] as JsonArray ?? []).OfType<JsonObject>().ToList();
        if (ReadCases(claim, rule, cases, parts ? hits : [unit], values))
        {
            return;
        }

        // No read case matched where the find ended: each inherit target in turn, nearest first, as Common.Drift read an
        // action's security from its controller.
        foreach (var target in Strings(recipe["inherit"]))
        {
            foreach (var level in Inherited(claim, target, unit, values, files))
            {
                if (ReadCases(claim, rule, cases, level, values))
                {
                    return;
                }
            }
        }

        // Found, and no read case matched: the recipe's default, as silence, else what the outcome order says.
        if (recipe["default"]?.ToString() is { } @default)
        {
            // The recipe's unstated says what the code lacks, for the finding: no href to /checkout in CartPage's files.
            Apply(claim, ClaimReading.Defaulted(Presence(claim, Naming.Fill(@default, values).Trim()), unit.At,
                recipe["unstated"]?.GetValue<string>() is { } unstated ? Naming.Fill(unstated, values) : null));
        }
        else if (claim.Stated == "whenSet")
        {
            claim.Outcome = "contradicted";
            claim.Reading = new Reading { At = unit.At };
        }
        else
        {
            Cant(claim, "notFound", new Reading { At = unit.At, Detail = $"`{searched}` found {unit.At}, but no read case of the recipe matches there." });
        }
    }

    /// <summary>A file with its code-behind, as one: <c>MainLayout.razor.cs</c> is <c>MainLayout.razor</c>'s.</summary>
    private static string CodeBehindOf(string file) =>
        file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) && Path.GetExtension(file[..^3]).Length > 0 ? file[..^3] : file;

    /// <summary>
    /// The read cases, in order, over the places of one level (the code the find found, or what an inherit target gives):
    /// true when one of them settled the claim.
    /// </summary>
    private bool ReadCases(Claim claim, RecipeRule rule, IReadOnlyList<JsonObject> cases, IReadOnlyList<RecipeHit> places, IReadOnlyDictionary<string, string> values) =>
        cases.Any(readCase => places.Any(place => ReadCase(claim, rule, readCase, place, values)));

    /// <summary>
    /// One read case at one place: a <c>node</c> case reads a field of the node there, a <c>match</c> case a regular expression
    /// over its text, and a case with only a <c>value</c> gives it wherever the find found code. True when it settled the
    /// claim; false when it doesn't apply there.
    /// </summary>
    private bool ReadCase(Claim claim, RecipeRule rule, JsonObject readCase, RecipeHit place, IReadOnlyDictionary<string, string> values)
    {
        var recipe = rule.Data;
        if (readCase["node"]?.GetValue<string>() is { } path)
        {
            if (In(place, readCase["in"]?.GetValue<string>()) is not { } node)
            {
                return false;
            }

            var (has, field) = NodeValue(node, path);
            if (!has)
            {
                // A field path the node reached doesn't have is the recipe's fault (rules.md, Plug-ins).
                Cant(claim, "ruleFailed", new Reading { At = node.At, Detail = $"Pack `{rule.Pack.Id}`'s recipe reads `{path}` from what its find found at {node.At}, which has no such field." });
                return true;
            }

            switch (field)
            {
                case null:
                    return false;
                case TypeRefNode type:
                    ReadValue(claim, rule, node, Transform(type.Text, recipe, readCase), node.At, type);
                    return true;
                default:
                    ReadValue(claim, rule, node, Transform(field.ToString()!, recipe, readCase), node.At);
                    return true;
            }
        }

        if (readCase["match"]?.GetValue<string>() is not { } pattern)
        {
            // A read case without a match gives its value wherever the find found code, as the engine's first recipes did.
            if (readCase["value"]?.ToString() is { } fixedValue)
            {
                ReadValue(claim, rule, place, Transform(Naming.Fill(fixedValue, values), recipe, readCase), place.At);
                return true;
            }

            return false;
        }

        if (In(place, readCase["in"]?.GetValue<string>()) is not { } target)
        {
            return false;
        }

        var text = Part(target, readCase["part"]?.GetValue<string>());
        var match = Compile(Naming.Fill(pattern, values, Regex.Escape)).Match(text.Text);
        if (!match.Success)
        {
            return false;
        }

        var at = text.At(match.Index);
        if (readCase["unreadable"]?.GetValue<bool>() == true)
        {
            Cant(claim, "unreadable", new Reading { At = at, Detail = $"The code states it at {at} in a form the recipe can't read: `{match.Value}`." });
            return true;
        }

        var value = readCase["value"]?.ToString() is { } template
            ? match.Result(Naming.Fill(template, values, x => x.Replace("$", "$$")))
            : match.Groups.Count > 1 ? match.Groups[1].Value : match.Value;
        ReadValue(claim, rule, target, Transform(value, recipe, readCase), at);
        return true;
    }

    /// <summary>The inherit targets that name code around the find's (Common.Drift's); any other is a find path of its own.</summary>
    private static readonly HashSet<string> InheritKinds = new(StringComparer.Ordinal) { "class", "type", "base", "method", "property", "statement", "file" };

    /// <summary>
    /// What an inherit target gives the read cases, a level at a time, nearest first: the type, method or property around the
    /// code the find found (never that code itself), its file, its in-scope base types one at a time, or what a find path of
    /// its own finds. <c>statement</c> gives nothing: the graph has no statement nodes.
    /// </summary>
    private List<IReadOnlyList<RecipeHit>> Inherited(Claim claim, string target, RecipeHit hit, IReadOnlyDictionary<string, string> values, IReadOnlySet<string> files)
    {
        List<IReadOnlyList<RecipeHit>> levels = [];
        switch (target)
        {
            case "class" or "type" when hit.Node is not TypeNode && hit.Type is { } type:
                levels.Add([hit with { Node = type, Member = null, At = type.At }]);
                break;
            case "method" when hit.Node is not MemberNode && hit.Member is { Kind: "method" or "constructor" } method:
                levels.Add([hit with { Node = method, At = method.At }]);
                break;
            case "property" when hit.Node is not MemberNode && hit.Member is { Kind: "property" or "field" or "event" } property:
                levels.Add([hit with { Node = property, At = property.At }]);
                break;
            case "file" when hit.Node is not null:
                levels.Add([new RecipeHit(hit.At.File, null, null, null, new Span(hit.At.File, 1))]);
                break;
            case "base" when (hit.Node as TypeNode ?? hit.Type) is { } derived:
                levels.AddRange(_graph.Bases(derived).Where(x => x.Files.Any(files.Contains)).Select(x => (IReadOnlyList<RecipeHit>)[new RecipeHit(x.At.File, x, x, null, x.At)]));
                break;
            case not null when !InheritKinds.Contains(target):
            {
                RecipeHit? near = null;
                var found = Find(claim, Steps(target), values, files, ref near);
                if (found.Count > 0)
                {
                    levels.Add(found.OrderBy(x => x.At.File, StringComparer.Ordinal).ThenBy(x => x.At.Line).ToList());
                }

                break;
            }
        }

        return levels;
    }

    // ---- Node read cases ----

    /// <summary>The fields a <c>node</c> read case may name: code-reader.md's node fields the graph has. Checked when the pack loads.</summary>
    private static readonly HashSet<string> NodeFields = new(StringComparer.Ordinal)
    {
        "name", "file", "kind", "text", "declaration", "body", "raw", "language", "typeKind", "namespace", "qualifiedName", "modifiers",
        "typeParameters", "supertypes", "annotations", "type", "returnType", "value", "parameters", "default", "optional", "arguments",
        "literal", "items", "callee", "receiver", "typeArguments", "types", "tag", "component", "content", "attributes", "children", "nullable"
    };

    /// <summary>A <c>node</c> read case's field path, <c>arguments[0].literal</c>: each field, with the index of a list's item it picks.</summary>
    private static IReadOnlyList<(string Field, int? Index)> NodePath(string path) => path.Split('.').Select(segment =>
    {
        var parts = Regex.Match(segment, @"^(\w+)(?:\[(\d+)\])?$", RegexOptions.CultureInvariant);
        if (!parts.Success)
        {
            throw new ArgumentException($"its node read case `{path}` isn't a field path such as `name` or `arguments[0].literal`.");
        }

        return (parts.Groups[1].Value, parts.Groups[2].Success ? int.Parse(parts.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture) : (int?)null);
    }).ToList();

    /// <summary>
    /// The value a field path reads at a place: text, or a type ref as it is. Has is false when a field isn't one the node
    /// reached has; a field that has nothing there (a parameter with no default, an item past a list's end) is null.
    /// </summary>
    private (bool Has, object? Value) NodeValue(RecipeHit place, string path)
    {
        object? current = place;
        foreach (var (name, index) in NodePath(path))
        {
            if (current is null)
            {
                return (true, null);
            }

            var (has, value) = FieldOf(current, name);
            if (!has)
            {
                return (false, null);
            }

            if (index is { } i)
            {
                if (value is string or not System.Collections.IEnumerable)
                {
                    return value is null ? (true, null) : (false, null);
                }

                value = ((System.Collections.IEnumerable)value).Cast<object?>().ElementAtOrDefault(i);
            }

            current = value;
        }

        return (true, current switch
        {
            null or string or TypeRefNode => current,
            LiteralNode literal => literal.Value,
            ValueNode value => value.Literal?.Value ?? value.Text,
            AttributeValueNode argument => argument.Literal ?? argument.Text,
            MarkupAttributeNode attribute => attribute.Literal?.Value ?? attribute.Value,
            // A list is its items' text, joined as a set's or list's value is; an empty one states nothing.
            System.Collections.IEnumerable list => list.Cast<object?>().Select(ItemText).OfType<string>().ToList() is { Count: > 0 } items ? string.Join(", ", items) : null,
            _ => ItemText(current)
        });

        static string? ItemText(object? item) => item switch
        {
            null => null,
            string text => text,
            TypeRefNode type => type.Text,
            ValueNode value => value.Text,
            AttributeValueNode argument => argument.Text,
            AttributeNode annotation => annotation.Name,
            ParameterNode parameter => parameter.Name,
            MemberNode member => member.Name,
            TypeNode type => type.Name,
            MarkupNode element => element.Tag,
            MarkupAttributeNode attribute => attribute.Name,
            LiteralNode literal => literal.Value,
            _ => item.ToString()
        };
    }

    /// <summary>A node's field (code-reader.md, The code graph), from the place a find reached down to a type ref's arguments.</summary>
    private (bool Has, object? Value) FieldOf(object node, string field) => (node, field) switch
    {
        // The place itself: the text the recipe reads there, its file, and a whole file's own fields.
        (RecipeHit hit, "file") => (true, hit.Node is null ? hit.File : hit.At.File),
        (RecipeHit hit, "text") => (true, TextOf(hit).Text),
        (RecipeHit { Node: not null } hit, "declaration" or "body") => (true, Part(hit, field).Text),
        (RecipeHit { Node: null } hit, "name") => (true, Path.GetFileName(hit.File)),
        (RecipeHit { Node: null } hit, "raw") => (true, SourceOf(hit.File)?.Text),
        (RecipeHit { Node: null } hit, "language") => (true, _graph.Files.FirstOrDefault(x => x.Path == hit.File)?.Language),
        (RecipeHit { Node: null }, "kind") => (true, "file"),
        (RecipeHit { Node: { } inner }, _) => FieldOf(inner, field),
        (TypeNode type, "name") => (true, type.Name),
        (TypeNode, "kind") => (true, "type"),
        (TypeNode type, "typeKind") => (true, type.Kind),
        (TypeNode type, "namespace") => (true, type.Namespace),
        (TypeNode type, "qualifiedName") => (true, type.Namespace.Length > 0 ? $"{type.Namespace}.{type.Name}" : type.Name),
        (TypeNode type, "modifiers") => (true, Modifiers(type.Access, type.IsStatic, type.IsAbstract ? ["abstract"] : [])),
        (TypeNode type, "typeParameters") => (true, type.TypeParameters),
        (TypeNode type, "supertypes") => (true, type.BaseTypes),
        (TypeNode type, "annotations") => (true, type.Attributes),
        (MemberNode member, "name") => (true, member.Name),
        (MemberNode member, "kind") => (true, member.Kind),
        (MemberNode member, "type" or "returnType") => (true, member.Type),
        (MemberNode member, "value") => (true, member.Value),
        (MemberNode member, "modifiers") => (true, Modifiers(member.Access, member.IsStatic, member.Modifiers)),
        (MemberNode member, "parameters") => (true, member.Parameters),
        (MemberNode member, "typeParameters") => (true, member.TypeParameters),
        (MemberNode member, "annotations") => (true, member.Attributes),
        (ParameterNode parameter, "name") => (true, parameter.Name),
        (ParameterNode, "kind") => (true, "parameter"),
        (ParameterNode parameter, "type") => (true, parameter.Type),
        (ParameterNode parameter, "default") => (true, parameter.Default),
        (ParameterNode parameter, "optional") => (true, parameter.IsOptional ? "true" : "false"),
        (ParameterNode parameter, "annotations") => (true, parameter.Attributes),
        (AttributeNode annotation, "name") => (true, annotation.Name),
        (AttributeNode, "kind") => (true, "annotation"),
        (AttributeNode annotation, "arguments") => (true, annotation.Values),
        (AttributeValueNode argument, "name") => (true, argument.Name),
        (AttributeValueNode argument, "text") => (true, argument.Text),
        (AttributeValueNode argument, "literal") => (true, argument.Literal),
        (AttributeValueNode argument, "items") => (true, argument.Items),
        (CallNode call, "name" or "callee") => (true, call.Name),
        (CallNode, "kind") => (true, "call"),
        (CallNode call, "receiver") => (true, call.Receiver),
        (CallNode call, "typeArguments") => (true, call.TypeArguments),
        (CallNode call, "arguments") => (true, call.Arguments),
        (ValueNode value, "text") => (true, value.Text),
        (ValueNode value, "literal") => (true, value.Literal),
        (ValueNode value, "types") => (true, value.Types),
        (LiteralNode literal, "value") => (true, literal.Value),
        (LiteralNode literal, "kind") => (true, literal.Kind),
        (LiteralNode literal, "type") => (true, literal.Type),
        (MarkupNode element, "tag" or "name") => (true, element.Tag),
        (MarkupNode, "kind") => (true, "markup"),
        (MarkupNode element, "component") => (true, element.Component),
        (MarkupNode element, "content") => (true, element.Content),
        (MarkupNode element, "attributes") => (true, element.Attributes),
        (MarkupNode element, "children") => (true, element.Children),
        (MarkupAttributeNode attribute, "name") => (true, attribute.Name),
        (MarkupAttributeNode attribute, "value") => (true, attribute.Value),
        (MarkupAttributeNode attribute, "literal") => (true, attribute.Literal),
        (TypeRefNode type, "text") => (true, type.Text),
        (TypeRefNode type, "name") => (true, type.Name),
        (TypeRefNode type, "qualifiedName") => (true, type.QualifiedName),
        (TypeRefNode type, "arguments") => (true, type.Arguments),
        (TypeRefNode type, "nullable") => (true, type.Nullable),
        _ => (false, null)
    };

    private static IReadOnlyList<string> Modifiers(string access, bool isStatic, IEnumerable<string> others) =>
        new[] { access }.Concat(isStatic ? ["static"] : []).Concat(others).ToList();

    /// <summary>A presence kind reads <c>true</c> as <c>present</c> and <c>false</c> as <c>absent</c>, as a <c>bool</c> kind reads them.</summary>
    private static string Presence(Claim claim, string value) => claim.Definition.ValueType != "presence" ? value : value switch
    {
        "true" => "present",
        "false" => "absent",
        _ => value
    };

    /// <summary>The find found nothing (claims-and-findings.md, Outcomes, steps 6, 9 to 12).</summary>
    private void NothingFound(Claim claim, RecipeRule rule, IReadOnlyDictionary<string, string> values, IReadOnlyList<string> paths, string searched, RecipeHit? near)
    {
        // What the find names may be in a file a scope row leaves out (step 6): a type, by its file's name, or a file glob.
        var steps = paths.SelectMany(Steps).ToList();
        var stems = steps.Where(x => x is { Kind: "type", Name: not null }).Select(x => Naming.Fill(x.Name!, values)).Distinct().ToList();
        var globs = steps.Where(x => x is { Kind: "file", Name: not null }).Select(x => new GlobPattern(Naming.Fill(x.Name!, values))).ToList();
        if (_files.FirstOrDefault(x => !x.Decision.In && x.Decision.Builtin is null && (stems.Any(stem => IsNamedFor(x.Path, stem)) || globs.Any(g => g.IsMatch(x.Path)))) is { Path: not null } shadow)
        {
            Cant(claim, "outOfScope", new Reading { ShadowFile = shadow.Path, ScopeRow = shadow.Decision.Row?.Id });
            return;
        }

        if (claim.Definition.Existence)
        {
            Missing(claim, stems, $"what `{searched}` finds");
            return;
        }

        var at = near?.At ?? LocatedType(claim)?.At;
        if (claim.Stated == "whenSet")
        {
            claim.Outcome = "contradicted";
            claim.Reading = new Reading { At = at };
        }
        else if (rule.Data["absent"]?.ToString() is { } absent)
        {
            Apply(claim, ClaimReading.Absent(at ?? new Span("", 1), Presence(claim, Naming.Fill(absent, values).Trim())));
        }
        else
        {
            Cant(claim, "notFound", new Reading { At = at, Detail = $"No code found: `{searched}` finds nothing in scope.", Searched = [searched] });
        }
    }

    /// <summary>
    /// A value read from the code. A <c>type</c> kind's text is the type the code writes there, as its reader resolved it, or
    /// the type ref a <c>node</c> read case gave as it is; an existence kind's code is what the claims waiting on it read.
    /// </summary>
    private void ReadValue(Claim claim, RecipeRule rule, RecipeHit target, string value, Span at, TypeRefNode? typed = null)
    {
        value = Presence(claim, value);
        if (claim.Definition.ValueType == "type")
        {
            var written = Regex.Replace(value, @"\s+", "");
            if ((typed is not null && Regex.Replace(typed.Text, @"\s+", "") == written ? typed : TypeRefs(target.Node).FirstOrDefault(x => Regex.Replace(x.Text, @"\s+", "") == written)) is not { } type)
            {
                Cant(claim, "unresolved", new Reading { At = at, Detail = $"The recipe reads `{value}` at {at}, but no type the code writes there reads so, so it can't be resolved." });
                return;
            }

            Apply(claim, ClaimReading.Compared(type.Text, at, claim.ExpectedValue is ITypeReference model && TypeMatches(model, type)));
            return;
        }

        // A value that doesn't fit the kind is the recipe's fault, not the code's (rules.md, Plug-ins).
        if (claim.Definition.ValueType switch
            {
                "bool" => value is not ("true" or "false"),
                "number" => !decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _),
                "presence" => value is not ("present" or "absent"),
                _ => false
            })
        {
            Cant(claim, "ruleFailed", new Reading
            {
                At = at,
                Detail = $"Pack `{rule.Pack.Id}`'s recipe read `{value}` at {at}, which isn't a {claim.Definition.ValueType} value. Give the read case a `value`."
            });
            return;
        }

        if (claim.Definition.Existence && Same(claim, value))
        {
            Locate(claim, target, at);
        }

        Apply(claim, rule.Data["compare"]?.GetValue<string>() is { } name && _comparers.TryGetValue(name, out var comparer)
            ? ClaimReading.Compared(value, at, comparer.Same(claim.Expected, value))
            : ClaimReading.Stated(value, at));
    }

    /// <summary>
    /// What an existence claim a recipe verified found, for the claims that wait on it: the type or member the find reached, or
    /// the one declared where the read case matched, as when a file is read as text.
    /// </summary>
    private void Locate(Claim claim, RecipeHit hit, Span at)
    {
        switch (claim.Kind)
        {
            case "type.exists" when (hit.Node as TypeNode ?? hit.Type ?? TypeAt(at)) is { } type:
                _types[claim] = type;
                return;
            case "member.exists" when (hit.Node as MemberNode ?? hit.Member ?? MemberAt(at)) is { } member:
                _members[claim] = member;
                return;
            case "type.exists" or "member.exists":
                _textOnly.Add(claim);
                return;
        }
    }

    private TypeNode? TypeAt(Span at) => _graph.Types.Where(x => x.At.File == at.File && Inside(x.At, at)).MaxBy(x => x.At.Line);

    private MemberNode? MemberAt(Span at) => _graph.Types.Where(x => x.Files.Contains(at.File)).SelectMany(x => x.Members).Where(x => Inside(x.At, at)).MaxBy(x => x.At.Line);

    /// <summary>
    /// A claim's placeholders (vocabulary.md, Placeholders): its bindings; then, for what no binding names, the element and
    /// its owners by their element types (<c>{Operation}</c>, <c>{Service}</c>), what a link's end points to, and the
    /// language-neutral <c>{Type}</c>, <c>{Folder}</c> and <c>{FolderPath}</c>.
    /// </summary>
    private static Dictionary<string, string> Placeholders(Claim claim)
    {
        var values = new Dictionary<string, string>(claim.Bindings ?? new Dictionary<string, string>(), StringComparer.Ordinal);
        for (var element = claim.Source; element is not null; element = Owner(element))
        {
            values.TryAdd(Key(element.SpecializationType), NameOf(element));
        }

        if (claim.Source is IAssociationEnd { TypeReference.Element: IElement target })
        {
            values.TryAdd(Key(target.SpecializationType), target.Name);
        }

        var folders = Folders(claim.Source);
        values.TryAdd("Type", NameOf(claim.Source));
        values.TryAdd("FolderPath", string.Join("/", folders));
        values.TryAdd("Folder", folders.LastOrDefault() ?? "");
        return values;

        static string Key(string specialization) => specialization.Replace(" ", "").Replace("-", "");
    }

    // ---- Finding ----

    private sealed record RecipeStep(string Kind, string? Name, string? Pattern);

    /// <summary>A find path's steps: <c>kind</c>, <c>kind:name</c> (a glob for <c>file</c>) or <c>kind with regex</c>.</summary>
    private static IReadOnlyList<RecipeStep> Steps(string path) => path.Split(" > ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(step =>
    {
        var with = step.IndexOf(" with ", StringComparison.Ordinal);
        var colon = step.IndexOf(':');
        (string Kind, string? Name, string? Pattern) parts = with >= 0 && (colon < 0 || with < colon)
            ? (step[..with].Trim(), null, step[(with + " with ".Length)..].Trim())
            : colon >= 0
                ? (step[..colon].Trim(), step[(colon + 1)..].Trim(), null)
                : (step, null, null);
        var (kind, name, pattern) = parts;
        if (!StepKinds.Contains(kind))
        {
            throw new ArgumentException($"its find has a step this engine doesn't read yet: '{step}'.");
        }

        return new RecipeStep(kind == "class" ? "type" : kind, name, pattern);
    }).ToList();

    private List<RecipeHit> Find(Claim claim, IReadOnlyList<RecipeStep> steps, IReadOnlyDictionary<string, string> values, IReadOnlySet<string> files, ref RecipeHit? near)
    {
        List<RecipeHit> hits = [];
        for (var i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            var name = step.Name is null ? null : Naming.Fill(step.Name, values);
            var regex = step.Pattern is null ? null : Compile(Naming.Fill(step.Pattern, values, Regex.Escape));
            var candidates = i == 0 ? First(claim, step.Kind, name, files) : hits.SelectMany(x => Within(x, step.Kind, name));
            hits = candidates
                .Where(x => regex is null || regex.IsMatch(TextOf(x).Text))
                .DistinctBy(x => x.Node ?? x.File)
                .ToList();
            // A chain's text holds the chains in its arguments, so only the innermost that matches is the place.
            if (step.Kind == "chain" && (name is not null || regex is not null))
            {
                hits = FindPath.Innermost(hits, x => x.Node!, x => x.At);
            }

            if (hits.Count == 0)
            {
                break;
            }

            if (i < steps.Count - 1 && hits.Count == 1)
            {
                near = hits[0];
            }
        }

        return hits;
    }

    /// <summary>A path's first step, over the files the recipe reads.</summary>
    private IEnumerable<RecipeHit> First(Claim claim, string kind, string? name, IReadOnlySet<string> files)
    {
        if (kind == "file")
        {
            var glob = name is null ? null : new GlobPattern(name);
            return files.Order(StringComparer.Ordinal).Where(x => glob is null || glob.IsMatch(x)).Select(x => new RecipeHit(x, null, null, null, new Span(x, 1)));
        }

        var types = _graph.Types.Where(x => x.Files.Any(files.Contains)).ToList();
        if (kind == "type")
        {
            var named = name is null ? types : LocatedTypes(claim, types.Where(x => x.Name == name).ToList());
            return named.Select(x => new RecipeHit(x.At.File, x, x, null, x.At));
        }

        var inTypes = types.SelectMany(x => Within(new RecipeHit(x.At.File, x, x, null, x.At), kind, name));
        return kind is "call" or "chain" ? inTypes.Concat(files.Order(StringComparer.Ordinal).SelectMany(x => TopLevel(x, kind, name))) : inTypes;
    }

    /// <summary>The calls or call chains a file's top-level statements make (Program.cs's registrations), which belong to no type.</summary>
    private IEnumerable<RecipeHit> TopLevel(string file, string kind, string? name) =>
        _graph.Files.Where(x => x.Path == file && x.TopLevel is not null)
            .SelectMany(x => Calls(x.TopLevel!.Calls, kind, name))
            .Select(x => new RecipeHit(file, x, null, null, x.At));

    /// <summary>
    /// A unit's calls of a name, or its call chains (<see cref="FindPath.Chains"/>) whose canonical text holds a step's text,
    /// as Common.Drift's <c>chain:Property(e =&gt; e.Name)</c> anchors a chain by text it contains.
    /// </summary>
    private static IEnumerable<CallNode> Calls(IReadOnlyList<CallNode> calls, string kind, string? name) => kind == "chain"
        ? FindPath.Chains(calls).Where(x => name is null || x.Canonical.Contains(CanonicalText.OfText(name), StringComparison.Ordinal))
        : calls.Where(x => name is null || x.Name == name);

    /// <summary>Of several types of a name, the one a claim on this element or its owners located.</summary>
    private List<TypeNode> LocatedTypes(Claim claim, List<TypeNode> candidates)
    {
        if (candidates.Count > 1)
        {
            for (var current = Dependency(claim); current is not null; current = Dependency(current))
            {
                if (_types.TryGetValue(current, out var located) && candidates.Contains(located))
                {
                    return [located];
                }
            }
        }

        return candidates;
    }

    /// <summary>What a step reaches inside a place: a file's types and their members, a type's members, a member's parameters, and so on.</summary>
    private IEnumerable<RecipeHit> Within(RecipeHit context, string kind, string? name)
    {
        switch (context.Node)
        {
            case null:
            {
                var types = _graph.Types.Where(x => x.Files.Contains(context.File)).ToList();
                if (kind == "type")
                {
                    return types.Where(x => name is null || x.Name == name).Select(x => new RecipeHit(context.File, x, x, null, x.At));
                }

                if (kind == "markup")
                {
                    return _graph.Files.Where(x => x.Path == context.File).SelectMany(x => x.Markup).SelectMany(x => x.DescendantsAndSelf())
                        .Where(x => name is null || x.Tag == name).Select(x => new RecipeHit(context.File, x, null, null, x.At));
                }

                // A part of a partial type in another file isn't this file's.
                var inTypes = types.SelectMany(x => Within(new RecipeHit(context.File, x, x, null, x.At), kind, name)).Where(x => x.At.File == context.File);
                return kind is "call" or "chain" ? inTypes.Concat(TopLevel(context.File, kind, name)) : inTypes;
            }
            case TypeNode type:
                return kind switch
                {
                    "annotation" => Annotations(context, type.Attributes, name),
                    "markup" => _graph.MarkupOf(type).Where(x => name is null || x.Tag == name).Select(x => context with { Node = x, At = x.At }),
                    "parameter" or "call" or "chain" => type.Members.SelectMany(x => Within(context with { Node = x, Member = x, At = x.At }, kind, name)),
                    "type" or "file" => [],
                    _ => Members(type, kind, name).Select(x => context with { Node = x, Member = x, At = x.At })
                };
            case MemberNode member:
                return kind switch
                {
                    "parameter" => member.Parameters.Where(x => name is null || x.Name == name).Select(x => context with { Node = x, At = x.At }),
                    "annotation" => Annotations(context, member.Attributes, name),
                    "call" or "chain" => Calls(member.Calls, kind, name).Select(x => context with { Node = x, At = x.At }),
                    _ => []
                };
            case ParameterNode parameter:
                return kind == "annotation" ? Annotations(context, parameter.Attributes, name) : [];
            case MarkupNode element:
                return kind == "markup" ? element.DescendantsAndSelf().Skip(1).Where(x => name is null || x.Tag == name).Select(x => context with { Node = x, At = x.At }) : [];
            default:
                return [];
        }
    }

    private static IEnumerable<RecipeHit> Annotations(RecipeHit context, IEnumerable<AttributeNode> attributes, string? name) =>
        attributes.Where(x => name is null || x.Name == AttributeName(name)).Select(x => context with { Node = x, At = x.At });

    private static string AttributeName(string name) => name.EndsWith("Attribute", StringComparison.Ordinal) ? name[..^"Attribute".Length] : name;

    /// <summary>
    /// A type's members of a step's kind, as Common.Drift's are: <c>method</c> is a method or constructor, and
    /// <c>property</c> a property or field. <c>method:Name(int, string)</c> picks an overload by its parameters' types.
    /// </summary>
    private static IEnumerable<MemberNode> Members(TypeNode type, string kind, string? name)
    {
        var signature = name is not null && name.IndexOf('(') is var open and > 0 && name.EndsWith(')')
            ? name[(open + 1)..^1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            : null;
        var bare = signature is null || name is null ? name : name[..name.IndexOf('(')];
        return type.Members.Where(x => kind switch
            {
                "method" => x.Kind is "method" or "constructor",
                "property" => x.Kind is "property" or "field" or "event",
                "member" => true,
                _ => x.Kind == kind
            })
            .Where(x => bare is null || x.Name == bare)
            .Where(x => signature is null || x.Parameters.Select(p => p.Type?.Name ?? "").SequenceEqual(signature.Select(s => s.Split('<')[0].Split('.')[^1]), StringComparer.Ordinal));
    }

    /// <summary>A read case's <c>in</c>: the code around the place found, of a kind, or the place itself.</summary>
    private static RecipeHit? In(RecipeHit hit, string? kind) => kind switch
    {
        null => hit,
        "file" => new RecipeHit(hit.File, null, null, null, new Span(hit.File, 1)),
        "class" or "type" => hit.Type is { } type ? hit with { Node = type, Member = null, At = type.At } : null,
        "method" or "property" => hit.Member is { } member ? hit with { Node = member, At = member.At } : null,
        "parameter" => hit.Node is ParameterNode ? hit : null,
        _ => null
    };

    // ---- Text ----

    /// <summary>
    /// The text a recipe reads at a place: a node's canonical text as its language gave it (code-reader.md, Canonical text),
    /// or, for a node its language gives none and for a whole file, its source read as text.
    /// </summary>
    private SourceText TextOf(RecipeHit hit)
    {
        var canonical = hit.Node switch
        {
            TypeNode type => type.Text,
            MemberNode member => member.Text,
            ParameterNode parameter => parameter.Text,
            MarkupNode element => element.Text,
            CallNode call => call.Canonical,
            AttributeNode attribute => attribute.Arguments.Count == 0 ? attribute.Name : $"{attribute.Name}({string.Join(",", attribute.Arguments)})",
            _ => ""
        };
        // A partial type's member may be in another file than the part the find started at.
        var file = hit.Node is null ? hit.File : hit.At.File;
        if (SourceOf(file) is not { } read)
        {
            return SourceText.Aligned(file, canonical, []);
        }

        var (source, lines) = read;
        if (hit.Node is null)
        {
            return SourceText.OfText(file, SourceText.Characters(source, lines));
        }

        var characters = SourceText.Characters(source, lines, hit.At);
        return canonical.Length > 0 ? SourceText.Aligned(file, canonical, characters) : SourceText.OfText(file, characters);
    }

    /// <summary>
    /// A read case's <c>part</c>: a node's <c>declaration</c> (attributes, modifiers, signature and bases) or its
    /// <c>body</c> (a type's members, a method's block or expression, a property's accessors). A member's body starts where
    /// its language says; otherwise at the first <c>{</c>, or a member's <c>=&gt;</c>, outside brackets.
    /// </summary>
    private SourceText Part(RecipeHit hit, string? part)
    {
        var text = TextOf(hit);
        if (part is null || hit.Node is null)
        {
            return text;
        }

        var start = hit.Node is MemberNode { Body.At: { } body } && body.Line >= hit.At.Line
            ? text.IndexAt(body.Line, body.Column ?? 1)
            : BodyStart(text.Text, hit.Node is MemberNode);
        return part == "declaration" ? text.Slice(0, start) : text.Slice(start, text.Text.Length);
    }

    private static int BodyStart(string text, bool arrow)
    {
        var depth = 0;
        char? quote = null;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (quote is not null)
            {
                if (c == '\\')
                {
                    i++;
                }
                else if (c == quote)
                {
                    quote = null;
                }

                continue;
            }

            switch (c)
            {
                case '"' or '\'':
                    quote = c;
                    break;
                case '(' or '[':
                    depth++;
                    break;
                case ')' or ']':
                    depth--;
                    break;
                case '{' when depth == 0:
                    return i;
                case '=' when arrow && depth == 0 && i + 1 < text.Length && text[i + 1] == '>':
                    return i;
            }
        }

        return text.Length;
    }

    /// <summary>An in-scope file's source and where its lines start, for a recipe to read as text; null for one that can't be read.</summary>
    private (string Text, IReadOnlyList<int> Lines)? SourceOf(string file)
    {
        if (!_sources.TryGetValue(file, out var source))
        {
            try
            {
                var text = File.ReadAllText(Path.Combine(_input.OutputFolder, file));
                source = (text, SourceText.LineStarts(text));
            }
            catch (IOException)
            {
                source = null;
            }

            _sources[file] = source;
        }

        return source;
    }

    /// <summary>The types the code writes at a place, outermost first and each with what it is written of.</summary>
    private static IEnumerable<TypeRefNode> TypeRefs(object? node)
    {
        var roots = node switch
        {
            MemberNode member => new[] { member.Type }.Concat(member.Parameters.Select(x => x.Type)),
            ParameterNode parameter => [parameter.Type],
            TypeNode type => type.BaseTypes,
            CallNode call => call.TypeArguments,
            _ => []
        };
        return roots.OfType<TypeRefNode>().SelectMany(Nested);

        static IEnumerable<TypeRefNode> Nested(TypeRefNode type) => type.Arguments.Concat(type.Union).SelectMany(Nested).Prepend(type);
    }

    /// <summary>A value read, through the recipe's <c>replace</c>, <c>map</c> and <c>case</c>, and the read case's own <c>case</c>.</summary>
    private static string Transform(string value, JsonObject recipe, JsonObject readCase)
    {
        foreach (var replacement in (recipe["replace"] as JsonArray ?? []).OfType<JsonObject>())
        {
            if (replacement["match"]?.GetValue<string>() is { } match && replacement["with"]?.ToString() is { } with)
            {
                value = Compile(match).Replace(value, with);
            }
        }

        if (recipe["map"] is JsonObject map && map[value.Trim()]?.ToString() is { } mapped)
        {
            value = mapped;
        }

        value = (readCase["case"] ?? recipe["case"])?.GetValue<string>() switch
        {
            "upper" => value.ToUpperInvariant(),
            "lower" => value.ToLowerInvariant(),
            "pascal" => value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..],
            "camel" => value.Length == 0 ? value : char.ToLowerInvariant(value[0]) + value[1..],
            _ => value
        };
        return value.Trim();
    }

    private static Regex Compile(string pattern) => new(pattern, RegexOptions.CultureInvariant, MatchTimeout);

    private static IReadOnlyList<string> Strings(JsonNode? node) => node switch
    {
        JsonArray list => list.Select(x => x!.GetValue<string>()).ToList(),
        JsonValue value => [value.GetValue<string>()],
        _ => []
    };

    private static IReadOnlyList<GlobPattern> Globs(JsonNode? files) => Strings(files).Select(x => new GlobPattern(x)).ToList();
}
