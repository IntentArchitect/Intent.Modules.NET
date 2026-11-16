using System.Text.Json.Nodes;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.OptIn;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Scope;
using Intent.Modules.Common.DriftEngine.Vocabulary;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>What the claims located: code the model describes (rules.md, Opt-in rules).</summary>
internal sealed record LocatedCode(HashSet<TypeNode> Types, HashSet<MemberNode> Members, HashSet<string> Files, ILookup<string, Span> Readings);

/// <summary>
/// Policy (block 5): the opt-in "must be modelled" rules, the exceptions to them and severity overrides, all from the Root
/// Folder's <c>Drift Check</c> stereotype. One place does it for every rule, so a module's rule finds code and nothing else.
/// </summary>
public sealed partial class DriftEngine
{
    /// <summary>The engine's own opt-in rules; a module's rule of the same id replaces one.</summary>
    private static readonly IOptInRule[] StockOptInRules =
        [new UnmodelledFile(), new UnmodelledType(), new UnmodelledMember(), new UnmodelledInteraction(), new UnmodelledExposed(), new UnmodelledValue()];

    private CheckModel _check = null!;
    private LocatedCode? _locatedCode;

    internal LocatedCode LocatedNodes => _locatedCode ??= LocateCode();

    /// <summary>
    /// Code is located when some claim's reading came from it or from code inside it, as Common.Drift's ownership is: an
    /// element's type or member, and any type or member a reading's place falls in, with its file.
    /// </summary>
    private LocatedCode LocateCode()
    {
        var types = new HashSet<TypeNode>(_types.Values);
        var members = new HashSet<MemberNode>(_members.Values);
        foreach (var node in _located.Values)
        {
            switch (node)
            {
                case TypeNode type:
                    types.Add(type);
                    break;
                case MemberNode member:
                    members.Add(member);
                    break;
                // A claim that locates several members at once: an enum's literals.
                case IEnumerable<MemberNode> several:
                    members.UnionWith(several);
                    break;
            }
        }

        var spans = _claims.Select(x => x.Reading?.At).OfType<Span>().Where(x => x.File.Length > 0).ToList();
        var typesByFile = _graph.Types.SelectMany(t => t.Files.Distinct().Select(f => (File: f, Type: t))).ToLookup(x => x.File, x => x.Type);
        foreach (var span in spans)
        {
            foreach (var type in typesByFile[span.File])
            {
                if (Inside(type.At, span))
                {
                    types.Add(type);
                }

                members.UnionWith(type.Members.Where(x => Inside(x.At, span)));
            }
        }

        types.UnionWith(_graph.Types.Where(x => x.Members.Any(members.Contains)));
        var files = new HashSet<string>(spans.Select(x => x.File), StringComparer.Ordinal);
        files.UnionWith(types.SelectMany(x => x.Files));
        files.UnionWith(members.Select(x => x.At.File));
        return new LocatedCode(types, members, files, spans.ToLookup(x => x.File, StringComparer.Ordinal));
    }

    internal static bool Inside(Span outer, Span inner) =>
        outer.File == inner.File && inner.Line >= outer.Line && inner.Line <= (outer.EndLine ?? outer.Line);

    internal (string Designer, IElement Element)? ElementOf(TypeNode type) =>
        _types.FirstOrDefault(x => x.Value == type).Key is { } claim ? (claim.Designer, claim.Source) : null;

    /// <summary>The types elements' <c>type.exists</c> claims located, each with its claim.</summary>
    internal IEnumerable<(Claim Claim, TypeNode Type)> LocatedTypes() =>
        _types.Where(x => x.Key.Kind == "type.exists").Select(x => (x.Key, x.Value));

    /// <summary>
    /// The kinds of member an element's type can state (its conventions' <c>states</c>): the last pack's that says, so a house
    /// pack's convention for the element type that says nothing of them keeps the stock pack's.
    /// </summary>
    internal IReadOnlySet<string> States(string designer, IElement element) =>
        (_packs.Select(x => (x.TypeConvention(designer, element.SpecializationType) as JsonObject)?["states"]).LastOrDefault(x => x is not null) as JsonArray)?
        .Select(x => x!.GetValue<string>()).ToHashSet(StringComparer.Ordinal) ?? [];

