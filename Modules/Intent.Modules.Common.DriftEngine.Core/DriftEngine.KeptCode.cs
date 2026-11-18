using System.Security.Cryptography;
using System.Text;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Scope;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>
/// What a read of the code produced, kept in this process for the next run over the same output folder
/// (<see cref="DriftInput.KeepCode"/>): the Software Factory's process outlives a run, and runs again over code that often
/// hasn't changed. It is reused whole or not at all, since one file's reading depends on the others (a call in one file
/// reaches a method in another), and only when nothing the read depends on has changed (<see cref="ReadKey"/>).
/// </summary>
internal sealed record KeptRead(
    string Key,
    ScopeModel Scope,
    IReadOnlyList<(string Path, ScopeDecision Decision)> Files,
    CodeScope CodeScope,
    IReadOnlyList<(TypeNode Type, ScopeRow Row)> ExcludedTypes,
    IReadOnlyDictionary<string, int> Read,
    CodeGraph Graph,
    IReadOnlyCollection<Type> Stored);

public sealed partial class DriftEngine
{
    /// <summary>The last read of each output folder, one each; a run takes its folder's, so no two runs share a graph.</summary>
    private static readonly Dictionary<string, KeptRead> Kept = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>What this run's read depends on (<see cref="ReadKey"/>); null when it isn't to be kept.</summary>
    private string? _readKey;

    /// <summary>What the readers stored on the graph (<see cref="CodeGraph.Set{T}"/>), as against what claims stored later.</summary>
    private IReadOnlyCollection<Type> _stored = [];

    /// <summary>
    /// Everything a read depends on, as one hash: every walked file's path, size and last-write time, the scope's rows, and
    /// each language and code reader with its assembly's build. Any change to them means the code is read afresh.
    /// </summary>
    private string ReadKey(WalkedFolder walked, ScopeModel scope)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        void Add(string text) => hash.AppendData(Encoding.UTF8.GetBytes(text + "\n"));

        foreach (var file in walked.Files)
        {
            Add($"file|{file.Path}|{file.Length}|{file.LastWrite.Ticks}");
        }

        foreach (var row in scope.Rows)
        {
            Add($"row|{row.Id}|{row.BasePath}|{string.Join(",", row.Globs.Select(x => x.Pattern))}|{string.Join(",", row.Types)}|{row.Scope}|{row.Reason}");
        }

        // A module rebuilt at the same version is another build, so each assembly is known by its build's id too.
        static string Build(Type type) => $"{type.FullName}, {type.Assembly.FullName}, {type.Assembly.ManifestModule.ModuleVersionId}";
        foreach (var (extension, language) in _languages)
        {
            Add($"language|{extension}|{language.Name}|{Build(language.GetType())}");
        }

        foreach (var extension in _extensions)
        {
            foreach (var reader in extension.Readers)
            {
                Add($"reader|{extension.Id}|{Build(reader.GetType())}");
            }
        }

        return Convert.ToHexString(hash.GetHashAndReset());
    }

    /// <summary>
    /// Takes the output folder's kept read when nothing it depends on has changed. A stale one is dropped either way, so it
    /// isn't held while the code is read again.
    /// </summary>
    private KeptRead? TakeKept(string key)
    {
        lock (Kept)
        {
            return Kept.Remove(Path.GetFullPath(_input.OutputFolder), out var kept) && kept.Key == key ? kept : null;
        }
    }

    /// <summary>
    /// Keeps this run's read for the next run over the folder, without what claims stored on the graph: a claim's reading
    /// depends on the model (the calls a link claim covered), and is worked out again. Not a read that was cancelled, or one
    /// a language or reader threw in, which may be the machine's doing (Node.js failing to start) rather than the code's.
    /// </summary>
    private void Keep()
    {
        if (_readKey is null || !_readDone || _readFailed)
        {
            return;
        }

        _graph.KeepOnly(_stored);
        lock (Kept)
        {
            Kept[Path.GetFullPath(_input.OutputFolder)] = new KeptRead(_readKey, _scope, _files.ToList(), _codeScope, _excludedTypes.ToList(), _readCounts, _graph, _stored);
        }
    }

    /// <summary>A language or code reader threw while reading (<c>config/pluginFailed</c>).</summary>
    private bool _readFailed;

    /// <summary>The read finished, or a kept one was taken: a cancelled read is never kept.</summary>
    private bool _readDone;

    /// <summary>How many files each language read.</summary>
    private IReadOnlyDictionary<string, int> _readCounts = new Dictionary<string, int>();

    /// <summary>Drops an output folder's kept read, so the next run over it reads its code afresh.</summary>
    internal static void ForgetKeptCode(string outputFolder)
    {
        lock (Kept)
        {
            Kept.Remove(Path.GetFullPath(outputFolder));
        }
    }

    /// <summary>Whether an output folder has a kept read.</summary>
    internal static bool HasKeptCode(string outputFolder)
    {
        lock (Kept)
        {
            return Kept.ContainsKey(Path.GetFullPath(outputFolder));
        }
    }
}
