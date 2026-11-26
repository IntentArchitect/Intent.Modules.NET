using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Scope;
using Intent.Modules.Common.DriftEngine.Vocabulary;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>
/// One drift run: scope decides which files are read, the readers build the code graph, the packs' vocabulary turns the
/// model into claims, and structure comparison gives each claim its outcome. Nothing is written (P4).
/// </summary>
public sealed partial class DriftEngine
{
    private const string CodebaseStructure = "Codebase Structure";

    private readonly DriftInput _input;
    private readonly IReadOnlyList<Pack> _packs;
    private readonly IReadOnlyList<IDriftExtension> _extensions;
    private readonly IReadOnlyDictionary<string, KindDefinition> _kinds;
    private readonly IReadOnlyDictionary<string, IValueComparer> _comparers;
    private readonly IReadOnlyList<(string Extension, ILanguage Language)> _languages;

    /// <summary>
    /// Each language's profile: its type names, collections, naming and type kinds. Code is compared under its own language's,
    /// so one run reads C# and TypeScript, or Java and TypeScript, each as written (C3).
    /// </summary>
    private readonly Dictionary<string, JsonNode> _profiles = new(StringComparer.Ordinal);

    /// <summary>The profile of the language that read the most in-scope files, for what no code says the language of.</summary>
    private JsonNode _profile = new JsonObject();

    /// <summary>The languages that read in-scope files, most files first.</summary>
    private IReadOnlyList<string> _read = [];
    private ScopeModel _scope = null!;
    private readonly List<(string Path, ScopeDecision Decision)> _files = [];
    private readonly List<Claim> _claims = [];
    private readonly Dictionary<ClaimRef, Claim> _index = [];
    private readonly Dictionary<Claim, TypeNode> _types = [];

    /// <summary>The type claims being located, so two elements of one name that each look at the other's type don't loop.</summary>
    private readonly HashSet<Claim> _locating = [];

    /// <summary>The types verified by the code's use of a library's type, which it references and doesn't declare.</summary>
    private readonly HashSet<Claim> _referenced = [];
    private readonly Dictionary<Claim, MemberNode> _members = [];

    /// <summary>The member a claim reads, as the refiners gave it (<see cref="IMemberRefiner"/>), when one did.</summary>
    private readonly Dictionary<Claim, MemberNode> _refined = [];

    /// <summary>What an extension's existence claim located that is neither a type nor a member: a route, a mapping.</summary>
    private readonly Dictionary<Claim, object> _located = [];
    private readonly List<(string Extension, string Error)> _extensionFailures = [];

    /// <summary>
    /// An extension's part failed: the part (a reader, rule or claim source) and the code that threw are named, so a house
    /// reader's bug is found from the finding alone.
    /// </summary>
    private void Failed(string extension, object part, Exception e)
    {
        var frame = e.StackTrace?.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        _extensionFailures.Add((extension, $"{part.GetType().Name} threw {e.GetType().Name}: {e.Message}{(frame is null ? "" : $" ({frame})")}"));
    }

    /// <summary>What the packs get wrong that is found without the code: an entry for a kind nothing defines.</summary>
    private readonly List<Finding> _packFindings = [];
    private CodeGraph _graph = new();
    private CodeScope _codeScope = null!;

