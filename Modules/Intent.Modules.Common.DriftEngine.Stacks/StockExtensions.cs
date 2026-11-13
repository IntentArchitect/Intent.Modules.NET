using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.EfCore;
using Intent.Modules.Common.DriftEngine.Stacks.Http;
using Intent.Modules.Common.DriftEngine.Stacks.Relational;

namespace Intent.Modules.Common.DriftEngine.Stacks;

/// <summary>
/// The extensions shipped with the engine. A language is one (CSharp): the engine reads no code itself. What the model
/// states is technology-neutral (Relational, Http); how one framework's code states it is a separate extension (EfCore,
/// AspNetCore), so another framework (JPA, Spring MVC) reads the same claims with no change to either. A framework's
/// extension answers only when its framework is in the code; otherwise it leaves the claim to another, or can't verify.
/// </summary>
public static class StockExtensions
{
    public static IReadOnlyList<IDriftExtension> All { get; } =
    [
        new CSharpExtension(),
        new RelationalExtension(),
        new HttpExtension(),
        new EfCoreExtension(),
        new AspNetCoreExtension()
    ];

    /// <summary>A stock extension by its id, or null.</summary>
    public static IDriftExtension? Find(string id) => All.FirstOrDefault(x => x.Id == id);

    internal static Pack LoadPack(string id)
    {
        using var stream = typeof(StockExtensions).Assembly.GetManifestResourceStream($"{id}.pack.json")
                           ?? throw new InvalidOperationException($"The {id} pack is missing from the stock extensions' resources.");
        return Pack.Parse(JsonNode.Parse(stream)!);
    }
}
