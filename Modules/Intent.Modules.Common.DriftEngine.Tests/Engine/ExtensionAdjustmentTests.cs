using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// A module adjusts what the engine does through its public API, never by reflection or by copying the engine's internals:
/// it runs a stock opt-in rule and keeps what it wants of the result (<see cref="StockOptInRules"/>), adjusts the reading the
/// claim readers after its own give (<see cref="ClaimContext.ReadNext"/>), changes one field of a stock convention and keeps
/// the rest, and asks which Drift Scope row decides a file (<see cref="CodeScope.RowOf"/>). Each was something the eShop
/// house module had to rebuild for itself.
/// </summary>
public class ExtensionAdjustmentTests
{
    private const string OrderClass = """
        namespace Shop.Domain;

        public class Order
        {
            public Guid Id { get; set; }

            public string LegacyCode { get; set; } = "";

            public string Note { get; set; } = "";

            public void Cancel()
            {
            }
        }
        """;

    private const string OrderRepository = """
        namespace Shop.Domain;

        public interface IOrderRepository
        {
            Task<Order?> FindAsync(Guid id);
        }

        public class SqlOrderRepository : IOrderRepository
        {
            public Task<Order?> FindAsync(Guid id) => Task.FromResult<Order?>(null);
        }
        """;

    [Fact]
    public void A_modules_rule_runs_the_stock_rule_and_keeps_what_it_wants()
    {
        var result = Run(OrderClass, check: true, extensions: [new LegacyMembersExtension()]);
        var members = result.Findings.Where(x => x.Rule == "optin/unmodelled.member").Select(x => x.Message).ToList();
        members.ShouldContain(x => x.Contains("`Order.Note`"));
        members.ShouldContain(x => x.Contains("`Order.Cancel`"));
        members.ShouldNotContain(x => x.Contains("LegacyCode"));
    }

    [Fact]
    public void A_reader_adjusts_what_the_readers_after_it_read()
    {
        var result = Run(OrderClass, packs: [NotePack], extensions: [new FirstNote(), new AdjustingNote()]);
        var note = result.Claims.Single(x => x.Kind == "house.note");
        note.Outcome.ShouldBe("verified");
        note.Reading!.Value.ShouldBe("first, adjusted");
    }

    [Fact]
    public void With_no_reader_after_it_ReadNext_is_null_and_the_engine_reads_the_claim()
    {
        var result = Run(OrderClass, packs: [NotePack], extensions: [new AdjustingNote()]);
        var note = result.Claims.Single(x => x.Kind == "house.note");
        note.Outcome.ShouldBe("cantVerify");
        note.Reason.ShouldBe("noRule");
    }

    [Fact]
    public void A_house_convention_changes_one_field_and_keeps_the_stock_packs_others()
    {
        // The house says what a Class and a Repository state, and nothing of their type kinds: the stock pack's still apply,
        // so the Repository with no type of its name is still the interface C# names it (IOrderRepository), and the class's
        // public method isn't asked for. Before, the house convention replaced the stock one whole, type kinds and all.
        var result = Run(OrderClass, check: true, packs: [StatesPack], files: new() { ["src/Domain/OrderRepository.cs"] = OrderRepository });
        result.Claims.Single(x => x.Kind == "type.exists" && x.Element == "order-repository").Outcome.ShouldBe("verified");
        var members = result.Findings.Where(x => x.Rule == "optin/unmodelled.member").Select(x => x.Message).ToList();
        members.ShouldContain(x => x.Contains("`Order.Note`"));
        members.ShouldNotContain(x => x.Contains("Cancel"));
        members.ShouldNotContain(x => x.Contains("FindAsync"));
    }

    [Fact]
    public void The_scope_says_which_row_decides_a_file_and_matches_globs_as_its_rows_do()
    {
        var result = Run(OrderClass, extensions: [new RowsExtension()]);
        result.Findings.Where(x => x.Rule == "house/rows").Select(x => x.Message).ShouldBe(
            ["other/Readme.cs: none", "src/Api/Startup.cs: Context (composition root)", "src/Domain/Order.cs: In"], ignoreOrder: true);
        CodeScope.GlobMatches("src/**/Order.cs", "src/Domain/Order.cs").ShouldBeTrue();
        CodeScope.GlobMatches("src/*.cs", "src/Domain/Order.cs").ShouldBeFalse();
    }

    [Fact]
    public void A_rows_glob_lines_skip_comments_and_the_last_matching_line_decides()
    {
        string[] lines = ["# What the Domain team has modelled", "src/**", "!src/Legacy/**", "src/Legacy/Kept/**"];
        CodeScope.GlobMatches(lines, "src/Domain/Order.cs").ShouldBeTrue();
        CodeScope.GlobMatches(lines, "src/Legacy/Order.cs").ShouldBeFalse();
        CodeScope.GlobMatches(lines, "src/Legacy/Kept/Order.cs").ShouldBeTrue();
        CodeScope.GlobMatches(lines, "tools/Seed.cs").ShouldBeFalse();
        CodeScope.GlobMatches(["# Only a note"], "tools/Seed.cs").ShouldBeTrue();
        CodeScope.GlobMatches(["!src/Legacy/**"], "src/Domain/Order.cs").ShouldBeFalse();
    }

