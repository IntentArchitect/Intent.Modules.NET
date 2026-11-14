using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// A module's own "must be modelled" rule finds code and nothing else: the engine turns it on from the Root Folder's
/// <c>Drift Check</c>, keeps what a Context row decides and what an exception matches out of the report, and applies the
/// model's severity overrides, as it does for its own rules. A module's <see cref="IDriftRule"/> finding about a Context
/// file is dropped too: that code is read, not checked.
/// </summary>
public class OptInRuleTests
{
    private const string Order = """
        namespace Shop.Domain;

        public class Order
        {
            public Guid Id { get; set; }
        }
        """;

    private const string PlaceOrderHandler = """
        namespace Shop.Domain;

        public class PlaceOrderHandler
        {
        }
        """;

    private const string LegacyHandler = """
        namespace Shop.Domain;

        public class LegacyHandler
        {
        }
        """;

    private const string ApiHandler = """
        namespace Shop.Api;

        public class ApiHandler
        {
        }
        """;

    [Fact]
    public void A_modules_opt_in_rule_is_off_until_the_Drift_Check_lists_it()
    {
        var result = Run(check: null);
        result.Findings.ShouldNotContain(x => x.Rule == "optin/house.handler");
    }

    [Fact]
    public void Turned_on_it_reports_checked_code_and_not_Context_code()
    {
        var result = Run("""{ "Opt-in Rules": [ { "Rule": "optin/house.handler" } ] }""");
        var findings = result.Findings.Where(x => x.Rule == "optin/house.handler").ToList();
        findings.Select(x => x.Subject.File).ShouldBe(["src/Domain/LegacyHandler.cs", "src/Domain/PlaceOrderHandler.cs"]);
        findings.ShouldAllBe(x => x.Severity == "error" && x.Area == "Domain (DriftEngine)" && x.Subject.Line == 3);
    }

    [Fact]
    public void An_exception_keeps_its_code_out_and_one_matching_nothing_is_stale()
    {
        var result = Run("""
            {
              "Opt-in Rules": [ { "Rule": "optin/house.handler" } ],
              "Exceptions": [
                { "Rule": "optin/house.handler", "Match": "LegacyHandler", "Reason": "Being replaced" },
                { "Rule": "optin/house.handler", "Match": "src/Domain/Gone/**", "Reason": "Removed last release" }
              ]
            }
            """);
        result.Findings.Where(x => x.Rule == "optin/house.handler").Select(x => x.Subject.File).ShouldBe(["src/Domain/PlaceOrderHandler.cs"]);
        result.Findings.Single(x => x.Rule == "optin/staleException").Message.ShouldContain("src/Domain/Gone/**");
    }

    [Fact]
    public void A_severity_override_applies_to_a_modules_rule()
    {
        var result = Run("""
            {
              "Opt-in Rules": [ { "Rule": "optin/house.handler" } ],
              "Severity Overrides": [ { "Rule": "optin/house.*", "Severity": "Warning", "Reason": "Handlers are being modelled this quarter" } ]
            }
            """);
        result.Findings.Where(x => x.Rule == "optin/house.handler").ShouldAllBe(x => x.Severity == "warning");
        result.Findings.Count(x => x.Rule == "optin/house.handler").ShouldBe(2);
    }

    [Fact]
    public void A_rule_the_installed_modules_dont_provide_is_a_configuration_error()
    {
        var result = Run("""{ "Opt-in Rules": [ { "Rule": "optin/house.missing" } ] }""");
        result.Findings.Single(x => x.Rule == "config/unknownRule").Message.ShouldContain("optin/house.handler");
    }

    [Fact]
    public void A_modules_rule_finding_about_a_Context_file_is_dropped()
    {
        var result = Run(check: null);
        result.Findings.Where(x => x.Rule == "house/handlerSuffix").Select(x => x.Subject.File)
            .ShouldBe(["src/Domain/LegacyHandler.cs", "src/Domain/PlaceOrderHandler.cs"], ignoreOrder: true);
    }

    private static RunResult Run(string? check)
    {
        var stereotypes = new JsonObject
        {
            ["Drift Scope"] = JsonNode.Parse("""
                { "Entries": [
                  { "Glob": "src/**", "Scope": "In" },
                  { "Glob": "src/Api/**", "Scope": "Context", "Reason": "The composition root, read for what it registers" }
                ] }
                """)
        };
        if (check is not null)
        {
            stereotypes["Drift Check"] = JsonNode.Parse(check);
        }

        var model = JsonNode.Parse("""
            {
              "designers": {
                "Domain": {
                  "packages": [{
                    "id": "domain", "name": "Shop.Domain",
                    "elements": [
                      { "id": "order", "type": "Class", "name": "Order",
                        "children": [ { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } } ] }
                    ]
                  }]
                },
                "Codebase Structure": {
                  "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder" }]
                }
              }
            }
            """)!;
        model["designers"]!["Codebase Structure"]!["packages"]![0]!["stereotypes"] = stereotypes;

        var root = Directory.CreateTempSubdirectory("driftengine-optin-").FullName;
        try
        {
            Write(root, "src/Domain/Order.cs", Order);
            Write(root, "src/Domain/PlaceOrderHandler.cs", PlaceOrderHandler);
            Write(root, "src/Domain/LegacyHandler.cs", LegacyHandler);
            Write(root, "src/Api/ApiHandler.cs", ApiHandler);
            string[] stock = ["intent.core", "intent.domain"];
            var packs = stock.Select(Pack.Stock).OfType<Pack>().ToList();
            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).Append(new HouseExtension()).ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(model), root, packs, extensions));
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

    private sealed class HouseExtension : IDriftExtension
    {
        public string Id => "house";

        public IEnumerable<IOptInRule> OptInRules => [new Handlers()];

        public IEnumerable<IDriftRule> Rules => [new HandlerSuffix()];
    }

    /// <summary>Every handler the model doesn't describe, wherever it is: the engine narrows it to checked code.</summary>
    private sealed class Handlers : IOptInRule
    {
        public string Id => "house.handler";

        public IEnumerable<UnmodelledCode> Find(RuleContext context) => context.Code.Types
            .Where(x => x.Name.EndsWith("Handler", StringComparison.Ordinal) && !context.IsLocated(x))
            .Select(x => new UnmodelledCode("Domain", x.At, x.Name, $"`{x.Name}` is a handler the model doesn't describe.", Type: x.Name));
    }

    /// <summary>A finding about every handler's file, Context or not: the engine drops the Context one.</summary>
    private sealed class HandlerSuffix : IDriftRule
    {
        public IEnumerable<Finding> Check(RuleContext context) => context.Code.Types
            .Where(x => x.Name.EndsWith("Handler", StringComparison.Ordinal))
            .Select(x => new Finding("house/handlerSuffix", "Handlers", "Code", "warning", $"`{x.Name}` is a handler.", new Anchor(File: x.At.File, Line: x.At.Line)));
    }
}
