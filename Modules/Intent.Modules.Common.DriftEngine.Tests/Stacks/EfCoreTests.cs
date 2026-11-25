using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// What EF reads that a tool bringing a model into line with unchanged code acts on: the model changes a finding the code
/// settles carries (<see cref="ModelCorrection"/>).
/// </summary>
public class EfCoreTests
{
    /// <summary>
    /// A property EF doesn't map, by <c>[NotMapped]</c> or its entity's <c>Ignore</c>, can't be an attribute of a persisted class,
    /// so the attribute's removal settles the <c>column.mapped</c> finding. A migrated EF6 application modelled its
    /// <c>[NotMapped]</c> enum wrappers of int columns as attributes.
    /// </summary>
    [Fact]
    public void An_unmapped_property_modelled_as_an_attribute_is_removed_by_its_correction()
    {
        var model = JsonNode.Parse("""
            {
              "designers": {
                "Domain": {
                  "packages": [{
                    "id": "domain", "name": "Ordering.Domain",
                    "elements": [
                      { "id": "order", "type": "Class", "name": "Order",
                        "stereotypes": { "Table": {} },
                        "children": [
                          { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
                          { "id": "order-status-id", "type": "Attribute", "name": "StatusId", "typeRef": { "type": "int" } },
                          { "id": "order-status", "type": "Attribute", "name": "Status", "typeRef": { "type": "int" } },
                          { "id": "order-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" } }
                        ] }
                    ]
                  }]
                },
                "Codebase Structure": {
                  "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
                    "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
                }
              }
            }
            """)!;
        var code = new Dictionary<string, string>
        {
            ["src/Domain/Order.cs"] = """
                using System.ComponentModel.DataAnnotations.Schema;

                namespace Ordering.Domain;

                public class Order
                {
                    public int Id { get; set; }

                    public int StatusId { get; set; }

                    [NotMapped]
                    public int Status { get => StatusId; set => StatusId = value; }

                    public decimal Total { get; set; }
                }
                """,
            ["src/Infrastructure/OrderConfiguration.cs"] = """
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Metadata.Builders;
                using Ordering.Domain;

                namespace Ordering.Infrastructure;

                public class OrderConfiguration : IEntityTypeConfiguration<Order>
                {
                    public void Configure(EntityTypeBuilder<Order> builder)
                    {
                        builder.Ignore(o => o.Total);
                    }
                }
                """
        };

        var result = Run(model, code);

        Corrections(result, "order-status").ShouldBe([new ModelCorrection(ModelCorrection.RemoveElement, "order-status")]);
        Corrections(result, "order-total").ShouldBe([new ModelCorrection(ModelCorrection.RemoveElement, "order-total")]);
        result.Findings.Where(x => x.Rule == "claim/column.mapped").Select(x => x.Subject.Element).Order().ShouldBe(["order-status", "order-total"]);
    }

    private static IReadOnlyList<ModelCorrection> Corrections(RunResult result, string element) =>
        result.Findings.Single(x => x.Rule == "claim/column.mapped" && x.Subject.Element == element).Corrections;

    private static RunResult Run(JsonNode model, IReadOnlyDictionary<string, string> code)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-efcore-").FullName;
        try
        {
            foreach (var (path, source) in code)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).Append(StockExtensions.Find("intent.efcore")!).ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(model), root, [Pack.Stock("intent.domain")!], extensions));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
