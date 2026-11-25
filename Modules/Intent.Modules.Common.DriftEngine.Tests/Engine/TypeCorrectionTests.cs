using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// What a type the code doesn't have gives a tool bringing a model drawn from unchanged code into line
/// (<see cref="ModelCorrection"/>): an element that states nothing but its name, with no type of the name anywhere in scope, is
/// removed. eShop10's User Interface held an empty Model Definition, <c>ModelType</c>, that v1 never checked.
/// </summary>
public class TypeCorrectionTests
{
    [Fact]
    public void An_empty_element_no_code_declares_is_removed_by_its_correction()
    {
        var result = Run("""
            [
              { "id": "stray", "type": "Class", "name": "Stray" },
              { "id": "planned", "type": "Class", "name": "Planned",
                "children": [ { "id": "planned-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } } ] }
            ]
            """);

        Missing(result, "stray").Corrections.ShouldBe([new ModelCorrection(ModelCorrection.RemoveElement, "stray")]);
        // One with members states more than its name, which removing it would lose: that is the developer's call.
        Missing(result, "planned").Corrections.ShouldBeEmpty();
    }

    private static Finding Missing(RunResult result, string element) =>
        result.Findings.Single(x => x.Rule == "claim/type.exists" && x.Subject.Element == element);

    private static RunResult Run(string elements)
    {
        var model = JsonNode.Parse($$"""
            {
              "designers": {
                "Domain": { "packages": [{ "id": "domain", "name": "Shop.Domain", "elements": {{elements}} }] },
                "Codebase Structure": {
                  "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                    "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
                }
              }
            }
            """)!;
        var root = Directory.CreateTempSubdirectory("driftengine-types-").FullName;
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "src", "Domain"));
            File.WriteAllText(Path.Combine(root, "src", "Domain", "Order.cs"), "namespace Shop.Domain; public class Order { }");
            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(model), root, [Pack.Stock("intent.domain")!], extensions));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
