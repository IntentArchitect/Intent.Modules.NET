using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>A recipe file to convert: its file name (<c>domain.drift.json</c>) and its text.</summary>
    internal sealed record RecipeSource(string FileName, string Json);

    /// <summary>A <c>Drift Scope</c> Entries row: its globs, one per line, <c>In</c>, <c>Out</c> or <c>Context</c>, and its reason.</summary>
    internal sealed record ScopeRowSpec(IReadOnlyList<string> Globs, string Scope, string Reason);

    /// <summary>A <c>Drift Check</c> Opt-in Rules row.</summary>
    internal sealed record OptInRowSpec(string Rule, IReadOnlyList<string> Globs, string Description);

    /// <summary>A <c>Drift Check</c> Exceptions row: one per rule and reason, its matches one per line.</summary>
    internal sealed record ExceptionRowSpec(string Rule, IReadOnlyList<string> Matches, string Reason);

    /// <summary>What a set of recipe files becomes, and what the migration couldn't carry over, for its log.</summary>
    internal sealed class ConvertedRecipes
    {
        public List<ScopeRowSpec> Scope { get; } = new();
        public List<OptInRowSpec> OptIn { get; } = new();
        public List<ExceptionRowSpec> Exceptions { get; } = new();
        public List<string> Notes { get; } = new();

        /// <summary>
        /// The globs v1 read code through, normalised as v1 read them: each recipe's files (every C# file when it gives none),
        /// and the files its facts and <c>own</c> entries name. With the contract files, which v1 compared wherever they were,
        /// that is all the code v1 read.
        /// </summary>
        public List<string> V1Read { get; } = new();

        public bool HasRows => Scope.Count > 0 || OptIn.Count > 0 || Exceptions.Count > 0;
    }

    /// <summary>
    /// Turns v1's recipe files into the drift engine's rows (docs/spec/conversion.md in Intent.Common.DriftEngine): one
    /// <c>**</c> In row, a Context row per file-pattern exception's reason, an <c>unmodelled.type</c> row per <c>own</c> entry over its
    /// <c>files</c> or its recipe's, one <c>unmodelled.member</c> row over all the C# when there is a Domain or Services recipe,
    /// and an Exceptions row per rule and reason. A recipe file is read as v1 read it (<see cref="RecipeFile.JsonOptions"/>, <see cref="Glob"/>, <see cref="Glob.LooksLikeGlob"/>).
    /// Its facts are dropped, since the engine's stock readers read those kinds by name, and so are notes and comments.
    /// </summary>
    internal static class RecipeConversion
    {
        public const string UnmodelledType = "optin/unmodelled.type";
        public const string UnmodelledMember = "optin/unmodelled.member";

        private static readonly Regex TypeName = new(@"^[A-Za-z_]\w*$", RegexOptions.CultureInvariant);
        private static readonly Regex MemberName = new(@"^[A-Za-z_]\w*(?:\.[A-Za-z_]\w*)+$", RegexOptions.CultureInvariant);
        private static readonly Regex Placeholder = new(@"\{[^{}]*\}|\$\d+", RegexOptions.CultureInvariant);
        private static readonly Regex Comments = new(@"//[^\n]*|/\*.*?\*/", RegexOptions.Singleline | RegexOptions.CultureInvariant);

        public static ConvertedRecipes Convert(IEnumerable<RecipeSource> recipes)
        {
            var result = new ConvertedRecipes();
            var sources = new List<string>();
            var structureChecked = false;
            var contextRows = new List<ScopeRowSpec>();
            var optIn = new List<OptInRowSpec>();
            var exceptions = new List<(string Rule, string Match, string Reason)>();
            var read = new List<string>();

            foreach (var recipe in recipes.OrderBy(x => x.FileName, StringComparer.OrdinalIgnoreCase))
            {
                var source = $"guardrails/{recipe.FileName}";
                var file = Read(recipe, source, result.Notes);
                if (file == null)
                {
                    continue;
                }

                // A recipe's files only said where its facts and conventions were read, so they bound its opt-in rows.
                sources.Add(source);
                var inGlobs = file.Files ?? Globs(new[] { "**/*.cs" });
                read.AddRange(inGlobs.Concat(file.FactFiles).Concat(file.Own.SelectMany(x => x.Files ?? Array.Empty<string>())));

                var excludedOwn = new HashSet<OwnEntry>();
                foreach (var exception in file.Exceptions)
                {
                    var reason = exception.Reason ?? $"From {source}, which gives no reason";
                    if (Glob.LooksLikeGlob(exception.Match))
                    {
                        // A v1 exception covered only code the model doesn't describe: the file was still read for the model's
                        // facts. A Context row is that (user decision, 2026-11-25); an Out row left a modelled type in the file
                        // unverifiable.
                        contextRows.Add(new ScopeRowSpec(Globs(new[] { exception.Match }), "Context", reason));
                    }
                    else if (TypeName.IsMatch(exception.Match))
                    {
                        exceptions.Add((UnmodelledType, exception.Match, reason));
                    }
                    else if (MemberName.IsMatch(exception.Match))
                    {
                        exceptions.Add((UnmodelledMember, exception.Match, reason));
                    }
                    else
                    {
                        // The name the report gave one of an own entry's findings, such as "CatalogApi: MapGet [items]".
                        var named = file.Own.Where(x => Names(x, exception.Match)).ToList();
                        excludedOwn.UnionWith(named);
                        result.Notes.Add(named.Count == 0
                            ? $"{source}: the exception '{exception.Match}' names nothing the drift engine has, so it wasn't carried over."
                            : $"{source}: the exception '{exception.Match}' names a finding of the '{named[0].Kind}' convention, so that convention wasn't carried over.");
                    }
                }

                foreach (var own in file.Own.Where(x => !excludedOwn.Contains(x)))
                {
                    optIn.Add(new OptInRowSpec(UnmodelledType, own.Files ?? inGlobs, own.Kind));
                }

                structureChecked |= V1Footprint.StructureCheckedRecipes.Contains(recipe.FileName);
            }

            // v1's contract templates reported a modelled type's unmodelled public members wherever the type was, so one row
            // over all the C#, which is all they generated: a UI component's members were never reported.
            if (structureChecked)
            {
                optIn.Add(new OptInRowSpec(UnmodelledMember, new[] { "**/*.cs" }, "Public members of modelled types"));
            }

            // Everything is in scope (user decision, 2026-11-23): v1 compared each contract file wherever it was, and each own
            // entry read its own files, so a recipe's top-level files left most modelled code out. The Context rows come after
            // it: within a folder, the last matching row decides a file. The migration adds an Out row last for the projects v1
            // never read (V1Coverage.UnreadProjects), which needs the code.
            if (sources.Count > 0)
            {
                result.Scope.Add(new ScopeRowSpec(new[] { "**" }, "In", $"From {string.Join(", ", sources)}: v1 checked code wherever it was"));
            }

            result.Scope.AddRange(contextRows
                .GroupBy(x => x.Reason, StringComparer.Ordinal)
                .Select(x => new ScopeRowSpec(x.SelectMany(r => r.Globs).Distinct(StringComparer.Ordinal).ToList(), "Context", x.Key)));

            // Conventions over the same globs are one row, named by each of their kinds.
            result.OptIn.AddRange(optIn
                .GroupBy(x => (x.Rule, Globs: string.Join("\n", x.Globs)))
                .Select(x => new OptInRowSpec(x.Key.Rule, x.First().Globs, string.Join(", ", x.Select(r => r.Description).Distinct(StringComparer.Ordinal)))));

            result.Exceptions.AddRange(exceptions
                .GroupBy(x => (x.Rule, x.Reason))
                .Select(x => new ExceptionRowSpec(x.Key.Rule, x.Select(e => e.Match).Distinct(StringComparer.Ordinal).ToList(), x.Key.Reason)));

            result.V1Read.AddRange(read.Distinct(StringComparer.Ordinal));
            return result;
        }

        /// <summary>
        /// A v1 glob as the engine's matcher reads it: v1's own normalisation (no leading <c>/</c> or <c>./</c>, forward
        /// slashes), with each <c>{a,b}</c> expanded into one glob per alternative, which the engine's matcher doesn't support.
        /// </summary>
        public static IReadOnlyList<string> Globs(IEnumerable<string> patterns)
        {
            return patterns
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .SelectMany(x => ExpandBraces(new Glob(x).Pattern))
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }

        private static IEnumerable<string> ExpandBraces(string pattern)
        {
            for (var open = pattern.IndexOf('{'); open >= 0; open = pattern.IndexOf('{', open + 1))
            {
                var close = MatchingBrace(pattern, open);
                if (close < 0)
                {
                    break;
                }

                var alternatives = SplitTopLevel(pattern[(open + 1)..close]);
                if (alternatives.Count < 2)
                {
                    continue;
                }

                var prefix = pattern[..open];
                var suffix = pattern[(close + 1)..];
                return alternatives.SelectMany(x => ExpandBraces(prefix + x + suffix));
            }

            return new[] { pattern };
        }

        private static int MatchingBrace(string text, int open)
        {
            var depth = 0;
            for (var i = open; i < text.Length; i++)
            {
                depth += text[i] == '{' ? 1 : text[i] == '}' ? -1 : 0;
                if (depth == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private static List<string> SplitTopLevel(string text)
        {
            var parts = new List<string>();
            var depth = 0;
            var start = 0;
            for (var i = 0; i < text.Length; i++)
            {
                depth += text[i] == '{' ? 1 : text[i] == '}' ? -1 : 0;
                if (text[i] == ',' && depth == 0)
                {
                    parts.Add(text[start..i]);
                    start = i + 1;
                }
            }

            parts.Add(text[start..]);
            return parts;
        }

        /// <summary>
        /// Whether a name is one an own entry gives its findings. Only an entry whose name has text that isn't part of a type or
        /// member name (<c>{Class}: Map$1 [$2]</c>) can be named this way: <c>{Class}.{Method}</c> names a member.
        /// </summary>
        private static bool Names(OwnEntry own, string name)
        {
            if (own.Name == null)
            {
                return false;
            }

            var literal = Placeholder.Replace(own.Name, string.Empty);
            if (literal.All(x => char.IsLetterOrDigit(x) || x == '_' || x == '.'))
            {
                return false;
            }

            var pattern = string.Concat(Placeholder.Split(own.Name).Select(Regex.Escape).Zip(
                Placeholder.Matches(own.Name).Select(_ => ".*?").Append(string.Empty), (text, hole) => text + hole));
            return Regex.IsMatch(name, $"^{pattern}$", RegexOptions.CultureInvariant);
        }

        private static RecipeContent Read(RecipeSource recipe, string source, List<string> notes)
        {
            if (string.IsNullOrWhiteSpace(Comments.Replace(recipe.Json ?? string.Empty, string.Empty)))
            {
                // Only comments: the file turned v1's check on with nothing in it, as {} does.
                return new RecipeContent(null, new List<string>(), new List<OwnEntry>(), new List<ExceptionEntry>());
            }

            try
            {
                using var document = JsonDocument.Parse(recipe.Json, RecipeFile.JsonOptions);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    notes.Add($"{source} isn't a JSON object, so nothing in it was carried over.");
                    return null;
                }

                var root = document.RootElement;
                return new RecipeContent(
                    root.TryGetProperty("files", out var files) ? Globs(Strings(files)) is { Count: > 0 } globs ? globs : null : null,
                    // A fact read its own files when it named them, else its recipe's.
                    root.TryGetProperty("facts", out var facts) && facts.ValueKind == JsonValueKind.Object
                        ? Globs(facts.EnumerateObject().Where(x => x.Value.ValueKind == JsonValueKind.Object && x.Value.TryGetProperty("files", out _))
                            .SelectMany(x => Strings(x.Value.GetProperty("files"))))
                        : new List<string>(),
                    root.TryGetProperty("own", out var own) && own.ValueKind == JsonValueKind.Array
                        ? own.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object).Select(ReadOwn).Where(x => x != null).ToList()
                        : new List<OwnEntry>(),
                    root.TryGetProperty("exceptions", out var exceptions) && exceptions.ValueKind == JsonValueKind.Array
                        ? exceptions.EnumerateArray().Select(ReadException).Where(x => x != null).ToList()
                        : new List<ExceptionEntry>());
            }
            catch (JsonException e)
            {
                notes.Add($"{source} isn't valid JSON, so nothing in it was carried over: {e.Message}");
                return null;
            }
        }

        private static OwnEntry ReadOwn(JsonElement element)
        {
            var kind = Text(element, "kind");
            if (string.IsNullOrWhiteSpace(kind))
            {
                return null;
            }

            var files = element.TryGetProperty("files", out var value) ? Globs(Strings(value)) : null;
            return new OwnEntry(kind.Trim(), files is { Count: > 0 } ? files : null, Text(element, "name"));
        }

        private static ExceptionEntry ReadException(JsonElement element)
        {
            var match = element.ValueKind == JsonValueKind.String ? element.GetString() : Text(element, "match");
            if (string.IsNullOrWhiteSpace(match))
            {
                return null;
            }

            var reason = element.ValueKind == JsonValueKind.Object ? Text(element, "reason") : null;
            return new ExceptionEntry(match.Trim(), string.IsNullOrWhiteSpace(reason) ? null : reason.Trim());
        }

        private static string Text(JsonElement element, string property) =>
            element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;

        private static IEnumerable<string> Strings(JsonElement value) => value.ValueKind switch
        {
            JsonValueKind.String => new[] { value.GetString() },
            JsonValueKind.Array => value.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()).ToList(),
            _ => Array.Empty<string>()
        };

        private sealed record RecipeContent(IReadOnlyList<string> Files, IReadOnlyList<string> FactFiles, List<OwnEntry> Own, List<ExceptionEntry> Exceptions);

        private sealed record OwnEntry(string Kind, IReadOnlyList<string> Files, string Name);

        private sealed record ExceptionEntry(string Match, string Reason);
    }
}
