using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// Each element is linked to the code its verified claims read, which Intent Architect's Changes Review counts as verified
/// (user decision, 2026-11-17: only verified, modelled code). A class is linked without its members, each of which is its own
/// element's; a member no claim located and a line with drift are linked to nothing, and an element whose code is all drift
/// is linked to its file. Offsets are into the file's text as <c>File.ReadAllText</c> reads it, line breaks and all.
/// </summary>
public class CodeLinkTests
{
    private static readonly JsonNode Model = JsonNode.Parse("""
        {
          "designers": {
            "Domain": {
              "packages": [{
                "id": "domain", "name": "Shop.Domain",
                "elements": [
                  { "id": "order", "type": "Class", "name": "Order",
                    "children": [
                      { "id": "order-total", "type": "Attribute", "name": "Total", "typeRef": { "type": "decimal" } },
                      { "id": "order-reference", "type": "Attribute", "name": "Reference", "typeRef": { "type": "string" } }
                    ] },
                  { "id": "buyer", "type": "Class", "name": "Buyer",
                    "children": [ { "id": "buyer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } } ] }
                ]
              }],
              "associations": [
                { "id": "order-buyer", "type": "Association",
                  "source": { "element": "order", "name": "Orders", "multiplicity": "*", "navigable": true },
                  "target": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": true } }
              ]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
            }
          }
        }
        """)!;

    private const string Project = """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net8.0</TargetFramework>
            <Nullable>enable</Nullable>
          </PropertyGroup>
        </Project>
        """;

    // Reference's type has drifted from the model's string, and nothing models Recalculate.
    private const string Order = """
        namespace Shop.Domain;

        public class Order
        {
            public decimal Total { get; set; }

            public int Reference { get; set; }

            public Buyer Buyer { get; set; }

            public void Recalculate()
            {
                Total = 0;
            }
        }
        """;

    private const string Buyer = """
        namespace Shop.Domain;

        public class Buyer
        {
            public string Name { get; set; }

            public List<Order> Orders { get; set; }
        }
        """;

    [Fact]
    public void A_class_is_linked_without_its_members()
    {
        var (result, texts) = Run(Model);
        Linked(result, texts, "order").ShouldBe(["public class Order\r\n{", "}"]);
        Linked(result, texts, "order-total").ShouldBe(["public decimal Total { get; set; }"]);
    }

    [Fact]
    public void Code_no_claim_located_is_linked_to_nothing()
    {
        var (result, texts) = Run(Model);
        result.Links.Where(x => x.Start is not null).Select(x => Text(texts, x))
            .ShouldNotContain(x => x.Contains("Recalculate", StringComparison.Ordinal) || x.Contains("Total = 0", StringComparison.Ordinal));
    }

    [Fact]
    public void An_element_whose_code_has_drift_is_linked_to_its_file_with_no_lines()
    {
        var (result, _) = Run(Model);
        result.Claims.Single(x => x.Kind == "member.type" && x.Element == "order-reference").Outcome.ShouldBe("contradicted");
        result.Links.Where(x => x.Element == "order-reference").ShouldBe([new CodeLink("order-reference", "src/Domain/Order.cs")]);
    }

    [Fact]
    public void An_association_end_is_linked_by_its_association()
    {
        var (result, texts) = Run(Model);
        Linked(result, texts, "order-buyer").ShouldBe(["public Buyer Buyer { get; set; }", "public List<Order> Orders { get; set; }"], ignoreOrder: true);
        result.Links.ShouldNotContain(x => x.Element == "order-buyer:source");
    }

    [Fact]
    public void Without_a_Drift_Scope_nothing_is_linked()
    {
        var model = Model.DeepClone();
        model["designers"]!.AsObject().Remove("Codebase Structure");
        Run(model).Result.Links.ShouldBeEmpty();
    }

    private static readonly JsonNode Persisted = JsonNode.Parse("""
        {
          "designers": {
            "Domain": {
              "packages": [{
                "id": "domain", "name": "Shop.Domain",
                "elements": [
                  { "id": "order", "type": "Class", "name": "Order", "stereotypes": { "Table": {} },
                    "children": [
                      { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
                      { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "int" },
                        "stereotypes": { "Foreign Key": { "Association": { "ref": "order-buyer" } } } }
                    ] },
                  { "id": "buyer", "type": "Class", "name": "Buyer", "stereotypes": { "Table": {} },
                    "children": [ { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } } ] }
                ]
              }],
              "associations": [
                { "id": "order-buyer", "type": "Association",
                  "source": { "element": "order", "name": "Orders", "multiplicity": "*", "navigable": true },
                  "target": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": true } }
              ]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
            }
          }
        }
        """)!;

