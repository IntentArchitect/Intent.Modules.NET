using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The Software Factory's process outlives a run, so the TypeScript worker's files in the temp folder are checked on every
/// run: a temp folder cleaned between two runs is written again, not a failure until Intent Architect restarts.
/// </summary>
[Collection(Collection)]
public class TypeScriptWorkerTests
{
    /// <summary>It deletes the folder every TypeScript test reads through, so it runs alone.</summary>
    public const string Collection = "TypeScript worker folder";

    [Fact]
    public void A_worker_folder_cleaned_between_two_runs_is_written_again()
    {
        var node = new RecordingNode();
        Read(node).Failures.ShouldBeEmpty();
        Directory.Delete(Path.GetDirectoryName(node.Script!)!, recursive: true);

        var graph = Read(node);

        graph.Failures.ShouldBeEmpty();
        graph.Types.Single().Name.ShouldBe("A");
    }

    private static CodeGraph Read(INodeRunner node)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-ts-worker-").FullName;
        try
        {
            File.WriteAllText(Path.Combine(root, "a.ts"), "export interface A { id: string }");
            var graph = new CodeGraph();
            new TypeScriptExtension(node).Languages.Single().Read(root, ["a.ts"], graph, TestContext.Current.CancellationToken);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    /// <summary>The machine's Node.js, noting the worker script it was given.</summary>
    private sealed class RecordingNode : INodeRunner
    {
        public string? Script { get; private set; }

        public NodeRun Run(string script, IReadOnlyList<string> arguments)
        {
            Script = script;
            return MachineNodeRunner.Instance.Run(script, arguments);
        }
    }
}

[CollectionDefinition(TypeScriptWorkerTests.Collection, DisableParallelization = true)]
public sealed class TypeScriptWorkerCollection
{
}
