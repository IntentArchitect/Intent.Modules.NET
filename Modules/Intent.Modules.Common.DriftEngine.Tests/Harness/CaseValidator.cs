using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// Integrity check 4 (acceptance-suite.md, What runs now): a case file parses, every block validates against its
/// schema, and everything its blocks refer to exists.
/// </summary>
internal static class CaseValidator
{
    private static readonly Regex Location = new(@"^(?<path>.+):(?<line>\d+)$", RegexOptions.CultureInvariant);
    private static readonly Regex ScopeRow = new(@"^(?<host>.+)#(?<row>\d+)$", RegexOptions.CultureInvariant);

    public static IReadOnlyList<string> Validate(CaseFile file)
    {
        var errors = new List<string>(file.Errors);
        foreach (var @case in file.Cases)
        {
            Validate(@case, errors);
        }

        return errors;
    }

    private static void Validate(AcceptanceCase @case, List<string> errors)
    {
        void Error(string message) => errors.Add($"{@case.FullId}: {message}");

        foreach (var block in @case.OwnBlocks.Where(x => x.Role != CaseBlock.File && @case.Parsed.ContainsKey(x)))
        {
            var schema = block.Role switch
            {
                CaseBlock.Case => "case.schema.json",
                CaseBlock.Model => "fixture-model.schema.json",
                CaseBlock.Expect => "expect.schema.json",
                CaseBlock.Pack => "pack.schema.json",
                _ => throw new InvalidOperationException($"Unhandled role '{block.Role}'.")
            };
            foreach (var error in SchemaSet.Instance.Validate(schema, @case.Parsed[block]))
            {
                Error($"{block.Role} block (line {block.Line}) against {schema}: {error}");
            }
        }

        var meta = @case.Meta;
        var model = @case.Model;
        var expect = @case.Expect;
        var files = @case.Files;
        var ids = model is null ? new Dictionary<string, JsonNode>() : ModelIds(model, Error);

        // What the case block names.
        foreach (var kind in meta.Texts("kinds").Where(x => !AcceptanceData.Kinds.ContainsKey(x)))
        {
            Error($"names kind '{kind}', which catalog.json doesn't list");
        }

        foreach (var name in meta.Texts("cases").Where(x => !KnownCase(x)))
        {
            Error($"names case '{name}', which catalog.json doesn't list");
        }

        foreach (var check in meta.Texts("traces").Where(x => !AcceptanceData.Dispositions.ContainsKey(x)))
        {
            Error($"traces '{check}', which checks.json doesn't list");
        }

        var inlinePacks = @case.Packs.Select(x => x.Text("id")).OfType<string>().ToHashSet(StringComparer.Ordinal);
        foreach (var pack in meta.Texts("packs").Where(x => !AcceptanceData.StockPacks.Contains(x) && !inlinePacks.Contains(x)))
        {
            Error($"uses pack '{pack}', which is neither a stock pack in catalog.json nor one of the case's pack blocks");
        }

        foreach (var pack in inlinePacks.Where(x => !meta.Texts("packs").Contains(x)))
        {
            Error($"has a pack block '{pack}' its case block doesn't list under packs");
        }

        var phase = meta?["phase"]?.ToString();
        var phaseNumber = int.TryParse(phase, out var number) ? number : int.MaxValue;

        // What the expect block refers to.
        foreach (var claim in expect.Items("claims"))
        {
            var kind = claim.Text("kind")!;
            var element = claim.Text("element")!;
            if (!AcceptanceData.Kinds.TryGetValue(kind, out var catalogKind))
            {
                Error($"expects a claim of kind '{kind}', which catalog.json doesn't list");
            }
            else if (claim.Text("outcome") is "verified" or "contradicted" && catalogKind.PhaseNumber > phaseNumber)
            {
                Error($"is phase {phase}, but expects {kind} (phase {catalogKind.Phase}) to be {claim.Text("outcome")}; a rule for it arrives only in phase {catalogKind.Phase}");
            }

            if (!ids.ContainsKey(element))
            {
                Error($"expects a claim on '{element}', which isn't in the case's model");
            }

            if (claim.Text("at") is { } at)
            {
                CheckLocation(at, files, "claim location", Error);
            }

            if (claim.Text("blockedBy") is { } blockedBy && !AcceptanceData.Kinds.ContainsKey(blockedBy))
            {
                Error($"expects a claim blocked by kind '{blockedBy}', which catalog.json doesn't list");
            }

            if (claim.Text("scopeRow") is { } scopeRow)
            {
                CheckScopeRow(scopeRow, ids, Error);
            }
        }

        foreach (var finding in expect.Items("findings").Concat(expect.Items("config")))
        {
            if (finding.Text("subject") is not { } subject)
            {
                continue;
            }

            if (Location.IsMatch(subject))
            {
                CheckLocation(subject, files, "finding subject", Error);
            }
            else if (!ids.ContainsKey(subject))
            {
                Error($"expects a finding on '{subject}', which isn't in the case's model");
            }
        }

        foreach (var node in expect.Items("nodes"))
        {
            if (node.Text("at") is { } at)
            {
                CheckLocation(at, files, "node location", Error);
            }
        }

        foreach (var path in expect?["filesRead"].Texts("include") ?? [])
        {
            if (!files.Keys.Any(new Glob(path).IsMatch))
            {
                Error($"expects a file matching '{path}' to be read, but the case has no such file");
            }
        }
    }

