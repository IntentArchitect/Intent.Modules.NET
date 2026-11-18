using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Api;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// A module's extension derives from <see cref="DriftEngineExtensionBase"/> and reaches as far as one that implements
/// <see cref="IDriftExtension"/> itself. A part the base didn't let it override would stay the interface's empty default,
/// whatever the module wrote, with nothing to say so: <c>Languages</c> was one.
/// </summary>
public class ModuleExtensionTests
{
    private static readonly JsonNode Model = JsonNode.Parse("""
        {
          "designers": {
            "Domain": {
              "packages": [{
                "id": "domain", "name": "Shop.Domain",
                "elements": [{ "id": "order", "type": "Class", "name": "Order" }]
              }]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
            }
          }
        }
        """)!;

    [Fact]
    public void Every_part_of_an_extension_can_be_overridden()
    {
        var fixedParts = typeof(IDriftExtension).GetProperties()
            .Where(x => typeof(DriftEngineExtensionBase).GetProperty(x.Name, x.PropertyType)?.GetMethod is not { IsPublic: true, IsVirtual: true, IsFinal: false })
            .Select(x => x.Name);

        fixedParts.ShouldBeEmpty();
    }

    [Fact]
    public void A_module_adds_a_language()
    {
        var root = Directory.CreateTempSubdirectory("driftengine-module-").FullName;
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "src"));
            File.WriteAllText(Path.Combine(root, "src", "Order.toy"), "");
            var result = DriftEngine.Run(new DriftInput(FixtureModel.Build(Model), root, [Pack.Stock("intent.domain")!], [new ToyModuleExtension()]));

            result.Claims.Single(x => x.Kind == "type.exists" && x.Element == "order").Outcome.ShouldBe("verified");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class ToyModuleExtension : DriftEngineExtensionBase
    {
        public override string Id => "toy.module";

        public override IEnumerable<Pack> Packs =>
        [
            Pack.Parse(JsonNode.Parse("""
                { "id": "toy", "version": "1.0.0", "languages": ["toy"], "profiles": { "toy": { "language": "toy", "typeKinds": ["class"] } } }
                """)!)
        ];

        public override IEnumerable<ILanguage> Languages => [new ToyLanguage()];
    }

    /// <summary>Reads nothing from its files: each declares a class named for it.</summary>
    private sealed class ToyLanguage : ILanguage
    {
        public string Name => "toy";

        public string Title => "Toy";

        public bool Reads(string path) => path.EndsWith(".toy", StringComparison.Ordinal);

        public void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken)
        {
            foreach (var file in files)
            {
                graph.Files.Add(new FileNode { Path = file, Language = Name });
                var type = new TypeNode { Name = Path.GetFileNameWithoutExtension(file), Namespace = "shop", Kind = "class", At = new Span(file, 1) };
                type.Files.Add(file);
                graph.Types.Add(type);
            }
        }
    }
}
