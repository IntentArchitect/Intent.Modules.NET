using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
    }

    /// <summary>
    /// What each recipe in a recipe file finds and reads in the code, run on demand by the ValidateDriftRecipes module
    /// task: per kind of fact, how many facts were read from the code, from the recipe's <c>default</c> or <c>absent</c>
    /// value, couldn't be verified, or contradict the model, with a few samples of each, and its read cases that matched
    /// nothing; per <c>own</c> entry, what it finds, with samples; and the recipe probe's warnings. Only the recipe file,
    /// the model's facts and the code on disk go in: nothing from a Software Factory run, and no drift result (what isn't
    /// modelled, what an exception hides), which is the drift report's. Data only: it says nothing about what to do with it.
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

        /// <summary>What is wrong with the recipe file itself: invalid JSON, an unknown key, a bad find path or regular expression.</summary>
        public IReadOnlyList<string> Problems { get; init; } = Array.Empty<string>();

        /// <summary>The kinds of fact the model states that the recipe file has no recipe for, with their counts.</summary>
        public IReadOnlyList<string> NotChecked { get; init; } = Array.Empty<string>();

        public IReadOnlyList<KindValidation> Facts { get; init; } = Array.Empty<KindValidation>();
        public IReadOnlyList<ConventionValidation> Conventions { get; init; } = Array.Empty<ConventionValidation>();

        /// <summary>The recipe probe's: read cases whose reading didn't change with the code they matched (see <see cref="RecipeProbe"/>).</summary>
        public IReadOnlyList<string> ProbeWarnings { get; init; } = Array.Empty<string>();

        public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

        public static RecipeValidation Run(RecipeValidationInput input)
        {
            if (input.RecipeJson == null)
            {
                return new RecipeValidation { Designer = input.Designer, RecipePath = input.RecipePath };
            }

            var recipes = RecipeFile.Parse(input.RecipeJson, input.Designer);
            var engine = new RecipeEngine(input.Code, recipes.Files);
            var readings = new List<(Recipe Recipe, ModelFact Fact, FactReading Reading)>();
            var kinds = new List<KindValidation>();
            var notChecked = new List<string>();
            foreach (var group in input.Facts.GroupBy(x => x.Kind).OrderBy(x => x.Key.Key, StringComparer.Ordinal))
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

                var read = group.Select(fact => (Recipe: recipe, Fact: fact, Reading: engine.Read(recipe, fact))).ToList();
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
                    Contradicted = Group(read.Where(x => x.Reading.Verified && !string.Equals(x.Reading.Value, x.Fact.Value?.Trim() ?? string.Empty, StringComparison.Ordinal))),
                    ReadCasesMatchingNothing = recipe.Read.Select((x, i) => (Case: x, Index: i)).Where(x => !matched.Contains(x.Index)).Select(x => $"read case {x.Index + 1}: {x.Case.Match}").ToList()
                });
            }

            var conventions = recipes.Own
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
                Problems = recipes.Problems.Select(x => x.Subject == null ? x.Message : $"{x.Subject}: {x.Message}").ToList(),
                NotChecked = notChecked,
                Facts = kinds,
                Conventions = conventions,
                ProbeWarnings = RecipeProbe.Run(engine, readings)
            };
        }

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
}
