using System.ComponentModel;
using System.Diagnostics;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// Runs a Node.js script to its end, for the TypeScript language's worker: Intent Architect's own Node.js in a Software
/// Factory run (the module gives the engine its runner), the machine's <c>node</c> anywhere else (<see cref="Machine"/>).
/// </summary>
public interface INodeRunner
{
    NodeRun Run(string script, IReadOnlyList<string> arguments);
}

/// <param name="ExitCode">Null when Node.js couldn't be started.</param>
/// <param name="Errors">What it wrote to its error output, or why it couldn't be started.</param>
public sealed record NodeRun(int? ExitCode, string Errors);

/// <summary>The machine's <c>node</c>, found on the path.</summary>
public sealed class MachineNodeRunner : INodeRunner
{
    public static MachineNodeRunner Instance { get; } = new();

    public NodeRun Run(string script, IReadOnlyList<string> arguments)
    {
        var start = new ProcessStartInfo("node") { RedirectStandardError = true, RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };
        start.ArgumentList.Add(script);
        foreach (var argument in arguments)
        {
            start.ArgumentList.Add(argument);
        }

        try
        {
            using var process = Process.Start(start)!;
            var errors = process.StandardError.ReadToEndAsync();
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return new NodeRun(process.ExitCode, errors.Result);
        }
        catch (Win32Exception e)
        {
            return new NodeRun(null, $"Node.js isn't on the path ({e.Message}).");
        }
    }
}