    /// <summary>Every pack's type conventions: what <c>unmodelled.type</c> looks for.</summary>
    internal IEnumerable<(string Designer, string ElementType, JsonNode Convention)> TypeConventions() => _packs.SelectMany(x => x.TypeConventions());

    /// <summary>Every pack's conventions of one list: <c>members</c>, <c>interactions</c> or <c>exposed</c>.</summary>
    internal IEnumerable<(string Designer, JsonNode Convention)> CodeConventions(string list) => _packs.SelectMany(x => x.CodeConventions(list));

    /// <summary>
    /// <c>unmodelled.value</c>'s reading (rules.md, Opt-in rules): for each element a <c>whenSet</c> kind's vocabulary entry
    /// selects, ignoring its <c>where</c> and <c>when</c>, that has no claim of the kind, a probe claim with no expected value
    /// is read by the kind's rule as a claim would be. A value the code states is reported; silence, a default and anything
    /// that can't be verified aren't. Probes are never claims of the run, and locate nothing.
    /// </summary>
    internal IReadOnlyList<UnmodelledCode> UnmodelledValues()
    {
        var found = new List<UnmodelledCode>();
        var failures = _extensionFailures.Count;
        var probed = new HashSet<(string, string, string)>();
        foreach (var designer in _input.Designers.Keys)
        {
            var elements = Elements(designer);
            foreach (var pack in _packs)
            {
                foreach (var entry in pack.Vocabulary(designer))
                {
                    if (entry["kind"]?.GetValue<string>() is not { } kind || !_kinds.TryGetValue(kind, out var definition) || definition.Stated != "whenSet" || entry["each"] is not null)
                    {
                        continue;
                    }

                    var specialization = entry["for"]?["element"]?.GetValue<string>();
                    var association = entry["for"]?["associationEnd"]?.GetValue<string>();
                    var whole = entry["for"]?["association"]?.GetValue<string>();
                    // `where` tests that the value is set, which a probe is for; `applies` says where the kind applies at all.
                    var applies = entry["for"]?["applies"]?.GetValue<string>();
                    var selected = whole is not null ? Associations(elements, whole)
                        : association is not null ? NavigableEnds(elements, association)
                        : specialization is not null ? elements.Where(x => x.SpecializationType == specialization) : [];
                    foreach (var element in selected.Where(x => applies is null || Expression.Evaluate(applies, x) is true))
                    {
                        if (ClaimOn(designer, element, kind) is not null || !probed.Add((designer, element.Id, kind)) || Probe(designer, pack, entry, element, definition) is not { } probe)
                        {
                            continue;
                        }

                        Report(probe, element, definition);
                    }
                }
            }
        }

        // What claim sources would claim if the model set it, such as a relationship's foreign key with no Foreign Key
        // attributes, probed the same way; a source states it with its value left unset.
        foreach (var (extension, spec) in _unsetSpecs)
        {
            var definition = _kinds[spec.Kind];
            if (ClaimOn(spec.Designer, spec.Element, spec.Kind) is not null || !probed.Add((spec.Designer, spec.Element.Id + spec.Discriminator, spec.Kind))
                || spec.DependsOn is { } needs && !_index.ContainsKey(needs) || spec.WaitsOn.Any(x => !_index.ContainsKey(x)))
            {
                continue;
            }

            Report(ClaimFrom(extension, spec, definition, ""), spec.Element, definition);
        }

        // A reader that fails on a probe has failed on nothing the model states.
        _extensionFailures.RemoveRange(failures, _extensionFailures.Count - failures);
        return found;

        void Report(Claim probe, IElement element, KindDefinition definition)
        {
            Evaluate(probe);
            // A flag the code turns off says what the model does by leaving it unset.
            if (probe.Reading is not { From: "code", Value: { Length: > 0 } value, At: { File.Length: > 0 } at } || definition.ValueType == "bool" && value == "false")
            {
                return;
            }

            var type = probe.Needs?.Kind == "member.exists" && Owner(element) is { } parent ? parent.Name : element.Name;
            // An association end is named by the class whose navigation it is: Order.Buyer.
            var name = element is IAssociationEnd && Owner(element) is { } holder ? $"{holder.Name}.{NameOf(element)}"
                : type == element.Name ? element.Name : $"{type}.{element.Name}";
            found.Add(new UnmodelledCode(probe.Area, at, name,
                $"`{name}` {definition.Label}: the code states `{value}`, which the model leaves unset; it would state it in {probe.SourceText}. Model it, or add an exception with a reason to the Root Folder's Drift Check.",
                element.Id, type));
        }
    }

