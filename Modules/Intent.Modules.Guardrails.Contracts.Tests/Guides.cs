using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The configure-drift-check skill's technology guides, rendered as their templates render them: each one's recipe
    /// file and snippets, run against fixture code and a model that agrees with it.
    /// </summary>
    internal static class Guides
    {
        public static string Render(string markdown) => new MarkdownFile("guide").FromMarkdown(markdown).ToString().Replace("\r\n", "\n");

        public static IReadOnlyList<string> JsonBlocks(string guide) =>
            Regex.Matches(Render(guide), "```json\n(.*?)\n```", RegexOptions.Singleline).Select(x => x.Groups[1].Value).ToList();

        /// <summary>The guide's recipe file: its one JSON block that is a whole file.</summary>
        public static string RecipeFile(string guide) => JsonBlocks(guide).Single(x => x.TrimStart().StartsWith("{"));

        /// <summary>The recipe file with the facts of the guide's snippet containing <paramref name="containing"/> in place of its own.</summary>
        public static string WithSnippet(string recipe, string guide, string containing)
        {
            var snippet = JsonBlocks(guide).Single(x => !x.TrimStart().StartsWith("{") && x.Contains(containing));
            var facts = JsonNode.Parse($"{{{snippet}}}")!.AsObject();
            var file = JsonNode.Parse(recipe)!.AsObject();
            foreach (var key in facts.Select(x => x.Key).ToList())
            {
                var value = facts[key];
                facts.Remove(key);
                file["facts"]![key] = value;
            }

            return file.ToJsonString();
        }

        public static Dictionary<string, string> Mutate(IReadOnlyDictionary<string, string> files, string path, string from, string to)
        {
            files[path].ShouldContain(from, customMessage: $"the planted change expects `{from}` in {path}");
            return files.ToDictionary(x => x.Key, x => x.Key == path ? x.Value.Replace(from, to) : x.Value);
        }

        /// <summary>The run reports no drift and no warnings.</summary>
        public static void ShouldBeClean(this DriftReport report)
        {
            report.WrongLines.ShouldBeEmpty(string.Join("\n", report.WrongLines));
            report.NotChecked.ShouldBeEmpty(string.Join("\n", report.NotChecked));
            report.UnusedExceptions.ShouldBeEmpty();
        }

        /// <summary>The run reports exactly these rows, each by the start of its line.</summary>
        public static void ShouldReport(this DriftReport report, params string[] expected)
        {
            var lines = report.WrongLines;
            foreach (var line in expected)
            {
                lines.Count(x => x.StartsWith(line)).ShouldBe(1, $"expected one row starting\n{line}\nin\n{string.Join("\n", lines)}");
            }

            lines.Count.ShouldBe(expected.Length, string.Join("\n", lines));
        }

        /// <summary>Validates the recipe file as the ValidateDriftRecipes task does, and checks it reads every fact cleanly.</summary>
        public static RecipeValidation ShouldValidate(string designer, string recipe, IReadOnlyList<ModelFact> facts, IReadOnlyDictionary<string, string> files)
        {
            var validation = RecipeValidation.Run(new RecipeValidationInput
            {
                Designer = designer,
                RecipePath = $"guardrails/{designer.ToLowerInvariant()}.drift.json",
                RecipeJson = recipe,
                Code = new CodeFiles(files),
                Facts = facts
            });
            validation.Problems.ShouldBeEmpty(string.Join("\n", validation.Problems));
            validation.NotChecked.ShouldBeEmpty(string.Join("\n", validation.NotChecked));
            validation.ProbeWarnings.ShouldBeEmpty(string.Join("\n", validation.ProbeWarnings));
            validation.WrongElementWarnings.ShouldBeEmpty(string.Join("\n", validation.WrongElementWarnings));
            foreach (var kind in validation.Facts)
            {
                kind.Unverifiable.Count.ShouldBe(0, $"{kind.Kind}: {string.Join("; ", kind.Unverifiable.Samples.Select(x => $"{x.Element} {x.Problem}"))}");
                kind.Contradicted.Count.ShouldBe(0, $"{kind.Kind}: {string.Join("; ", kind.Contradicted.Samples.Select(x => $"{x.Element} model {x.Model} code {x.Code}"))}");
            }

            return validation;
        }

        /// <summary>How many of a kind's facts were read from the code, from <c>default</c> and from <c>absent</c>.</summary>
        public static (int Code, int Default, int Absent) Sources(this RecipeValidation validation, FactKind kind)
        {
            var facts = validation.Facts.Single(x => x.Kind == kind.Key);
            return (facts.ReadFromCode.Count, facts.FromDefault.Count, facts.FromAbsent.Count);
        }

        public static ModelFact Relationship(FactKind kind, string source, string target, string sourceEnd, string targetEnd, string value)
        {
            var element = targetEnd != "" ? $"{source}.{targetEnd}" : sourceEnd != "" ? $"{target}.{sourceEnd}" : $"{source} → {target}";
            return Fact(kind, element, value, ("Source", source), ("Target", target), ("SourceEnd", sourceEnd), ("TargetEnd", targetEnd));
        }

        public static ModelFact Index(FactKind kind, string entity, string index, string value) =>
            Fact(kind, $"{entity}.{index}", value, ("Entity", entity), ("Index", index));

        /// <summary>A persisted class's own facts: its table's, when it has one, and its key.</summary>
        public static IEnumerable<ModelFact> Entity(string entity, string key, string table = null, string schema = "", string kind = "Table")
        {
            if (table != null)
            {
                yield return Table(FactKinds.TableName, entity, table);
                yield return Table(FactKinds.TableSchema, entity, schema);
                yield return Table(FactKinds.TableKind, entity, kind);
            }

            yield return Table(FactKinds.Key, entity, key);
        }

        /// <summary>An attribute's facts that every type has.</summary>
        public static IEnumerable<ModelFact> Plain(string entity, string attribute, string name = "", string type = "", string @default = "")
        {
            yield return Column(FactKinds.ColumnName, entity, attribute, name);
            yield return Column(FactKinds.ColumnType, entity, attribute, type);
            yield return Column(FactKinds.ColumnDefault, entity, attribute, @default);
        }

        /// <summary>A <c>string</c> attribute's facts; a primary key states no <c>column.required</c>.</summary>
        public static IEnumerable<ModelFact> Text(string entity, string attribute, string textType = "DEFAULT", string maxLength = "", string required = "true",
            string name = "", string type = "", string @default = "", bool key = false)
        {
            foreach (var fact in Plain(entity, attribute, name, type, @default))
            {
                yield return fact;
            }

            yield return Column(FactKinds.ColumnTextType, entity, attribute, textType);
            yield return Column(FactKinds.ColumnMaxLength, entity, attribute, maxLength);
            if (!key)
            {
                yield return Column(FactKinds.ColumnRequired, entity, attribute, required);
            }
        }

        public static IEnumerable<ModelFact> Decimal(string entity, string attribute, string precision = "", string scale = "", string name = "", string type = "", string @default = "")
        {
            foreach (var fact in Plain(entity, attribute, name, type, @default))
            {
                yield return fact;
            }

            yield return Column(FactKinds.ColumnPrecision, entity, attribute, precision);
            yield return Column(FactKinds.ColumnScale, entity, attribute, scale);
        }

        /// <summary>An endpoint's facts, for a Service operation or (with no Service) a Command or Query.</summary>
        public static ModelFact Endpoint(FactKind kind, string service, string operation, string value)
        {
            return Fact(kind, service == "" ? operation : $"{service}.{operation}", value, ("Service", service), ("Operation", operation), ("Folder", ""));
        }

        public static ModelFact Binding(string service, string operation, string parameter, string value)
        {
            var element = service == "" ? operation : $"{service}.{operation}";
            return Fact(FactKinds.ParameterSource, $"{element}({parameter})", value, ("Service", service), ("Operation", operation), ("Folder", ""), ("Parameter", parameter));
        }
    }
}
