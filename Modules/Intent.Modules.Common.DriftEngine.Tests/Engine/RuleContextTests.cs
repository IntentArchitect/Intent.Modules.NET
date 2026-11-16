using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// A module's rule reads the model as a claim source does (C4): its stereotypes and links, a type compared as the engine
/// compares one, and the packages the application references from another application, which nothing claims on.
/// </summary>
public class RuleContextTests
{
    private static readonly JsonNode Model = JsonNode.Parse("""
        {
          "designers": {
            "Domain": {
              "packages": [{
                "id": "domain", "name": "Shop.Domain",
                "elements": [
                  { "id": "order", "type": "Class", "name": "Order", "stereotypes": { "Audited": { "Log": "orders" } },
                    "children": [
                      { "id": "order-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" } },
                      { "id": "order-placed", "type": "Attribute", "name": "Placed", "typeRef": { "type": "datetime" } }
                    ] }
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

    private static readonly JsonNode Billing = JsonNode.Parse("""
        {
          "designers": {
            "Services": {
              "packages": [{
                "id": "billing", "name": "Billing.Services",
                "elements": [
                  { "id": "invoices-service", "type": "Service", "name": "InvoicesService",
                    "children": [ { "id": "create-invoice", "type": "Operation", "name": "CreateInvoice" } ] }
                ]
              }]
            }
          }
        }
        """)!;

    private const string Order = """
        namespace Shop.Domain;

        public class Order
        {
            public decimal Total { get; set; }

            public string Placed { get; set; }
        }
        """;

    [Fact]
    public void A_rule_reads_stereotypes_and_compares_types_as_the_engine_does()
    {
        var findings = Run().Findings.Where(x => x.Rule == "house/audit").Select(x => x.Message).ToList();
        findings.ShouldContain("Order logs to orders.");
        findings.ShouldContain("Total matches.");
        findings.ShouldContain("Placed differs.");
    }

    [Fact]
    public void A_rule_reaches_another_applications_packages_but_nothing_claims_on_them()
    {
        var result = Run();
        result.Findings.ShouldContain(x => x.Rule == "house/audit" && x.Message == "References InvoicesService.CreateInvoice.");
        result.Claims.ShouldNotContain(x => x.Element == "invoices-service" || x.Element == "create-invoice");
    }

    private static RunResult Run()
    {
        var root = Directory.CreateTempSubdirectory("driftengine-rulecontext-").FullName;
        try
        {
            var file = Path.Combine(root, "src/Domain/Order.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            File.WriteAllText(file, Order);
            string[] stock = ["intent.core", "intent.domain", "intent.services"];
            var packs = stock.Select(Pack.Stock).OfType<Pack>().ToList();
            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).Append(new HouseExtension()).ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(Model), root, packs, extensions) { References = FixtureModel.Build(Billing) });
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class HouseExtension : IDriftExtension
    {
        public string Id => "house";

        public IEnumerable<IDriftRule> Rules => [new Audit()];
    }

    private sealed class Audit : IDriftRule
    {
        public string Id => "house/audit";

        public IEnumerable<Finding> Check(RuleContext context)
        {
            var model = context.Model;
            foreach (var element in model.Elements("Domain", "Class").Where(x => model.HasStereotype(x, "Audited")))
            {
                yield return Note($"{element.Name} logs to {model.Stereotype(element, "Audited", "Log")}.");
                foreach (var attribute in element.ChildElements)
                {
                    if (context.MemberOf("Domain", attribute) is { Type: { } type })
                    {
                        yield return Note($"{attribute.Name} {(context.TypeMatches(attribute.TypeReference, type) ? "matches" : "differs")}.");
                    }
                }
            }

            foreach (var operation in model.ReferencedElements("Services").Where(x => x.SpecializationType == "Operation"))
            {
                yield return Note($"References {model.Owner(operation)?.Name}.{operation.Name}.");
            }
        }

        private static Finding Note(string message) => new("house/audit", "House", "Code", "warning", message, new Anchor(Text: "audit"));
    }
}