    private static bool KnownCase(string name)
    {
        var parts = name.Split('/', 2);
        return parts.Length == 2 && parts[0] switch
        {
            "scope" => AcceptanceData.ScopeCases.Contains(parts[1]),
            "engine" => AcceptanceData.EngineCases.Contains(parts[1]),
            "optin" => AcceptanceData.OptInRules.Contains(parts[1]),
            _ => false
        };
    }

    /// <summary>Every id in the model, and the node it names; reports duplicates and dangling references.</summary>
    private static Dictionary<string, JsonNode> ModelIds(JsonNode model, Action<string> error)
    {
        var ids = new Dictionary<string, JsonNode>(StringComparer.Ordinal);
        var references = new List<(string Id, string From)>();

        void Add(JsonNode node)
        {
            var id = node.Text("id");
            if (id is null)
            {
                return;
            }

            if (!ids.TryAdd(id, node))
            {
                error($"model id '{id}' is used twice");
            }
        }

        void TypeRef(JsonNode? typeRef, string from)
        {
            if (typeRef is null)
            {
                return;
            }

            if (typeRef.Text("element") is { } target)
            {
                references.Add((target, from));
            }

            foreach (var argument in typeRef.Items("arguments"))
            {
                TypeRef(argument, from);
            }
        }

        // A stereotype property holding an element is { "ref": "<id>" }, alone, in a list, or in an item list's row.
        void StereotypeValue(JsonNode? value, string from)
        {
            switch (value)
            {
                case JsonObject reference when reference.Text("ref") is { } target:
                    references.Add((target, from));
                    break;
                case JsonObject row:
                    foreach (var (_, cell) in row)
                    {
                        StereotypeValue(cell, from);
                    }

                    break;
                case JsonArray items:
                    foreach (var item in items)
                    {
                        StereotypeValue(item, from);
                    }

                    break;
            }
        }

        void Stereotypes(JsonNode? owner, string from)
        {
            if (owner?["stereotypes"] is not JsonObject stereotypes)
            {
                return;
            }

            foreach (var (name, properties) in stereotypes)
            {
                if (properties is JsonObject values)
                {
                    foreach (var (property, value) in values)
                    {
                        StereotypeValue(value, $"{from} stereotype '{name}' {property}");
                    }
                }
            }
        }

        void Element(JsonNode element)
        {
            Add(element);
            var from = $"element '{element.Text("id")}'";
            TypeRef(element["typeRef"], from);
            Stereotypes(element, from);
            if (element.Text("generalization") is { } generalization)
            {
                references.Add((generalization, from));
            }

            if (element.Text("mappedTo") is { } mappedTo)
            {
                references.Add((mappedTo, $"{from} mappedTo"));
            }

            foreach (var child in element.Items("children"))
            {
                Element(child);
            }
        }

        if (model["designers"] is JsonObject designers)
        {
            foreach (var (_, designer) in designers)
            {
                foreach (var package in designer.Items("packages"))
                {
                    Add(package);
                    Stereotypes(package, $"package '{package.Text("id")}'");
                    foreach (var element in package.Items("elements"))
                    {
                        Element(element);
                    }
                }

                foreach (var association in designer.Items("associations"))
                {
                    Add(association);
                    var from = $"association '{association.Text("id")}'";
                    Stereotypes(association, from);
                    foreach (var end in new[] { "source", "target" })
                    {
                        if (association[end].Text("element") is { } target)
                        {
                            references.Add((target, $"{from} {end}"));
                        }

                        Stereotypes(association[end], $"{from} {end}");
                    }
                }
            }
        }

        foreach (var (id, from) in references.Where(x => !ids.ContainsKey(x.Id)))
        {
            error($"{from} refers to '{id}', which isn't in the model");
        }

        return ids;
    }

    private static void CheckLocation(string location, IReadOnlyDictionary<string, CaseBlock> files, string what, Action<string> error)
    {
        var match = Location.Match(location);
        if (!match.Success)
        {
            error($"{what} '{location}' isn't path:line");
            return;
        }

        var path = match.Groups["path"].Value;
        var line = int.Parse(match.Groups["line"].Value);
        if (!files.TryGetValue(path, out var file))
        {
            error($"{what} '{location}' names no file block of the case");
        }
        else if (line < 1 || line > file.Lines.Count)
        {
            error($"{what} '{location}' is past the end of {path}, which has {file.Lines.Count} lines");
        }
    }

    private static void CheckScopeRow(string scopeRow, IReadOnlyDictionary<string, JsonNode> ids, Action<string> error)
    {
        var match = ScopeRow.Match(scopeRow);
        if (!match.Success)
        {
            error($"scope row '{scopeRow}' isn't <host element id>#<row number>");
            return;
        }

        var host = match.Groups["host"].Value;
        var row = int.Parse(match.Groups["row"].Value);
        if (!ids.TryGetValue(host, out var node))
        {
            error($"scope row '{scopeRow}' names host '{host}', which isn't in the case's model");
            return;
        }

        var rows = node["stereotypes"]?["Drift Scope"]?["Entries"] as JsonArray;
        if (rows is null || row < 1 || row > rows.Count)
        {
            error($"scope row '{scopeRow}': '{host}' has {rows?.Count ?? 0} Drift Scope rows");
        }
    }
}