    // The constraint's name is nothing the model states.
    private const string Context = """
        using Microsoft.EntityFrameworkCore;

        namespace Shop.Domain;

        public class ShopContext : DbContext
        {
            public DbSet<Order> Orders { get; set; }

            public DbSet<Buyer> Buyers { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Order>(entity =>
                {
                    entity.HasOne(d => d.Buyer)
                        .WithMany(p => p.Orders)
                        .HasForeignKey(d => d.BuyerId)
                        .HasConstraintName("FK_Orders_Buyers");
                });
            }
        }
        """;

    [Fact]
    public void A_relationship_is_linked_through_the_calls_that_pick_it_out()
    {
        var (result, texts) = Run(Persisted, new Dictionary<string, string>
        {
            ["src/Domain/Order.cs"] = """
                namespace Shop.Domain;

                public class Order
                {
                    public int Id { get; set; }

                    public int BuyerId { get; set; }

                    public Buyer Buyer { get; set; }
                }
                """,
            ["src/Domain/Buyer.cs"] = """
                namespace Shop.Domain;

                public class Buyer
                {
                    public int Id { get; set; }

                    public List<Order> Orders { get; set; }
                }
                """,
            ["src/Domain/ShopContext.cs"] = Context
        }, efCore: true);
        result.Claims.Single(x => x.Kind == "relationship.foreignKey").Outcome.ShouldBe("verified");

        var lines = texts["src/Domain/ShopContext.cs"].Split("\r\n");
        int LineOf(string text) => Array.FindIndex(lines, x => x.Contains(text, StringComparison.Ordinal)) + 1;
        LinkedLines(result, texts, "order-buyer", "src/Domain/ShopContext.cs")
            .ShouldBe([LineOf("entity.HasOne("), LineOf(".WithMany("), LineOf(".HasForeignKey(")]);
    }

    /// <summary>The text of each line-range link of the element.</summary>
    private static List<string> Linked(RunResult result, IReadOnlyDictionary<string, string> texts, string element) =>
        result.Links.Where(x => x.Element == element && x.Start is not null).Select(x => Text(texts, x)).ToList();

    /// <summary>The 1-based lines the element's links cover in the file, as Intent Architect turns a range into lines.</summary>
    private static List<int> LinkedLines(RunResult result, IReadOnlyDictionary<string, string> texts, string element, string file) =>
        result.Links.Where(x => x.Element == element && x.File == file && x.Start is not null)
            .SelectMany(x => Enumerable.Range(LineAt(texts[file], x.Start!.Value), LineAt(texts[file], x.Start!.Value + x.Length!.Value - 1) - LineAt(texts[file], x.Start!.Value) + 1))
            .Distinct()
            .Order()
            .ToList();

    private static int LineAt(string text, int offset) => text[..offset].Count(x => x == '\n') + 1;

    private static string Text(IReadOnlyDictionary<string, string> texts, CodeLink link) => texts[link.File].Substring(link.Start!.Value, link.Length!.Value);

    private static (RunResult Result, IReadOnlyDictionary<string, string> Texts) Run(JsonNode model) =>
        Run(model, new Dictionary<string, string> { ["src/Domain/Shop.Domain.csproj"] = Project, ["src/Domain/Order.cs"] = Order, ["src/Domain/Buyer.cs"] = Buyer });

    /// <summary>The run, and each file's text as Intent Architect reads it, with Windows line breaks.</summary>
    private static (RunResult Result, IReadOnlyDictionary<string, string> Texts) Run(JsonNode model, IReadOnlyDictionary<string, string> files, bool efCore = false)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-links-").FullName;
        try
        {
            foreach (var (path, content) in files)
            {
                Write(root, path, content);
            }

            string[] stock = efCore ? ["intent.core", "intent.domain", "intent.efcore"] : ["intent.core", "intent.domain"];
            var packs = stock.Select(Pack.Stock).OfType<Pack>().ToList();
            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any() || efCore && x.Id == "intent.efcore").ToList();
            var result = DriftEngine.Run(new DriftInput(FixtureModel.Build(model), root, packs, extensions));
            var texts = result.Links.Select(x => x.File).Distinct().ToDictionary(x => x, x => File.ReadAllText(Path.Combine(root, x)));
            return (result, texts);
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
        File.WriteAllText(file, content.ReplaceLineEndings("\r\n"));
    }
}
