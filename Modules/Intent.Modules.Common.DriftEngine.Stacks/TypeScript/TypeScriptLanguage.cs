using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// TypeScript as a language the engine reads: <c>.ts</c>, <c>.tsx</c>, <c>.mts</c> and <c>.cts</c> files, JavaScript
/// (<c>.js</c>, <c>.jsx</c>, <c>.mjs</c>, <c>.cjs</c>, which TypeScript's parser reads as it does TypeScript, without its
/// types), and Vue single-file components' scripts (<c>.vue</c>), parsed by TypeScript's own parser on Node.js
/// (<see cref="INodeRunner"/>). Its profile (the <c>typescript</c> pack) names TypeScript's types, collections and type kinds.
/// </summary>
/// <param name="node">How Node.js is run: Intent Architect's own in a Software Factory run; the machine's <c>node</c> by default.</param>
public sealed class TypeScriptExtension(INodeRunner? node = null) : IDriftExtension
{
    public string Id => "intent.typescript";

    public IEnumerable<ILanguage> Languages => [new TypeScriptLanguage(node ?? MachineNodeRunner.Instance)];

    public IEnumerable<Pack> Packs => [StockExtensions.LoadPack(TypeScriptLanguage.LanguageName)];
}

internal sealed class TypeScriptLanguage(INodeRunner node) : ILanguage
{
    internal const string LanguageName = "typescript";

    private static readonly string[] Extensions = [".ts", ".tsx", ".mts", ".cts", ".vue", ".js", ".jsx", ".mjs", ".cjs"];

    public string Name => LanguageName;

    public string Title => "TypeScript";

    /// <summary>A minified bundle (<c>.min.js</c>) is a build's output, not code anyone writes, so it isn't read.</summary>
    public bool Reads(string path) => Extensions.Any(x => path.EndsWith(x, StringComparison.OrdinalIgnoreCase)) && !path.EndsWith(".min.js", StringComparison.OrdinalIgnoreCase);

    public void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken) => TypeScriptReader.Read(root, files, graph, node, cancellationToken);
}

/// <summary>
/// The TypeScript language's one parse of the in-scope files, kept on the graph for every extension that reads TypeScript:
/// a stack such as one for Vue or Angular, or a module's own reader. Get it with <see cref="Of"/>; null when no TypeScript
/// file is in scope.
/// </summary>
public sealed class TypeScriptSyntax
{
    internal TypeScriptSyntax()
    {
    }

    /// <summary>Each in-scope file's tree, by path relative to the output folder.</summary>
    public SortedDictionary<string, TypeScriptFile> Trees { get; } = new(StringComparer.Ordinal);

    public static TypeScriptSyntax? Of(CodeGraph graph) => graph.Get<TypeScriptSyntax>();

    /// <summary>A type as written, as the graph has it.</summary>
    public static TypeRefNode TypeOf(TypeScriptNode type) => TypeScriptReader.TypeRef(type, null);
}