    /// <summary>A claim as the vocabulary entry would state it, with no value: null when what it waits on isn't claimed, as it then reads nothing.</summary>
    private Claim? Probe(string designer, Pack pack, JsonNode entry, IElement element, KindDefinition definition)
    {
        var needs = Needs(designer, element, entry["dependsOn"]?.GetValue<string>());
        if (needs is not null && !_index.ContainsKey(needs))
        {
            return null;
        }

        // What it waits on says where the kind applies: a parameter's source waits on its operation being an endpoint, which a
        // business class's operation isn't claimed to be, so there is nothing there to probe.
        if ((entry["waitsOn"] as JsonArray)?.Select(x => Needs(designer, element, x!.GetValue<string>())).Any(x => x is not null && !_index.ContainsKey(x)) == true)
        {
            return null;
        }

        IReadOnlyDictionary<string, string>? bindings;
        try
        {
            bindings = (entry["bindings"] as JsonObject)?.ToDictionary(x => x.Key, x => Expression.Text(Expression.Evaluate(x.Value!.GetValue<string>(), element)) ?? "");
        }
        catch (FormatException)
        {
            bindings = null;
        }

        return new Claim
        {
            Kind = definition.Kind,
            Designer = designer,
            Element = element.Id,
            Path = PathOf(element),
            Expected = "",
            Stated = definition.Stated,
            Bindings = bindings,
            Origin = pack.Id,
            Source = element,
            Definition = definition,
            Needs = needs,
            WaitsOn = (entry["waitsOn"] as JsonArray)?.Select(x => Needs(designer, element, x!.GetValue<string>())).OfType<ClaimRef>().ToList() ?? [],
            SourceText = entry["source"]?.GetValue<string>() ?? definition.Kind,
            Area = pack.Data["vocabulary"]?[designer]?["area"]?.GetValue<string>() ?? designer
        };
    }

    /// <summary>
    /// The opt-in rules the Drift Check turns on, each narrowed by its rows' globs and its exceptions. Code a rule already
    /// reports as differing from the model (a <c>ModelCode</c> finding at its file and line, such as the raise
    /// <c>messaging/unmodelled.raise</c> reports) isn't reported again, so one place is one finding.
    /// </summary>
    private List<Finding> OptIn(IReadOnlyList<Finding> reported)
    {
        var places = reported.Where(x => x.Category == "ModelCode" && x.Subject is { File: not null, Line: not null })
            .Select(x => (x.Subject.File!, x.Subject.Line!.Value))
            .ToHashSet();
        var rules = StockOptInRules.Select(x => (Extension: "intent.core", Rule: x))
            .Concat(_extensions.SelectMany(x => x.OptInRules.Select(r => (Extension: x.Id, Rule: r))))
            .GroupBy(x => $"optin/{x.Rule.Id}")
            .ToDictionary(x => x.Key, x => x.Last());
        var findings = CheckFindings(rules.Keys.ToList());
        var context = new RuleContext(this, _graph, _codeScope, _claims);
        foreach (var (id, (extension, rule)) in rules.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            var rows = _check.OptIn.Where(x => x.Rule == id).ToList();
            if (rows.Count == 0)
            {
                continue;
            }

            List<UnmodelledCode> found;
            try
            {
                found = rule.Find(context).ToList();
            }
            catch (ArgumentException e)
            {
                // A convention's find path the engine can't read is the pack's error, not the rule's.
                findings.Add(Config("config/ruleInvalid", $"`{id}` can't run: {e.Message}"));
                continue;
            }
            catch (Exception e)
            {
                _extensionFailures.Add((extension, $"{e.GetType().Name}: {e.Message}"));
                continue;
            }

            // Each entry of a row's Match that excepted something: one that excepts nothing is stale on its own.
            var matched = new HashSet<(ExceptionRow, string)>();
            foreach (var code in found.OrderBy(x => x.At.File, StringComparer.Ordinal).ThenBy(x => x.At.Line))
            {
                // A row's glob narrows the rule within scope; it never widens it, and Context code is read, not checked.
                if (!_codeScope.Checks(code.At.File) || !rows.Any(x => x.Globs.Count == 0 || x.Globs.Any(g => g.IsMatch(code.At.File))))
                {
                    continue;
                }

                if (_check.Exceptions.LastOrDefault(x => x.Rule == id && x.Matches(code.At.File, code.Name, code.Type)) is { } exception)
                {
                    matched.Add((exception, exception.MatchOf(code.At.File, code.Name, code.Type)!));
                    continue;
                }

                if (places.Contains((code.At.File, code.At.Line)))
                {
                    continue;
                }

                findings.Add(new Finding(id, code.Designer is null ? ScopeArea : $"{code.Designer} (DriftEngine)", "ModelCode", "error", code.Message,
                    new Anchor(File: code.At.File, Line: code.At.Line), Against: code.Owner is null ? null : [new Anchor(Element: code.Owner)]));
            }

            // Only a rule that ran can say its exception matches nothing: one for a rule that's off waits for it.
            foreach (var exception in _check.Exceptions.Where(x => x.Rule == id))
            {
                foreach (var entry in exception.Entries.Where(x => !matched.Contains((exception, x))))
                {
                    findings.Add(new Finding("optin/staleException", ScopeArea, "Code", "warning",
                        $"Exception row {exception.Number} of the Drift Check ({id}, `{entry}`) matches nothing. Remove it, or correct its Match.", Root));
                }
            }
        }

        return findings;
    }

