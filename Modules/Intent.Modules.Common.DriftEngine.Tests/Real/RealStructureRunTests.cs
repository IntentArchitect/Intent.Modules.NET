using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Real;

/// <summary>
/// Runs the structure checks over a real codebase: its model exported through Intent's query tool in the fixture format,
/// and its code read in place. The model and results stay on this machine (a client's code is never committed).
/// Set DRIFTENGINE_REAL_MODEL (the export), DRIFTENGINE_REAL_OUTPUT (the application's output folder) and
/// DRIFTENGINE_REAL_IN (the In globs, comma separated); the summary is written beside the export.
/// </summary>
public class RealStructureRunTests
{
    [Fact]
    public void Structure_checks_run_over_a_real_codebase_without_changing_it()
    {
        var modelPath = Environment.GetEnvironmentVariable("DRIFTENGINE_REAL_MODEL");
        var output = Environment.GetEnvironmentVariable("DRIFTENGINE_REAL_OUTPUT");
        var globs = Environment.GetEnvironmentVariable("DRIFTENGINE_REAL_IN");
        if (modelPath is null || output is null || globs is null)
        {
            Assert.Skip("Set DRIFTENGINE_REAL_MODEL, DRIFTENGINE_REAL_OUTPUT and DRIFTENGINE_REAL_IN to run over a real codebase.");
        }

        // The codebase has no Drift Scope yet; the In rows stand for the one its adoption would add.
        var model = JsonNode.Parse(File.ReadAllText(modelPath))!;
        var entries = new JsonArray(globs.Split(',', StringSplitOptions.TrimEntries).Select(x => (JsonNode)new JsonObject { ["Glob"] = x, ["Scope"] = "In" }).ToArray());
        model["designers"]!["Codebase Structure"] = JsonNode.Parse($$"""
            { "packages": [{ "id": "root", "name": "Root", "type": "Root Folder", "stereotypes": { "Drift Scope": { "Entries": {{entries.ToJsonString()}} } } }] }
            """);

        var before = GitStatus(output);
        var watch = Stopwatch.StartNew();
        // A codebase's house pack (DRIFTENGINE_REAL_HOUSE, a single-file pack) comes last, so its conventions win.
        var packs = new[] { "intent.domain", "intent.services" }.Select(x => Pack.Stock(x)!).ToList();
        if (Environment.GetEnvironmentVariable("DRIFTENGINE_REAL_HOUSE") is { Length: > 0 } house)
        {
            packs.Add(Pack.Parse(JsonNode.Parse(File.ReadAllText(house))!));
        }

        // Every stock extension, as the Software Factory runs it: the engine reads no language itself. It keeps the read, so a
        // second run over the unchanged code reuses it, as the Software Factory's next run does.
        var input = new DriftInput(FixtureModel.Build(model), output, packs, StockExtensions.All) { KeepCode = true };
        DriftEngine.ForgetKeptCode(output);
        var result = DriftEngine.Run(input);
        watch.Stop();
        var again = Stopwatch.StartNew();
        var second = DriftEngine.Run(input);
        again.Stop();
        var kept = Memory();
        DriftEngine.ForgetKeptCode(output);
        var freed = Memory();
        GitStatus(output).ShouldBe(before, "Checking never writes code (P4, R17).");

        var json = JsonSerializer.SerializeToNode(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        })!;
        SchemaSet.Instance.Validate("run-result.schema.json", json).ShouldBeEmpty();

        var folder = Path.GetDirectoryName(modelPath)!;
        File.WriteAllText(Path.Combine(folder, "result.json"), json.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        List<string> summary =
        [
            $"{result.Files.Count} files read, {result.Claims.Count} claims, {watch.ElapsedMilliseconds} ms",
            "timings: " + Timings(result),
            $"second run, {again.ElapsedMilliseconds} ms: {Timings(second)}",
            $"memory with the read kept: {kept.Managed / 1048576} MB managed, {kept.WorkingSet / 1048576} MB working set; " +
            $"once dropped: {freed.Managed / 1048576} MB managed, {freed.WorkingSet / 1048576} MB working set",
            .. result.Claims.GroupBy(x => $"{x.Kind} {x.Outcome}{(x.Reason is null ? "" : $" ({x.Reason})")}").OrderBy(x => x.Key).Select(x => $"  {x.Key}: {x.Count()}"),
            "",
            .. result.Findings.Select(x => $"{x.Severity} {x.Rule}: {x.Message}")
        ];
        File.WriteAllLines(Path.Combine(folder, "summary.txt"), summary);
    }

    /// <summary>As the Software Factory log has them: every part of the run, then the 8 slowest kinds.</summary>
    private static string Timings(RunResult result) =>
        string.Join(", ", result.Timings.Take(result.Timings.Count(x => !x.What.StartsWith("kind ", StringComparison.Ordinal)) + 8)
            .Select(x => $"{x.What} {x.Milliseconds:0} ms{(x.Count is { } count ? $" ({count})" : "")}"));

    /// <summary>The process's memory once everything unreachable is collected: the managed heap and the working set.</summary>
    private static (long Managed, long WorkingSet) Memory()
    {
        System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
        GC.WaitForPendingFinalizers();
        return (GC.GetTotalMemory(forceFullCollection: true), Environment.WorkingSet);
    }

    private static string GitStatus(string folder)
    {
        using var git = Process.Start(new ProcessStartInfo("git", "status --porcelain")
        {
            WorkingDirectory = folder,
            RedirectStandardOutput = true,
            UseShellExecute = false
        })!;
        var output = git.StandardOutput.ReadToEnd();
        git.WaitForExit();
        return output;
    }
}