    private static readonly Pack NotePack = Pack.Parse(JsonNode.Parse("""
        {
          "id": "house.notes",
          "version": "1.0.0",
          "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
          "designers": ["Domain"],
          "kinds": [
            { "kind": "house.note", "label": "note", "valueType": "text", "comparer": "exact", "stated": "always", "existence": false }
          ],
          "vocabulary": {
            "Domain": {
              "entries": [
                { "kind": "house.note", "for": { "element": "Class" }, "value": "'first, adjusted'", "dependsOn": "type.exists", "source": "the house's note" }
              ]
            }
          }
        }
        """)!);

    private static readonly Pack StatesPack = Pack.Parse(JsonNode.Parse("""
        {
          "id": "house.states",
          "version": "1.0.0",
          "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
          "designers": ["Domain"],
          "conventions": {
            "Domain": { "designer": "Domain", "types": { "Class": { "states": ["property"] }, "Repository": { "states": [] } } }
          }
        }
        """)!);

    private static RunResult Run(string order, bool check = false, IReadOnlyList<Pack>? packs = null, IReadOnlyList<IDriftExtension>? extensions = null,
        Dictionary<string, string>? files = null)
    {
        var stereotypes = new JsonObject
        {
            ["Drift Scope"] = JsonNode.Parse("""
                { "Entries": [
                  { "Glob": "src/**", "Scope": "In" },
                  { "Glob": "src/Api/**", "Scope": "Context", "Reason": "composition root" }
                ] }
                """)
        };
        if (check)
        {
            stereotypes["Drift Check"] = JsonNode.Parse("""{ "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }""");
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
        if (files is not null)
        {
            ((JsonArray)model["designers"]!["Domain"]!["packages"]![0]!["elements"]!).Add(JsonNode.Parse("""{ "id": "order-repository", "type": "Repository", "name": "OrderRepository" }"""));
        }

        var root = Directory.CreateTempSubdirectory("driftengine-adjust-").FullName;
        try
        {
            Write(root, "src/Domain/Order.cs", order);
            foreach (var (path, content) in files ?? [])
            {
                Write(root, path, content);
            }

            Write(root, "src/Api/Startup.cs", "namespace Shop.Api;\n\npublic static class Startup\n{\n}\n");
            Write(root, "other/Readme.cs", "namespace Other;\n\npublic class Readme\n{\n}\n");
            string[] stock = ["intent.core", "intent.domain"];
            var all = stock.Select(Pack.Stock).OfType<Pack>().Concat(packs ?? []).ToList();
            var with = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).Concat(extensions ?? []).ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(model), root, all, with));
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

    /// <summary>Replaces <c>unmodelled.member</c> with the stock rule less what its house keeps unmodelled on purpose.</summary>
    private sealed class LegacyMembersExtension : IDriftExtension
    {
        public string Id => "house.legacy";

        public IEnumerable<IOptInRule> OptInRules => [new WithoutLegacy()];

        private sealed class WithoutLegacy : IOptInRule
        {
            public string Id => "unmodelled.member";

            public IEnumerable<UnmodelledCode> Find(RuleContext context) =>
                StockOptInRules.Find(Id)!.Find(context).Where(x => !x.Name.Contains(".Legacy", StringComparison.Ordinal));
        }
    }

    /// <summary>An earlier extension's reader of the note: what the code states, as it reads it.</summary>
    private sealed class FirstNote : IDriftExtension
    {
        public string Id => "house.first";

        public IEnumerable<IClaimReader> ClaimReaders => [new Reader()];

        private sealed class Reader : IClaimReader
        {
            public IReadOnlyCollection<string> Kinds { get; } = ["house.note"];

            public ClaimReading? Read(ClaimContext context) => ClaimReading.Stated("first", context.Type!.At);
        }
    }

    /// <summary>A later extension's reader that adjusts what the readers after it read, and passes on when none reads it.</summary>
    private sealed class AdjustingNote : IDriftExtension
    {
        public string Id => "house.adjusting";

        public IEnumerable<IClaimReader> ClaimReaders => [new Reader()];

        private sealed class Reader : IClaimReader
        {
            public IReadOnlyCollection<string> Kinds { get; } = ["house.note"];

            public ClaimReading? Read(ClaimContext context) =>
                context.ReadNext() is { From: "code", Value: { } value, At: { } at } ? ClaimReading.Stated($"{value}, adjusted", at) : null;
        }
    }

    /// <summary>Reports which row decides each file, so the test sees what <see cref="CodeScope.RowOf"/> says.</summary>
    private sealed class RowsExtension : IDriftExtension
    {
        public string Id => "house.rows";

        public IEnumerable<IDriftRule> Rules => [new Rows()];

        private sealed class Rows : IDriftRule
        {
            public string Id => "house/rows";

            public IEnumerable<Finding> Check(RuleContext context) => context.Scope.Files
                .Where(x => x.EndsWith(".cs", StringComparison.Ordinal))
                .Select(x => new Finding("house/rows", "Rows", "Code", "warning",
                    $"{x}: {(context.Scope.RowOf(x) is { } row ? row.Scope + (row.Reason is { } reason ? $" ({reason})" : "") : "none")}",
                    new Anchor(File: "src/Domain/Order.cs", Line: 1)));
        }
    }
}
