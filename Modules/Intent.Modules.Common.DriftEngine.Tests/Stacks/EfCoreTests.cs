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

    /// <summary>
    /// A class the model marks no key on, whose code has one, is marked as the Domain designer does: Primary Key on the
    /// attribute of the key's name, else a new attribute of its name and type. eShop10's Ordering entities inherit SeedWork's
    /// <c>int Id</c>, which its v1 model left out. A key of a type the profile doesn't map (a value object) gives no correction:
    /// the model's type for it can't be named from the code.
    /// </summary>
    [Fact]
    public void A_key_the_model_leaves_out_is_marked_by_its_correction()
    {
        var model = JsonNode.Parse("""
            {
              "designers": {
                "Domain": {
                  "packages": [{
                    "id": "domain", "name": "Ordering.Domain",
                    "elements": [
                      { "id": "buyer", "type": "Class", "name": "Buyer", "stereotypes": { "Table": {} },
                        "children": [ { "id": "buyer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } } ] },
                      { "id": "card", "type": "Class", "name": "Card", "stereotypes": { "Table": {} },
                        "children": [ { "id": "card-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } } ] },
                      { "id": "line", "type": "Class", "name": "Line", "stereotypes": { "Table": {} } }
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
            ["src/Domain/Entity.cs"] = "namespace Ordering.Domain; public abstract class Entity { public virtual int Id { get; protected set; } }",
            ["src/Domain/Buyer.cs"] = "namespace Ordering.Domain; public class Buyer : Entity { public string Name { get; set; } }",
            ["src/Domain/Card.cs"] = "namespace Ordering.Domain; public class Card { public System.Guid Id { get; set; } }",
            ["src/Domain/Line.cs"] = "namespace Ordering.Domain; public class Line { public LineNumber Number { get; set; } } public record LineNumber(int Value);",
            ["src/Infrastructure/OrderingContext.cs"] = """
                using Microsoft.EntityFrameworkCore;
                using Ordering.Domain;

                namespace Ordering.Infrastructure;

                public class OrderingContext : DbContext
                {
                    public DbSet<Buyer> Buyers { get; set; }
                    public DbSet<Card> Cards { get; set; }
                    public DbSet<Line> Lines { get; set; }

                    protected override void OnModelCreating(ModelBuilder modelBuilder)
                    {
                        modelBuilder.Entity<Line>(b => b.HasKey(x => x.Number));
                    }
                }
                """
        };

        var result = Run(model, code);

        KeyFinding(result, "buyer").Corrections.ShouldBe([new ModelCorrection(ModelCorrection.AddAttribute, "buyer") { Name = "Id", Type = "int", Stereotype = "Primary Key" }]);
        var card = KeyFinding(result, "card").Corrections.ShouldHaveSingleItem();
        (card.Kind, card.Element, card.Stereotype, card.Properties!.Count).ShouldBe((ModelCorrection.SetStereotype, "card-id", "Primary Key", 0));
        KeyFinding(result, "line").Corrections.ShouldBeEmpty();
    }

    private static Finding KeyFinding(RunResult result, string element) =>
        result.Findings.Single(x => x.Rule == "claim/key" && x.Subject.Element == element);

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
