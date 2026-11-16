using System.Text.RegularExpressions;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.DriftEngine.Scope;

/// <summary>An <c>Opt-in Rules</c> row: a rule turned on, narrowed to the files its globs match (none: every file in scope).</summary>
internal sealed record OptInRow(int Number, string Rule, IReadOnlyList<GlobPattern> Globs);

/// <summary>
/// An <c>Exceptions</c> row: code an opt-in rule must not report, and why. Its Match lists one or more entries, one per line,
/// so code skipped for one reason (helper classes) is one row.
/// </summary>
internal sealed record ExceptionRow(int Number, string Rule, string Match, string? Reason)
{
    private readonly (string Entry, GlobPattern Glob)[] _entries =
        Match.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x => (x, new GlobPattern(x))).ToArray();

    public IEnumerable<string> Entries => _entries.Select(x => x.Entry);

    /// <summary>The entry that matches: a file glob, a type's name, <c>Type.Member</c>, or the name the rule gives the code.</summary>
    public string? MatchOf(string file, string name, string? type) =>
        _entries.FirstOrDefault(x => string.Equals(x.Entry, name, StringComparison.Ordinal) || string.Equals(x.Entry, type, StringComparison.Ordinal) || x.Glob.IsMatch(file)).Entry;

    public bool Matches(string file, string name, string? type) => MatchOf(file, name, type) is not null;
}

/// <summary>A <c>Severity Overrides</c> row: findings whose rule id matches (<c>*</c> for any run of characters) take its severity.</summary>
internal sealed record OverrideRow(int Number, string Rule, string? Severity, string? Reason)
{
    private readonly Regex _rule = new($"^{Regex.Escape(Rule).Replace("\\*", ".*")}$", RegexOptions.CultureInvariant);

    public bool Matches(string rule) => _rule.IsMatch(rule);
}

/// <summary>
/// The Root Folder's <c>Drift Check</c> stereotype (scope.md, The Drift Check stereotype): the opt-in rules a team turns
/// on, the exceptions to them, and severity overrides. Its <c>Packs</c> list isn't read yet: every stock pack runs.
/// </summary>
internal sealed class CheckModel
{
    private CheckModel(string? host, IReadOnlyList<OptInRow> optIn, IReadOnlyList<ExceptionRow> exceptions, IReadOnlyList<OverrideRow> overrides)
    {
        Host = host;
        OptIn = optIn;
        Exceptions = exceptions;
        Overrides = overrides;
    }

    /// <summary>The Root Folder that holds the stereotype, for configuration findings to name.</summary>
    public string? Host { get; }

    public IReadOnlyList<OptInRow> OptIn { get; }

    public IReadOnlyList<ExceptionRow> Exceptions { get; }

    public IReadOnlyList<OverrideRow> Overrides { get; }

    public static CheckModel From(IReadOnlyList<IPackage>? codebaseStructure)
    {
        var root = codebaseStructure?.FirstOrDefault(x => x.Stereotypes.Any(s => s.Name == "Drift Check"));
        var stereotype = root?.Stereotypes.First(x => x.Name == "Drift Check");
        return new CheckModel(
            root?.Id ?? codebaseStructure?.FirstOrDefault()?.Id,
            Rows(stereotype, "Opt-in Rules").Select((x, i) => new OptInRow(i + 1, Text(x, "Rule") ?? "", Lines(x, "Glob").Select(g => new GlobPattern(g)).ToList())).ToList(),
            Rows(stereotype, "Exceptions").Select((x, i) => new ExceptionRow(i + 1, Text(x, "Rule") ?? "", Text(x, "Match") ?? "", Text(x, "Reason"))).ToList(),
            Rows(stereotype, "Severity Overrides").Select((x, i) => new OverrideRow(i + 1, Text(x, "Rule") ?? "", Text(x, "Severity"), Text(x, "Reason"))).ToList());
    }

    // An item list: Intent hands each row over as a stereotype of its own.
    private static IStereotype[] Rows(IStereotype? stereotype, string list) =>
        stereotype?.Properties.FirstOrDefault(x => x.Key == list) is IStereotypeProperty<IStereotype[]> { Value: { } items } ? items : [];

    private static string? Text(IStereotype item, string key) =>
        item.Properties.FirstOrDefault(x => x.Key == key)?.Value is { } value && !string.IsNullOrWhiteSpace(value) ? value.Trim() : null;

    private static IEnumerable<string> Lines(IStereotype item, string key) =>
        Text(item, key)?.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
}
