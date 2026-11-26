using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Integrity;

/// <summary>Integrity check 3: every disposition names something the catalog lists, and the catalog is consistent.</summary>
public class CatalogTests
{
    [Fact]
    public void Every_disposition_names_a_catalog_entry()
    {
        var unknown = AcceptanceData.Dispositions
            .SelectMany(x => x.Value.Select(d => (Check: x.Key, Disposition: d)))
            .Where(x => !Known(x.Disposition))
            .Select(x => $"{x.Check}: {x.Disposition}")
            .ToList();

        unknown.ShouldBeEmpty();
    }

    [Fact]
    public void Kinds_are_unique()
    {
        AcceptanceData.CatalogJson.Items("kinds").Select(x => x.Text("kind")).GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ShouldBeEmpty();
    }

    [Fact]
    public void Existence_kinds_and_only_they_use_the_presence_comparer()
    {
        var mismatched = AcceptanceData.Kinds.Values
            .Where(x => x.Existence != (x.Comparer == "presence") || x.Existence != (x.ValueType == "presence"))
            .Select(x => x.Kind)
            .ToList();

        mismatched.ShouldBeEmpty();
    }

    [Fact]
    public void Every_kind_is_used_by_a_check_a_Common_Drift_recipe_or_stock_vocabulary()
    {
        var byChecks = AcceptanceData.Dispositions.Values.SelectMany(x => x).Where(x => x.StartsWith("claim:", StringComparison.Ordinal)).Select(x => x["claim:".Length..]);
        // A kind the engine claims from a standard Intent stereotype that no old check or recipe read, such as OpenAPI Settings
        // and the Domain Constraints, is used by the stock vocabulary that claims it.
        var byStockVocabulary = new[] { "intent.core", "intent.domain", "intent.services", "intent.ui" }.Select(x => Pack.Stock(x)!)
            .Concat(StockExtensions.All.SelectMany(x => x.Packs))
            .SelectMany(pack => (pack.Data["vocabulary"] as JsonObject ?? []).SelectMany(designer => pack.Vocabulary(designer.Key)))
            .Select(x => x["kind"]?.GetValue<string>())
            .OfType<string>();
        var used = byChecks.Concat(SpecDocuments.CommonDriftKinds(Paths.Conversion)).Concat(byStockVocabulary).ToHashSet(StringComparer.Ordinal);

        AcceptanceData.Kinds.Keys.Where(x => !used.Contains(x)).ShouldBeEmpty();
    }

    [Fact]
    public void Every_Common_Drift_kind_is_in_the_catalog()
    {
        var kinds = SpecDocuments.CommonDriftKinds(Paths.Conversion);

        kinds.ShouldNotBeEmpty("conversion.md lists the Common.Drift kinds under '### Kinds'.");
        kinds.Where(x => !AcceptanceData.Kinds.ContainsKey(x)).ShouldBeEmpty();
    }

    [Fact]
    public void The_claim_catalog_summary_counts_each_check_by_its_first_disposition()
    {
        var summary = SpecDocuments.CatalogSummary(Paths.ClaimCatalog);
        foreach (var source in SpecDocuments.Sources)
        {
            var counted = AcceptanceData.Dispositions
                .Where(x => x.Key.StartsWith(source + "-", StringComparison.Ordinal))
                .GroupBy(x => SummaryColumn(x.Value[0]))
                .ToDictionary(x => x.Key, x => x.Count());
            foreach (var column in new[] { "Claim", "Scope", "Engine", "Opt-in", "Model rule", "Dropped" })
            {
                counted.GetValueOrDefault(column).ShouldBe(summary[source][column], $"claim-catalog.md's summary, {source}, {column}");
            }
        }
    }

    private static bool Known(string disposition)
    {
        var (type, name) = disposition.IndexOf(':') is var colon and > 0 ? (disposition[..colon], disposition[(colon + 1)..]) : (disposition, "");
        return type switch
        {
            "claim" => AcceptanceData.Kinds.ContainsKey(name),
            "scope" => AcceptanceData.ScopeCases.Contains(name),
            "engine" => AcceptanceData.EngineCases.Contains(name),
            "optin" => AcceptanceData.OptInRules.Contains(name),
            "model-rule" => true,
            "dropped" => name is "inventory" or "codeOnly",
            _ => false
        };
    }

    private static string SummaryColumn(string disposition)
    {
        return disposition.Split(':')[0] switch
        {
            "claim" => "Claim",
            "scope" => "Scope",
            "engine" => "Engine",
            "optin" => "Opt-in",
            "model-rule" => "Model rule",
            "dropped" => "Dropped",
            var other => throw new InvalidOperationException($"Unhandled disposition type '{other}'.")
        };
    }
}
