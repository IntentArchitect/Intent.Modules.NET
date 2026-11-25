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

    /// <summary>
    /// A tree nesting deeper than JSON's default 64 levels is read: eShop10's jQuery bundle nests 107, which failed every
    /// file's read. One nesting deeper than the worker reads is that file's failure, and the others are read.
    /// </summary>
    [Fact]
    public void A_deeply_nested_file_is_read_and_one_too_deep_fails_alone()
    {
        static string Nested(int depth) => $"export const value = {new string('(', depth)}1{new string(')', depth)};";
        // TypeScript parses a chain of + in a loop, nesting a level a term, where nested brackets overflow its own stack first.
        static string Chained(int terms) => $"export const value = {string.Join(" + ", Enumerable.Repeat("1", terms))};";

        var graph = Read(new RecordingNode(), ("deep.ts", $"export interface Deep {{ id: string }}\n{Nested(60)}"), ("too-deep.ts", Chained(1200)));

        graph.Types.Select(x => x.Name).ShouldContain("Deep");
        graph.Failures.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.File.ShouldBe("too-deep.ts"),
            x => x.Error.ShouldContain("too deep to read"));
    }

    private static CodeGraph Read(INodeRunner node, params (string Path, string Text)[] extra)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-ts-worker-").FullName;
        try
        {
            File.WriteAllText(Path.Combine(root, "a.ts"), "export interface A { id: string }");
            foreach (var (path, text) in extra)
            {
                File.WriteAllText(Path.Combine(root, path), text);
            }

            var graph = new CodeGraph();
            new TypeScriptExtension(node).Languages.Single().Read(root, ["a.ts", .. extra.Select(x => x.Path)], graph, TestContext.Current.CancellationToken);
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
