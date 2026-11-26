using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// A language whose declarations may leave their type out, as JavaScript's all do, says so in its profile's <c>untyped</c>,
/// and the core takes a type the code leaves out as not stated: each claim on it is can't verify, <c>unresolved</c>, and one
/// <c>engine/notStated</c> note lists them, never a finding of its own. A made-up language shows the core's part with no
/// reader in the way, and the same language with no <c>untyped</c> in its profile shows a missing type is still not found.
/// </summary>
public class UntypedCodeTests
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
                      { "id": "order-cancel", "type": "Operation", "name": "Cancel", "typeRef": { "type": "string" } },
                      { "id": "order-close", "type": "Operation", "name": "Close" },
                      { "id": "order-rename", "type": "Operation", "name": "Rename",
                        "children": [
                          { "id": "order-rename-name", "type": "Parameter", "name": "name", "typeRef": { "type": "string" } }
                        ] },
                      { "id": "order-retitle", "type": "Operation", "name": "Retitle",
                        "children": [
                          { "id": "order-retitle-title", "type": "Parameter", "name": "title", "typeRef": { "type": "string" } }
                        ] }
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

    private static readonly RunResult Untyped = Run(mayLeaveTypesOut: true);

    private static readonly RunResult Typed = Run(mayLeaveTypesOut: false);

    [Fact]
    public void A_member_whose_code_leaves_its_type_out_is_not_stated()
    {
        var type = Claim(Untyped, "member.type", "order-customer-name");
        type.Outcome.ShouldBe("cantVerify");
        type.Reason.ShouldBe("unresolved");
        type.Reading!.Detail.ShouldBe("src/Order.toy declares no type for `customerName`.");
        Claim(Untyped, "member.nullable", "order-customer-name").Reason.ShouldBe("unresolved");
    }

    [Fact]
    public void A_return_type_the_code_leaves_out_is_not_stated_against_a_modelled_type_and_void_against_none()
    {
        Claim(Untyped, "operation.returns", "order-cancel").Reason.ShouldBe("unresolved");
        Claim(Untyped, "operation.nullable", "order-cancel").Reason.ShouldBe("unresolved");
        Claim(Untyped, "operation.returns", "order-close").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void Parameters_whose_code_leaves_their_types_out_are_compared_by_name()
    {
        Claim(Untyped, "operation.parameters", "order-rename").Reason.ShouldBe("unresolved");
        Claim(Untyped, "parameter.nullable", "order-rename-name").Reason.ShouldBe("unresolved");
        Claim(Untyped, "operation.parameters", "order-retitle").Outcome.ShouldBe("contradicted");
    }

    [Fact]
    public void One_note_lists_them_and_none_is_a_finding_of_its_own()
    {
        // Retitle's parameter isn't named in its code, so its nullability waits on the parameters' finding.
        var note = Untyped.Findings.Where(x => x.Rule == "engine/notStated").ShouldHaveSingleItem();
        note.Severity.ShouldBe("info");
        note.Message.ShouldStartWith("Not checked: 6 claims on types their code leaves out (Domain 6). Toy declares no types.");
        Untyped.Findings.Where(x => x.Rule.StartsWith("claim/", StringComparison.Ordinal)).Select(x => x.Rule).ToList()
            .ShouldBe(new[] { "claim/operation.parameters" });
    }

    [Fact]
    public void A_language_that_always_states_its_types_still_finds_a_missing_type_not_found()
    {
        Claim(Typed, "member.type", "order-customer-name").Reason.ShouldBe("notFound");
        Claim(Typed, "operation.returns", "order-cancel").Outcome.ShouldBe("contradicted");
        Claim(Typed, "operation.parameters", "order-rename").Outcome.ShouldBe("contradicted");
        Typed.Findings.ShouldNotContain(x => x.Rule == "engine/notStated" && x.Message.Contains("types their code leaves out"));
    }

    private static Claim Claim(RunResult result, string kind, string element) => result.Claims.Single(x => x.Kind == kind && x.Element == element);

    private static RunResult Run(bool mayLeaveTypesOut)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-untyped-").FullName;
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "src"));
            File.WriteAllText(Path.Combine(root, "src", "Order.toy"), "");
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(Model), root, [Pack.Stock("intent.domain")!], [new ToyExtension(mayLeaveTypesOut)]));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class ToyExtension(bool mayLeaveTypesOut) : IDriftExtension
    {
        public string Id => "toy";

        public IEnumerable<Pack> Packs
        {
            get
            {
                var profile = JsonNode.Parse("""
                    {
                      "language": "toy",
                      "types": { "string": ["String"] },
                      "naming": { "field": "{Name|camel}", "method": "{Name|camel}", "parameter": "{Name|camel}" },
                      "typeKinds": ["class"]
                    }
                    """)!.AsObject();
                if (mayLeaveTypesOut)
                {
                    profile["untyped"] = new JsonObject { ["notStated"] = "Toy declares no types." };
                }

                return [Pack.Parse(new JsonObject { ["id"] = "toy", ["version"] = "1.0.0", ["languages"] = new JsonArray("toy"), ["profiles"] = new JsonObject { ["toy"] = profile } })];
            }
        }

        public IEnumerable<ILanguage> Languages => [new ToyLanguage()];
    }

    /// <summary>Reads nothing from its files: it puts in the graph a class whose declarations state no types.</summary>
    private sealed class ToyLanguage : ILanguage
    {
        private const string File = "src/Order.toy";

        public string Name => "toy";

        public string Title => "Toy";

        public bool Reads(string path) => path.EndsWith(".toy", StringComparison.Ordinal);

        public void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken)
        {
            graph.Files.Add(new FileNode { Path = File, Language = Name });
            var order = new TypeNode { Name = "Order", Namespace = "shop", Kind = "class", At = new Span(File, 1) };
            order.Files.Add(File);
            order.Members.AddRange(
            [
                new MemberNode { Name = "customerName", Kind = "field", At = new Span(File, 2) },
                new MemberNode { Name = "cancel", Kind = "method", At = new Span(File, 3) },
                new MemberNode { Name = "close", Kind = "method", At = new Span(File, 4) },
                new MemberNode { Name = "rename", Kind = "method", Parameters = [new ParameterNode { Name = "name", At = new Span(File, 5) }], At = new Span(File, 5) },
                new MemberNode { Name = "retitle", Kind = "method", Parameters = [new ParameterNode { Name = "name", At = new Span(File, 6) }], At = new Span(File, 6) }
            ]);
            graph.Types.Add(order);
        }
    }
}
