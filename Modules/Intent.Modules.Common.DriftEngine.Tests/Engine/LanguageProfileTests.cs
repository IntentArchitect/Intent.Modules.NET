using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// How a language names and wraps the model's members is its profile's, never the core's: a made-up language, whose profile
/// names fields and methods in camel case and wraps async results in <c>Future&lt;T&gt;</c>, is checked by the stock Domain
/// claims with no C# in the way. No acceptance case can show it while the only language the suite reads is C#.
/// </summary>
public class LanguageProfileTests
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
                      { "id": "order-customer-name", "type": "Attribute", "name": "CustomerName", "typeRef": { "type": "string" } },
                      { "id": "order-lines", "type": "Attribute", "name": "Lines", "typeRef": { "element": "page", "arguments": [{ "element": "order-line" }] } },
                      { "id": "order-archive", "type": "Attribute", "name": "Archive", "typeRef": { "element": "page", "arguments": [{ "element": "order-line" }] } },
                      { "id": "order-cancel", "type": "Operation", "name": "Cancel", "typeRef": { "type": "string" } },
                      { "id": "order-close", "type": "Operation", "name": "Close" }
                    ] },
                  { "id": "order-line", "type": "Class", "name": "OrderLine" },
                  { "id": "page", "type": "Class", "name": "Page" },
                  { "id": "order-repository", "type": "Repository", "name": "OrderRepository" }
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

    private static readonly RunResult Result = Run();

    [Fact]
    public void A_member_is_found_by_the_profiles_naming()
    {
        Outcome("member.exists", "order-customer-name").ShouldBe("verified");
        Outcome("member.exists", "order-cancel").ShouldBe("verified");
    }

    [Fact]
    public void A_return_type_is_unwrapped_of_the_profiles_async_wrappers()
    {
        Outcome("operation.returns", "order-cancel").ShouldBe("verified");
        Outcome("operation.returns", "order-close").ShouldBe("verified");
    }

    [Fact]
    public void An_interface_only_type_is_named_as_modelled_when_the_profile_has_no_interface_naming()
    {
        Outcome("type.exists", "order-repository").ShouldBe("verified");
    }

    [Fact]
    public void Type_arguments_the_model_states_are_compared()
    {
        Outcome("member.type", "order-lines").ShouldBe("verified");
        Outcome("member.type", "order-archive").ShouldBe("contradicted");
    }

    private static string Outcome(string kind, string element) => Result.Claims.Single(x => x.Kind == kind && x.Element == element).Outcome;

    private static RunResult Run()
    {
        var root = Directory.CreateTempSubdirectory("driftengine-profile-").FullName;
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "src"));
            File.WriteAllText(Path.Combine(root, "src", "Order.toy"), "");
            File.WriteAllText(Path.Combine(root, "src", "OrderRepository.toy"), "");
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(Model), root, [Pack.Stock("intent.domain")!], [new ToyExtension()]));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class ToyExtension : IDriftExtension
    {
        public string Id => "toy";

        public IEnumerable<Pack> Packs =>
        [
            Pack.Parse(JsonNode.Parse("""
                {
                  "id": "toy", "version": "1.0.0", "languages": ["toy"],
                  "profiles": {
                    "toy": {
                      "language": "toy",
                      "types": { "string": ["String"] },
                      "async": [{ "wrapper": "Future<T>", "void": true }],
                      "naming": { "field": "{Name|camel}", "method": "{Name|camel}" },
                      "typeKinds": ["class", "interface", "enum"]
                    }
                  }
                }
                """)!)
        ];

        public IEnumerable<ILanguage> Languages => [new ToyLanguage()];
    }

    /// <summary>Reads nothing from its files: it puts the types a camel-case language would declare in the graph.</summary>
    private sealed class ToyLanguage : ILanguage
    {
        public string Name => "toy";

        public string Title => "Toy";

        public bool Reads(string path) => path.EndsWith(".toy", StringComparison.Ordinal);

        public void Read(string root, IReadOnlyList<string> files, CodeGraph graph)
        {
            foreach (var file in files)
            {
                graph.Files.Add(new FileNode { Path = file, Language = Name });
            }

            var order = Type("Order", "class", "src/Order.toy");
            order.Members.AddRange(
            [
                new MemberNode { Name = "customerName", Kind = "field", Type = Ref("String"), At = new Span("src/Order.toy", 2) },
                new MemberNode { Name = "lines", Kind = "field", Type = Ref("Page", Ref("OrderLine")), At = new Span("src/Order.toy", 3) },
                new MemberNode { Name = "archive", Kind = "field", Type = Ref("Page", Ref("Order")), At = new Span("src/Order.toy", 4) },
                new MemberNode { Name = "cancel", Kind = "method", Type = Ref("Future", Ref("String")), At = new Span("src/Order.toy", 5) },
                new MemberNode { Name = "close", Kind = "method", Type = Ref("Future"), At = new Span("src/Order.toy", 6) }
            ]);
            graph.Types.Add(order);
            graph.Types.Add(Type("OrderRepository", "interface", "src/OrderRepository.toy"));
        }

        private static TypeNode Type(string name, string kind, string file)
        {
            var type = new TypeNode { Name = name, Namespace = "shop", Kind = kind, At = new Span(file, 1) };
            type.Files.Add(file);
            return type;
        }

        private static TypeRefNode Ref(string name, params TypeRefNode[] arguments)
        {
            var text = arguments.Length == 0 ? name : $"{name}<{string.Join(", ", arguments.Select(x => x.Text))}>";
            return new TypeRefNode { Text = text, Name = name, QualifiedName = name, Arguments = arguments };
        }
    }
}
