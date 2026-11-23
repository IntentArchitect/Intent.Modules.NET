using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.Python;

/// <summary>
/// Python as a language the engine reads: <c>.py</c> files and <c>.pyi</c> stubs, parsed by the engine's own parser
/// (<see cref="PythonParser"/>), so no Python is needed. Its profile (the <c>python</c> pack) names Python's types,
/// collections, names and type kinds.
/// </summary>
public sealed class PythonExtension : IDriftExtension
{
    public string Id => "intent.python";

    public IEnumerable<ILanguage> Languages => [new PythonLanguage()];

    public IEnumerable<Pack> Packs => [StockExtensions.LoadPack(PythonLanguage.LanguageName)];
}

internal sealed class PythonLanguage : ILanguage
{
    internal const string LanguageName = "python";

    public string Name => LanguageName;

    public string Title => "Python";

    public bool Reads(string path) => path.EndsWith(".py", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".pyi", StringComparison.OrdinalIgnoreCase);

    /// <summary>A module uses a library's type only through an import: what it doesn't import is a builtin or its own.</summary>
    public bool LibrariesNeedImports => true;

    public void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken) => PythonReader.Read(root, files, graph, cancellationToken);
}

/// <summary>
/// The Python language's one parse of the in-scope files, kept on the graph for every extension that reads Python: a stack
/// such as one for FastAPI or SQLAlchemy, or a module's own reader. Get it with <see cref="Of"/>; null when no Python file
/// is in scope.
/// </summary>
public sealed class PythonSyntax
{
    internal PythonSyntax()
    {
    }

    /// <summary>Each in-scope Python file's tree, by path relative to the output folder.</summary>
    public SortedDictionary<string, PythonModule> Trees { get; } = new(StringComparer.Ordinal);

    /// <summary>Each in-scope Python file's module name, by path: <c>shop.domain.order</c> for <c>src/shop/domain/order.py</c>.</summary>
    public SortedDictionary<string, string> Modules { get; } = new(StringComparer.Ordinal);

    internal Dictionary<string, string> Sources { get; } = new(StringComparer.Ordinal);

    public static PythonSyntax? Of(CodeGraph graph) => graph.Get<PythonSyntax>();

    /// <summary>Where a node is, as findings give it: its file and lines.</summary>
    public static Span SpanOf(PythonNode node, string file) => PythonReader.SpanOf(node, file);

    /// <summary>A node's source text, whitespace normalised.</summary>
    public string TextOf(PythonNode node, string file) => PythonReader.TextOf(Sources[file], node);

    /// <summary>An annotation as a type, as the graph has it.</summary>
    public static TypeRefNode TypeOf(PythonExpression annotation) => PythonTypes.TypeRef(annotation);
}
