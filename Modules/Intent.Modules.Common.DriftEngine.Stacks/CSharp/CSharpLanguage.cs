using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// C# as a language the engine reads: <c>.cs</c> files, Razor components (<c>.razor</c>), MVC views and Razor Pages
/// (<c>.cshtml</c>, each a component named after its file), and the <c>.csproj</c> files that set their nullable context and
/// root namespace. Its profile (the <c>csharp</c> pack) names C#'s types, collections and type kinds.
/// </summary>
public sealed class CSharpExtension : IDriftExtension
{
    public string Id => "csharp";

    public IEnumerable<ILanguage> Languages => [new CSharpLanguage()];

    public IEnumerable<Pack> Packs => [StockExtensions.LoadPack(Id)];
}

internal sealed class CSharpLanguage : ILanguage
{
    internal const string LanguageName = "csharp";

    public string Name => LanguageName;

    public string Title => "C#";

    public bool Reads(string path) =>
        path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".razor", StringComparison.OrdinalIgnoreCase) || RazorFile.IsCshtml(path)
        || path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);

    public void Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken) => CSharpReader.Read(root, files, graph, cancellationToken);
}

/// <summary>
/// The C# language's one parse of the in-scope files, kept on the graph for every extension that reads C#: a stock stack
/// such as EF Core, or a module's own reader. Get it with <see cref="Of"/>; null when no C# file is in scope.
/// </summary>
public sealed class CSharpSyntax
{
    private readonly CodeGraph _graph;

    internal CSharpSyntax(CodeGraph graph) => _graph = graph;

    /// <summary>
    /// Each in-scope C# file's syntax tree, by path relative to the output folder, with Roslyn's full syntax. A Razor
    /// component's, MVC view's or Razor Page's is the C# the Razor compiler generates for it; <see cref="SpanOf"/> places its
    /// nodes in the <c>.razor</c> or <c>.cshtml</c> file they are written in. A <c>_ViewStart.cshtml</c>'s is here too, though
    /// it has no file or type in the graph: it holds no view, only code its folder's views run first (their <c>Layout</c>).
    /// </summary>
    public SortedDictionary<string, SyntaxTree> Trees { get; } = new(StringComparer.Ordinal);

    /// <summary>C# files in a nullable context, by their project's <c>Nullable</c> or a <c>#nullable enable</c>.</summary>
    internal HashSet<string> NullableContexts { get; } = new(StringComparer.Ordinal);

    public static CSharpSyntax? Of(CodeGraph graph) => graph.Get<CSharpSyntax>();

    /// <summary>The file is in a nullable context: its project sets <c>Nullable</c>, or it has <c>#nullable enable</c>.</summary>
    public bool InNullableContext(string file) => NullableContexts.Contains(file);

    /// <summary>
    /// A type as written in <paramref name="file"/>, with its nullability decided as for a member's type: for a member an
    /// extension finds that the syntax doesn't declare, such as an EF Core shadow property's <c>Property&lt;int&gt;("BuyerId")</c>.
    /// </summary>
    public TypeRefNode TypeOf(TypeSyntax syntax, string file) => CSharpReader.TypeOf(syntax, file, this, _graph);

    /// <summary>Where a node is, as findings give it: its file and lines, from its first attribute.</summary>
    public static Span SpanOf(SyntaxNode node, string file) => CSharpReader.SpanOf(node, file);
}
