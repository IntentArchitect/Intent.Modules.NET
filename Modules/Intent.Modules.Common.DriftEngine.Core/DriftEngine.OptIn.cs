using System.Text.Json.Nodes;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.OptIn;
using Intent.Modules.Common.DriftEngine.Scope;

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
    private static readonly IOptInRule[] StockOptInRules = [new UnmodelledFile(), new UnmodelledType(), new UnmodelledMember()];

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

    /// <summary>The opt-in rules the Drift Check turns on, each narrowed by its rows' globs and its exceptions.</summary>
    private List<Finding> OptIn()
    {
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

            var matched = new HashSet<ExceptionRow>();
            foreach (var code in found.OrderBy(x => x.At.File, StringComparer.Ordinal).ThenBy(x => x.At.Line))
            {
                // A row's glob narrows the rule within scope; it never widens it, and Context code is read, not checked.
                if (!_codeScope.Checks(code.At.File) || !rows.Any(x => x.Globs.Count == 0 || x.Globs.Any(g => g.IsMatch(code.At.File))))
                {
                    continue;
                }

                if (_check.Exceptions.LastOrDefault(x => x.Rule == id && x.Matches(code.At.File, code.Name, code.Type)) is { } exception)
                {
                    matched.Add(exception);
                    continue;
                }

                findings.Add(new Finding(id, code.Designer is null ? ScopeArea : $"{code.Designer} (DriftEngine)", "ModelCode", "error", code.Message,
                    new Anchor(File: code.At.File, Line: code.At.Line), Against: code.Owner is null ? null : [new Anchor(Element: code.Owner)]));
            }

            // Only a rule that ran can say its exception matches nothing: one for a rule that's off waits for it.
            foreach (var exception in _check.Exceptions.Where(x => x.Rule == id && !matched.Contains(x)))
            {
                findings.Add(new Finding("optin/staleException", ScopeArea, "Code", "warning",
                    $"Exception row {exception.Number} of the Drift Check ({id}, `{exception.Match}`) matches nothing. Remove it, or correct its Match.", Root));
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
