using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Domain.Guardrails.Contracts.Drift
{
    /// <summary>Everything a recipe validation needs: the recipe file, the model's facts and the code.</summary>
    internal sealed class RecipeValidationInput
    {
        public string Designer { get; init; }

        /// <summary>The recipe file's path as the result names it, e.g. <c>guardrails/domain.drift.json</c>.</summary>
        public string RecipePath { get; init; }

        /// <summary>The recipe file's content, or <see langword="null"/> when it doesn't exist.</summary>
        public string RecipeJson { get; init; }

        public CodeFiles Code { get; init; }
        public IReadOnlyList<ModelFact> Facts { get; init; }

        /// <summary>What a targeted validation looks at, or <see langword="null"/> to validate every recipe.</summary>
        public RecipeFilter Filter { get; init; }
    }

    /// <summary>
    /// What each recipe in a recipe file finds and reads in the code, run on demand by the ValidateDriftRecipes module
    /// task: per kind of fact, how many facts were read from the code, from the recipe's <c>default</c> or <c>absent</c>
    /// value, couldn't be verified, or contradict the model, with a few samples of each, and its read cases that matched
    /// nothing; per <c>own</c> entry, what it finds, with samples; the recipe probe's warnings; and the read cases that
    /// read facts from another element's code (<see cref="OwnCodeCheck"/>). A <see cref="RecipeFilter"/> narrows it to a
    /// kind of fact, an element or a file, lists every reading it selects in full, and leaves the conventions out. Only
    /// the recipe file, the model's facts and the code on disk go in: nothing from a Software Factory run, and no drift
    /// result (what isn't modelled, what an exception hides), which is the drift report's. Data only: it says nothing
    /// about what to do with it.
    /// </summary>
    internal sealed class RecipeValidation
    {
        private const int SampleCount = 3;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public string Designer { get; init; }
        public string RecipePath { get; init; }
        public bool RecipeFileExists { get; init; }

        /// <summary>The filter this validation was narrowed to, or <see langword="null"/> when it covers every recipe.</summary>
        public RecipeFilter Filter { get; init; }

        /// <summary>What is wrong with the recipe file itself: invalid JSON, an unknown key, a bad find path or regular expression.</summary>
        public IReadOnlyList<string> Problems { get; init; } = Array.Empty<string>();

        /// <summary>The kinds of fact the model states that the recipe file has no recipe for, with their counts.</summary>
        public IReadOnlyList<string> NotChecked { get; init; } = Array.Empty<string>();

        public IReadOnlyList<KindValidation> Facts { get; init; } = Array.Empty<KindValidation>();
        public IReadOnlyList<ConventionValidation> Conventions { get; init; } = Array.Empty<ConventionValidation>();

        /// <summary>The recipe probe's: read cases whose reading didn't change with the code they matched (see <see cref="RecipeProbe"/>).</summary>
        public IReadOnlyList<string> ProbeWarnings { get; init; } = Array.Empty<string>();

        /// <summary>Read cases that read facts from code that names another element and not their own (see <see cref="OwnCodeCheck"/>).</summary>
        public IReadOnlyList<string> WrongElementWarnings { get; init; } = Array.Empty<string>();

        /// <summary>With a filter, every fact it selects as its recipe read it; otherwise <see langword="null"/>.</summary>
        public IReadOnlyList<ReadingDetail> Readings { get; init; }

        public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

        public static RecipeValidation Run(RecipeValidationInput input)
        {
            var filter = input.Filter;
            if (input.RecipeJson == null)
            {
                return new RecipeValidation { Designer = input.Designer, RecipePath = input.RecipePath, Filter = filter };
            }

            var recipes = RecipeFile.Parse(input.RecipeJson, input.Designer);
            var engine = new RecipeEngine(input.Code, recipes.Files);
            var readings = new List<(Recipe Recipe, ModelFact Fact, FactReading Reading)>();
            var kinds = new List<KindValidation>();
            var notChecked = new List<string>();
            var selected = filter == null ? input.Facts : input.Facts.Where(filter.Selects).ToList();
            foreach (var group in selected.GroupBy(x => x.Kind).OrderBy(x => x.Key.Key, StringComparer.Ordinal))
            {
                // A broken recipe is one of the file's problems.
                if (recipes.BrokenKinds.Contains(group.Key.Key))
                {
                    continue;
                }

                if (!recipes.Facts.TryGetValue(group.Key.Key, out var recipe))
                {
                    notChecked.Add($"{group.Key.Key} ({DriftCheck.Count(group.Count(), "fact")})");
                    continue;
                }

                var read = group
                    .Select(fact => (Recipe: recipe, Fact: fact, Reading: engine.Read(recipe, fact)))
                    .Where(x => filter == null || filter.SelectsFile(x.Reading))
                    .ToList();
                if (read.Count == 0)
                {
                    continue;
                }

                readings.AddRange(read);
                var matched = new HashSet<int>(read.Where(x => x.Reading.Case != null).Select(x => x.Reading.Case.Value));
                kinds.Add(new KindValidation
                {
                    Kind = group.Key.Key,
                    Facts = read.Count,
                    ReadFromCode = Group(read.Where(x => x.Reading.Verified && x.Reading.From == ValueSource.Code)),
                    FromDefault = Group(read.Where(x => x.Reading.Verified && x.Reading.From == ValueSource.Default)),
                    FromAbsent = Group(read.Where(x => x.Reading.Verified && x.Reading.From == ValueSource.Absent)),
                    Unverifiable = Group(read.Where(x => !x.Reading.Verified)),
                    Contradicted = Group(read.Where(x => IsContradicted(x.Fact, x.Reading))),
                    ReadCasesMatchingNothing = recipe.Read.Select((x, i) => (Case: x, Index: i)).Where(x => !matched.Contains(x.Index)).Select(x => $"read case {x.Index + 1}: {x.Case.Match}").ToList()
                });
            }

            // A targeted validation is about facts, so it leaves the conventions out.
            IReadOnlyList<ConventionValidation> conventions = filter != null
                ? Array.Empty<ConventionValidation>()
                : recipes.Own
                    .Select(own =>
                    {
                        var found = engine.Own(own);
                        return new ConventionValidation(own.Kind, found.Count, found.Take(SampleCount).Select(x => $"{x.Name} ({x.Hit.Location})").ToList());
                    })
                    .ToList();

            return new RecipeValidation
            {
                Designer = input.Designer,
                RecipePath = input.RecipePath,
                RecipeFileExists = true,
                Filter = filter,
                Problems = recipes.Problems.Select(x => x.Subject == null ? x.Message : $"{x.Subject}: {x.Message}").ToList(),
                NotChecked = notChecked,
                Facts = kinds,
                Conventions = conventions,
                ProbeWarnings = RecipeProbe.Run(engine, readings),
                // Every fact of a kind is another element's sibling, so the check sees them all, not only the selected ones.
                WrongElementWarnings = OwnCodeCheck.Run(readings, input.Facts),
                Readings = filter == null ? null : readings.Select(x => ReadingDetail.Of(x.Fact, x.Reading)).ToList()
            };
        }

        /// <summary>Whether the code states a different value from the model's.</summary>
        internal static bool IsContradicted(ModelFact fact, FactReading reading) =>
            reading.Verified && !string.Equals(reading.Value, fact.Value?.Trim() ?? string.Empty, StringComparison.Ordinal);

        private static FactGroup Group(IEnumerable<(Recipe Recipe, ModelFact Fact, FactReading Reading)> readings)
        {
            var list = readings.ToList();
            return new FactGroup(list.Count, list.Take(SampleCount)
                .Select(x => new FactSample(x.Fact.Element, x.Fact.Value?.Trim() ?? string.Empty, x.Reading.Verified ? x.Reading.Value : null, x.Reading.Location, x.Reading.Near, x.Reading.Problem))
                .ToList());
        }
    }

    /// <summary>What one recipe read: its facts, counted by where each value came from, with samples.</summary>
    internal sealed class KindValidation
    {
        public string Kind { get; init; }
        public int Facts { get; init; }
        public FactGroup ReadFromCode { get; init; }
        public FactGroup FromDefault { get; init; }
        public FactGroup FromAbsent { get; init; }
        public FactGroup Unverifiable { get; init; }

        /// <summary>The facts whose value the code contradicts, wherever it was read from.</summary>
        public FactGroup Contradicted { get; init; }

        public IReadOnlyList<string> ReadCasesMatchingNothing { get; init; } = Array.Empty<string>();
    }

    /// <summary>A number of facts, and the first few of them.</summary>
    internal sealed record FactGroup(int Count, IReadOnlyList<FactSample> Samples);

    /// <summary>One fact as a recipe read it: the model's value, the code's (none when it can't be verified), and where.</summary>
    internal sealed record FactSample(string Element, string Model, string Code, string Location, string Near, string Problem);

    /// <summary>What an <c>own</c> entry finds in the code: how much, and the first few, by the names the report would give them.</summary>
    internal sealed record ConventionValidation(string Kind, int Found, IReadOnlyList<string> Samples);

    /// <summary>
    /// What a targeted validation looks at: a kind of fact (<c>column.maxLength</c>), an element together with the elements
    /// it holds (<c>Order</c> also selects <c>Order.Notes</c>), the code file a fact was read from or would be stated in (a
    /// path relative to the codebase, or its end, such as <c>AppDbContext.cs</c>), or any combination. Each is optional.
    /// </summary>
    internal sealed record RecipeFilter(string Fact, string Element, string File)
    {
        /// <summary>The filter, or <see langword="null"/> when every part is empty.</summary>
        public static RecipeFilter Of(string fact, string element, string file)
        {
            var path = Value(file)?.Replace('\\', '/');
            if (path != null && path.StartsWith("./", StringComparison.Ordinal))
            {
                path = path.Substring(2);
            }

            var filter = new RecipeFilter(Value(fact), Value(element), path);
            return filter.Fact == null && filter.Element == null && filter.File == null ? null : filter;
        }

        public bool Selects(ModelFact fact)
        {
            return (Fact == null || fact.Kind.Key == Fact) &&
                   (Element == null || fact.Element == Element || fact.Element?.StartsWith(Element + ".", StringComparison.Ordinal) == true);
        }

        /// <summary>Whether the fact was read in the filter's file, or would be stated there.</summary>
        public bool SelectsFile(FactReading reading)
        {
            return File == null || InFile(reading.Location) || InFile(reading.Near);
        }

        private bool InFile(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return false;
            }

            var colon = location.LastIndexOf(':');
            var path = (colon > 0 && int.TryParse(location.Substring(colon + 1), out _) ? location.Substring(0, colon) : location).Replace('\\', '/');
            return string.Equals(path, File, StringComparison.OrdinalIgnoreCase) || path.EndsWith("/" + File, StringComparison.OrdinalIgnoreCase);
        }

        private static string Value(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    /// <summary>
    /// One fact as its recipe read it, in full, for a targeted validation: the model's value and the code's (none when it
    /// can't be verified); where the value came from (<c>code</c>, <c>default</c>, <c>absent</c> or <c>unverifiable</c>);
    /// the find path and read case that produced it, counted from 1 as the recipe file lists them; for a value read from
    /// the code, how far it was inherited (0 for the code the find found, 1 for the first inherit target, and so on) and the
    /// text the case matched; and where.
    /// </summary>
    internal sealed record ReadingDetail(
        string Kind,
        string Element,
        string Model,
        string Code,
        string From,
        bool Contradicted,
        int? FindPath,
        int? ReadCase,
        int? Level,
        string Matched,
        string Location,
        string Near,
        string Problem)
    {
        public static ReadingDetail Of(ModelFact fact, FactReading reading)
        {
            var fromCode = reading.Verified && reading.From == ValueSource.Code;
            return new ReadingDetail(
                fact.Kind.Key,
                fact.Element,
                fact.Value?.Trim() ?? string.Empty,
                reading.Verified ? reading.Value : null,
                !reading.Verified ? "unverifiable" : reading.From switch { ValueSource.Code => "code", ValueSource.Default => "default", _ => "absent" },
                RecipeValidation.IsContradicted(fact, reading),
                reading.Path + 1,
                reading.Case + 1,
                fromCode ? reading.Level : null,
                fromCode ? reading.Match?.Match.Value : null,
                reading.Location,
                reading.Near,
                reading.Problem);
        }
    }

    /// <summary>
    /// Checks that each recipe reads the code where it says it does. For one fact per find path and read case that read a
    /// value from the code, it changes the text the case matched, in memory, and reads the fact again: a value the case
    /// captured gets a suffix, and a match the case maps to a fixed value has its first word changed so that it no
    /// longer matches. If the same case, in the same code or nearer, still reads the same value, the recipe isn't reading
    /// the code there. A value that then falls back to the recipe's <c>default</c>, or to one inherited from further out
    /// (an action's security that its controller states too), still counts as read. The probe can't prove that a recipe
    /// reads the right code, only that it reads some; nor can it change a value from <c>default</c> or <c>absent</c>,
    /// which have no matched text.
    /// </summary>
    internal static class RecipeProbe
    {
        private const string Suffix = "Probe";

        private static readonly Regex Word = new("[A-Za-z_][A-Za-z0-9_]*", RegexOptions.CultureInvariant);
        private static readonly Regex GroupReference = new(@"\$(?:(\d+)|\{(\w+)\}|(&))", RegexOptions.CultureInvariant);

        /// <summary>One warning per read case whose reading didn't change with the code it matched.</summary>
        public static IReadOnlyList<string> Run(RecipeEngine engine, IEnumerable<(Recipe Recipe, ModelFact Fact, FactReading Reading)> readings)
        {
            var warnings = new List<string>();
            var probed = readings
                .Where(x => x.Reading.Verified && x.Reading.From == ValueSource.Code && x.Reading.Match != null)
                .GroupBy(x => (x.Recipe.Key, x.Reading.Path, x.Reading.Case));
            foreach (var group in probed)
            {
                var (recipe, fact, reading) = group.First();
                if (!ChangesWithTheCode(engine, recipe, fact, reading))
                {
                    warnings.Add($"`{recipe.Key}` (read case {reading.Case + 1}): changing {DriftCheck.Code(reading.Match.Match.Value)} at `{reading.Location}` doesn't change what the recipe reads");
                }
            }

            return warnings;
        }

        /// <summary>Whether changing what the reading's case matched changes what the recipe reads; true when nothing in it can be changed.</summary>
        private static bool ChangesWithTheCode(RecipeEngine engine, Recipe recipe, ModelFact fact, FactReading reading)
        {
            var match = reading.Match.Match;
            var group = Captured(recipe.Read[reading.Case.Value], match);
            int start, end;
            string suffix;
            if (group != null)
            {
                (start, end) = (group.Index, group.Index + group.Length);
                suffix = group.Length > 0 && group.Value.All(char.IsDigit) ? "1" : Suffix;
            }
            else
            {
                var word = Word.Match(match.Value);
                if (!word.Success)
                {
                    return true;
                }

                (start, end) = (match.Index + word.Index, match.Index + word.Index + word.Length);
                suffix = Suffix;
            }

            // The match is in the file's canonical text; the change is made to the file's own text, after the last
            // character of the captured value or word, or where an empty capture is.
            var file = reading.Match.File;
            var offset = reading.Match.Offset + end;
            var position = end > start ? file.Canonical.PositionAt(offset - 1) + 1 : file.Canonical.PositionAt(offset);
            var again = engine.WithText(file.RelativePath, file.Text.Insert(Math.Min(position, file.Text.Length), suffix)).Read(recipe, fact);
            return !(again.Verified && again.From == ValueSource.Code && again.Value == reading.Value && again.Level <= reading.Level);
        }

        /// <summary>The group whose text becomes the value: group 1 or the whole match without a <c>value</c>, else the first one it uses.</summary>
        private static Group Captured(ReadCase readCase, Match match)
        {
            if (readCase.Value == null)
            {
                return match.Groups.Count == 1 ? match.Groups[0] : match.Groups[1].Success ? match.Groups[1] : null;
            }

            foreach (Match reference in GroupReference.Matches(readCase.Value))
            {
                var group = reference.Groups[1].Success ? (int.TryParse(reference.Groups[1].Value, out var number) ? match.Groups[number] : null)
                    : reference.Groups[2].Success ? match.Groups[reference.Groups[2].Value]
                    : match.Groups[0];
                if (group is { Success: true })
                {
                    return group;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Checks that each recipe reads a fact from code about the fact's own element. A value read in the code the find found
    /// (not inherited from further out) is suspect when the statement, parameter or declaration it was read from names
    /// another element of the same kind and owner (another attribute of the class, another action of the controller), and
    /// not its own: a read case that isn't tied to its element takes whichever such code comes first. Code that names no
    /// element, such as a global setting or a route group, isn't suspect. Names are compared with the code's identifiers
    /// and the words in its string literals. Only C# files are checked.
    /// </summary>
    internal static class OwnCodeCheck
    {
        private static readonly Regex Word = new("[A-Za-z_][A-Za-z0-9_]*", RegexOptions.CultureInvariant);

        /// <summary>One warning per read case that read facts from another element's code: how many, and the first.</summary>
        public static IReadOnlyList<string> Run(IEnumerable<(Recipe Recipe, ModelFact Fact, FactReading Reading)> readings, IEnumerable<ModelFact> facts)
        {
            var owned = facts
                .GroupBy(x => (x.Kind.Key, Owner(x)))
                .ToDictionary(x => x.Key, x => x.Select(fact => (fact.Element, Names: OwnNames(fact))).ToList());
            var warnings = new List<string>();
            var suspect = readings
                .Where(x => x.Reading.Verified && x.Reading.From == ValueSource.Code && x.Reading.Level == 0 && x.Reading.Match?.File?.Syntax != null)
                .Select(x => (x.Recipe, x.Fact, x.Reading, Others: OthersNamed(x.Fact, x.Reading, owned)))
                .Where(x => x.Others.Count > 0)
                .GroupBy(x => (x.Recipe.Key, x.Reading.Path, x.Reading.Case));
            foreach (var group in suspect)
            {
                var (recipe, fact, reading, others) = group.First();
                var count = group.Count();
                warnings.Add($"`{recipe.Key}` (read case {reading.Case + 1}): {DriftCheck.Count(count, "fact")} read code that names another element and not {(count == 1 ? "its" : "their")} own, " +
                             $"e.g. `{fact.Element}` read {DriftCheck.Code(reading.Match.Match.Value)} at `{reading.Location}`, in code that names {string.Join(", ", others.Select(x => $"`{x}`"))}");
            }

            return warnings;
        }

        /// <summary>The names of the fact's siblings that the code it was read from names, when it doesn't name the fact's own element.</summary>
        private static IReadOnlyList<string> OthersNamed(ModelFact fact, FactReading reading, IReadOnlyDictionary<(string, string), List<(string Element, IReadOnlyList<string> Names)>> owned)
        {
            var own = OwnNames(fact);
            if (own.Count == 0 || !owned.TryGetValue((fact.Kind.Key, Owner(fact)), out var siblings))
            {
                return Array.Empty<string>();
            }

            var others = siblings
                .Where(x => x.Element != fact.Element)
                .SelectMany(x => x.Names)
                .Where(x => !own.Contains(x))
                .ToHashSet(StringComparer.Ordinal);
            if (others.Count == 0)
            {
                return Array.Empty<string>();
            }

            var names = NamesAround(reading);
            if (names == null || own.Any(names.Contains))
            {
                return Array.Empty<string>();
            }

            return others.Where(names.Contains).OrderBy(x => x, StringComparer.Ordinal).ToList();
        }

        /// <summary>
        /// The names in the smallest statement, method parameter or declaration header (attributes and signature, without the
        /// body) around the start of what the reading's case matched; <see langword="null"/> when there is none.
        /// </summary>
        private static HashSet<string> NamesAround(FactReading reading)
        {
            var file = reading.Match.File;
            var position = file.Canonical.PositionAt(reading.Match.Offset + reading.Match.Match.Index);
            for (var node = file.Syntax.FindToken(Math.Min(position, file.Syntax.FullSpan.End)).Parent; node != null; node = node.Parent)
            {
                switch (node)
                {
                    case ParameterSyntax { Parent: BaseParameterListSyntax { Parent: not AnonymousFunctionExpressionSyntax } }:
                    case StatementSyntax and not BlockSyntax:
                        return Names(node.DescendantTokens());
                    case MemberDeclarationSyntax member:
                        var bodyStart = BodyStart(member);
                        return Names(member.DescendantTokens().TakeWhile(x => x.SpanStart < bodyStart));
                }
            }

            return null;
        }

        private static int BodyStart(MemberDeclarationSyntax member) => member switch
        {
            BaseTypeDeclarationSyntax type when !type.OpenBraceToken.IsKind(SyntaxKind.None) && !type.OpenBraceToken.IsMissing => type.OpenBraceToken.SpanStart,
            BaseMethodDeclarationSyntax method => method.Body?.SpanStart ?? method.ExpressionBody?.SpanStart ?? method.Span.End,
            PropertyDeclarationSyntax property => property.AccessorList?.SpanStart ?? property.ExpressionBody?.SpanStart ?? property.Span.End,
            _ => member.Span.End
        };

        private static HashSet<string> Names(IEnumerable<SyntaxToken> tokens)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var token in tokens)
            {
                if (token.IsKind(SyntaxKind.IdentifierToken))
                {
                    names.Add(token.ValueText);
                }
                else if (token.IsKind(SyntaxKind.StringLiteralToken) || token.IsKind(SyntaxKind.InterpolatedStringTextToken) ||
                         token.IsKind(SyntaxKind.SingleLineRawStringLiteralToken) || token.IsKind(SyntaxKind.MultiLineRawStringLiteralToken))
                {
                    foreach (Match word in Word.Matches(token.ValueText))
                    {
                        names.Add(word.Value);
                    }
                }
            }

            return names;
        }

        /// <summary>The names of the fact's own element (see <see cref="FactKinds.OwnPlaceholders"/>).</summary>
        private static IReadOnlyList<string> OwnNames(ModelFact fact)
        {
            return FactKinds.OwnPlaceholders(fact.Kind)
                .Select(x => fact.Placeholders?.GetValueOrDefault(x))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        /// <summary>What the fact's element belongs to, as one key: its owner placeholders' values.</summary>
        private static string Owner(ModelFact fact)
        {
            var own = FactKinds.OwnPlaceholders(fact.Kind);
            return string.Join("\u001f", fact.Kind.Placeholders.Where(x => !own.Contains(x)).Select(x => fact.Placeholders?.GetValueOrDefault(x) ?? string.Empty));
        }
    }
}
