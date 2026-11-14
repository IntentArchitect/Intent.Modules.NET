using Antlr4.Runtime;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks.Java.Grammar;

namespace Intent.Modules.Common.DriftEngine.Stacks.Java;

/// <summary>
/// Java as a language the engine reads: <c>.java</c> files, parsed with ANTLR's Java grammar (grammars-v4 java/java, patched;
/// see Grammar/regen.ps1). Its profile (the <c>java</c> pack) names Java's types, collections, names and type kinds.
/// </summary>
public sealed class JavaExtension : IDriftExtension
{
    public string Id => "intent.java";

    public IEnumerable<ILanguage> Languages => [new JavaLanguage()];

    public IEnumerable<Pack> Packs => [StockExtensions.LoadPack(JavaLanguage.LanguageName)];
}

internal sealed class JavaLanguage : ILanguage
{
    internal const string LanguageName = "java";

    public string Name => LanguageName;

    public string Title => "Java";

    public bool Reads(string path) => path.EndsWith(".java", StringComparison.OrdinalIgnoreCase);

    public void Read(string root, IReadOnlyList<string> files, CodeGraph graph) => JavaReader.Read(root, files, graph);
}

/// <summary>
/// The Java language's one parse of the in-scope files, kept on the graph for every extension that reads Java: a stock
/// stack such as JPA, or a module's own reader. Get it with <see cref="Of"/>; null when no Java file is in scope.
/// </summary>
public sealed class JavaSyntax
{
    internal JavaSyntax()
    {
    }

    /// <summary>Each in-scope Java file's parse tree, by path relative to the output folder.</summary>
    public SortedDictionary<string, JavaParser.CompilationunitContext> Trees { get; } = new(StringComparer.Ordinal);

    public static JavaSyntax? Of(CodeGraph graph) => graph.Get<JavaSyntax>();

    /// <summary>Where a node is, as findings give it: its file and lines.</summary>
    public static Span SpanOf(ParserRuleContext node, string file) => JavaReader.SpanOf(node, file);

    /// <summary>
    /// A node's source text, whitespace normalised. ANTLR's <c>GetText()</c> drops the whitespace between tokens
    /// (<c>new Check()</c> is <c>newCheck()</c>), so this slices the source instead.
    /// </summary>
    public static string TextOf(ParserRuleContext node) => JavaReader.TextOf(node);

    /// <summary>A type as written, as the graph has it.</summary>
    public static TypeRefNode TypeOf(JavaParser.TypeTypeContext type) => JavaReader.TypeRef(type);
}