    /// <summary>The engine's own comparers; an extension adds those its kinds need.</summary>
    private static readonly IValueComparer[] CoreComparers =
    [
        new ValueComparer("exact", (expected, actual) => expected == actual),
        new ValueComparer("ignoreCase", (expected, actual) => string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase)),
        new ValueComparer("number", (expected, actual) =>
            decimal.TryParse(actual, NumberStyles.Number, CultureInfo.InvariantCulture, out var a) && decimal.TryParse(expected, NumberStyles.Number, CultureInfo.InvariantCulture, out var e) && a == e),
        // A comma-separated set: a key's columns, a policy's roles.
        new ValueComparer("set", (expected, actual) => Items(expected).SetEquals(Items(actual))),
        // Every item the model states is in the code's set, which may have more: a type's required bases.
        new ValueComparer("subset", (expected, actual) => Items(expected).IsSubsetOf(Items(actual))),
        // The same items in the same order: a composite key's columns.
        new ValueComparer("list", (expected, actual) => Ordered(expected).SequenceEqual(Ordered(actual), StringComparer.Ordinal)),
        // The model's items in the code in the model's order, the code may have more: an enum's literals.
        new ValueComparer("subsequence", (expected, actual) => IsSubsequence(Ordered(expected), Ordered(actual))),
        // The code's text holds the model's: a page title within a heading.
        new ValueComparer("contains", (expected, actual) => actual.Contains(expected, StringComparison.Ordinal)),
        // Versions equal ignoring a leading v and trailing .0 parts: v1 is 1.0.
        new ValueComparer("version", (expected, actual) => Version(expected) == Version(actual))
    ];

    private static HashSet<string> Items(string text) => text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToHashSet(StringComparer.Ordinal);

    private static List<string> Ordered(string text) => text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

    private static bool IsSubsequence(IReadOnlyList<string> items, IReadOnlyList<string> within)
    {
        var next = 0;
        foreach (var item in within)
        {
            if (next < items.Count && item == items[next])
            {
                next++;
            }
        }

        return next == items.Count;
    }

    private static string Version(string text)
    {
        var trimmed = text.Trim().TrimStart('v', 'V');
        while (trimmed.EndsWith(".0", StringComparison.Ordinal))
        {
            trimmed = trimmed[..^2];
        }

        return trimmed;
    }

    private DriftEngine(DriftInput input)
    {
        _input = input;
        _extensions = input.Extensions ?? [];
        var core = Pack.Stock("intent.core") ?? throw new InvalidOperationException("The engine's core pack is missing from its resources.");
        _packs = new[] { core }.Concat(input.Packs).Concat(_extensions.SelectMany(x => x.Packs))
            .GroupBy(x => x.Id).Select(x => x.First()).ToList();
        _kinds = _packs.SelectMany(x => x.Kinds).Concat(_extensions.SelectMany(x => x.Kinds)).GroupBy(x => x.Kind).ToDictionary(x => x.Key, x => x.Last());
        _comparers = CoreComparers.Concat(_extensions.SelectMany(x => x.Comparers)).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.Last());

        // Of two extensions reading a language, the later one's reader is used.
        _languages = _extensions.SelectMany(x => x.Languages.Select(l => (x.Id, l))).GroupBy(x => x.l.Name).Select(x => x.Last()).ToList();
    }

    /// <exception cref="OperationCanceledException">The input's <see cref="DriftInput.CancellationToken"/> was cancelled.</exception>
    public static RunResult Run(DriftInput input) => new DriftEngine(input).Run();

    private RunResult Run()
    {
        // The read is kept for the next run once this one is done with the graph, however it ends: a run cancelled or failed
        // after its read finished leaves a read as good as any.
        try
        {
            return Check();
        }
        finally
        {
            Keep();
        }
    }

    private RunResult Check()
    {
        var findings = new List<Finding>();
        var run = Stopwatch.GetTimestamp();
        var cancellation = _input.CancellationToken;
        Timed("code", ReadCode);
        Timed("claims.build", BuildClaims);
        Timed("claims.read", () =>
        {
            foreach (var claim in _claims)
            {
                cancellation.ThrowIfCancellationRequested();
                Evaluate(claim);
            }
        });

        if (_scope.Rows.Count == 0)
        {
            // One error says no code is in scope (R21); the claims it leaves unverified aren't reported one by one.
            var root = _input.Designers.GetValueOrDefault(CodebaseStructure)?.FirstOrDefault();
            findings.Add(new Finding("scope/missing", ScopeArea, "Code", "error",
                $"No Drift Scope is set on the Codebase Structure designer, so no code is in scope and {_claims.Count} claims can't be verified. Add a Drift Scope to the Root Folder with In rows for the code to check.",
                root is null ? new Anchor(Text: CodebaseStructure) : new Anchor(Element: root.Id)));
        }
        else
        {
            findings.AddRange(ScopeFindings());
            findings.AddRange(_claims.Where(x => x.Outcome != "verified" && x.Reason is not "blocked" && !NotStated(x)).Select(Report));
            // One note per language and per what its code leaves unstated, a type or a nullability, since each says why and
            // how to state it.
            findings.AddRange(_claims.Where(NotStated)
                .GroupBy(x => (Language: LanguageOf(x.Reading!.At!.File)?.Name ?? "", Untyped: _untyped.Contains(x)))
                .OrderBy(x => x.Key.Language, StringComparer.Ordinal)
                .ThenBy(x => x.Key.Untyped)
                .Select(x => NotStatedFinding(x.ToList(), x.Key.Untyped)));

            Timed("rules", () => findings.AddRange(Rules()));
            Timed("optin", () => findings.AddRange(OptIn(findings)));
            Timed("unread", () => findings.AddRange(Unread()));
        }

        findings.AddRange(_graph.Failures.Select(x => new Finding("engine/readerFailed", ScopeArea, "Code", "error",
            $"The {LanguageOf(x.File)?.Title ?? "code"} reader can't read {x.File}: {x.Error} Claims that need it can't be verified.", new Anchor(File: x.File, Line: 1))));
        findings.AddRange(_extensionFailures.GroupBy(x => x.Extension).Select(x => new Finding("config/pluginFailed", ScopeArea, "Code", "error",
            $"The {x.Key} extension failed: {x.First().Error}{(x.Count() > 1 ? $" ({x.Count() - 1} more failures)" : "")} Claims it reads can't be verified.", new Anchor(Text: x.Key))));
        findings.AddRange(_packFindings.DistinctBy(x => x.Message));
        var reported = Overridden(findings);
        List<CodeLink> links = [];
        if (_scope.Rows.Count > 0)
        {
            cancellation.ThrowIfCancellationRequested();
            Timed("links", () => links = Links(reported));
        }

        _timings.Add(new Timing("total", Stopwatch.GetElapsedTime(run).TotalMilliseconds));
        _timings.AddRange(_kindTimes.OrderByDescending(x => x.Value.Ticks)
            .Select(x => new Timing($"kind {x.Key}", TimeSpan.FromTicks(x.Value.Ticks * TimeSpan.TicksPerSecond / Stopwatch.Frequency).TotalMilliseconds, x.Value.Count)));
        return new RunResult(_claims, reported, Coverage(), _packs.Select(x => new PackRef(x.Id, x.Version)).ToList(), FilesRead()) { Timings = _timings, Links = links };
    }

    /// <summary>How long each part of the run took, for the log.</summary>
    private readonly List<Timing> _timings = [];

    private void Timed(string what, Action action, int? count = null)
    {
        var started = Stopwatch.GetTimestamp();
        action();
        _timings.Add(new Timing(what, Stopwatch.GetElapsedTime(started).TotalMilliseconds, count));
    }

    private const string ScopeArea = "Drift Scope (DriftEngine)";

    /// <summary>
    /// The extensions' rules, over in-scope code, the last registered of each id: a module's rule replaces a stock one of its
    /// id, and the Software Factory log says so. Their Areas are marked as the engine's, like its claims'.
    /// </summary>
    private List<Finding> Rules()
    {
        var findings = new List<Finding>();
        var context = new RuleContext(this, _graph, _codeScope, _claims);
        var rules = _extensions.SelectMany(x => x.Rules.Select(r => (Extension: x.Id, Rule: r))).GroupBy(x => x.Rule.Id, StringComparer.Ordinal).ToList();
        findings.AddRange(rules.Where(x => x.Count() > 1).Select(x => new Finding("scope/replacedRule", ScopeArea, "Code", "info",
            $"The {x.Last().Extension} extension's rule `{x.Key}` replaces the {string.Join(" and ", x.SkipLast(1).Select(r => r.Extension).Distinct())} extension's, which doesn't run.",
            new Anchor(Text: x.Last().Extension))));
        foreach (var (extension, rule) in rules.Select(x => x.Last()))
        {
            _input.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                // Code a Context row decides is read for the rules, never reported on itself.
                findings.AddRange(rule.Check(context)
                    .Where(x => x.Subject.File is not { } file || !_codeScope.Includes(file) || _codeScope.Checks(file))
                    .Select(x => x with { Area = $"{x.Area} (DriftEngine)" })
                    .ToList());
            }
            catch (Exception e)
            {
                Failed(extension, rule, e);
            }
        }

        return findings;
    }

    /// <summary>
    /// A claim the code can't state in its language: a C# reference type outside a nullable context, or an unannotated Java
    /// one, or a type a JavaScript declaration leaves out (<see cref="_untyped"/>). It is can't verify, <c>unresolved</c>,
    /// like any fact the reader can't supply (R35), but neither side can be wrong, so it isn't drift: it has no finding of
    /// its own, and an <c>engine/notStated</c> note per language lists them.
    /// </summary>
    private bool NotStated(Claim claim) =>
        claim is { Reason: "unresolved", Reading: { Value: "unknown", At: not null } }
        && (claim.Kind.EndsWith(".nullable", StringComparison.Ordinal) || _untyped.Contains(claim));

    /// <summary>
    /// Claims on a type the code leaves out, in a language that lets a declaration go without one: JavaScript declares none,
    /// TypeScript infers what a declaration leaves out, and Python states a type only in an annotation. The model can state
    /// it and the code can't be wrong about it, so, as with a nullability the code can't state, each is can't verify,
    /// <c>unresolved</c>, has no finding of its own, and is listed in the language's <c>engine/notStated</c> note on types.
    /// </summary>
    private readonly HashSet<Claim> _untyped = new(ReferenceEqualityComparer.Instance);

    /// <summary>Whether the file's language lets a declaration leave its type out: its profile has an <c>untyped</c> note.</summary>
    private bool MayLeaveTypesOut(string file) => ProfileOf(file)["untyped"] is not null;

    /// <summary>The claim can't be checked: the code declares no type for <paramref name="what"/> (<see cref="_untyped"/>).</summary>
    private void Untyped(Claim claim, Span at, string what)
    {
        _untyped.Add(claim);
        Cant(claim, "unresolved", new Reading { At = at, Value = "unknown", Detail = $"{at.File} declares no type for {what}." });
    }

    /// <summary>The note's why and how-to come from the language profile's <c>nullable.notStated</c>, or its <c>untyped.notStated</c>.</summary>
    private Finding NotStatedFinding(IReadOnlyList<Claim> claims, bool untyped)
    {
        var designers = claims.GroupBy(x => x.Designer).OrderBy(x => x.Key).Select(x => $"{x.Key} {x.Count()}");
        var folders = claims.Select(x => x.Reading!.At!.File).Distinct().GroupBy(x => x.Split('/')[0]).OrderBy(x => x.Key).Select(x => $"{x.Key} {x.Count()}");
        var profile = ProfileOf(claims[0].Reading!.At!.File);
        var (what, why) = untyped
            ? ("claims on types their code leaves out", profile["untyped"]?["notStated"]?.GetValue<string>() ?? "Their code declares no type for these.")
            : ("nullability claims", profile["nullable"]?["notStated"]?.GetValue<string>() ?? "Their code doesn't state whether these types are nullable.");
        return new Finding("engine/notStated", ScopeArea, "Code", "info",
            $"Not checked: {claims.Count} {what} ({string.Join(", ", designers)}). {why} Files: {string.Join(", ", folders)}.",
            new Anchor(Text: untyped ? "types" : "nullability"));
    }

    /// <summary>Rows that need attention, and the coverage listing (R15, R23, R24).</summary>
    private IEnumerable<Finding> ScopeFindings()
    {
        var listing = new List<string>();
        foreach (var row in _scope.Rows)
        {
            var subject = new Anchor(Element: row.Host);
            var what = row.Types.Count > 0 ? $"types {string.Join(", ", row.Types)}" : string.Join(", ", row.Globs.Select(x => x.Pattern));
            // A Context row keeps its code from being checked as much as an Out row does, so it needs a reason too.
            if ((!row.In || row.Context) && string.IsNullOrWhiteSpace(row.Reason))
            {
                yield return new Finding("scope/reason", ScopeArea, "Code", "error", row.Context
                    ? $"Scope row {row.Number} ({what}) is Context with no reason. Say why this code is read but not checked."
                    : $"Scope row {row.Number} ({what}) is Out with no reason. Say why this code isn't checked.", subject);
            }

            var decides = row.Types.Count > 0
                ? _excludedTypes.Count(x => x.Row == row)
                : _files.Count(x => x.Decision.Row == row);
            if (decides == 0)
            {
                yield return new Finding("scope/stale", ScopeArea, "Code", "warning",
                    $"Scope row {row.Number} ({what}) decides no {(row.Types.Count > 0 ? "type" : "file")}. Remove it, or correct it.", subject);
            }
            else if (!row.In)
            {
                listing.Add($"row {row.Number} ({what}), {decides} {(row.Types.Count > 0 ? "types" : "files")}: {row.Reason}");
            }
            else if (row.Context)
            {
                listing.Add($"row {row.Number} ({what}), {decides} files read for context only: {row.Reason}");
            }
        }

        listing.AddRange(_builtin.Select(x => $"built in, {x.Key}, {x.Value} {(x.Value == 1 ? "folder" : "folders")}"));
        var unmatched = _files.Where(x => x.Decision.Row is null)
            .GroupBy(x => x.Path.Split('/')[0])
            .Select(x => $"{x.Key} {x.Count()}")
            .ToList();
        if (unmatched.Count > 0)
        {
            listing.Add($"not in any row: {string.Join(", ", unmatched)}");
        }

        // One listing of what isn't read and why, never a finding per file.
        if (listing.Count > 0)
        {
            var root = _input.Designers.GetValueOrDefault(CodebaseStructure)?.FirstOrDefault();
            yield return new Finding("scope/coverage", ScopeArea, "Code", "info", $"Out of scope: {string.Join("; ", listing)}.",
                root is null ? new Anchor(Text: CodebaseStructure) : new Anchor(Element: root.Id));
        }
    }

    private readonly List<(TypeNode Type, ScopeRow Row)> _excludedTypes = [];

    /// <summary>How many folders each built-in exclusion took; the walk never entered them, so their files aren't in <see cref="_files"/>.</summary>
    private IReadOnlyDictionary<string, int> _builtin = new Dictionary<string, int>();

    private void ReadCode()
    {
        // The walk and the scope's decisions, apart from the languages' reads.
        var walk = Stopwatch.GetTimestamp();
        var walked = ScopeModel.Walk(_input.OutputFolder);
        _builtin = walked.Builtin;
        _scope = ScopeModel.From(_input.Designers.GetValueOrDefault(CodebaseStructure));
        _check = CheckModel.From(_input.Designers.GetValueOrDefault(CodebaseStructure));

        // The last run's read of this folder, when nothing it depends on has changed since: its scope, decisions, graph and
        // what its readers stored, all of a piece. The Drift Check is the model's, so it is this run's either way.
        _readKey = _input.KeepCode ? ReadKey(walked, _scope) : null;
        if (_readKey is not null && TakeKept(_readKey) is { } kept)
        {
            (_scope, _codeScope, _graph, _stored, _readCounts) = (kept.Scope, kept.CodeScope, kept.Graph, kept.Stored, kept.Read);
            _files.AddRange(kept.Files);
            _excludedTypes.AddRange(kept.ExcludedTypes);
            _graph.KeepOnly(_stored);
            _timings.Add(new Timing("code files", Stopwatch.GetElapsedTime(walk).TotalMilliseconds, _files.Count));
            _timings.Add(new Timing("code cached", 0, _readCounts.Values.Sum()));
            Profiles();
            _readDone = true;
            return;
        }

        foreach (var file in walked.Files)
        {
            _files.Add((file.Path, _scope.Decide(file.Path)));
        }

        _codeScope = new CodeScope(_files.Select(x => (x.Path, x.Decision)));
        _timings.Add(new Timing("code files", Stopwatch.GetElapsedTime(walk).TotalMilliseconds, _files.Count));

        // Each language reads its in-scope files; a file no language reads isn't read.
        var failures = _extensionFailures.Count;
        var read = new Dictionary<string, int>();
        foreach (var (extension, language) in _languages)
        {
            var files = _files.Where(x => x.Decision.In && LanguageOf(x.Path) == language).Select(x => x.Path).OrderBy(x => x, StringComparer.Ordinal).ToList();
            if (files.Count == 0)
            {
                continue;
            }

            _input.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                Timed($"code {language.Name}", () => language.Read(_input.OutputFolder, files, _graph, _input.CancellationToken), files.Count);
                read[language.Name] = files.Count;
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                Failed(extension, language, e);
            }

            TimedParts($"code {language.Name}");
        }

        _readCounts = read;
        Profiles();

        // A type a scope row excludes by name is not read, though its file is (R26).
        foreach (var type in _graph.Types.ToList())
        {
            if (type.Files.Select(f => _scope.ExcludesType(f, type.Name)).FirstOrDefault(x => x is not null) is { } row)
            {
                _excludedTypes.Add((type, row));
                _graph.Types.Remove(type);
            }
        }

        foreach (var extension in _extensions)
        {
            foreach (var reader in extension.Readers)
            {
                _input.CancellationToken.ThrowIfCancellationRequested();
                try
                {
                    Timed($"code {extension.Id} {reader.GetType().Name}", () => reader.Read(_graph));
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Failed(extension.Id, reader, e);
                }

                TimedParts($"code {extension.Id} {reader.GetType().Name}");
            }
        }

        _stored = _graph.Stored;
        _readFailed = _extensionFailures.Count > failures;
        _readDone = true;
    }

    /// <summary>
    /// Each language's profile, from the packs (the last pack's that has one), and the primary one: the profile of the language
    /// that read the most files and has one. A markup language that declares no types of its own (XAML, whose views are their
    /// code-behind's) has no profile, so it never stands for the code's.
    /// The packs are the model's, so this is worked out every run, a kept read's too.
    /// </summary>
    private void Profiles()
    {
        foreach (var name in _languages.Select(x => x.Language.Name))
        {
            if (_packs.Select(x => x.Profile(name)).LastOrDefault(x => x is not null) is { } profile)
            {
                _profiles[name] = profile;
            }
        }

        _read = _readCounts.OrderByDescending(x => x.Value).ThenBy(x => x.Key, StringComparer.Ordinal).Select(x => x.Key).ToList();
        var candidates = _read.Concat(_languages.Select(x => x.Language.Name)).ToList();
        var primary = candidates.FirstOrDefault(_profiles.ContainsKey) ?? candidates.FirstOrDefault();
        _profile = primary is not null && _profiles.TryGetValue(primary, out var primaryProfile) ? primaryProfile : new JsonObject();
    }

    /// <summary>The parts a reader timed on the graph (<see cref="CodeGraph.Timed"/>), each logged under the reader: <c>code csharp parse</c>.</summary>
    private void TimedParts(string reader) =>
        _timings.AddRange(_graph.TakeTimed().Select(x => new Timing($"{reader} {x.What}", x.Milliseconds)));

    private readonly Dictionary<string, ILanguage?> _languageOf = new(StringComparer.Ordinal);

    /// <summary>The language that reads a file; null when none does.</summary>
    private ILanguage? LanguageOf(string path) =>
        _languageOf.TryGetValue(path, out var language) ? language : _languageOf[path] = _languages.Select(x => x.Language).FirstOrDefault(x => x.Reads(path));

    /// <summary>
    /// The in-scope types named as each one's own language names something (<paramref name="nameIn"/>, given its profile), in
    /// the graph's order: the name is worked out once per language and looked up, not worked out again for every type.
    /// </summary>
    private List<TypeNode> TypesNamedIn(Func<JsonNode, string> nameIn)
    {
        var names = new Dictionary<JsonNode, string>(ReferenceEqualityComparer.Instance);
        string Name(JsonNode profile) => names.TryGetValue(profile, out var name) ? name : names[profile] = nameIn(profile);

        // A type's profile is its language's, else the primary one (ProfileOf).
        return _graph.TypesNamed(_profiles.Values.Append(_profile).Select(Name)).Where(x => x.Name == Name(ProfileOf(x))).ToList();
    }

    /// <summary>The profile of the language a file is written in; the run's primary one when no language reads it.</summary>
    internal JsonNode ProfileOf(string file) => LanguageOf(file) is { } language && _profiles.TryGetValue(language.Name, out var profile) ? profile : _profile;

    internal JsonNode ProfileOf(TypeNode type) => ProfileOf(type.At.File);

    internal JsonNode ProfileOf(MemberNode member) => ProfileOf(member.At.File);

    /// <summary>The profile of the language a type reference is written in, as its reader says.</summary>
    internal JsonNode ProfileOf(TypeRefNode type) => type.Language is { } language && _profiles.TryGetValue(language, out var profile) ? profile : _profile;

    /// <summary>
    /// The model's name for a type the code writes, when its language's profile maps it (a primitive: <c>int</c> for C#'s
    /// <c>int</c> or <c>Int32</c>, <c>guid</c> for <c>Guid</c>); null for any other type, a generic or an array.
    /// </summary>
    internal string? ModelTypeName(TypeRefNode code)
    {
        if (code.IsArray || code.Arguments.Count > 0 || code.Union.Count > 0)
        {
            return null;
        }

        var written = code.Text.TrimEnd('?');
        foreach (var (name, spellings) in ProfileOf(code)["types"] as JsonObject ?? [])
        {
            if (spellings is JsonArray names && names.Any(x => x?.GetValue<string>() is { } spelling && (spelling == written || spelling == code.Name || spelling == code.QualifiedName)))
            {
                return name;
            }
        }

        return null;
    }

    /// <summary>The profiles of the languages that read files, the primary one first; the primary alone when none did.</summary>
    private IEnumerable<JsonNode> ReadProfiles() => _read.Count == 0 ? [_profile] : _read.Select(x => _profiles.GetValueOrDefault(x)).OfType<JsonNode>();

    private IReadOnlyList<FileRead> FilesRead() =>
        _files.Where(x => x.Decision.In).Select(x => (x.Path, Language: LanguageOf(x.Path))).Where(x => x.Language is not null)
            .Select(x => new FileRead(x.Path, x.Language!.Name)).ToList();

    private Coverage Coverage()
    {
        var rows = _scope.Rows.Select(row => new CoverageRow(
            row.Host, row.Number, row.Globs.Select(x => x.Pattern).ToList(), row.Types.Count == 0 ? null : row.Types, row.Scope, row.Reason,
            _files.Count(x => x.Decision.Row == row))).ToList();
        var unmatched = _files.Count(x => x.Decision.Row is null);
        return new Coverage(rows, _builtin, unmatched == 0 ? new Dictionary<string, int>() : new Dictionary<string, int> { ["not in any row"] = unmatched });
    }

    // ---- Claims (block 1) ----

    private void BuildClaims()
    {
        foreach (var designer in _input.Designers.Keys)
        {
            var elements = Elements(designer);
            foreach (var pack in _packs)
            {
                foreach (var entry in pack.Vocabulary(designer))
                {
                    // An extending entry makes no claims: it adds placeholders to those another entry makes (Extended).
                    if (entry["extends"] is not null)
                    {
                        continue;
                    }

                    // An entry for a kind no pack or extension defines can't make a claim; the pack's other entries still run.
                    if (entry["kind"]?.GetValue<string>() is not { } kind || !_kinds.ContainsKey(kind))
                    {
                        _packFindings.Add(new Finding("config/ruleInvalid", ScopeArea, "Code", "error",
                            $"Pack `{pack.Id}`'s {designer} vocabulary has an entry for the kind `{entry["kind"]}`, which no installed pack or extension defines, so nothing it states is checked.", new Anchor(Text: pack.Id)));
                        continue;
                    }

                    var specialization = entry["for"]?["element"]?.GetValue<string>();
                    var association = entry["for"]?["associationEnd"]?.GetValue<string>();
                    var whole = entry["for"]?["association"]?.GetValue<string>();
                    var where = entry["for"]?["where"]?.GetValue<string>();
                    var applies = entry["for"]?["applies"]?.GetValue<string>();
                    // The claim exists only where `when` is true (vocabulary.md, Entries).
                    var when = entry["when"]?.GetValue<string>();
                    var selected = whole is not null ? Associations(elements, whole)
                        : association is null ? elements.Where(x => x.SpecializationType == specialization)
                        : NavigableEnds(elements, association);
                    foreach (var element in selected)
                    {
                        if (applies is not null && Expression.Evaluate(applies, element) is not true
                            || where is not null && Expression.Evaluate(where, element) is not true
                            || when is not null && Expression.Evaluate(when, element) is not true)
                        {
                            continue;
                        }

                        // An entry with each makes a claim per item of its list, as item, told apart by its discriminator: one
                        // registration per message a handler subscribes to.
                        var items = entry["each"]?.GetValue<string>() is { } each
                            ? (Expression.Evaluate(each, element) as System.Collections.IEnumerable)?.Cast<object?>().ToList() ?? []
                            : new List<object?> { null };
                        foreach (var item in items)
                        {
                            if (Build(designer, pack, entry, element, item) is { } claim)
                            {
                                Add(claim, replace: false);
                            }
                        }
                    }
                }
            }

            // A convention that says what kinds an element's type may be states its type.kind, where no vocabulary does (R43).
            foreach (var element in elements.Where(x => ClaimOn(designer, x, "type.exists") is not null && ClaimOn(designer, x, "type.kind") is null))
            {
                if (_packs.LastOrDefault(x => x.TypeConvention(designer, element.SpecializationType)?["typeKinds"] is JsonArray) is { } conventions
                    && Build(designer, conventions, new JsonObject { ["kind"] = "type.kind", ["dependsOn"] = "type.exists", ["source"] = $"the {element.SpecializationType} conventions' type kinds" }, element) is { } kind)
                {
                    Add(kind, replace: false);
                }
            }
        }

        // The claims extensions state in code come after the packs', so one restating a pack's claim replaces it.
        var model = new ModelContext(this);
        foreach (var extension in _extensions)
        {
            foreach (var source in extension.ClaimSources)
            {
                try
                {
                    foreach (var spec in source.Claims(model).ToList())
                    {
                        if (FromSpec(extension.Id, spec) is { } claim)
                        {
                            Add(claim, replace: true);
                        }
                    }
                }
                catch (Exception e)
                {
                    Failed(extension.Id, source, e);
                }
            }
        }
    }

    private void Add(Claim claim, bool replace)
    {
        var key = new ClaimRef(claim.Designer, claim.Element, claim.Kind, claim.Discriminator);
        if (_index.TryGetValue(key, out var existing))
        {
            if (!replace)
            {
                return;
            }

            _claims.Remove(existing);
        }

        _index[key] = claim;
        _claims.Add(claim);
    }

    private Claim? FromSpec(string extension, ClaimSpec spec)
    {
        if (!_kinds.TryGetValue(spec.Kind, out var definition))
        {
            throw new InvalidOperationException($"It states a {spec.Kind} claim on {spec.Element.Name}, but no pack or extension defines the kind {spec.Kind}.");
        }

        var expected = spec.Expected;
        if (string.IsNullOrEmpty(expected))
        {
            // No claim, but what the source would claim if the model set it: unmodelled.value's probe, as a vocabulary
            // entry's is.
            if (definition.Stated == "whenSet")
            {
                _unsetSpecs.Add((extension, spec));
                return null;
            }

            expected = definition.ValueType switch { "bool" => "false", "set" => "none", "presence" => "present", _ => "" };
        }

        return ClaimFrom(extension, spec, definition, expected);
    }

    /// <summary>A claim source's claims of a <c>whenSet</c> kind that the model leaves unset, for <c>unmodelled.value</c>.</summary>
    private readonly List<(string Extension, ClaimSpec Spec)> _unsetSpecs = [];

    private Claim ClaimFrom(string extension, ClaimSpec spec, KindDefinition definition, string expected)
    {
        return new Claim
        {
            Kind = spec.Kind,
            Designer = spec.Designer,
            Element = spec.Element.Id,
            Discriminator = spec.Discriminator,
            Path = PathOf(spec.Element),
            Expected = expected,
            Stated = definition.Stated,
            Bindings = spec.Bindings,
            Origin = extension,
            Source = spec.Element,
            Definition = definition,
            Needs = spec.DependsOn ?? Needs(spec.Designer, spec.Element, Locating(spec.Kind)),
            WaitsOn = spec.WaitsOn,
            ExpectedValue = spec.Value,
            SourceText = spec.Source ?? spec.Kind,
            Area = spec.Area ?? spec.Designer
        };
    }

    private Claim? Build(string designer, Pack pack, JsonNode entry, IElement element, object? item = null)
    {
        var kind = entry["kind"]!.GetValue<string>();
        var definition = _kinds[kind];
        var value = entry["value"] is { } valueExpression ? Expression.Evaluate(valueExpression.GetValue<string>(), element, item) : null;
        string? expected = kind switch
        {
            "type.kind" => string.Join(", ", TypeKinds(designer, element)),
            "operation.parameters" => string.Join(", ", Parameters(element).Select(x => $"{x.Name}: {(x.TypeReference is { Element: not null } type ? Expression.TypeText(type) : "?")}")),
            "operation.returns" when value is null => "void",
            // A presence the model states as absent (a proxy Operation with no Backend Operation sends no request) is one.
            _ when definition.ValueType == "presence" => Expression.Text(value) == "absent" ? "absent" : "present",
            _ => Expression.Text(value)
        };

        // An empty value is no claim for a kind stated only when set (decision 10).
        if (expected is null)
        {
            if (definition.Stated == "whenSet")
            {
                return null;
            }

            // An empty set reads "none", as one read from the code does.
            expected = definition.ValueType switch { "bool" => "false", "set" => "none", _ => "" };
        }

        return new Claim
        {
            Kind = kind,
            Designer = designer,
            Element = element.Id,
            // Several claims of a kind on one element are told apart by it: a marker's name, a subscription's message.
            Discriminator = entry["discriminator"]?.GetValue<string>() is { } discriminator ? Expression.Text(Expression.Evaluate(discriminator, element, item)) : null,
            Path = PathOf(element),
            Expected = expected,
            Stated = definition.Stated,
            Bindings = Extended(designer, kind, element, item,
                (entry["bindings"] as JsonObject)?.ToDictionary(x => x.Key, x => Expression.Text(Expression.Evaluate(x.Value!.GetValue<string>(), element, item)) ?? "", StringComparer.Ordinal)),
            Origin = pack.Id,
            Source = element,
            Definition = definition,
            Needs = Needs(designer, element, entry["dependsOn"]?.GetValue<string>() ?? Locating(kind)),
            WaitsOn = (entry["waitsOn"] as JsonArray)?.Select(x => Needs(designer, element, x!.GetValue<string>())).OfType<ClaimRef>().ToList() ?? [],
            ExpectedValue = value,
            SourceText = entry["source"]?.GetValue<string>() ?? kind,
            Area = pack.Data["vocabulary"]?[designer]?["area"]?.GetValue<string>() ?? designer
        };
    }

    /// <summary>
    /// A claim's bindings with the placeholders extending entries add (vocabulary.md, Extending another pack's entry): each
    /// pack's entry whose <c>extends</c> names the claim's kind and whose <c>for</c> selects its element. The extended entry
    /// still decides whether the claim exists, its value and its source. Only the kind of <c>extends</c> is matched: one
    /// entry claims a kind on an element, and the stock vocabularies have moved between packs (what cases call
    /// <c>intent.domain/column.name</c> is <c>intent.rdbms</c>'s entry now). A placeholder the claim already binds is a
    /// <c>config/vocabularyConflict</c> error, and the first binding stands.
    /// </summary>
    private Dictionary<string, string>? Extended(string designer, string kind, IElement element, object? item, Dictionary<string, string>? bindings)
    {
        foreach (var pack in _packs)
        {
            foreach (var entry in pack.Vocabulary(designer))
            {
                if (entry["extends"]?.GetValue<string>() is not { } extends || extends[(extends.LastIndexOf('/') + 1)..] != kind || !Selects(entry["for"], element))
                {
                    continue;
                }

                foreach (var (key, expression) in entry["bindings"] as JsonObject ?? [])
                {
                    bindings ??= new Dictionary<string, string>(StringComparer.Ordinal);
                    if (!bindings.TryAdd(key, Expression.Text(Expression.Evaluate(expression!.GetValue<string>(), element, item)) ?? "") && _conflicts.Add((pack.Id, designer, extends, key)))
                    {
                        _packFindings.Add(new Finding("config/vocabularyConflict", ScopeArea, "Code", "error",
                            $"Pack `{pack.Id}`'s {designer} vocabulary extends `{extends}` with `{{{key}}}`, which its claims already bind, so the extension's binding isn't used. Bind another placeholder, or remove it.",
                            new Anchor(Text: pack.Id)));
                    }
                }
            }
        }

        return bindings;
    }

    /// <summary>Each extending entry's placeholder reported as a conflict, so it is reported once, not once per claim.</summary>
    private readonly HashSet<(string Pack, string Designer, string Extends, string Key)> _conflicts = [];

    /// <summary>An extending entry's <c>for</c>: the element type, or the association a claim's end belongs to, and its <c>where</c>.</summary>
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

    /// <summary>
    /// The existence claim whose located type or member the engine's own rule for a kind reads, which a claim depends on
    /// when its vocabulary entry or claim source names none: its element's type or member, an enum literal's enum.
    /// </summary>
    private static string? Locating(string kind) => kind switch
    {
        "type.kind" or "type.abstract" or "type.namespace" or "type.base" or "type.generalization" or "type.file" or "type.location" or "type.generics"
            or "type.marker" or "enum.literals" => "type.exists",
        "enum.literal.value" => "parent:type.exists",
        "member.type" or "member.nullable" or "member.static" or "operation.returns" or "operation.nullable" or "operation.parameters" => "member.exists",
        _ => null
    };

    /// <summary>A vocabulary entry's <c>dependsOn</c>: a kind on the same element, or <c>parent:</c> a kind on its owner.</summary>
    private static ClaimRef? Needs(string designer, IElement element, string? dependsOn)
    {
        if (dependsOn is null)
        {
            return null;
        }

        var onParent = dependsOn.StartsWith("parent:", StringComparison.Ordinal);
        var owner = onParent ? Owner(element) : element;
        return owner is null ? null : new ClaimRef(designer, owner.Id, onParent ? dependsOn["parent:".Length..] : dependsOn);
    }

    internal IReadOnlyCollection<string> DesignerNames => _input.Designers.Keys.ToList();

    internal IReadOnlyList<IPackage> Packages(string designer) => _input.Designers.GetValueOrDefault(designer) ?? [];

    internal IReadOnlyList<IElement> Elements(string designer) =>
        _input.Designers.GetValueOrDefault(designer)?.SelectMany(x => x.ChildElements).SelectMany(Descendants).ToList() ?? [];

    internal IReadOnlyList<IPackage> References(string designer) => _input.References.GetValueOrDefault(designer) ?? [];

    internal IReadOnlyList<IElement> ReferencedElements(string designer) => References(designer).SelectMany(x => x.ChildElements).SelectMany(Descendants).ToList();

    private static IEnumerable<IElement> Descendants(IElement element) => new[] { element }.Concat(element.ChildElements.SelectMany(Descendants));

    /// <summary>
    /// The navigable ends of an association type: each is a member of the element at its other end (a navigation property),
    /// reached from that element's associated ends.
    /// </summary>
    private static IEnumerable<IElement> NavigableEnds(IEnumerable<IElement> elements, string association) =>
        elements.SelectMany(Ends)
            .Where(x => x.IsNavigable && x.Association?.SpecializationType == association)
            .GroupBy(x => x.Id)
            .Select(x => x.First());

    /// <summary>
    /// The associations of a type (vocabulary.md, <c>{ "association": ... }</c>), each as its target end, which Intent gives
    /// the association's id: the claim is the association's, and <c>source</c> and <c>target</c> read its ends.
    /// </summary>
    private static IEnumerable<IElement> Associations(IEnumerable<IElement> elements, string association) =>
        elements.SelectMany(Ends)
            .Where(x => x.Association?.SpecializationType == association && IsTargetEnd(x))
            .GroupBy(x => x.Id)
            .Select(x => x.First());

    /// <summary>
    /// The ends of an element's links, and of the links that start at a link's end in turn: a subscription's handler end
    /// performs invocations and entity actions of its own.
    /// </summary>
    public static IEnumerable<IAssociationEnd> Ends(IElement element)
    {
        var seen = new HashSet<IElement>(ReferenceEqualityComparer.Instance) { element };
        var queue = new Queue<IElement>([element]);
        while (queue.TryDequeue(out var current))
        {
            foreach (var end in current.AssociatedElements ?? [])
            {
                yield return end;
                if (IsTargetEnd(end) && seen.Add(end))
                {
                    queue.Enqueue(end);
                }
            }
        }
    }

    internal static bool IsTargetEnd(IAssociationEnd end) =>
        end.Association is { } association && (ReferenceEquals(association.TargetEnd, end) || association.TargetEnd?.Id == end.Id && association.SourceEnd?.Id != end.Id);

    /// <summary>What an element is a member of: its parent, or for an association end, the element at the other end.</summary>
    internal static IElement? Owner(IElement element) =>
        element is IAssociationEnd end ? end.OtherEnd()?.TypeReference?.Element as IElement : element.ParentElement;

    /// <summary>An element's name; for a link's end that has none, the name of what it points to.</summary>
    private static string NameOf(IElement element) =>
        element is IAssociationEnd { Name: null or "" } end ? end.TypeReference?.Element?.Name ?? "" : element.Name ?? "";

    private static IReadOnlyList<IElement> Parameters(IElement element) => element.ChildElements.Where(x => x.SpecializationType == "Parameter").ToList();

    private static IReadOnlyList<string> PathOf(IElement element)
    {
        var path = new List<string>();
        for (var current = element; current is not null; current = Owner(current))
        {
            path.Insert(0, current.Name);
        }

        return path;
    }

    private readonly Dictionary<(string Designer, string Type), JsonObject?> _conventions = [];

    /// <summary>
    /// An element type's convention, each field from the last pack whose convention for the type states it, so a house pack
    /// changes one field (what the type states, its name) and keeps the stock pack's others (its type kinds, locations)
    /// without copying them. Before, the last pack's convention replaced the stock one whole.
    /// </summary>
    private JsonObject? Convention(string designer, IElement element)
    {
        var key = (designer, element.SpecializationType);
        if (_conventions.TryGetValue(key, out var known))
        {
            return known;
        }

        JsonObject? merged = null;
        foreach (var convention in _packs.Select(x => x.TypeConvention(designer, element.SpecializationType)).OfType<JsonObject>())
        {
            merged ??= new JsonObject();
            foreach (var (name, value) in convention)
            {
                merged[name] = value?.DeepClone();
            }
        }

        return _conventions[key] = merged;
    }

    /// <summary>The kinds an element's type may be: its convention's, else every kind the languages read have.</summary>
    private IReadOnlyList<string> TypeKinds(string designer, IElement element) =>
        (Convention(designer, element)?["typeKinds"] as JsonArray)?.Select(x => x!.GetValue<string>()).ToList()
        ?? ReadProfiles().SelectMany(x => (x["typeKinds"] as JsonArray)?.Select(k => k!.GetValue<string>()) ?? []).Distinct().ToList();

    // ---- Outcomes (block 3) ----

    private void Evaluate(Claim claim)
    {
        if (claim.Reading is not null || claim.Reason is not null)
        {
            return;
        }

        var withoutMember = false;
        if (Dependency(claim) is { } dependency)
        {
            Evaluate(dependency);
            claim.DependsOn = new ClaimRef(dependency.Designer, dependency.Element, dependency.Kind);

            // A member the code lacks still has what other code must state about it read, by a reader that reads that code by
            // the member's name (an EF Core configuration's IsRequired()), so both fixes are reported in one run (user,
            // 2026-11-24). Its other claims wait on it.
            withoutMember = dependency is { Kind: "member.exists", Outcome: "contradicted" } && ClaimReaders(claim.Kind).Any(x => x.Reader.KindsWithoutMember.Contains(claim.Kind));
            if (dependency.Outcome != "verified" && !withoutMember)
            {
                Cant(claim, "blocked", new Reading { Detail = $"Waits on {dependency.Kind}" });
                return;
            }

            if (_referenced.Contains(dependency))
            {
                Cant(claim, "blocked", new Reading { Detail = "Its type is a library's the code references, whose declaration isn't read." });
                return;
            }
        }

        // The claims it also waits on, where the model states them: a column waits on its table's entity.persisted. Only one
        // the code contradicts makes it moot; one that can't be verified leaves it to be checked on its own evidence.
        foreach (var gate in claim.WaitsOn.Select(x => _index.GetValueOrDefault(x)).OfType<Claim>())
        {
            Evaluate(gate);
            if (gate.Outcome == "contradicted")
            {
                claim.DependsOn = new ClaimRef(gate.Designer, gate.Element, gate.Kind);
                Cant(claim, "blocked", new Reading { Detail = $"Waits on {gate.Kind}" });
                return;
            }
        }

        if (_scope.Rows.Count == 0)
        {
            Cant(claim, "outOfScope", new Reading { Detail = "No Drift Scope is set, so no file is in scope." });
            return;
        }

        // Each kind's own reading time: what a reading spends evaluating other claims counts to theirs.
        var outer = _nested;
        _nested = 0;
        var started = Stopwatch.GetTimestamp();
        try
        {
            ReadClaim(claim, withoutMember);
        }
        finally
        {
            var total = Stopwatch.GetTimestamp() - started;
            var (ticks, count) = _kindTimes.GetValueOrDefault(claim.Kind);
            _kindTimes[claim.Kind] = (ticks + total - _nested, count + 1);
            _nested = outer + total;
        }
    }

    /// <summary>Ticks spent reading the claims a reading evaluates, so each kind's time is its own.</summary>
    private long _nested;

    /// <summary>Each kind's reading time and claim count.</summary>
    private readonly Dictionary<string, (long Ticks, int Count)> _kindTimes = new(StringComparer.Ordinal);

    /// <summary>An extension's claim readers of a kind, a later extension's first.</summary>
    private IEnumerable<(string Extension, IClaimReader Reader)> ClaimReaders(string kind) =>
        _extensions.SelectMany(x => x.ClaimReaders.Select(r => (x.Id, r))).Reverse().Where(x => x.r.Kinds.Contains(kind));

    /// <summary>
    /// The first reading of the readers from <paramref name="start"/> on, each able to ask the ones after it in turn
    /// (<see cref="ClaimContext.ReadNext"/>). A reader that fails is reported, and reads as can't verify.
    /// </summary>
    private ClaimReading? ReadFrom(Claim claim, IReadOnlyList<(string Extension, IClaimReader Reader)> readers, int start)
    {
        for (var i = start; i < readers.Count; i++)
        {
            var (extension, reader) = readers[i];
            var after = i + 1;
            try
            {
                if (reader.Read(new ClaimContext(this, claim, _graph, _codeScope, () => ReadFrom(claim, readers, after))) is { } reading)
                {
                    return reading;
                }
            }
            catch (Exception e)
            {
                Failed(extension, reader, e);
                return ClaimReading.CantVerify("ruleFailed", $"The {extension} extension failed reading it.");
            }
        }

        return null;
    }

    /// <param name="withoutMember">
    /// Its member isn't in the code: only a reader that reads the kind without it (<see cref="IClaimReader.KindsWithoutMember"/>)
    /// reads it, and with none answering it waits on the member as any other claim does.
    /// </param>
    private void ReadClaim(Claim claim, bool withoutMember = false)
    {
        if (withoutMember)
        {
            foreach (var (extension, reader) in ClaimReaders(claim.Kind).Where(x => x.Reader.KindsWithoutMember.Contains(claim.Kind)))
            {
                ClaimReading? reading;
                try
                {
                    reading = reader.Read(new ClaimContext(this, claim, _graph, _codeScope));
                }
                catch (Exception e)
                {
                    Failed(extension, reader, e);
                    Cant(claim, "ruleFailed", new Reading { Detail = $"The {extension} extension failed reading it." });
                    return;
                }

                if (reading is not null)
                {
                    Apply(claim, reading);
                    return;
                }
            }

            Cant(claim, "blocked", new Reading { Detail = "Waits on member.exists" });
            return;
        }

        // A parameter's claims read its operation's method as the house reads it for the operation's parameters: a String
        // its house binds as an enum (eShop.Backend's Http.parseEnum) is that enum for parameter.type too.
        if (claim.Kind.StartsWith("parameter.", StringComparison.Ordinal) && Owner(claim.Source) is { } operation
            && ClaimOn(claim.Designer, operation, "operation.parameters") is { } signature)
        {
            Evaluate(signature);
            if (_refined.TryGetValue(signature, out var refinedSignature))
            {
                _refined[claim] = refinedSignature;
            }
        }

        // A house's refiners give the located member as the house reads it, in extension order, so a later one refines what
        // an earlier one gave; everything below reads that member.
        foreach (var (extension, refiner) in _extensions.SelectMany(x => x.Refiners.Select(r => (x.Id, r))).Where(x => x.r.Kinds.Contains(claim.Kind)))
        {
            try
            {
                if (refiner.Refine(new ClaimContext(this, claim, _graph, _codeScope)) is { } refined)
                {
                    _refined[claim] = refined;
                }
            }
            catch (Exception e)
            {
                Failed(extension, refiner, e);
                Cant(claim, "ruleFailed", new Reading { Detail = $"The {extension} extension failed refining it." });
                return;
            }
        }

        // An extension's claim reader comes before the engine's own structure rules, and a later extension's before an
        // earlier one's, so a module's reader can read any kind, the engine's included. A reader can ask the readers after it
        // (ClaimContext.ReadNext) and adjust what they read.
        var ordered = ClaimReaders(claim.Kind).ToList();
        for (var i = 0; i < ordered.Count; i++)
        {
            var (extension, reader) = ordered[i];
            var after = i + 1;
            ClaimReading? reading;
            try
            {
                reading = reader.Read(new ClaimContext(this, claim, _graph, _codeScope, () => ReadFrom(claim, ordered, after)));
            }
            catch (Exception e)
            {
                Failed(extension, reader, e);
                Cant(claim, "ruleFailed", new Reading { Detail = $"The {extension} extension failed reading it." });
                return;
            }

            if (reading is not null)
            {
                Apply(claim, reading);
                return;
            }
        }

        // The engine's own rule for these kinds reads the type or member their existence claim located. Nothing located one
        // when the model states no such claim, or a claim reader verified it without the code's declaration.
        if (Locating(claim.Kind) is { } existence && (existence == "member.exists" ? LocatedMember(claim) is null : LocatedType(claim) is null))
        {
            Cant(claim, "blocked", new Reading { Detail = $"Waits on {existence.Replace("parent:", "its owner's ")}, which located no declaration." });
            return;
        }

        switch (claim.Kind)
        {
            case "type.exists":
                TypeExists(claim);
                break;
            case "type.kind":
            {
                // A language's own kind stands for the kinds its profile's kindOf names: a Protobuf message is a class or a record.
                var type = OwnType(claim);
                var kinds = new[] { type.Kind }.Concat((ProfileOf(type)["kindOf"]?[type.Kind] as JsonArray)?.Select(x => x!.GetValue<string>()) ?? []);
                Compare(claim, type.Kind, type.At, "code", kinds.Any(claim.Expected.Split(", ").Contains));
                break;
            }
            case "type.abstract":
            {
                var type = OwnType(claim);
                Compare(claim, type.IsAbstract ? "true" : "false", type.At, type.IsAbstract ? "code" : "absent", claim.Expected == (type.IsAbstract ? "true" : "false"));
                break;
            }
            case "member.exists":
                MemberExists(claim);
                break;
            case "member.type":
            {
                var member = OwnMember(claim);
                if (member.Type is null && MayLeaveTypesOut(member.At.File))
                {
                    Untyped(claim, member.At, $"`{member.Name}`");
                    break;
                }

                if (member.Type is null || claim.ExpectedValue is not ITypeReference model)
                {
                    Cant(claim, "notFound", new Reading { At = member.At, Detail = "The member has no type." });
                    break;
                }

                // A member a generic type of the code declares is typed by its type parameters as the element's type binds them
                // (ValidatableObject<string>'s Value is a string), and one left unbound is at least an object, which a
                // designer with no generics can state: less precise, but true.
                var bound = TypeParameters.Bound(_graph, Owner(claim.Source) is { } parent ? TypeOf(claim.Designer, parent) : null, member,
                    ProfileOf(member)["types"]?["object"]?[0]?.GetValue<string>() ?? "object");
                Compare(claim, (bound?.Bound ?? member.Type).Text, member.At, "code",
                    TypeMatches(model, member.Type) || bound is { } typed && (TypeMatches(model, typed.Bound) || TypeMatches(model, typed.Erased)));
                break;
            }
            case "member.nullable":
            {
                var member = OwnMember(claim);
                if (_graph.UngovernedFiles.TryGetValue(member.At.File, out var project))
                {
                    Cant(claim, "readerFailed", new Reading { At = member.At, Detail = $"{project}, which sets {member.At.File}'s nullable context, can't be read." });
                    break;
                }

                // With no type, the nullability is unstated for want of the type, which the types note says how to state.
                if (member.Type is null && MayLeaveTypesOut(member.At.File))
                {
                    Untyped(claim, member.At, $"`{member.Name}`");
                    break;
                }

                if (member.Type?.Nullable is null or "unknown")
                {
                    Cant(claim, "unresolved", new Reading { At = member.At, Value = "unknown", Detail = $"{member.At.File} doesn't state whether `{member.Type?.Text}` is nullable." });
                    break;
                }

                Compare(claim, member.Type.Nullable, member.At, "code", member.Type.Nullable == claim.Expected);
                break;
            }
            case "member.static":
            {
                var member = OwnMember(claim);
                var value = member.IsStatic ? "true" : "false";
                Compare(claim, value, member.At, member.IsStatic ? "code" : "absent", value == claim.Expected);
                break;
            }
            case "operation.returns":
            {
                // An async method returns what the model states, through the profile's async wrappers: C#'s Task<T> is T,
                // and Task is void. A function that leaves its return type out, as every JavaScript one does, can't be
                // compared with a type the model states, so the claim is not stated; against no type it is void, as before.
                var member = OwnMember(claim);
                if (member.Type is null && claim.ExpectedValue is ITypeReference && MayLeaveTypesOut(member.At.File))
                {
                    Untyped(claim, member.At, $"what `{member.Name}` returns");
                    break;
                }

                var returns = Unwrapped(member);
                var actual = returns?.Text ?? "void";
                Compare(claim, member.Type?.Text ?? "void", member.At, "code", claim.ExpectedValue is ITypeReference model
                    ? returns is not null && returns.Text != "void" && TypeMatches(model, returns)
                    : actual == "void");
                break;
            }
            case "operation.nullable":
            {
                // Whether what the method returns, through the profile's async wrappers, may be null: C#'s Task<Order?>.
                var member = OwnMember(claim);
                if (_graph.UngovernedFiles.TryGetValue(member.At.File, out var project))
                {
                    Cant(claim, "readerFailed", new Reading { At = member.At, Detail = $"{project}, which sets {member.At.File}'s nullable context, can't be read." });
                    break;
                }

                if (member.Type is null && MayLeaveTypesOut(member.At.File))
                {
                    Untyped(claim, member.At, $"what `{member.Name}` returns");
                    break;
                }

                var returns = Unwrapped(member);
                if (returns?.Nullable is null or "unknown")
                {
                    Cant(claim, "unresolved", new Reading { At = member.At, Value = "unknown", Detail = $"{member.At.File} doesn't state whether `{returns?.Text}` is nullable." });
                    break;
                }

                Compare(claim, returns.Nullable, member.At, "code", returns.Nullable == claim.Expected);
                break;
            }
            case "type.namespace":
            {
                var type = OwnType(claim);
                Compare(claim, type.Namespace, type.At, "code", type.Namespace == claim.Expected);
                break;
            }
            case "type.base":
            {
                // The type's supertypes, through its in-scope bases (code-reader.md, Capabilities).
                // An in-scope supertype is read qualified; the model may name it alone (IBasketState for eShop.WebApp.Services.IBasketState).
                var type = OwnType(claim);
                var all = _graph.Supertypes(type).ToList();
                var supertypes = string.Join(", ", all);
                var named = claim.Expected.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .All(x => all.Any(s => s == x || s.EndsWith("." + x, StringComparison.Ordinal)));
                Compare(claim, supertypes.Length > 0 ? supertypes : "none", type.At, supertypes.Length > 0 ? "code" : "absent", Same(claim, supertypes) || named && claim.Expected.Length > 0);
                break;
            }
            case "type.generalization":
            {
                // The modelled class the type derives from. A base the model doesn't have, such as a codebase's own Entity, is
                // type.base's to state, so the code's is the first of its bases that is one of the designer's types.
                var type = OwnType(claim);
                var elements = Elements(claim.Designer).Where(x => ClaimOn(claim.Designer, x, "type.exists") is not null).ToList();
                var modelled = type.BaseTypes
                    .Select(x => _graph.TypeFor(x, type) is { } resolved
                        ? elements.FirstOrDefault(e => TypeOf(claim.Designer, e) == resolved)?.Name
                        : elements.FirstOrDefault(e => e.Name == x.Name)?.Name)
                    .FirstOrDefault(x => x is not null);
                var expected = claim.Expected.Length > 0 ? claim.Expected : null;
                claim.Outcome = modelled == expected ? "verified" : "contradicted";
                claim.Reading = new Reading { Value = modelled, From = modelled is null ? "absent" : "code", At = type.At };
                break;
            }
            case "type.file":
            {
                // The file a type is declared in, without its extension; a partial type is in each of its files.
                var type = OwnType(claim);
                var names = type.Files.Select(Path.GetFileNameWithoutExtension).ToList();
                Compare(claim, string.Join(", ", names), type.At, "code", names.Contains(claim.Expected));
                break;
            }
            case "type.location":
            {
                // The path of the file declaring the type, from the output folder; a partial type is in each of its files.
                var type = OwnType(claim);
                var same = type.Files.Any(x => Same(claim, x));
                Compare(claim, same ? claim.Expected : type.At.File, type.At, "code", same);
                break;
            }
            case "type.generics":
            {
                // The type's own type parameters by name, in order; constraints aren't part of their names.
                var type = OwnType(claim);
                var parameters = string.Join(", ", type.TypeParameters);
                Compare(claim, parameters.Length > 0 ? parameters : "none", type.At, "code", Same(claim, parameters));
                break;
            }
            case "type.marker":
                Marker(claim);
                break;
            case "file.exists":
                FileExists(claim);
                break;
            case "enum.literals":
                EnumLiterals(claim);
                break;
            case "enum.literal.value":
                LiteralValue(claim);
                break;
            case "repository.entity":
                RepositoryEntity(claim);
                break;
            case "repository.implementation":
                RepositoryImplementation(claim);
                break;
            case "parameter.type" or "parameter.nullable" or "parameter.default":
                Parameter(claim);
                break;
            case "operation.parameters":
            {
                // The parameters in order, each its name and type; injected ones aren't part of the signature. A class may
                // rename the parameters its interface declares (CatalogAI's items for ICatalogAI's item), and the diagram
                // can't show both names, so the method's declaration on either side is its signature.
                var member = OwnMember(claim);
                var code = Stated(member);
                var model = Parameters(claim.Source);
                // A model that also lists what is injected (a gRPC ServerCallContext), as one drawn from the method does, states
                // the whole signature, which is as true. C#'s @ only escapes a keyword: @event is event. A parameter with a
                // parameter.type claim of its own (an HTTP endpoint's) has its type compared there, so a mismatch is one finding.
                // A parameter whose code leaves its type out, where its language lets it (JavaScript's), matches on its name
                // alone, and the signature is then not stated rather than verified, unless another declaration states the types.
                (bool Same, bool Untyped) Matches(IReadOnlyList<ParameterNode> stated)
                {
                    if (stated.Count != model.Count)
                    {
                        return default;
                    }

                    var untyped = false;
                    foreach (var (parameter, modelled) in stated.Zip(model))
                    {
                        if (parameter.Name.TrimStart('@') != modelled.Name.TrimStart('@') || modelled.TypeReference is not { Element: not null } modelType
                            || parameter.Type is null && !MayLeaveTypesOut(parameter.At.File))
                        {
                            return default;
                        }

                        if (ClaimOn(claim.Designer, modelled, "parameter.type") is not null)
                        {
                            continue;
                        }

                        if (parameter.Type is null)
                        {
                            untyped = true;
                        }
                        else if (!TypeMatches(modelType, parameter.Type))
                        {
                            return default;
                        }
                    }

                    return (true, untyped);
                }

                // The method's own declaration first, and its counterparts only when that doesn't match.
                IEnumerable<MemberNode> Declarations()
                {
                    yield return member;
                    foreach (var counterpart in Counterparts(member))
                    {
                        yield return counterpart;
                    }
                }

                var same = false;
                var named = false;
                foreach (var match in Declarations().SelectMany(x => new[] { Stated(x), x.Parameters }).Select(Matches).Where(x => x.Same))
                {
                    same = !match.Untyped;
                    named = true;
                    if (same)
                    {
                        break;
                    }
                }

                if (named && !same)
                {
                    Untyped(claim, member.At, $"`{member.Name}`'s parameters");
                    break;
                }

                Compare(claim, string.Join(", ", code.Select(x => $"{x.Name}: {x.Type?.Text ?? "?"}")), member.At, "code", same);
                break;
            }
            default:
            {
                // Readers that found no code of their framework in scope: the file that would state it may be one a scope row
                // leaves out, named for the claim's type as a configuration is (OrderConfiguration for Order).
                var readers = _extensions.Where(x => x.ClaimReaders.Any(r => r.Kinds.Contains(claim.Kind))).Select(x => x.Id).ToList();
                var stems = new[] { claim.Source, Owner(claim.Source) }.OfType<IElement>().Select(NameOf).Where(x => x.Length > 0).ToList();
                var shadow = readers.Count == 0 ? default
                    : _files.FirstOrDefault(x => !x.Decision.In && OwnCode(x) && LanguageOf(x.Path) is not null
                                                 && stems.Any(stem => Path.GetFileName(x.Path).StartsWith(stem, StringComparison.Ordinal)));
                if (shadow.Path is not null)
                {
                    Cant(claim, "outOfScope", new Reading { ShadowFile = shadow.Path, ScopeRow = shadow.Decision.Row?.Id });
                    break;
                }

                Cant(claim, "noRule", new Reading
                {
                    Detail = readers.Count > 0
                        ? $"Its readers ({string.Join(", ", readers)}) found none of their framework's code in scope to read it from."
                        : $"There's no rule for {claim.Kind} in the active packs."
                });
                break;
            }
        }
    }

    private Claim? Dependency(Claim claim) => claim.Needs is { } needs ? _index.GetValueOrDefault(needs) : null;

    /// <summary>
    /// The same method on the other side of an interface: for a class's method, the declarations its interfaces have of it;
    /// for an interface's, its implementations'. Of the same name and parameter count.
    /// </summary>
    private IEnumerable<MemberNode> Counterparts(MemberNode member)
    {
        if (_graph.Types.FirstOrDefault(x => x.Members.Contains(member)) is not { } type)
        {
            return [];
        }

        var others = type.Kind == "interface"
            ? _graph.ImplementationsOf(type)
            : _graph.Bases(type).Where(x => x.Kind == "interface");
        return others.SelectMany(x => x.Members).Where(x => x.Kind == "method" && x.Name == member.Name && x.Parameters.Count == member.Parameters.Count);
    }

    private void TypeExists(Claim claim)
    {
        // The conventions' name, else the profile's: an element whose type may only be an interface is named as the
        // language names interfaces (C#'s I{Name}).
        // Each type is looked for by the name its own language gives it.
        var element = claim.Source;
        var pattern = Convention(claim.Designer, element)?["name"]?.GetValue<string>();
        var kinds = TypeKinds(claim.Designer, element) is ["interface"] ? new[] { "interface", "type" } : ["type"];
        var nameIn = new Dictionary<JsonNode, string>(ReferenceEqualityComparer.Instance);
        string NameIn(JsonNode profile) => nameIn.TryGetValue(profile, out var known) ? known : nameIn[profile] = pattern is not null
            ? Naming.Fill(pattern, new Dictionary<string, string> { ["Name"] = element.Name })
            : Naming.InCode(profile, element.Name, kinds);
        var names = ReadProfiles().Select(NameIn).Distinct().ToList();
        var name = string.Join("` or `", names);
        // A location may name the element and its designer folders: src/{FolderPath}/*.ts.
        var folderPath = string.Join("/", Folders(element));
        var locations = (Convention(claim.Designer, element)?["locations"] as JsonArray)
            ?.Select(x => new GlobPattern(Naming.Fill(x!.GetValue<string>(), new Dictionary<string, string> { ["Name"] = element.Name, ["FolderPath"] = folderPath }).Replace("//", "/")))
            .ToList() ?? [];
        bool Located(TypeNode type) => locations.Count == 0 || type.Files.Any(f => locations.Any(g => g.IsMatch(f)));
        var candidates = TypesNamedIn(NameIn).Where(Located).ToList();

        // Designer folders become namespaces, or the folders the files are in: of several types with the name, the one
        // whose namespace or folder ends with most of the element's folders is it. A folder named as a namespace
        // (eShop10's User Interface folder HybridApp.Components.Layout) is each of its segments.
        var folders = Folders(element).SelectMany(x => x.Split('.', StringSplitOptions.RemoveEmptyEntries)).ToList();

        int Score(TypeNode type) => type.Files
            .Select(f => TrailingMatch(Path.GetDirectoryName(f)?.Split('/', '\\') ?? [], folders))
            .Append(TrailingMatch(type.Namespace.Split('.'), folders))
            .Max();

        // An element its conventions say may be a class or an interface (a Service, a Repository) with no type of its name
        // is the interface its language names it (C#'s IBasketRepository), which a class of another name implements
        // (RedisBasketRepository). With a type of its name, that is it, as the code's own classes are read first: the model
        // drawn from a class (DocumentSearchService) and one drawn from its interface (CatalogAI) both hold. Unless the
        // element's folders place that interface and not the type of its name: each feature's own IOrderData, in its own
        // Data namespace, beside a data layer's OrderData class elsewhere.
        var typeKinds = (Convention(claim.Designer, element)?["typeKinds"] as JsonArray)?.Select(x => x!.GetValue<string>()).ToList() ?? [];
        if (pattern is null && typeKinds.Count > 1 && typeKinds.Contains("interface"))
        {
            var contracts = TypesNamedIn(profile => Naming.InCode(profile, element.Name, ["interface", "type"]))
                .Where(x => x.Kind == "interface" && Located(x) && x.Name != NameIn(ProfileOf(x)))
                .ToList();
            if (candidates.Count == 0 || (contracts.Count > 0 && contracts.Max(Score) > candidates.Max(Score)))
            {
                candidates = contracts;
            }
        }

        // A Razor component and its code-behind are one component (R37), whatever namespace each part ends up in: the
        // component the .razor file declares is it.
        if (candidates.Count > 1 && candidates.SelectMany(x => x.Files).Select(x => x.EndsWith(".razor.cs", StringComparison.OrdinalIgnoreCase) ? x[..^3] : x).Distinct().Count() == 1
            && candidates.Where(x => x.Kind == "component").ToList() is [var component])
        {
            candidates = [component];
        }

        // A module is named as its file is, without the extension, so a file declaring a type of that name (OrderList.tsx's
        // OrderList component, beside its helpers) has two: the type it declares is the element's, not the file's loose code.
        if (candidates.Count > 1)
        {
            candidates = candidates.Where(x => x.Kind != "module" || !candidates.Any(y => y.Kind != "module" && y.Files.Intersect(x.Files).Any())).ToList();
        }

        // A generic type and the type of its name without type parameters are two types (Result<T> : Result): the element's
        // is the one with as many type parameters as it has generic types.
        if (candidates.Count > 1 && candidates.Where(x => x.TypeParameters.Count == (element.GenericTypes?.Count() ?? 0)).ToList() is { Count: > 0 } sameArity)
        {
            candidates = sameArity;
        }

        if (candidates.Count > 1)
        {
            var scored = candidates.Select(x => (Type: x, Score: Score(x))).ToList();
            var best = scored.Max(x => x.Score);
            var top = scored.Where(x => x.Score == best).Select(x => x.Type).ToList();

            // Projects repeat each other's folders (each service its own IntegrationEvents/Events, holding its copy of a
            // message): of the types its folders match equally, the one whose namespace or path names most of the element's
            // package's segments is it, as eShop's Catalog.API.Eventing package names src/Catalog.API. Equally when the folders
            // match none, as for an element at its package's root: ClientB's DTO ApiMessage in Acme.Api.DataContracts and its
            // Eventing DTO ApiMessage in Acme.Api.Commands.Internal, each in the project of its package's name.
            if (top.Count > 1)
            {
                var package = (element.Package?.Name ?? "").Split('.', StringSplitOptions.RemoveEmptyEntries);
                int Named(TypeNode type)
                {
                    var parts = type.Namespace.Split('.')
                        .Concat(type.Files.SelectMany(f => (Path.GetDirectoryName(f) ?? "").Split('/', '\\', '.')))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    return package.Count(parts.Contains);
                }

                var named = top.Select(x => (Type: x, Score: Named(x))).ToList();
                var most = named.Max(x => x.Score);
                if (most > 0 && named.Count(x => x.Score == most) == 1)
                {
                    top = [named.First(x => x.Score == most).Type];
                }
            }

            if (top.Count == 1)
            {
                candidates = top;
            }
        }

        // Two elements of one name each have their own type (a Domain entity, and the Services DTO of its name in a contracts
        // project): a type another element of the name is located at isn't this one's.
        if (candidates.Count > 1)
        {
            var taken = OthersLocated(claim, candidates);
            if (candidates.Count(x => !taken.Contains(x)) == 1)
            {
                candidates = candidates.Where(x => !taken.Contains(x)).ToList();
            }
        }

        switch (candidates.Count)
        {
            case 1:
                _types[claim] = candidates[0];
                Verify(claim, new Reading { Value = "present", From = "code", At = candidates[0].At });
                break;
            case 0 when !_files.Any(x => !x.Decision.In && OwnCode(x) && names.Any(stem => IsNamedFor(x.Path, stem)))
                         && LibraryUseOf(names, PlacedNamespaces(element)) is { } use:
                // A type the code uses and doesn't declare is a library's it references, as a message from a contracts
                // package is (IHandleMessages<Ping>): it exists, and what the model says of its shape can't be read. Not
                // when a use can name the code's own types where the model places it: a deleted entity its configuration
                // still names.
                _referenced.Add(claim);
                Verify(claim, new Reading { Value = "present", From = "code", At = use });
                break;
            case 0:
                Missing(claim, names, $"a {string.Join(" or ", TypeKinds(claim.Designer, element))} named `{name}`");
                // An element that states nothing but its name, where no in-scope code declares a type of the name at all, is
                // corrected by removing it: eShop10's User Interface held an empty Model Definition, ModelType, that no code
                // has, which v1 never checked. Not one with members, nor one whose name a type has outside its locations.
                if (claim is { Outcome: "contradicted", Reading: { } missing } && !element.ChildElements.Any() && TypesNamedIn(NameIn).Count == 0)
                {
                    claim.Reading = new Reading
                    {
                        Value = missing.Value, At = missing.At, Searched = missing.Searched, Detail = missing.Detail,
                        Corrections = [new ModelCorrection(ModelCorrection.RemoveElement, element.Id)]
                    };
                }

                break;
            default:
                Cant(claim, "ambiguous", new Reading { Candidates = candidates.Select(x => x.At).ToList() });
                break;
        }
    }

    /// <summary>
    /// The <paramref name="candidates"/> other type claims of the element's name are located at, each located first if it
    /// hasn't been. One already being located is skipped: two elements of one name that each wait on the other stay
    /// ambiguous.
    /// </summary>
    private HashSet<TypeNode> OthersLocated(Claim claim, IReadOnlyCollection<TypeNode> candidates)
    {
        var taken = new HashSet<TypeNode>();
        _locating.Add(claim);
        try
        {
            foreach (var other in _claims.Where(x => x.Kind == "type.exists" && x != claim && !_locating.Contains(x)
                                                     && string.Equals(x.Source?.Name, claim.Source.Name, StringComparison.Ordinal)).ToList())
            {
                Evaluate(other);
                if (_types.TryGetValue(other, out var type) && candidates.Contains(type))
                {
                    taken.Add(type);
                }
            }
        }
        finally
        {
            _locating.Remove(claim);
        }

        return taken;
    }

    /// <summary>
    /// The value written on the enum member: one the language's numbering would give isn't stated (decision 4). The claim
    /// waits on the enum; a literal that isn't there is its enum's <c>enum.literals</c> finding (or a pack's <c>member.exists</c>
    /// on the literal), so this one waits on that too.
    /// </summary>
    private void LiteralValue(Claim claim)
    {
        if (ClaimOn(claim.Designer, claim.Source, "member.exists") is { } exists)
        {
            Evaluate(exists);
            if (exists.Outcome != "verified")
            {
                claim.DependsOn = new ClaimRef(exists.Designer, exists.Element, exists.Kind);
                Cant(claim, "blocked", new Reading { Detail = $"Waits on {exists.Kind}" });
                return;
            }
        }

        var type = OwnType(claim);
        if (type.Members.FirstOrDefault(x => x.Kind == "enumMember" && x.Name == claim.Source.Name) is not { } literal)
        {
            if (Owner(claim.Source) is { } owner && ClaimOn(claim.Designer, owner, "enum.literals") is { } literals)
            {
                Evaluate(literals);
                if (literals.Outcome != "verified")
                {
                    claim.DependsOn = new ClaimRef(literals.Designer, literals.Element, literals.Kind);
                    Cant(claim, "blocked", new Reading { Detail = $"Waits on {literals.Kind}" });
                    return;
                }
            }

            Cant(claim, "notFound", new Reading { At = type.At, Detail = $"`{type.Name}` has no member named `{claim.Source.Name}`." });
            return;
        }

        // Nothing is read, and it isn't from silence: a whenSet kind never takes a value the code doesn't write.
        if (literal.Value is null)
        {
            claim.Outcome = "contradicted";
            claim.Reading = new Reading { At = literal.At };
            return;
        }

        // A string member (Python's TODO = "todo", TypeScript's Todo = "todo") is its string, which the model may write with or
        // without its quotes; a number is compared as one.
        if (literal.Value.Literal is { Kind: "string", Value: { } text })
        {
            var expected = claim.Expected.Trim();
            if (expected.Length >= 2 && expected[0] is '"' or '\'' && expected[^1] == expected[0])
            {
                expected = expected[1..^1];
            }

            Compare(claim, text, literal.At, "code", expected == text);
            return;
        }

        Compare(claim, literal.Value.Text, literal.At, "code", Same(claim, literal.Value.Text));
    }

    /// <summary>
    /// The model's literals, in model order, against the enum's members: each must be there and in that order, and the code
    /// may have more (<c>subsequence</c>). One finding names what is missing or out of order. The literals it finds are
    /// located, so the <c>unmodelled.member</c> rule reports only the code's others.
    /// </summary>
    private void EnumLiterals(Claim claim)
    {
        var type = OwnType(claim);
        var members = type.Members.Where(x => x.Kind == "enumMember").ToList();
        var model = Ordered(claim.Expected);
        _located[claim] = members.Where(x => model.Contains(x.Name)).ToList();
        var actual = string.Join(", ", members.Select(x => x.Name));
        Compare(claim, actual.Length > 0 ? actual : "none", type.At, "code", Same(claim, actual));
        if (claim.Outcome != "contradicted")
        {
            return;
        }

        var missing = model.Where(x => members.All(m => m.Name != x)).ToList();
        var what = missing.Count > 0
            ? $"the code has no {string.Join(", ", missing.Select(x => $"`{x}`"))}"
            : "the code has them all, in another order";
        claim.Reading = new Reading
        {
            Value = claim.Reading!.Value, From = claim.Reading.From, At = claim.Reading.At,
            Message = $"`{Display(claim)}` {Label(claim)}: {what} (model `{claim.Expected}`, code `{claim.Reading.Value}`, {type.At}). The model states it in {claim.SourceText}."
        };
    }

    /// <summary>
    /// A file the model names (the <c>File</c> binding, such as an Implementation stereotype's Owned File) is in the code: the
    /// file of that path, or one whose path ends with it, as a path from a front end's own root does. A file the scope leaves
    /// out is out of scope, never missing.
    /// </summary>
    private void FileExists(Claim claim)
    {
        if (claim.Bindings?.GetValueOrDefault("File") is not { Length: > 0 } named)
        {
            Cant(claim, "unresolved", new Reading { Detail = "Its vocabulary names no file: give the entry a File binding." });
            return;
        }

        var path = Regex.Replace(named.Trim().Replace('\\', '/'), @"^(\./)+|^/+", "");
        var file = _files.Where(x => x.Path == path || x.Path.EndsWith("/" + path, StringComparison.Ordinal))
            .OrderByDescending(x => x.Decision.In).ThenBy(x => x.Path.Length).FirstOrDefault();
        Apply(claim, file.Path is null
            ? ClaimReading.Missing("the code", $"`{path}`")
            : file.Decision.In
                ? ClaimReading.Found(file.Path, new Span(file.Path, 1))
                : ClaimReading.OutOfScope(file.Path));
    }

    /// <summary>
    /// A named marker, both ways (the discriminator, else the <c>Marker</c> binding): the type has it when one of its
    /// supertypes, through its in-scope bases, or one of its annotations is named for it. Without it the code states
    /// <c>false</c> by saying nothing. A module's claim reader reads a marker the code states some other way.
    /// </summary>
    private void Marker(Claim claim)
    {
        var type = OwnType(claim);
        if ((claim.Discriminator ?? claim.Bindings?.GetValueOrDefault("Marker")) is not { Length: > 0 } marker)
        {
            Cant(claim, "unresolved", new Reading { At = type.At, Detail = "Its vocabulary names no marker: give the entry a discriminator naming it." });
            return;
        }

        var annotation = type.Attributes.FirstOrDefault(x => x.Name == marker || x.Name == AttributeName(marker));
        var present = annotation is not null || _graph.Supertypes(type).Any(x => x == marker || x.EndsWith("." + marker, StringComparison.Ordinal));
        var value = present ? "true" : "false";
        Compare(claim, value, annotation?.At ?? type.At, present ? "code" : "absent", Same(claim, value));
    }

    private static string AttributeName(string name) => name.EndsWith("Attribute", StringComparison.Ordinal) ? name[..^"Attribute".Length] : name;

    /// <summary>
    /// A modelled parameter, found by name in its operation's method (rules.md, Locating an element's code): its type, its
    /// nullability, or its default as written. A parameter the method doesn't have is <c>notFound</c>: that it is missing is
    /// <c>operation.parameters</c>' contradiction.
    /// </summary>
    private void Parameter(Claim claim)
    {
        if (LocatedMember(claim) is not { } member)
        {
            Cant(claim, "unresolved", new Reading { Detail = $"No claim locates the method that takes {claim.Source.Name}." });
            return;
        }

        var name = claim.Source.Name.TrimStart('@');
        var named = Naming.InCode(ProfileOf(member), name, ["parameter"]);
        ParameterNode? Named(MemberNode declared) =>
            declared.Parameters.FirstOrDefault(x => x.Name.TrimStart('@') == name) ?? declared.Parameters.FirstOrDefault(x => x.Name == named);
        // A class may rename the parameters its interface declares, as operation.parameters allows.
        if ((Named(member) ?? Counterparts(member).Select(Named).FirstOrDefault(x => x is not null)) is not { } parameter)
        {
            // A parameter that isn't there is its parameter.type claim's one finding, else its operation's parameters'; its
            // nullability and default wait on it.
            if (claim.Kind != "parameter.type" && (ClaimOn(claim.Designer, claim.Source, "parameter.type")
                                                   ?? (Owner(claim.Source) is IElement operation ? ClaimOn(claim.Designer, operation, "operation.parameters") : null)) is { } type)
            {
                claim.DependsOn = new ClaimRef(type.Designer, type.Element, type.Kind);
                Cant(claim, "blocked", new Reading { Detail = $"Waits on {type.Kind}" });
                return;
            }

            Cant(claim, "notFound", new Reading { At = member.At, Detail = $"`{member.Name}` has no parameter named `{name}`." });
            return;
        }

        switch (claim.Kind)
        {
            case "parameter.type":
                if (parameter.Type is null && MayLeaveTypesOut(parameter.At.File))
                {
                    Untyped(claim, parameter.At, $"`{member.Name}`'s `{parameter.Name}`");
                    break;
                }

                if (parameter.Type is null || claim.ExpectedValue is not ITypeReference model)
                {
                    Cant(claim, "notFound", new Reading { At = parameter.At, Detail = $"`{member.Name}`'s `{parameter.Name}` has no type." });
                    break;
                }

                Compare(claim, parameter.Type.Text, parameter.At, "code", TypeMatches(model, parameter.Type));
                break;
            case "parameter.nullable":
                if (_graph.UngovernedFiles.TryGetValue(parameter.At.File, out var project))
                {
                    Cant(claim, "readerFailed", new Reading { At = parameter.At, Detail = $"{project}, which sets {parameter.At.File}'s nullable context, can't be read." });
                    break;
                }

                if (parameter.Type is null && MayLeaveTypesOut(parameter.At.File))
                {
                    Untyped(claim, parameter.At, $"`{member.Name}`'s `{parameter.Name}`");
                    break;
                }

                if (parameter.Type?.Nullable is null or "unknown")
                {
                    Cant(claim, "unresolved", new Reading { At = parameter.At, Value = "unknown", Detail = $"{parameter.At.File} doesn't state whether `{parameter.Type?.Text}` is nullable." });
                    break;
                }

                Compare(claim, parameter.Type.Nullable, parameter.At, "code", parameter.Type.Nullable == claim.Expected);
                break;
            default:
                // A default the code doesn't write isn't stated, whatever a binder would supply (decision 4).
                if (parameter.Default is null)
                {
                    claim.Outcome = "contradicted";
                    claim.Reading = new Reading { At = parameter.At };
                    break;
                }

                Compare(claim, parameter.Default, parameter.At, "code", Same(claim, parameter.Default));
                break;
        }
    }

    private void MemberExists(Claim claim)
    {
        if (Dependency(claim) is not { } ownerClaim || !_types.TryGetValue(ownerClaim, out var owner))
        {
            Cant(claim, "unresolved", new Reading { Detail = $"No claim locates the type that owns {claim.Source.Name}, so its members can't be found." });
            return;
        }

        var element = claim.Source;
        var parameters = Parameters(element).Select(x => x.Name).ToList();
        MemberNode? member;
        string searched;
        if (element.SpecializationType.EndsWith("Constructor", StringComparison.Ordinal))
        {
            member = Overload(owner.Members.Where(x => x.Kind == "constructor").ToList(), element);
            searched = $"a constructor of `{owner.Name}` taking ({string.Join(", ", parameters)})";
        }
        else if (element.SpecializationType.EndsWith("Operation", StringComparison.Ordinal))
        {
            // A designer's own operations are methods too: the Java spike's Proxy Operation.
            var name = Naming.InCode(ProfileOf(owner), element.Name, ["method"]);
            var methods = _graph.MembersOf(owner).Where(x => x.Kind == "method" && x.Name == name).ToList();
            member = methods.Count == 1 ? methods[0] : Overload(methods, element);
            searched = $"a method named `{name}` in `{owner.Name}`";
        }
        else if (owner.Kind == "enum")
        {
            member = owner.Members.FirstOrDefault(x => x.Name == element.Name && x.Kind == "enumMember");
            searched = $"a member named `{element.Name}` in `{owner.Name}`";
        }
        else
        {
            // Java names a field after the model's attribute in camel case; C# writes it as modelled.
            var property = Naming.InCode(ProfileOf(owner), element.Name, ["property"]);
            var field = Naming.InCode(ProfileOf(owner), element.Name, ["field"]);
            member = _graph.MembersOf(owner).FirstOrDefault(x => x is { Kind: "property" } && x.Name == property || x is { Kind: "field" } && x.Name == field);
            searched = property == field ? $"a property named `{property}` in `{owner.Name}`" : $"a property `{property}` or a field `{field}` in `{owner.Name}`";

            // A static attribute is a constant, which a language's naming leaves as modelled (Java's MAXIMUM_QUANTITY, not
            // its camel case mAXIMUM_QUANTITY).
            if (member is null && element.IsStatic && field != element.Name)
            {
                member = _graph.MembersOf(owner).FirstOrDefault(x => x is { Kind: "field" or "property", IsStatic: true } && x.Name == element.Name);
                searched += $", or a static field `{element.Name}`";
            }

            // An attribute persisted from a private field, with no property of its name, is the field its member.backingField
            // claim names (rules.md, Locating an element's code).
            if (member is null && ClaimOn(claim.Designer, element, "member.backingField") is { Expected.Length: > 0 } backing)
            {
                member = _graph.MembersOf(owner).FirstOrDefault(x => x is { Kind: "field" } && x.Name == backing.Expected);
                searched += $", or the field `{backing.Expected}` its backing field names";
            }
        }

        // A member the code lacks belongs in its owner: the finding links the owner's declaration.
        if (member is null)
        {
            Missing(claim, [owner.Name], searched, owner.At, owner);
            return;
        }

        _members[claim] = member;
        Verify(claim, new Reading { Value = "present", From = "code", At = member.At });
    }

    /// <summary>
    /// Overloads are told apart by parameter names, then by their types; with no names matching, by their types alone, since
    /// a class may rename the parameters its interface declares (AutoMapper's <c>rows</c> for <c>IAutoMapper</c>'s <c>list</c>),
    /// then by count (rules.md, Locating an element's code). Injected parameters, such as a CancellationToken, aren't compared.
    /// </summary>
    private MemberNode? Overload(IReadOnlyList<MemberNode> candidates, IElement element)
    {
        var parameters = Parameters(element);
        bool Typed(MemberNode member) => Stated(member) is var stated && stated.Count == parameters.Count
            && stated.Zip(parameters).All(p => p.First.Type is { } code && p.Second.TypeReference is { Element: not null } model && TypeMatches(model, code));

        var named = candidates.Where(x => Stated(x).Select(p => p.Name).SequenceEqual(parameters.Select(p => p.Name), StringComparer.OrdinalIgnoreCase)).ToList();
        return named.FirstOrDefault(Typed)
               ?? named.FirstOrDefault()
               ?? candidates.FirstOrDefault(Typed)
               ?? candidates.FirstOrDefault(x => Stated(x).Count == parameters.Count);
    }

    /// <summary>
    /// A return type without the profile's async wrapper (rules.md, Language profiles): <c>Task&lt;T&gt;</c> is <c>T</c>, and
    /// a <c>void</c> wrapper without its argument, <c>Task</c>, is null, as no result. So is a type the profile's <c>void</c>
    /// names, Python's <c>-&gt; None</c>.
    /// </summary>
    private TypeRefNode? Unwrapped(MemberNode member)
    {
        var type = member.Type;
        if (type is not null && (ProfileOf(member)["void"] as JsonArray ?? []).Any(x => x?.GetValue<string>() == type.Text))
        {
            return null;
        }

        foreach (var wrapper in ProfileOf(member)["async"] as JsonArray ?? [])
        {
            if (type is null || wrapper?["wrapper"]?.GetValue<string>().Split('<')[0] != type.Name)
            {
                continue;
            }

            switch (type.Arguments.Count)
            {
                case 1:
                    return type.Arguments[0];
                case 0 when wrapper["void"]?.GetValue<bool>() == true:
                    return null;
            }
        }

        return type;
    }

    /// <summary>A method's parameters, less injected ones: those the framework supplies (rules.md, Locating an element's code).</summary>
    private IReadOnlyList<ParameterNode> Stated(MemberNode member)
    {
        var injected = ProfileOf(member)["injectedParameters"]?.AsArray().Select(x => x!.GetValue<string>()).ToHashSet() ?? [];
        return member.Parameters.Where(x => x.Type is null || !injected.Contains(x.Type.Name)).ToList();
    }

    /// <summary>
    /// Nothing in scope: contradicted, unless an out-of-scope file is named for it, which makes it can't verify (R25). That
    /// is a file an Out row excludes or one no row includes; never a built-in exclusion (bin, obj, ...), which is junk by
    /// definition. Only the file's path is looked at, never its content.
    /// </summary>
    /// <summary>
    /// Where the code uses a type of one of the names, in a base type, a member's or parameter's type, a creation or a call's
    /// type argument, nested type arguments included.
    /// </summary>
    /// <summary>The designer folders holding an element, outermost first.</summary>
    private static List<string> Folders(IElement element)
    {
        var folders = new List<string>();
        for (var parent = element.ParentElement; parent is { SpecializationType: "Folder" }; parent = parent.ParentElement)
        {
            folders.Insert(0, parent.Name);
        }

        return folders;
    }

    /// <summary>
    /// The in-scope namespaces where the model places the element: those ending with the last two segments of its package's
    /// and folders' names (all, when fewer). A reverse-engineered model files a contracts package's messages under folders
    /// named for their namespace, which no in-scope code declares; a deleted entity's siblings still live in its own.
    /// </summary>
    private HashSet<string> PlacedNamespaces(IElement element)
    {
        var place = (element.Package?.Name ?? "").Split('.', StringSplitOptions.RemoveEmptyEntries).ToList();
        var folders = new List<string>();
        for (var parent = element.ParentElement; parent is not null; parent = parent.ParentElement)
        {
            if (parent.SpecializationType == "Folder")
            {
                folders.InsertRange(0, parent.Name.Split('.', StringSplitOptions.RemoveEmptyEntries));
            }
        }

        var tail = place.Concat(folders).TakeLast(2).ToList();
        return tail.Count == 0
            ? []
            : _graph.Types.Select(x => x.Namespace)
                .Where(x => x.Split('.') is { } segments && segments.Length >= tail.Count && segments.TakeLast(tail.Count).SequenceEqual(tail, StringComparer.OrdinalIgnoreCase))
                .ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>
    /// Where the code uses a library's type of one of the names: in a base type, a member's or parameter's type, a creation or
    /// a call's type argument, nested type arguments included. Null when it uses none, or when a use can name a type in
    /// <paramref name="placed"/>, where the code's own types live where the model places the element: that use names the
    /// code's own type, deleted, as a configuration importing its entity's namespace still names a removed entity. A use that
    /// can't is a library's, whatever namespaces the code's own types repeat: local copies of some of a contracts package's
    /// messages in <c>Acme.Finance.Contracts.Internal.Commands.Workflow</c>, beside handlers importing the package's
    /// <c>Acme.Internal.Commands.Workflow</c>.
    /// </summary>
    private Span? LibraryUseOf(IReadOnlyCollection<string> names, IReadOnlySet<string> placed)
    {
        IEnumerable<TypeRefNode> Named(TypeRefNode? type) =>
            type is null ? [] : (names.Contains(type.Name) ? [type] : Enumerable.Empty<TypeRefNode>()).Concat(type.Arguments.SelectMany(Named));

        IEnumerable<(TypeRefNode Type, IReadOnlyList<string> Files, Span At)> Uses(TypeNode type) =>
            type.BaseTypes.SelectMany(Named).Select(x => (x, (IReadOnlyList<string>)type.Files, type.At))
                .Concat(type.Members.SelectMany(member => Named(member.Type)
                    .Concat(member.Parameters.SelectMany(p => Named(p.Type)))
                    .Concat(member.Creations.SelectMany(c => Named(c.Type)))
                    .Concat(member.Calls.SelectMany(c => c.TypeArguments.SelectMany(Named)))
                    .Select(x => (x, (IReadOnlyList<string>)[member.At.File], member.At))));

        Span? first = null;
        foreach (var (type, files, at) in _graph.Types.Where(x => MayUseLibrary(x.At.File)).SelectMany(Uses))
        {
            if (placed.Count == 0)
            {
                return at;
            }

            if (files.Any(file => CanName(file, type, placed)))
            {
                return null;
            }

            first ??= at;
        }

        return first;
    }

    /// <summary>
    /// The use of <paramref name="type"/> in <paramref name="path"/> can name a type of its name in one of the namespaces.
    /// Qualified, the namespace qualifying it is one: what an alias names, or the qualifier inside a namespace the file
    /// declares or one around it, or the global one, as C# looks its first name up (<c>Internal.Commands.Workflow.X</c> in
    /// <c>Acme.Finance.Services</c> is <c>Acme.Internal.Commands.Workflow.X</c>). Unqualified, the file imports one,
    /// declares it or is inside it, or imports the type itself (Java's single-type import). A file the graph doesn't hold
    /// can, as nothing says otherwise.
    /// </summary>
    private bool CanName(string path, TypeRefNode type, IReadOnlySet<string> namespaces)
    {
        if (_graph.Files.FirstOrDefault(x => x.Path == path) is not { } file)
        {
            return true;
        }

        var imports = _graph.ImportsOf(file).ToList();
        if (type.QualifiedName.EndsWith("." + type.Name, StringComparison.Ordinal))
        {
            var qualifier = type.QualifiedName[..^(type.Name.Length + 1)];
            var head = qualifier.Split('.')[0];
            if (imports.FirstOrDefault(x => x.Alias == head) is { } aliased)
            {
                return namespaces.Contains(aliased.Target + qualifier[head.Length..]);
            }

            static IEnumerable<string> Around(string ns)
            {
                for (var at = ns.Length; at > 0; at = ns.LastIndexOf('.', at - 1))
                {
                    yield return ns[..at];
                }
            }

            return file.Namespaces.SelectMany(Around).Select(x => $"{x}.{qualifier}").Append(qualifier).Any(namespaces.Contains);
        }

        return namespaces.Any(x => _graph.Reaches(file, x) || imports.Any(i => !i.Static && i.Target == $"{x}.{type.Name}"));
    }

    /// <summary>
    /// The file can use a library's type: its language reaches one without an import, or it imports a file the engine
    /// doesn't read (<see cref="ILanguage.LibrariesNeedImports"/>).
    /// </summary>
    private bool MayUseLibrary(string file) =>
        LanguageOf(file) is not { LibrariesNeedImports: true } || _graph.Files.FirstOrDefault(x => x.Path == file) is not { } node || node.Imports.Any(x => x.File is null);

    private void Missing(Claim claim, string fileStem, string searchedFor, Span? at = null) => Missing(claim, [fileStem], searchedFor, at);

    /// <summary>
    /// An out-of-scope file is the application's own code, so a lookup may name it as holding a claim's code: one a scope row
    /// excludes, or one no row covers in a project the in-scope code builds with (an in-scope project, or one it references,
    /// as a data project does its DTO project). A file in a project nothing in scope references is another application's,
    /// since several applications over one output folder each keep their own copies (eShop's WebApp and Catalog.API both
    /// declare `CatalogBrand`), and naming it turned drift into "can't be verified". Where no language says which projects
    /// the code builds with (<see cref="CodeGraph.ProjectReferences"/> is empty), a file no row covers is named.
    /// </summary>
    private bool OwnCode((string Path, ScopeDecision Decision) file)
    {
        if (file.Decision.Row is not null || _graph.ProjectReferences.Count == 0)
        {
            return true;
        }

        return _graph.ProjectReferences.Keys.Concat(_graph.ProjectReferences.Values.SelectMany(x => x))
            .Select(x => Path.GetDirectoryName(x)?.Replace('\\', '/') ?? "")
            .Any(folder => folder.Length == 0 || file.Path.StartsWith(folder + "/", StringComparison.Ordinal));
    }

    /// <param name="at">Where the missing code belongs, when that is known: a member's owner.</param>
    /// <param name="owner">
    /// The in-scope type a missing member belongs in. Only another part of a partial type can hold it, so for any other
    /// type no out-of-scope file is looked for: a file of its name there is another type's, such as another application's
    /// over the same output folder.
    /// </param>
    private void Missing(Claim claim, IReadOnlyList<string> fileStems, string searchedFor, Span? at = null, TypeNode? owner = null)
    {
        var shadow = owner is { IsPartial: false } ? default
            : _files.FirstOrDefault(x => !x.Decision.In && OwnCode(x) && fileStems.Any(stem => IsNamedFor(x.Path, stem)));
        if (shadow.Path is not null)
        {
            Cant(claim, "outOfScope", new Reading { ShadowFile = shadow.Path, ScopeRow = shadow.Decision.Row?.Id, Detail = searchedFor });
            return;
        }

        claim.Outcome = "contradicted";
        claim.Reading = new Reading { Value = "absent", At = at, Searched = [searchedFor], Detail = $"{_files.Count(x => x.Decision.In && LanguageOf(x.Path) is not null)} in-scope files" };
    }

    /// <summary>How many of <paramref name="folders"/>' last names end <paramref name="ns"/>, compared case-insensitively.</summary>
    private static int TrailingMatch(IReadOnlyList<string> ns, IReadOnlyList<string> folders)
    {
        var count = 0;
        while (count < ns.Count && count < folders.Count && string.Equals(ns[^(count + 1)], folders[^(count + 1)].Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
        {
            count++;
        }

        return count;
    }

    /// <summary>A source file named for the type: one a language reads, whose name is the type's and an extension.</summary>
    private bool IsNamedFor(string path, string stem) => Path.GetFileName(path).StartsWith(stem + ".", StringComparison.Ordinal) && LanguageOf(path) is not null;

    /// <summary>A rule's reading made the claim's outcome: a value read is compared by the kind's comparer.</summary>
    private void Apply(Claim claim, ClaimReading reading)
    {
        // A value read where the reader couldn't parse the code cleanly may not be what was meant (code-reader.md, Failures).
        if (reading.From is not ("cantVerify" or "found" or "missing") && reading.At is { } read && ErrorAt(read) is { } error)
        {
            Cant(claim, "readerFailed", new Reading
            {
                At = read,
                Detail = $"{read.File} can't be parsed cleanly at line {error.At.Line} ({error.Message}), where the rule read it, so the value there may not be what was meant."
            });
            return;
        }

        switch (reading.From)
        {
            case "cantVerify":
                Cant(claim, reading.Reason!, new Reading { At = reading.At, Detail = reading.Detail, Candidates = reading.Candidates });
                break;
            case "outOfScope":
                Cant(claim, "outOfScope", new Reading { ShadowFile = reading.Detail, ScopeRow = _files.FirstOrDefault(x => x.Path == reading.Detail).Decision?.Row?.Id });
                break;
            // A whenSet kind takes no value from silence: the code doesn't state it (R4).
            case "absent" when reading.Value is null && claim.Stated == "whenSet":
                claim.Outcome = "contradicted";
                claim.Reading = new Reading { At = reading.At };
                break;
            case "absent" when reading.Value is null:
                Compare(claim, "none", reading.At!, "absent", false);
                break;
            case "compared:same" or "compared:different":
                Compare(claim, reading.Value!, reading.At!, "code", reading.From == "compared:same");
                claim.Reading = new Reading { Value = Canonical(claim, reading.Value!), From = "code", At = reading.At, Message = reading.Message, Corrections = reading.Corrections };
                break;
            case "missing":
                claim.Outcome = "contradicted";
                claim.Reading = new Reading { Value = "none", From = "absent", Detail = reading.Detail, Searched = [reading.Searched!] };
                break;
            case "found":
                switch (reading.Located)
                {
                    case MemberNode member:
                        _members[claim] = member;
                        break;
                    case TypeNode type:
                        _types[claim] = type;
                        break;
                    case { } located:
                        _located[claim] = located;
                        break;
                }

                Verify(claim, new Reading { Value = "present", From = "code", At = reading.At });
                break;
            default:
                Compare(claim, Canonical(claim, reading.Value!), reading.At!, reading.From, Same(claim, reading.Value!));
                if (reading.Detail is not null || reading.Corrections.Count > 0)
                {
                    claim.Reading = new Reading
                    {
                        Value = claim.Reading!.Value, From = claim.Reading.From, At = claim.Reading.At, Detail = reading.Detail ?? claim.Reading.Detail, Corrections = reading.Corrections
                    };
                }

                break;
        }

        if (reading.Code.Count > 0 && claim.Reading is { } applied)
        {
            applied.Through = reading.Code;
        }

        // Compared, both sides are shown as canonical text.
        claim.Expected = Canonical(claim, claim.Expected);
    }

    private bool Same(Claim claim, string actual)
    {
        // A set whose items have a comparer of their own: each item the model states is one the code has, compared by it.
        if (claim.Definition is { Comparer: "subset" or "set", ItemComparer: { } name } && _comparers.TryGetValue(name, out var item))
        {
            var expected = Ordered(claim.Expected);
            var code = Ordered(actual);
            bool Within(List<string> items, List<string> of) => items.All(x => of.Any(y => item.Same(x, y)));
            return Within(expected, code) && (claim.Definition.Comparer == "subset" || Within(code, expected));
        }

        return _comparers.TryGetValue(claim.Definition.Comparer, out var comparer) ? comparer.Same(claim.Expected, actual) : actual == claim.Expected;
    }

    private Dictionary<string, IReadOnlyList<SyntaxError>>? _errors;

    /// <summary>A syntax error on the lines a span covers.</summary>
    private SyntaxError? ErrorAt(Span at)
    {
        _errors ??= _graph.Files.Where(x => x.Errors.Count > 0).GroupBy(x => x.Path).ToDictionary(x => x.Key, x => (IReadOnlyList<SyntaxError>)x.SelectMany(f => f.Errors).ToList());
        return _errors.TryGetValue(at.File, out var errors)
            ? errors.FirstOrDefault(x => x.At.Line <= (at.EndLine ?? at.Line) && (x.At.EndLine ?? x.At.Line) >= at.Line)
            : null;
    }

    /// <summary>A value's canonical text (claims-and-findings.md, Values): a set's items sorted, and as its comparer writes it.</summary>
    private string Canonical(Claim claim, string value)
    {
        if (claim.Definition.ValueType == "set" && value is not ("none" or ""))
        {
            // Each item as its own comparer writes it: /cart as the route cart.
            var item = claim.Definition.ItemComparer is { } name && _comparers.TryGetValue(name, out var found) ? found : null;
            value = string.Join(", ", value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(x => item?.Canonical(x) ?? x).Where(x => x.Length > 0).Order(StringComparer.Ordinal));
        }

        return _comparers.TryGetValue(claim.Definition.Comparer, out var comparer) ? comparer.Canonical(value) : value;
    }

    // ---- What a claim reader can ask (ClaimContext) ----

    internal TypeNode? LocatedType(Claim claim) => Dependency(claim) is { } dependency && _types.TryGetValue(dependency, out var type) ? type : null;

    /// <summary>The member the claim's dependency located, as the refiners give it for this claim.</summary>
    internal MemberNode? LocatedMember(Claim claim) =>
        _refined.TryGetValue(claim, out var refined) ? refined : Dependency(claim) is { } dependency && _members.TryGetValue(dependency, out var member) ? member : null;

    /// <summary>What the claim's dependencies located, nearest first, that is a <typeparamref name="T"/>.</summary>
    internal T? Located<T>(Claim claim) where T : class
    {
        if (_refined.TryGetValue(claim, out var refined) && refined is T own)
        {
            return own;
        }

        for (var current = Dependency(claim); current is not null; current = Dependency(current))
        {
            if (LocatedBy(current) is T found)
            {
                return found;
            }
        }

        return null;
    }

    private object? LocatedBy(Claim claim) =>
        _types.TryGetValue(claim, out var type) ? type : _members.TryGetValue(claim, out var member) ? member : _located.GetValueOrDefault(claim);

    internal TypeNode? TypeOf(string designer, IElement element) => Locate(designer, element, "type.exists") as TypeNode;

    internal MemberNode? MemberOf(string designer, IElement element) => Locate(designer, element, "member.exists") as MemberNode;

    /// <summary>What an element's existence claim of <paramref name="kind"/> located, evaluating it first; null when nothing did.</summary>
    internal object? Locate(string designer, IElement element, string kind)
    {
        if (ClaimOn(designer, element, kind) is not { } exists)
        {
            return null;
        }

        Evaluate(exists);
        return LocatedBy(exists);
    }

    internal bool TypeMatches(ITypeReference model, TypeRefNode code) => TypeComparer.Matches(model, code, ProfileOf(code));

    internal Claim? ClaimOn(string designer, IElement element, string kind, string? discriminator = null) =>
        _index.GetValueOrDefault(new ClaimRef(designer, element.Id, kind, discriminator));

    private TypeNode OwnType(Claim claim) => LocatedType(claim)!;

    private MemberNode OwnMember(Claim claim) => LocatedMember(claim)!;

    private static void Verify(Claim claim, Reading reading)
    {
        claim.Outcome = "verified";
        claim.Reading = reading;
    }

    /// <summary>The claim's outcome, with both sides as canonical text: whether they're the same is decided on them as read.</summary>
    private void Compare(Claim claim, string actual, Span at, string from, bool same)
    {
        claim.Outcome = same ? "verified" : "contradicted";
        claim.Expected = Canonical(claim, claim.Expected);
        claim.Reading = new Reading { Value = Canonical(claim, actual), From = from, At = at };
    }

    private static void Cant(Claim claim, string reason, Reading reading)
    {
        claim.Outcome = "cantVerify";
        claim.Reason = reason;
        claim.Reading = reading;
    }

    // ---- Findings (P6) ----

    /// <summary>
    /// What a finding calls the claim's element. A member, or a member's parameter, is named with its owner: Order.Total,
    /// Orders.Get.id. A link that starts at another link's end is named with that end's owner: a subscription's handler.
    /// </summary>
    private string Display(Claim claim) =>
        Owner(claim.Source) is { } parent && OnMember(claim)
            ? $"{(parent is IAssociationEnd && Owner(parent) is { } holder ? holder.Name : NameOf(parent))}.{NameOf(claim.Source)}"
            : NameOf(claim.Source);

    /// <summary>What a finding calls the value; one of several claims of its kind says which: `Customer` (IX_Customer_Email) columns.</summary>
    private static string Label(Claim claim) => claim.Discriminator is { } key ? $"({key}) {claim.Definition.Label}" : claim.Definition.Label;

    private Finding Report(Claim claim)
    {
        var display = Display(claim);
        var label = Label(claim);
        var reading = claim.Reading!;
        // What disagrees and where the model states it. Where the code states it is the finding's file link, so the
        // message doesn't repeat it; how the engine searched and the claims a missing element blocks are its own business.
        var message = (claim.Outcome, claim.Reason) switch
        {
            ("contradicted", _) when reading.Message is not null => reading.Message,
            // A navigation's type and nullability state its association end's multiplicity, so that is what is said to differ.
            ("contradicted", _) when claim.Source is IAssociationEnd { TypeReference: { } end } && EndMultiplicity(claim, end, reading) is { } code =>
                $"`{display}` multiplicity: model `{TypeComparer.Multiplicity(end)}`, code {code}. The model states it in the association end's multiplicity.",
            ("contradicted", _) when claim.Definition.Existence && reading.Searched is { Count: > 0 } =>
                $"`{display}` isn't in the code.",
            // Something there or not: "in the model, not in the code", not "model `present`, code `absent`".
            ("contradicted", _) when claim.Definition.ValueType == "presence" =>
                claim.Expected == "absent"
                    ? $"`{display}` {label}: in the code, not in the model. The model would state it in {claim.SourceText}."
                    : $"`{display}` {label}: in the model, not in the code. The model states it in {claim.SourceText}.",
            // Silence that states a value (R3): no IsUnique() is an index that isn't unique.
            ("contradicted", _) when reading is { From: "absent", Value: not (null or "none") } =>
                $"`{display}` {label}: model `{claim.Expected}`, code `{reading.Value}`, which it states by saying nothing. The model states it in {claim.SourceText}.",
            ("contradicted", _) when reading.From == "absent" || reading is { From: null, Value: null } =>
                $"`{display}` {label}: model `{claim.Expected}`, the code doesn't state it. The model states it in {claim.SourceText}.",
            ("contradicted", _) when reading.From == "default" =>
                $"`{display}` {label}: model `{claim.Expected}`, the code doesn't state it{(reading.Detail is { } unstated ? $" ({unstated})" : "")}, so it is `{reading.Value}`. The model states it in {claim.SourceText}.",
            ("contradicted", _) =>
                $"`{display}` {label}: model `{claim.Expected}`, code `{reading.Value}`. The model states it in {claim.SourceText}.",
            (_, "ambiguous") =>
                $"`{display}` {label} can't be verified: {reading.Candidates!.Count} {(claim.Definition.Existence ? "in-scope types match" : "places in the in-scope code state it")}, at {string.Join(", ", reading.Candidates)}.",
            (_, "outOfScope") when reading.ShadowFile is not null && reading.ScopeRow is not null =>
                $"`{display}` {label} can't be verified: {reading.ShadowFile} may hold it, but scope row {ScopeRowText(reading.ScopeRow)} excludes that file, so it isn't read.",
            (_, "outOfScope") when reading.ShadowFile is not null =>
                $"`{display}` {label} can't be verified: {reading.ShadowFile} may hold it, but no Drift Scope row includes that file, so it isn't read.",
            (_, "unresolved") =>
                $"`{display}` {label} can't be verified: {reading.Detail}",
            _ => $"`{display}` {label} can't be verified ({claim.Reason}). {reading.Detail}"
        };

        return new Finding($"claim/{claim.Kind}", $"{claim.Area} (DriftEngine)", "ModelCode", "error", message, new Anchor(Element: claim.Element), [claim.Kind])
        {
            Corrections = claim.Outcome == "contradicted" ? reading.Corrections : []
        };
    }

    /// <summary>
    /// The multiplicity a navigation's code states, when that is all a <c>member.type</c> or <c>member.nullable</c> claim on its
    /// association end disagrees on: a collection is <c>*</c>, an optional reference <c>0..1</c> and a required one <c>1</c>.
    /// Null for any other difference, such as another item type.
    /// </summary>
    private string? EndMultiplicity(Claim claim, ITypeReference end, Reading reading)
    {
        if (claim.Kind == "member.nullable")
        {
            return !end.IsCollection && reading.Value is "true" or "false" ? $"`{(reading.Value == "true" ? "0..1" : "1")}`" : null;
        }

        if (claim.Kind != "member.type" || LocatedMember(claim)?.Type is not { } type || !TypeComparer.MatchesButForCollection(end, type, ProfileOf(type)))
        {
            return null;
        }

        return TypeComparer.IsCollection(type, ProfileOf(type))
            ? $"`*` (`{type.Text}`)"
            : type.Nullable switch
            {
                "true" => $"`0..1` (`{type.Text}`)",
                "false" => $"`1` (`{type.Text}`)",
                _ => $"a single `{type.Text}`"
            };
    }

    /// <summary>
    /// Whether the claim is on a member or a member's parameter: <c>member.exists</c>, or a claim waiting on it, directly
    /// or through other claims on the same element (a house kind can wait on another, such as a column on its mapping).
    /// </summary>
    private bool OnMember(Claim claim)
    {
        for (Claim? current = claim; current is not null; current = current.Needs?.Element == current.Element ? Dependency(current) : null)
        {
            if (current.Kind == "member.exists" || current.Needs?.Kind == "member.exists")
            {
                return true;
            }
        }

        return false;
    }

    private string ScopeRowText(string id)
    {
        var row = _scope.Rows.First(x => x.Id == id);
        return $"{row.Number} ({string.Join(", ", row.Globs.Select(x => x.Pattern))}{(row.Reason is null ? "" : $": {row.Reason}")})";
    }
}
