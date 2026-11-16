using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;

namespace Intent.Modules.Common.DriftEngine.Stacks.Protobuf;

/// <summary>
/// Protocol Buffers as a language the engine reads: <c>.proto</c> files (proto2, proto3 and editions), the messages, enums
/// and gRPC services they declare. Its profile (the <c>protobuf</c> pack) names the scalar and well-known types the model's
/// types are written as.
/// </summary>
public sealed class ProtobufExtension : IDriftExtension
{
    public string Id => "intent.protobuf";

    public IEnumerable<ILanguage> Languages => [new ProtobufLanguage()];

    public IEnumerable<Pack> Packs => [StockExtensions.LoadPack(ProtobufLanguage.LanguageName)];
}

internal sealed class ProtobufLanguage : ILanguage
{
    internal const string LanguageName = "protobuf";

    public string Name => LanguageName;

    public string Title => "Protobuf";

    public bool Reads(string path) => path.EndsWith(".proto", StringComparison.OrdinalIgnoreCase);

    public bool LibrariesNeedImports => true;

    public void Read(string root, IReadOnlyList<string> files, CodeGraph graph) => ProtobufReader.Read(root, files, graph);
}

/// <summary>
/// What the Protobuf language read of each in-scope <c>.proto</c> file besides its types and members, kept on the graph for
/// every extension that reads Protobuf, such as a gRPC stack's: its syntax and its file options. Get it with
/// <see cref="Of"/>; null when no <c>.proto</c> file is in scope.
/// </summary>
public sealed class ProtobufSyntax
{
    internal ProtobufSyntax()
    {
    }

    /// <summary>Each in-scope <c>.proto</c> file, by path relative to the output folder.</summary>
    public SortedDictionary<string, ProtobufFile> Files { get; } = new(StringComparer.Ordinal);

    public static ProtobufSyntax? Of(CodeGraph graph) => graph.Get<ProtobufSyntax>();
}

/// <summary>A <c>.proto</c> file's own declarations.</summary>
public sealed class ProtobufFile
{
    /// <summary><c>proto2</c> (also when the file states none), <c>proto3</c> or <c>editions</c>.</summary>
    public required string Syntax { get; init; }

    /// <summary>The edition it is written in (<c>2023</c>); null for proto2 and proto3.</summary>
    public string? Edition { get; init; }

    /// <summary>Its <c>package</c>; empty when it declares none.</summary>
    public required string Package { get; init; }

    /// <summary>
    /// Its file options, as annotations are: <c>option csharp_namespace = "Shop.Grpc";</c> is <c>csharp_namespace</c> with
    /// <c>Shop.Grpc</c>.
    /// </summary>
    public IReadOnlyList<AttributeNode> Options { get; init; } = [];
}
