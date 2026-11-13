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
public sealed class DriftEngine
{
    private const string CodebaseStructure = "Codebase Structure";

    private readonly DriftInput _input;
    private readonly IReadOnlyList<Pack> _packs;
    private readonly IReadOnlyList<IDriftExtension> _extensions;
    private readonly IReadOnlyDictionary<string, KindDefinition> _kinds;
    private readonly IReadOnlyDictionary<string, IValueComparer> _comparers;
    private readonly IReadOnlyList<(string Extension, ILanguage Language)> _languages;

    /// <summary>The profile of the language that read the most in-scope files: its type names, collections and type kinds.</summary>
    private JsonNode _profile = new JsonObject();
    private ScopeModel _scope = null!;
    private readonly List<(string Path, ScopeDecision Decision)> _files = [];
    private readonly List<Claim> _claims = [];
    private readonly Dictionary<ClaimRef, Claim> _index = [];
    private readonly Dictionary<Claim, TypeNode> _types = [];
    private readonly Dictionary<Claim, MemberNode> _members = [];

    /// <summary>What an extension's existence claim located that is neither a type nor a member: a route, a mapping.</summary>
    private readonly Dictionary<Claim, object> _located = [];
    private readonly List<(string Extension, string Error)> _extensionFailures = [];
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
        new ValueComparer("subset", (expected, actual) => Items(expected).IsSubsetOf(Items(actual)))
    ];

    private static HashSet<string> Items(string text) => text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToHashSet(StringComparer.Ordinal);

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

    public static RunResult Run(DriftInput input) => new DriftEngine(input).Run();

    private RunResult Run()
    {
        var findings = new List<Finding>();
        ReadCode();
        BuildClaims();
        foreach (var claim in _claims)
        {
            Evaluate(claim);
        }

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
            if (_claims.Where(NotStated).ToList() is { Count: > 0 } notStated)
            {
                findings.Add(NotStatedFinding(notStated));
            }

            findings.AddRange(Rules());
        }

        findings.AddRange(_graph.Failures.Select(x => new Finding("engine/readerFailed", ScopeArea, "Code", "error",
            $"The {LanguageOf(x.File)?.Title ?? "code"} reader can't read {x.File}: {x.Error} Claims that need it can't be verified.", new Anchor(File: x.File, Line: 1))));
        findings.AddRange(_extensionFailures.GroupBy(x => x.Extension).Select(x => new Finding("config/pluginFailed", ScopeArea, "Code", "error",
            $"The {x.Key} extension failed: {x.First().Error} Claims it reads can't be verified.", new Anchor(Text: x.Key))));
        return new RunResult(_claims, findings, Coverage(), _packs.Select(x => new PackRef(x.Id, x.Version)).ToList(), FilesRead());
    }

    private const string ScopeArea = "Drift Scope (DriftEngine)";

    /// <summary>The extensions' rules, over in-scope code. Their Areas are marked as the engine's, like its claims'.</summary>
    private List<Finding> Rules()
    {
        var findings = new List<Finding>();
        var context = new RuleContext(this, _graph, _codeScope, _claims);
        foreach (var (extension, rule) in _extensions.SelectMany(x => x.Rules.Select(r => (x.Id, r))))
        {
            try
            {
                findings.AddRange(rule.Check(context).Select(x => x with { Area = $"{x.Area} (DriftEngine)" }).ToList());
            }
            catch (Exception e)
            {
                _extensionFailures.Add((extension, $"{e.GetType().Name}: {e.Message}"));
            }
        }

        return findings;
    }

    /// <summary>
    /// A claim the code can't state in its language context: C# nullability outside a nullable context. It is can't
    /// verify, <c>unresolved</c>, like any fact the reader can't supply (R35), but neither side can be wrong, so it isn't
    /// drift: it has no finding of its own, and one <c>engine/notStated</c> note lists them all.
    /// </summary>
    private static bool NotStated(Claim claim) =>
        claim is { Reason: "unresolved", Reading: { Value: "unknown", At: not null } } && claim.Kind.EndsWith(".nullable", StringComparison.Ordinal);

    private static Finding NotStatedFinding(IReadOnlyList<Claim> claims)
    {
        var designers = claims.GroupBy(x => x.Designer).OrderBy(x => x.Key).Select(x => $"{x.Key} {x.Count()}");
        var folders = claims.Select(x => x.Reading!.At!.File).Distinct().GroupBy(x => x.Split('/')[0]).OrderBy(x => x.Key).Select(x => $"{x.Key} {x.Count()}");
        return new Finding("engine/notStated", ScopeArea, "Code", "info",
            $"Not checked: {claims.Count} nullability claims ({string.Join(", ", designers)}). Their code is outside a nullable context, so it doesn't state whether a reference type is nullable; turn nullable reference types on to check them. Files outside a nullable context: {string.Join(", ", folders)}.",
            new Anchor(Text: "nullable context"));
    }

    /// <summary>Rows that need attention, and the coverage listing (R15, R23, R24).</summary>
    private IEnumerable<Finding> ScopeFindings()
    {
        var listing = new List<string>();
        foreach (var row in _scope.Rows)
        {
            var subject = new Anchor(Element: row.Host);
            var what = row.Types.Count > 0 ? $"types {string.Join(", ", row.Types)}" : string.Join(", ", row.Globs.Select(x => x.Pattern));
            if (!row.In && string.IsNullOrWhiteSpace(row.Reason))
            {
                yield return new Finding("scope/reason", ScopeArea, "Code", "error",
                    $"Scope row {row.Number} ({what}) is Out with no reason. Say why this code isn't checked.", subject);
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
        }

        listing.AddRange(_files.Where(x => x.Decision.Builtin is not null).GroupBy(x => x.Decision.Builtin!)
            .Select(x => $"built in, {x.Key}, {x.Count()} files"));
        var unmatched = _files.Where(x => x.Decision is { Row: null, Builtin: null })
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

    private void ReadCode()
    {
        var all = Directory.EnumerateFiles(_input.OutputFolder, "*", SearchOption.AllDirectories)
            .Select(x => Path.GetRelativePath(_input.OutputFolder, x).Replace('\\', '/'))
            .ToList();
        _scope = ScopeModel.From(_input.Designers.GetValueOrDefault(CodebaseStructure), all);
        foreach (var path in all)
        {
            _files.Add((path, _scope.Decide(path)));
        }

        _codeScope = new CodeScope(_files.Select(x => (x.Path, x.Decision.In)));

        // Each language reads its in-scope files; a file no language reads isn't read.
        var read = new Dictionary<string, int>();
        foreach (var (extension, language) in _languages)
        {
            var files = _files.Where(x => x.Decision.In && LanguageOf(x.Path) == language).Select(x => x.Path).OrderBy(x => x, StringComparer.Ordinal).ToList();
            if (files.Count == 0)
            {
                continue;
            }

            try
            {
                language.Read(_input.OutputFolder, files, _graph);
                read[language.Name] = files.Count;
            }
            catch (Exception e)
            {
                _extensionFailures.Add((extension, $"{e.GetType().Name}: {e.Message}"));
            }
        }

        var primary = read.OrderByDescending(x => x.Value).Select(x => x.Key).Concat(_languages.Select(x => x.Language.Name)).FirstOrDefault();
        _profile = _packs.Select(x => primary is null ? null : x.Profile(primary)).LastOrDefault(x => x is not null) ?? new JsonObject();

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
                try
                {
                    reader.Read(_graph);
                }
                catch (Exception e)
                {
                    _extensionFailures.Add((extension.Id, $"{e.GetType().Name}: {e.Message}"));
                }
            }
        }
    }

    /// <summary>The language that reads a file; null when none does.</summary>
    private ILanguage? LanguageOf(string path) => _languages.Select(x => x.Language).FirstOrDefault(x => x.Reads(path));

    private IReadOnlyList<FileRead> FilesRead() =>
        _files.Where(x => x.Decision.In).Select(x => (x.Path, Language: LanguageOf(x.Path))).Where(x => x.Language is not null)
            .Select(x => new FileRead(x.Path, x.Language!.Name)).ToList();

    private Coverage Coverage()
    {
        var rows = _scope.Rows.Select(row => new CoverageRow(
            row.Host, row.Number, row.Globs.Select(x => x.Pattern).ToList(), row.Types.Count == 0 ? null : row.Types, row.In ? "In" : "Out", row.Reason,
            _files.Count(x => x.Decision.Row == row))).ToList();
        var builtin = _files.Where(x => x.Decision.Builtin is not null).GroupBy(x => x.Decision.Builtin!).ToDictionary(x => x.Key, x => x.Count());
        var unmatched = _files.Count(x => x.Decision is { Row: null, Builtin: null });
        return new Coverage(rows, builtin, unmatched == 0 ? new Dictionary<string, int>() : new Dictionary<string, int> { ["not in any row"] = unmatched });
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
                    var specialization = entry["for"]?["element"]?.GetValue<string>();
                    var association = entry["for"]?["associationEnd"]?.GetValue<string>();
                    var where = entry["for"]?["where"]?.GetValue<string>();
                    var selected = association is null
                        ? elements.Where(x => x.SpecializationType == specialization)
                        : NavigableEnds(elements, association);
                    foreach (var element in selected)
                    {
                        if (where is not null && Expression.Evaluate(where, element) is not true)
                        {
                            continue;
                        }

                        if (Build(designer, pack, entry, element) is { } claim)
                        {
                            Add(claim, replace: false);
                        }
                    }
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
                    _extensionFailures.Add((extension.Id, $"{e.GetType().Name}: {e.Message}"));
                }
            }
        }
    }

    private void Add(Claim claim, bool replace)
    {
        var key = new ClaimRef(claim.Designer, claim.Element, claim.Kind);
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
            if (definition.Stated == "whenSet")
            {
                return null;
            }

            expected = definition.ValueType switch { "bool" => "false", "set" => "none", "presence" => "present", _ => "" };
        }

        return new Claim
        {
            Kind = spec.Kind,
            Designer = spec.Designer,
            Element = spec.Element.Id,
            Path = PathOf(spec.Element),
            Expected = expected,
            Stated = definition.Stated,
            Origin = extension,
            Source = spec.Element,
            Definition = definition,
            Needs = spec.DependsOn,
            ExpectedValue = spec.Value,
            SourceText = spec.Source ?? spec.Kind,
            Area = spec.Area ?? spec.Designer
        };
    }

    private Claim? Build(string designer, Pack pack, JsonNode entry, IElement element)
    {
        var kind = entry["kind"]!.GetValue<string>();
        var definition = _kinds[kind];
        var value = entry["value"] is { } valueExpression ? Expression.Evaluate(valueExpression.GetValue<string>(), element) : null;
        string? expected = kind switch
        {
            "type.kind" => string.Join(", ", TypeKinds(designer, element)),
            "operation.parameters" => string.Join(", ", Parameters(element).Select(x => $"{x.Name}: {(x.TypeReference is { Element: not null } type ? Expression.TypeText(type) : "?")}")),
            "operation.returns" when value is null => "void",
            _ when definition.ValueType == "presence" => "present",
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
            Path = PathOf(element),
            Expected = expected,
            Stated = definition.Stated,
            Bindings = (entry["bindings"] as JsonObject)?.ToDictionary(x => x.Key, x => Expression.Text(Expression.Evaluate(x.Value!.GetValue<string>(), element)) ?? ""),
            Origin = pack.Id,
            Source = element,
            Definition = definition,
            Needs = Needs(designer, element, entry["dependsOn"]?.GetValue<string>()),
            ExpectedValue = value,
            SourceText = entry["source"]?.GetValue<string>() ?? kind,
            Area = pack.Data["vocabulary"]?[designer]?["area"]?.GetValue<string>() ?? designer
        };
    }

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

    private static IEnumerable<IElement> Descendants(IElement element) => new[] { element }.Concat(element.ChildElements.SelectMany(Descendants));

    /// <summary>
    /// The navigable ends of an association type: each is a member of the element at its other end (a navigation property),
    /// reached from that element's associated ends.
    /// </summary>
    private static IEnumerable<IElement> NavigableEnds(IEnumerable<IElement> elements, string association) =>
        elements.SelectMany(x => x.AssociatedElements ?? [])
            .Where(x => x.IsNavigable && x.Association?.SpecializationType == association)
            .GroupBy(x => x.Id)
            .Select(x => x.First());

    /// <summary>What an element is a member of: its parent, or for an association end, the element at the other end.</summary>
    internal static IElement? Owner(IElement element) =>
        element is IAssociationEnd end ? end.OtherEnd()?.TypeReference?.Element as IElement : element.ParentElement;

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

    private JsonNode? Convention(string designer, IElement element) => _packs.Select(x => x.TypeConvention(designer, element.SpecializationType)).LastOrDefault(x => x is not null);

    private IReadOnlyList<string> TypeKinds(string designer, IElement element) =>
        (Convention(designer, element)?["typeKinds"] as JsonArray)?.Select(x => x!.GetValue<string>()).ToList()
        ?? (_profile["typeKinds"] as JsonArray)?.Select(x => x!.GetValue<string>()).ToList()
        ?? [];

    // ---- Outcomes (block 3) ----

    private void Evaluate(Claim claim)
    {
        if (claim.Reading is not null || claim.Reason is not null)
        {
            return;
        }

        if (Dependency(claim) is { } dependency)
        {
            Evaluate(dependency);
            claim.DependsOn = new ClaimRef(dependency.Designer, dependency.Element, dependency.Kind);
            if (dependency.Outcome != "verified")
            {
                Cant(claim, "blocked", new Reading { Detail = $"Waits on {dependency.Kind}" });
                return;
            }
        }

        if (_scope.Rows.Count == 0)
        {
            Cant(claim, "outOfScope", new Reading { Detail = "No Drift Scope is set, so no file is in scope." });
            return;
        }

        // A pack's recipe for the kind replaces the built-in rule; of several, the last pack's wins, so a house pack
        // (loaded after the stock packs) overrides a stock pack's.
        if (_packs.Select(x => x.Recipe(claim.Designer, claim.Kind)).LastOrDefault(x => x is not null) is { } recipe)
        {
            Recipe(claim, recipe);
            return;
        }

        // An extension's claim reader comes before the engine's own structure rules, and a later extension's before an
        // earlier one's, so a module's reader can read any kind, the engine's included.
        foreach (var (extension, reader) in _extensions.SelectMany(x => x.ClaimReaders.Select(r => (x.Id, r))).Reverse())
        {
            if (!reader.Kinds.Contains(claim.Kind))
            {
                continue;
            }

            ClaimReading? reading;
            try
            {
                reading = reader.Read(new ClaimContext(this, claim, _graph, _codeScope));
            }
            catch (Exception e)
            {
                _extensionFailures.Add((extension, $"{e.GetType().Name}: {e.Message}"));
                Cant(claim, "ruleFailed", new Reading { Detail = $"The {extension} extension failed reading it." });
                return;
            }

            if (reading is not null)
            {
                Apply(claim, reading);
                return;
            }
        }

        switch (claim.Kind)
        {
            case "type.exists":
                TypeExists(claim);
                break;
            case "type.kind":
            {
                var type = OwnType(claim);
                Compare(claim, type.Kind, type.At, "code", claim.Expected.Split(", ").Contains(type.Kind));
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
                if (member.Type is null || claim.ExpectedValue is not ITypeReference model)
                {
                    Cant(claim, "notFound", new Reading { At = member.At, Detail = "The member has no type." });
                    break;
                }

                Compare(claim, member.Type.Text, member.At, "code", TypeComparer.Matches(model, member.Type, _profile));
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

                if (member.Type?.Nullable is null or "unknown")
                {
                    Cant(claim, "unresolved", new Reading { At = member.At, Value = "unknown", Detail = $"{member.At.File} is outside a nullable context, so a reference type's nullability is unknown." });
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
                // and Task is void.
                var member = OwnMember(claim);
                var returns = Unwrapped(member.Type);
                var actual = returns?.Text ?? "void";
                Compare(claim, member.Type?.Text ?? "void", member.At, "code", claim.ExpectedValue is ITypeReference model
                    ? returns is not null && returns.Text != "void" && TypeComparer.Matches(model, returns, _profile)
                    : actual == "void");
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
                var type = OwnType(claim);
                var supertypes = string.Join(", ", _graph.Supertypes(type));
                Compare(claim, supertypes.Length > 0 ? supertypes : "none", type.At, supertypes.Length > 0 ? "code" : "absent", Same(claim, supertypes));
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
            case "enum.literal.value":
                LiteralValue(claim);
                break;
            case "operation.parameters":
            {
                // The parameters in order, each its name and type; injected ones aren't part of the signature.
                var member = OwnMember(claim);
                var code = Stated(member);
                var model = Parameters(claim.Source);
                var same = code.Count == model.Count && code.Zip(model).All(x =>
                    x.First.Name == x.Second.Name && x.First.Type is { } type && x.Second.TypeReference is { Element: not null } modelType && TypeComparer.Matches(modelType, type, _profile));
                Compare(claim, string.Join(", ", code.Select(x => $"{x.Name}: {x.Type?.Text ?? "?"}")), member.At, "code", same);
                break;
            }
            default:
                Cant(claim, "noRule", new Reading { Detail = $"No active pack has a rule for {claim.Kind}." });
                break;
        }
    }

    private Claim? Dependency(Claim claim) => claim.Needs is { } needs ? _index.GetValueOrDefault(needs) : null;

    private void TypeExists(Claim claim)
    {
        // The conventions' name, else the profile's: an element whose type may only be an interface is named as the
        // language names interfaces (C#'s I{Name}).
        var element = claim.Source;
        var name = Convention(claim.Designer, element)?["name"]?.GetValue<string>() is { } pattern
            ? Naming.Fill(pattern, new Dictionary<string, string> { ["Name"] = element.Name })
            : Naming.InCode(_profile, element.Name, TypeKinds(claim.Designer, element) is ["interface"] ? ["interface", "type"] : ["type"]);
        var locations = (Convention(claim.Designer, element)?["locations"] as JsonArray)?.Select(x => new GlobPattern(x!.GetValue<string>())).ToList() ?? [];
        var candidates = _graph.Types
            .Where(x => x.Name == name && (locations.Count == 0 || x.Files.Any(f => locations.Any(g => g.IsMatch(f)))))
            .ToList();
        if (candidates.Count > 1)
        {
            // Designer folders become namespaces: of several types with the name, the one whose namespace ends with most of
            // the element's folders is it.
            var folders = new List<string>();
            for (var parent = element.ParentElement; parent is { SpecializationType: "Folder" }; parent = parent.ParentElement)
            {
                folders.Insert(0, parent.Name);
            }

            var scored = candidates.Select(x => (Type: x, Score: TrailingMatch(x.Namespace.Split('.'), folders))).ToList();
            var best = scored.Max(x => x.Score);
            if (best > 0 && scored.Count(x => x.Score == best) == 1)
            {
                candidates = [scored.First(x => x.Score == best).Type];
            }
        }

        switch (candidates.Count)
        {
            case 1:
                _types[claim] = candidates[0];
                Verify(claim, new Reading { Value = "present", From = "code", At = candidates[0].At });
                break;
            case 0:
                Missing(claim, name, $"a {string.Join(" or ", TypeKinds(claim.Designer, element))} named `{name}`");
                break;
            default:
                Cant(claim, "ambiguous", new Reading { Candidates = candidates.Select(x => x.At).ToList() });
                break;
        }
    }

    /// <summary>
    /// The value written on the enum member: one the language's numbering would give isn't stated (decision 4). The claim
    /// waits on the enum; a literal that isn't there is its <c>member.exists</c> claim's finding, so this one waits on that too.
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

        Compare(claim, literal.Value.Text, literal.At, "code", Same(claim, literal.Value.Text));
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
        else if (element.SpecializationType == "Operation")
        {
            var name = Naming.InCode(_profile, element.Name, ["method"]);
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
            var property = Naming.InCode(_profile, element.Name, ["property"]);
            var field = Naming.InCode(_profile, element.Name, ["field"]);
            member = _graph.MembersOf(owner).FirstOrDefault(x => x is { Kind: "property" } && x.Name == property || x is { Kind: "field" } && x.Name == field);
            searched = property == field ? $"a property named `{property}` in `{owner.Name}`" : $"a property `{property}` or a field `{field}` in `{owner.Name}`";
        }

        if (member is null)
        {
            Missing(claim, owner.Name, searched);
            return;
        }

        _members[claim] = member;
        Verify(claim, new Reading { Value = "present", From = "code", At = member.At });
    }

    /// <summary>
    /// Overloads are told apart by parameter names, then by their types, then by count (rules.md, Locating an element's
    /// code). Injected parameters, such as a CancellationToken, aren't compared.
    /// </summary>
    private MemberNode? Overload(IReadOnlyList<MemberNode> candidates, IElement element)
    {
        var parameters = Parameters(element);
        var named = candidates.Where(x => Stated(x).Select(p => p.Name).SequenceEqual(parameters.Select(p => p.Name), StringComparer.OrdinalIgnoreCase)).ToList();
        return named.FirstOrDefault(x => Stated(x).Zip(parameters).All(p => p.First.Type is { } code && p.Second.TypeReference is { Element: not null } model && TypeComparer.Matches(model, code, _profile)))
               ?? named.FirstOrDefault()
               ?? candidates.FirstOrDefault(x => Stated(x).Count == parameters.Count);
    }

    /// <summary>
    /// A return type without the profile's async wrapper (rules.md, Language profiles): <c>Task&lt;T&gt;</c> is <c>T</c>, and
    /// a <c>void</c> wrapper without its argument, <c>Task</c>, is null, as no result.
    /// </summary>
    private TypeRefNode? Unwrapped(TypeRefNode? type)
    {
        foreach (var wrapper in _profile["async"] as JsonArray ?? [])
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
        var injected = _profile["injectedParameters"]?.AsArray().Select(x => x!.GetValue<string>()).ToHashSet() ?? [];
        return member.Parameters.Where(x => x.Type is null || !injected.Contains(x.Type.Name)).ToList();
    }

    /// <summary>
    /// Nothing in scope: contradicted, unless an out-of-scope file is named for it, which makes it can't verify (R25). That
    /// is a file an Out row excludes or one no row includes; never a built-in exclusion (bin, obj, ...), which is junk by
    /// definition. Only the file's path is looked at, never its content.
    /// </summary>
    private void Missing(Claim claim, string fileStem, string searchedFor)
    {
        var shadow = _files.FirstOrDefault(x => !x.Decision.In && x.Decision.Builtin is null && IsNamedFor(x.Path, fileStem));
        if (shadow.Path is not null)
        {
            Cant(claim, "outOfScope", new Reading { ShadowFile = shadow.Path, ScopeRow = shadow.Decision.Row?.Id, Detail = searchedFor });
            return;
        }

        claim.Outcome = "contradicted";
        claim.Reading = new Reading { Value = "absent", Searched = [searchedFor], Detail = $"{_files.Count(x => x.Decision.In && LanguageOf(x.Path) is not null)} in-scope files" };
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
        switch (reading.From)
        {
            case "cantVerify":
                Cant(claim, reading.Reason!, new Reading { At = reading.At, Detail = reading.Detail, Candidates = reading.Candidates });
                break;
            case "absent" when reading.Value is null:
                Compare(claim, "none", reading.At!, "absent", false);
                break;
            case "compared:same" or "compared:different":
                Compare(claim, reading.Value!, reading.At!, "code", reading.From == "compared:same");
                claim.Reading = new Reading { Value = reading.Value, From = "code", At = reading.At, Message = reading.Message };
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
                Compare(claim, reading.Value!, reading.At!, reading.From, Same(claim, reading.Value!));
                break;
        }
    }

    private bool Same(Claim claim, string actual) =>
        _comparers.TryGetValue(claim.Definition.Comparer, out var comparer) ? comparer.Same(claim.Expected, actual) : actual == claim.Expected;

    // ---- What a claim reader can ask (ClaimContext) ----

    internal TypeNode? LocatedType(Claim claim) => Dependency(claim) is { } dependency && _types.TryGetValue(dependency, out var type) ? type : null;

    internal MemberNode? LocatedMember(Claim claim) => Dependency(claim) is { } dependency && _members.TryGetValue(dependency, out var member) ? member : null;

    /// <summary>What the claim's dependencies located, nearest first, that is a <typeparamref name="T"/>.</summary>
    internal T? Located<T>(Claim claim) where T : class
    {
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

    internal bool TypeMatches(ITypeReference model, TypeRefNode code) => TypeComparer.Matches(model, code, _profile);

    internal Claim? ClaimOn(string designer, IElement element, string kind) => _index.GetValueOrDefault(new ClaimRef(designer, element.Id, kind));

    /// <summary>
    /// A recipe (rules.md, Recipes), the subset built so far: a <c>find</c> of <c>type:</c>, <c>class:</c>,
    /// <c>method:</c>, <c>property:</c>, <c>annotation:</c> and <c>annotation with &lt;regex&gt;</c> steps, and
    /// <c>read</c> cases giving a fixed <c>value</c>. Placeholders name the element and its owners by element type:
    /// an Operation's recipe has <c>{Operation}</c> and <c>{Service}</c>.
    /// </summary>
    private void Recipe(Claim claim, JsonNode recipe)
    {
        var placeholders = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var element = claim.Source; element is not null; element = Owner(element))
        {
            placeholders.TryAdd(element.SpecializationType.Replace(" ", "").Replace("-", ""), element.Name);
        }

        string Fill(string text) => Naming.Fill(text, placeholders);

        IReadOnlyList<object> nodes = [];
        Span? at = null;
        foreach (var step in (recipe["find"]?.GetValue<string>() ?? "").Split('>', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(Fill))
        {
            nodes = step switch
            {
                _ when step.StartsWith("type:", StringComparison.Ordinal) || step.StartsWith("class:", StringComparison.Ordinal) =>
                    LocatedTypes(claim, step[(step.IndexOf(':') + 1)..]),
                _ when step.StartsWith("method:", StringComparison.Ordinal) =>
                    nodes.OfType<TypeNode>().SelectMany(x => x.Members).Where(x => x.Kind == "method" && x.Name == step["method:".Length..]).ToList<object>(),
                _ when step.StartsWith("property:", StringComparison.Ordinal) =>
                    nodes.OfType<TypeNode>().SelectMany(x => x.Members).Where(x => x.Kind == "property" && x.Name == step["property:".Length..]).ToList<object>(),
                _ when step.StartsWith("annotation with ", StringComparison.Ordinal) =>
                    Annotations(nodes).Where(x => new Regex(step["annotation with ".Length..], RegexOptions.CultureInvariant).IsMatch(x.Name)).ToList<object>(),
                _ when step.StartsWith("annotation:", StringComparison.Ordinal) =>
                    Annotations(nodes).Where(x => x.Name == AttributeName(step["annotation:".Length..])).ToList<object>(),
                _ => throw new InvalidOperationException($"The recipe for {claim.Kind} has a find step this engine doesn't read yet: '{step}'.")
            };
            if (nodes.Count == 0)
            {
                break;
            }

            at = SpanOf(nodes[0]);
        }

        at ??= _types.Values.FirstOrDefault()?.At;
        if (nodes.Count == 0)
        {
            Compare(claim, "none", at ?? new Span("", 1, 1, 1, 1), "absent", false);
            return;
        }

        if (nodes.Count > 1 && recipe["matches"]?.GetValue<string>() != "any")
        {
            Cant(claim, "ambiguous", new Reading { Candidates = nodes.Select(SpanOf).ToList() });
            return;
        }

        var value = (recipe["read"] as JsonArray)?.Select(x => x?["value"]?.ToString()).FirstOrDefault(x => x is not null);
        Apply(claim, value is null ? ClaimReading.Absent(SpanOf(nodes[0])) : ClaimReading.Stated(value, SpanOf(nodes[0])));
    }

    /// <summary>The types of a name; of several, the ones a claim on this element or its owners located.</summary>
    private List<object> LocatedTypes(Claim claim, string name)
    {
        var candidates = _graph.Types.Where(x => x.Name == name).ToList();
        if (candidates.Count > 1)
        {
            for (var current = Dependency(claim); current is not null; current = Dependency(current))
            {
                if (_types.TryGetValue(current, out var located) && located.Name == name)
                {
                    return [located];
                }
            }
        }

        return candidates.ToList<object>();
    }

    private static IEnumerable<AttributeNode> Annotations(IEnumerable<object> nodes) => nodes.SelectMany(x => x switch
    {
        TypeNode type => type.Attributes,
        MemberNode member => member.Attributes,
        _ => (IEnumerable<AttributeNode>)[]
    });

    private static string AttributeName(string name) => name.EndsWith("Attribute", StringComparison.Ordinal) ? name[..^"Attribute".Length] : name;

    private static Span SpanOf(object node) => node switch
    {
        TypeNode type => type.At,
        MemberNode member => member.At,
        AttributeNode attribute => attribute.At,
        _ => throw new InvalidOperationException($"No location for {node.GetType().Name}.")
    };

    private TypeNode OwnType(Claim claim) => _types[Dependency(claim)!];

    private MemberNode OwnMember(Claim claim) => _members[Dependency(claim)!];

    private static void Verify(Claim claim, Reading reading)
    {
        claim.Outcome = "verified";
        claim.Reading = reading;
    }

    private static void Compare(Claim claim, string actual, Span at, string from, bool same)
    {
        claim.Outcome = same ? "verified" : "contradicted";
        claim.Reading = new Reading { Value = actual, From = from, At = at };
    }

    private static void Cant(Claim claim, string reason, Reading reading)
    {
        claim.Outcome = "cantVerify";
        claim.Reason = reason;
        claim.Reading = reading;
    }

    // ---- Findings (P6) ----

    private Finding Report(Claim claim)
    {
        // A member, or a member's parameter, is named with its owner: Order.Total, Orders.Get.id.
        var display = Owner(claim.Source) is { } parent && OnMember(claim)
            ? $"{parent.Name}.{claim.Source.Name}"
            : claim.Source.Name;
        var label = claim.Definition.Label;
        var reading = claim.Reading!;
        var message = (claim.Outcome, claim.Reason) switch
        {
            ("contradicted", _) when reading.Message is not null => reading.Message,
            ("contradicted", _) when claim.Definition.Existence =>
                $"`{display}` isn't in the code. Searched {reading.Detail} for {reading.Searched![0]}.{Blocked(claim)}",
            ("contradicted", _) when reading.From == "absent" || reading is { From: null, Value: null } =>
                $"`{display}` {label}: model `{claim.Expected}`, the code doesn't state it ({reading.At}). The model states it in {claim.SourceText}.",
            ("contradicted", _) when reading.From == "default" =>
                $"`{display}` {label}: model `{claim.Expected}`, the code doesn't state it, so it is `{reading.Value}` ({reading.At}). The model states it in {claim.SourceText}.",
            ("contradicted", _) =>
                $"`{display}` {label}: model `{claim.Expected}`, code `{reading.Value}` ({reading.At}). The model states it in {claim.SourceText}.",
            (_, "ambiguous") =>
                $"`{display}` {label} can't be verified: {reading.Candidates!.Count} in-scope types match, at {string.Join(", ", reading.Candidates)}.{Blocked(claim)}",
            (_, "outOfScope") when reading.ShadowFile is not null && reading.ScopeRow is not null =>
                $"`{display}` {label} can't be verified: {reading.ShadowFile} may hold it, but scope row {ScopeRowText(reading.ScopeRow)} excludes that file, so it isn't read.{Blocked(claim)}",
            (_, "outOfScope") when reading.ShadowFile is not null =>
                $"`{display}` {label} can't be verified: {reading.ShadowFile} may hold it, but no Drift Scope row includes that file, so it isn't read.{Blocked(claim)}",
            (_, "unresolved") =>
                $"`{display}` {label} can't be verified: {reading.Detail}",
            _ => $"`{display}` {label} can't be verified ({claim.Reason}). {reading.Detail}"
        };

        return new Finding($"claim/{claim.Kind}", $"{claim.Area} (DriftEngine)", "ModelCode", "error", message, new Anchor(Element: claim.Element), [claim.Kind]);
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

    private string Blocked(Claim claim)
    {
        var count = BlockedOn(claim);
        return count == 0 ? "" : $" {count} claim{(count == 1 ? "" : "s")} on it can't be checked until it is.";
    }

    /// <summary>The claims waiting on <paramref name="claim"/>, and those waiting on them.</summary>
    private int BlockedOn(Claim claim) =>
        _claims.Where(x => x.Reason == "blocked" && x.DependsOn is { } on && on.Element == claim.Element && on.Kind == claim.Kind).Sum(x => 1 + BlockedOn(x));

    private string ScopeRowText(string id)
    {
        var row = _scope.Rows.First(x => x.Id == id);
        return $"{row.Number} ({string.Join(", ", row.Globs.Select(x => x.Pattern))}{(row.Reason is null ? "" : $": {row.Reason}")})";
    }
}
