using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// One run reads several languages, as the Java eShop's Java and TypeScript, and compares each file's code under its own
/// language's profile (C3): its names, its types and its nullability. Under one profile for the run, the language with
/// fewer files would read as drift.
/// </summary>
public class MixedLanguageTests
{
    private static readonly JsonNode Model = JsonNode.Parse("""
        {
          "designers": {
            "Domain": {
              "packages": [{
                "id": "domain", "name": "Shop.Domain",
                "elements": [
                  { "id": "order", "type": "Class", "name": "Order",
                    "children": [ { "id": "order-reference", "type": "Attribute", "name": "Reference", "typeRef": { "type": "string" } } ] },
                  { "id": "customer", "type": "Class", "name": "Customer",
                    "children": [ { "id": "customer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } } ] },
                  { "id": "invoice", "type": "Class", "name": "Invoice",
                    "children": [ { "id": "invoice-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" } } ] }
                ]
              }]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
            }
          }
        }
        """)!;

    private const string Order = """
        namespace Shop.Domain;

        public class Order
        {
            public string Reference { get; set; }
        }
        """;

    private const string Customer = """
        namespace Shop.Domain;

        public class Customer
        {
            public string Name { get; set; }
        }
        """;

    private const string Invoice = """
        package com.shop.billing;

        import java.math.BigDecimal;

        public class Invoice {
            private BigDecimal total;
        }
        """;

    [Fact]
    public void Each_file_is_read_under_its_own_languages_profile()
    {
        var result = Run(Invoice);
        result.Files.Select(x => x.Reader).Distinct().Order().ShouldBe(["csharp", "java"]);
        Outcome(result, "member.exists", "order-reference").ShouldBe("verified");
        Outcome(result, "member.exists", "invoice-total").ShouldBe("verified");
        Outcome(result, "member.type", "invoice-total").ShouldBe("verified");
        Outcome(result, "member.type", "order-reference").ShouldBe("verified");
    }

    [Fact]
    public void A_type_the_other_language_would_accept_is_still_drift_in_its_own()
    {
        // C#'s profile reads `decimal` as a decimal; Java's doesn't.
        var result = Run(Invoice.Replace("private BigDecimal total;", "private decimal total;", StringComparison.Ordinal));
        Outcome(result, "member.type", "invoice-total").ShouldBe("contradicted");
    }

    private static string Outcome(RunResult result, string kind, string element) => result.Claims.Single(x => x.Kind == kind && x.Element == element).Outcome;

    private static RunResult Run(string invoice)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-mixed-").FullName;
        try
        {
            Write(root, "src/Domain/Order.cs", Order);
            Write(root, "src/Domain/Customer.cs", Customer);
            Write(root, "src/billing/com/shop/billing/Invoice.java", invoice);
            string[] stock = ["intent.core", "intent.domain"];
            var packs = stock.Select(Pack.Stock).OfType<Pack>().ToList();
            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(Model), root, packs, extensions));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static void Write(string root, string path, string content)
    {
        var file = Path.Combine(root, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, content);
    }
}
