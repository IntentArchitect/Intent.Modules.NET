using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Common.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>Runs the drift module's check on code held in memory, for a Guardrails designer's kinds of fact.</summary>
    internal static class Drift
    {
        public static DriftReport Check(
            string designer,
            string recipe,
            IReadOnlyList<ModelFact> facts,
            IReadOnlyDictionary<string, string> files,
            IReadOnlyList<GeneratedFile> generated = null,
            IReadOnlyCollection<string> outputPaths = null)
        {
            return DriftCheck.Run(new DriftCheckInput
            {
                Designer = designer,
                Kinds = FactKinds.For(designer),
                RecipePath = $"guardrails/{designer.ToLowerInvariant()}.drift.json",
                RecipeJson = recipe,
                Code = new CodeFiles(files),
                Facts = facts,
                Generated = generated ?? Array.Empty<GeneratedFile>(),
                OutputPaths = outputPaths
            });
        }

        public static ModelFact Fact(FactKind kind, string element, string value, params (string Name, string Value)[] placeholders)
        {
            return new ModelFact
            {
                Kind = kind,
                Element = element,
                Value = value,
                Placeholders = placeholders.ToDictionary(x => x.Name, x => x.Value)
            };
        }

        public static ModelFact Operation(FactKind kind, string service, string operation, string value, params string[] parameterNames)
        {
            return new ModelFact
            {
                Kind = kind,
                Element = $"{service}.{operation}",
                Value = value,
                Placeholders = new Dictionary<string, string> { ["Service"] = service, ["Operation"] = operation, ["Folder"] = string.Empty },
                ParameterNames = parameterNames
            };
        }

        public static ModelFact Parameter(string service, string operation, string parameter, string value)
        {
            return new ModelFact
            {
                Kind = FactKinds.ParameterSource,
                Element = $"{service}.{operation}({parameter})",
                Value = value,
                Placeholders = new Dictionary<string, string> { ["Service"] = service, ["Operation"] = operation, ["Folder"] = string.Empty, ["Parameter"] = parameter }
            };
        }

        public static ModelFact Column(FactKind kind, string entity, string attribute, string value)
        {
            return Fact(kind, $"{entity}.{attribute}", value, ("Entity", entity), ("Attribute", attribute));
        }

        public static ModelFact Table(FactKind kind, string entity, string value) => Fact(kind, entity, value, ("Entity", entity));

        public static IReadOnlyList<string> Wrong(this DriftReport report, DriftSection section) => report.Lines[section];

        public static string Fixture(string relativePath)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", relativePath));
        }
    }
}