    private Anchor Root => _check.Host is null ? new Anchor(Text: CodebaseStructure) : new Anchor(Element: _check.Host);

    /// <summary>The Drift Check's own rows: each needs a reason, a known rule, and an override a severity the report has.</summary>
    private List<Finding> CheckFindings(IReadOnlyList<string> optInRules)
    {
        var findings = new List<Finding>();
        var known = string.Join(", ", optInRules.Order(StringComparer.Ordinal).Select(x => $"`{x}`"));
        foreach (var row in _check.OptIn.Where(x => !optInRules.Contains(x.Rule)))
        {
            findings.Add(Config("config/unknownRule", $"Opt-in Rules row {row.Number} of the Drift Check names `{row.Rule}`, which no installed module provides. The opt-in rules are {known}."));
        }

        foreach (var row in _check.Exceptions)
        {
            if (!optInRules.Contains(row.Rule))
            {
                findings.Add(Config("config/unknownRule", $"Exceptions row {row.Number} of the Drift Check names `{row.Rule}`, which no installed module provides. Exceptions apply to the opt-in rules: {known}."));
            }

            if (row.Reason is null)
            {
                findings.Add(Config("config/exceptionReason", $"Exceptions row {row.Number} of the Drift Check ({row.Rule}, `{row.Match}`) has no reason. Say why this code isn't modelled."));
            }
        }

        foreach (var row in _check.Overrides)
        {
            if (row.Reason is null)
            {
                findings.Add(Config("config/overrideReason", $"Severity Overrides row {row.Number} of the Drift Check ({row.Rule}) has no reason. Say why these findings take another severity."));
            }

            if (OverrideSeverity(row) is null)
            {
                findings.Add(Config("config/overrideSeverity", $"Severity Overrides row {row.Number} of the Drift Check ({row.Rule}) has severity `{row.Severity}`. Use Error or Warning."));
            }

            // The engine's own families are checked; a module's rule names its findings as it likes.
            var family = row.Rule.Split('/')[0];
            if ((family == "optin" && !optInRules.Append("optin/staleException").Any(row.Matches)) || (family == "claim" && !_kinds.Keys.Any(x => row.Matches($"claim/{x}"))))
            {
                findings.Add(Config("config/unknownRule", $"Severity Overrides row {row.Number} of the Drift Check names `{row.Rule}`, which matches no rule the installed modules provide."));
            }
        }

        return findings;
    }

    private Finding Config(string rule, string message) => new(rule, ScopeArea, "Code", "error", message, Root);

    /// <summary>A finding takes the last override row that matches its rule; one with no usable severity changes nothing.</summary>
    private List<Finding> Overridden(IEnumerable<Finding> findings) => findings
        .Select(x => x.Severity != "info" && _check?.Overrides.LastOrDefault(o => OverrideSeverity(o) is not null && o.Matches(x.Rule)) is { } row
            ? x with { Severity = OverrideSeverity(row)! }
            : x)
        .ToList();

    private static string? OverrideSeverity(OverrideRow row) => row.Severity?.ToLowerInvariant() is "error" or "warning" ? row.Severity.ToLowerInvariant() : null;
}
