using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// Parses TypeScript with TypeScript's own parser: the worker (Worker/ts-reader.js) and <c>typescript.js</c> are embedded in
/// this assembly, written to a folder named for their content, and run by Node.js once for all of a run's files.
/// </summary>
internal static class TypeScriptWorker
{
    private const string Resources = "typescript-worker/";

    private static readonly string[] Names = ["ts-reader.js", "typescript.js"];

    private static readonly Lazy<string> Folder = new(FolderName);

    /// <summary>
    /// How deep a file's tree may nest, in syntax nodes; a deeper file is that file's error, read no further. Real code nests a
    /// few dozen levels; a long chain of <c>+</c> or calls nests one level a link.
    /// </summary>
    private const int TreeDepth = 1000;

    /// <summary>
    /// The response's JSON nests up to three levels a syntax node (the node, its children, a list of them), under four of its
    /// own. System.Text.Json's default of 64 failed every file's read on eShop10, whose jQuery, jQuery Validation and Bootstrap
    /// bundles nest 107, 114 and 73 levels.
    /// </summary>
    private static readonly JsonDocumentOptions Response = new() { MaxDepth = 3 * TreeDepth + 8 };

    /// <param name="files">Each file's path, the text to parse and how (<c>ts</c>, <c>tsx</c>, <c>js</c> or <c>jsx</c>).</param>
    /// <returns>Each file the worker read; or, when it couldn't run, why.</returns>
    public static (IReadOnlyList<TypeScriptFile> Files, string? Failure) Parse(IReadOnlyList<(string Path, string Text, string Kind)> files, INodeRunner node)
    {
        var work = Directory.CreateTempSubdirectory("intent-drift-ts-").FullName;
        try
        {
            var request = Path.Combine(work, "request.json");
            var response = Path.Combine(work, "response.json");
            File.WriteAllText(request, new JsonObject
            {
                ["maxDepth"] = TreeDepth,
                ["files"] =new JsonArray(files.Select(x => (JsonNode)new JsonObject { ["path"] = x.Path, ["text"] = x.Text, ["kind"] = x.Kind }).ToArray())
            }.ToJsonString());

            var run = node.Run(Path.Combine(Extract(), "ts-reader.js"), [request, response]);
            if (run.ExitCode is null)
            {
                return ([], $"The TypeScript reader runs TypeScript's own parser on Node.js, which couldn't be started: {run.Errors.Trim()}");
            }

            // The worker writes why it failed into the response, as a host may not pass on its error output.
            using var document = File.Exists(response) ? JsonDocument.Parse(File.ReadAllBytes(response), Response) : null;
            if (run.ExitCode is not 0 || document is null || document.RootElement.TryGetProperty("error", out _))
            {
                var why = document?.RootElement.TryGetProperty("error", out var error) == true ? error.GetString() : run.Errors;
                return ([], $"TypeScript's parser failed (exit code {run.ExitCode}): {why?.Trim()}");
            }

            var texts = files.ToDictionary(x => x.Path, x => x.Text, StringComparer.Ordinal);
            var kinds = document.RootElement.GetProperty("kinds").EnumerateArray().Select(x => x.ValueKind == JsonValueKind.String ? x.GetString()! : "").ToArray();
            var read = new List<TypeScriptFile>();
            foreach (var file in document.RootElement.GetProperty("files").EnumerateArray())
            {
                var path = file.GetProperty("path").GetString()!;
                var text = texts[path];
                if (file.TryGetProperty("error", out var error))
                {
                    read.Add(new TypeScriptFile(path, text, []) { Errors = [error.GetString()!] });
                    continue;
                }

                var positions = file.GetProperty("tokens").EnumerateArray().Select(x => x.GetInt32()).ToArray();
                var tokens = Enumerable.Range(0, positions.Length / 2).Select(i => (positions[2 * i], positions[2 * i + 1])).ToList();
                var parsed = new TypeScriptFile(path, text, tokens)
                {
                    Errors = file.GetProperty("diagnostics").EnumerateArray()
                        .Select(x => $"{x.GetProperty("m").GetString()} (line {LineOf(text, x.GetProperty("s").GetInt32())})")
                        .ToList()
                };
                parsed.Root = Node(file.GetProperty("tree"), parsed, kinds);
                read.Add(parsed);
            }

            return (read, null);
        }
        finally
        {
            Directory.Delete(work, recursive: true);
        }
    }

    private static TypeScriptNode Node(JsonElement json, TypeScriptFile file, string[] kinds)
    {
        var node = new TypeScriptNode(file, kinds[json.GetProperty("k").GetInt32()], json.GetProperty("s").GetInt32(), json.GetProperty("e").GetInt32())
        {
            Value = json.TryGetProperty("t", out var text) ? text.GetString() : null,
            Operator = json.TryGetProperty("o", out var op) ? kinds[op.GetInt32()] : null,
            Declared = json.TryGetProperty("d", out var declared) ? declared.GetString() : null,
            Flags = json.TryGetProperty("b", out var flags) ? flags.EnumerateArray().Select(x => x.GetString()!).ToHashSet(StringComparer.Ordinal) : new HashSet<string>()
        };
        if (json.TryGetProperty("c", out var children))
        {
            foreach (var property in children.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    node.Add(property.Name, property.Value.EnumerateArray().Select(x => Node(x, file, kinds)).ToList());
                }
                else
                {
                    node.Add(property.Name, Node(property.Value, file, kinds));
                }
            }
        }

        return node;
    }

    private static int LineOf(string text, int position) => text.AsSpan(0, Math.Min(position, text.Length)).Count('\n') + 1;

    /// <summary>
    /// Writes the worker and <c>typescript.js</c> to their folder unless they are there, on every run: the Software Factory's
    /// process outlives a run, and the temp folder may be cleaned between two. A later run, or another run at the same time,
    /// finds them there.
    /// </summary>
    private static string Extract()
    {
        var folder = Folder.Value;
        Directory.CreateDirectory(folder);
        foreach (var name in Names)
        {
            var target = Path.Combine(folder, name);
            if (File.Exists(target))
            {
                continue;
            }

            var partial = $"{target}.{Guid.NewGuid():N}.tmp";
            using (var stream = Resource(name))
            using (var file = File.Create(partial))
            {
                stream.CopyTo(file);
            }

            try
            {
                File.Move(partial, target);
            }
            catch (IOException) when (File.Exists(target))
            {
                // Another run wrote it first.
                File.Delete(partial);
            }
        }

        return folder;
    }

    /// <summary>The folder, named for the files' content, so a module upgrade that changes them writes a new one.</summary>
    private static string FolderName()
    {
        using var sha = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var name in Names)
        {
            using var stream = Resource(name);
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            sha.AppendData(memory.ToArray());
        }

        var hash = Convert.ToHexString(sha.GetHashAndReset())[..16].ToLowerInvariant();
        return Path.Combine(Path.GetTempPath(), "intent-drift-engine", $"typescript-{hash}");
    }

    private static Stream Resource(string name) =>
        typeof(TypeScriptWorker).Assembly.GetManifestResourceStream(Resources + name)
        ?? throw new InvalidOperationException($"The TypeScript worker's {name} is missing from the stock extensions' resources.");
}
