using System.Text.Json;
using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// A host whose process outlives a run, as the Software Factory's does, keeps the code's read for the next run over the same
/// output folder (<see cref="DriftInput.KeepCode"/>). It is reused only when nothing the read depends on has changed, and a run
/// over it gives exactly what a fresh read gives: what claims kept on the graph depends on the model, so it isn't kept.
/// </summary>
public class KeptCodeTests
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

    private static readonly IReadOnlyDictionary<string, string> Files = new Dictionary<string, string>
    {
        ["src/Domain/Shop.Domain.csproj"] = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <Nullable>enable</Nullable>
              </PropertyGroup>
            </Project>
            """,
        ["src/Domain/Order.cs"] = """
            namespace Shop.Domain;

            public class Order
            {
                public decimal Total { get; set; }

                public string Reference { get; set; } = "";
            }
            """,
        ["src/Domain/Buyer.cs"] = """
            namespace Shop.Domain;

            public class Buyer
            {
                public string Name { get; set; } = "";
            }
            """
    };

    [Fact]
    public void Two_runs_over_unchanged_code_give_identical_results_the_second_from_the_kept_read()
    {
        using var folder = new Folder(Files);
        var first = folder.Run(Model);
        var second = folder.Run(Model);

        Cached(first).ShouldBeFalse();
        Cached(second).ShouldBeTrue();
        Json(second).ShouldBe(Json(first));
        second.Links.ShouldBe(first.Links);
    }

    [Fact]
    public void A_touched_file_is_read_afresh()
    {
        using var folder = new Folder(Files);
        folder.Run(Model).Claims.Single(x => x.Kind == "member.type" && x.Element == "order-reference").Outcome.ShouldBe("verified");

        folder.Write("src/Domain/Order.cs", Files["src/Domain/Order.cs"].Replace("public string Reference { get; set; } = \"\";", "public int Reference { get; set; }"));
        var second = folder.Run(Model);

        Cached(second).ShouldBeFalse();
        second.Claims.Single(x => x.Kind == "member.type" && x.Element == "order-reference").Outcome.ShouldBe("contradicted");
    }

    [Fact]
    public void A_changed_scope_row_is_read_afresh()
    {
        using var folder = new Folder(Files);
        folder.Run(Model);

        var model = Model.DeepClone();
        model["designers"]!["Codebase Structure"]!["packages"]![0]!["stereotypes"]!["Drift Scope"]!["Entries"]!.AsArray()
            .Add(JsonNode.Parse("""{ "Glob": "src/Domain/Buyer.cs", "Scope": "Out", "Reason": "Not modelled yet" }"""));
        var second = folder.Run(model);

        Cached(second).ShouldBeFalse();
        second.Files.ShouldNotContain(x => x.Path == "src/Domain/Buyer.cs");
    }

    [Fact]
    public void A_kept_read_gives_another_model_what_a_fresh_read_gives_it()
    {
        var model = Model.DeepClone();
        model["designers"]!["Domain"]!["packages"]![0]!["elements"]![0]!["children"]![1]!["typeRef"] = JsonNode.Parse("""{ "type": "int" }""");

        using var kept = new Folder(Files);
        kept.Run(Model);
        var reused = kept.Run(model);
        using var fresh = new Folder(Files);
        var read = fresh.Run(model, keep: false);

        Cached(reused).ShouldBeTrue();
        reused.Claims.Single(x => x.Kind == "member.type" && x.Element == "order-reference").Outcome.ShouldBe("contradicted");
        Json(reused).ShouldBe(Json(read));
    }

    [Fact]
    public void What_claims_keep_on_the_graph_is_gone_on_the_next_run()
    {
        // A claim reader that keeps something on the graph, as Messaging keeps the calls its link claims covered.
        var leftover = new Leftover();
        using var folder = new Folder(Files);
        folder.Run(Model, extra: leftover);
        var second = folder.Run(Model, extra: leftover);

        Cached(second).ShouldBeTrue();
        leftover.Kept.ShouldBe(2);
        leftover.Found.ShouldBe(0, "What a claim kept on the graph in one run was still there in the next.");
    }

    [Fact]
    public void A_cancelled_read_stops_and_keeps_nothing()
    {
        using var folder = new Folder(Files);
        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();

        Should.Throw<OperationCanceledException>(() => folder.Run(Model, cancellation: cancelled));
        DriftEngine.HasKeptCode(folder.Root).ShouldBeFalse();
        Cached(folder.Run(Model)).ShouldBeFalse();
    }

    [Fact]
    public void A_run_cancelled_after_its_read_keeps_the_read()
    {
        // Cancelled while its claims are read, as a save during a run would cancel it.
        using var folder = new Folder(Files);
        using var cancellation = new CancellationTokenSource();
        Should.Throw<OperationCanceledException>(() => folder.Run(Model, extra: new Canceller(cancellation), cancellation: cancellation));
        DriftEngine.HasKeptCode(folder.Root).ShouldBeTrue();

        var next = folder.Run(Model);
        using var fresh = new Folder(Files);
        Cached(next).ShouldBeTrue();
        Json(next).ShouldBe(Json(fresh.Run(Model, keep: false)));
    }

    [Fact]
    public void Without_KeepCode_nothing_is_kept()
    {
        using var folder = new Folder(Files);
        folder.Run(Model, keep: false);

        DriftEngine.HasKeptCode(folder.Root).ShouldBeFalse();
        Cached(folder.Run(Model)).ShouldBeFalse();
    }

    private static bool Cached(RunResult result) => result.Timings.Any(x => x.What == "code cached");

    /// <summary>The run's result as the schema has it: claims, findings, coverage, packs and files read.</summary>
    private static string Json(RunResult result) =>
        JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });

    /// <summary>An output folder of its own, its kept read dropped with it.</summary>
    private sealed class Folder : IDisposable
    {
        public Folder(IReadOnlyDictionary<string, string> files)
        {
            Root = Directory.CreateTempSubdirectory("driftengine-kept-").FullName;
            foreach (var (path, content) in files)
            {
                Write(path, content);
            }
        }

        public string Root { get; }

        public void Write(string path, string content)
        {
            var file = Path.Combine(Root, path);
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            File.WriteAllText(file, content.ReplaceLineEndings("\r\n"));
        }

        /// <param name="cancellation">What cancels the run; the test's own cancellation by default.</param>
        public RunResult Run(JsonNode model, bool keep = true, IDriftExtension? extra = null, CancellationTokenSource? cancellation = null)
        {
            var packs = new[] { "intent.core", "intent.domain" }.Select(Pack.Stock).OfType<Pack>().ToList();
            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).Concat(extra is null ? [] : [extra]).ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(model), Root, packs, extensions)
            {
                KeepCode = keep,
                CancellationToken = cancellation?.Token ?? TestContext.Current.CancellationToken
            });
        }

        public void Dispose()
        {
            DriftEngine.ForgetKeptCode(Root);
            Directory.Delete(Root, recursive: true);
        }
    }

    /// <summary>
    /// Keeps a value on the graph while reading <c>Order</c>'s <c>type.exists</c>, which each run reads once, and counts the
    /// runs that found one there already: the last run's.
    /// </summary>
    private sealed class Leftover : IDriftExtension
    {
        public int Kept { get; set; }

        public int Found { get; set; }

        public string Id => "test.leftover";

        public IEnumerable<IClaimReader> ClaimReaders => [new Reader(this)];

        private sealed class Reader(Leftover leftover) : IClaimReader
        {
            public IReadOnlyCollection<string> Kinds => ["type.exists"];

            public ClaimReading? Read(ClaimContext context)
            {
                if (context.Element.Name == "Order" && context.Code.Get<Covered>() is null)
                {
                    leftover.Kept++;
                    context.Code.Set(new Covered());
                }
                else if (context.Element.Name == "Order")
                {
                    leftover.Found++;
                }

                // The stock reading decides the claim.
                return null;
            }
        }
    }

    private sealed class Covered;

    /// <summary>Cancels the run when the first <c>type.exists</c> claim is read, once the code's read is done.</summary>
    private sealed class Canceller(CancellationTokenSource cancellation) : IDriftExtension
    {
        public string Id => "test.canceller";

        public IEnumerable<IClaimReader> ClaimReaders => [new Reader(cancellation)];

        private sealed class Reader(CancellationTokenSource cancellation) : IClaimReader
        {
            public IReadOnlyCollection<string> Kinds => ["type.exists"];

            public ClaimReading? Read(ClaimContext context)
            {
                cancellation.Cancel();
                return null;
            }
        }
    }
}
