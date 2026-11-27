using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// A file of an extension several frameworks share goes to the one whose code claims it (code-reader.md, Claimed files): the
/// strongest evidence, then the one whose syntax the file is written in, then the higher priority, then the extension
/// registered last. A file nothing reads is a <c>scope/unread</c> finding, which the Drift Check's exceptions take.
/// </summary>
public class ClaimedFileTests
{
    private const string Scope = """
        {
          "designers": {
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Site", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "web/**", "Scope": "In" } ] }EXCEPTIONS } }]
            }
          }
        }
        """;

    [Fact]
    public void Stronger_evidence_wins_then_syntax_then_priority()
    {
        var a = new Framework("alpha", priority: 0, syntax: "{%", ("web/named.html", ClaimEvidence.Named), ("web/both.html", ClaimEvidence.Folder), ("web/tie.html", ClaimEvidence.Folder));
        var b = new Framework("beta", priority: 1, syntax: "@if", ("web/named.html", ClaimEvidence.Folder), ("web/both.html", ClaimEvidence.Folder), ("web/tie.html", ClaimEvidence.Folder));

        var result = Run(new Dictionary<string, string>
        {
            ["web/named.html"] = "<p>@if</p>",
            ["web/both.html"] = "<p>{% if x %}</p>",
            ["web/tie.html"] = "<p>plain</p>",
            ["web/lonely.html"] = "<p>nobody</p>"
        }, a, b);

        Read(result, "web/named.html").Reader.ShouldBe("alpha");
        Read(result, "web/named.html").Claimed.ShouldBe("alpha evidence; also claimed by beta, decided by evidence");
        Read(result, "web/both.html").Reader.ShouldBe("alpha");
        Read(result, "web/both.html").Claimed.ShouldBe("alpha evidence; also claimed by beta, decided by its syntax");
        Read(result, "web/tie.html").Reader.ShouldBe("beta");
        Read(result, "web/tie.html").Claimed.ShouldBe("beta evidence; also claimed by alpha, decided by priority");
        a.Read.ShouldBe(["web/both.html", "web/named.html"]);
        b.Read.ShouldBe(["web/tie.html"]);

        var unread = result.Findings.Single(x => x.Rule == "scope/unread");
        unread.Subject.File.ShouldBe("web/lonely.html");
        unread.Message.ShouldContain("the alpha and beta readers read `.html` files only where their code names one");
    }

    [Fact]
    public void An_exception_takes_an_unread_file_and_one_that_matches_nothing_is_stale()
    {
        var result = Run(new Dictionary<string, string> { ["web/site.css"] = "body {}" }, exceptions: """
            , "Drift Check": { "Exceptions": [
                { "Rule": "scope/unread", "Match": "web/**/*.css", "Reason": "Stylesheets" },
                { "Rule": "scope/unread", "Match": "web/**/*.svg", "Reason": "Images" } ] }
            """);

        result.Findings.ShouldNotContain(x => x.Rule == "scope/unread");
        result.Findings.Single(x => x.Rule == "optin/staleException").Message.ShouldContain("web/**/*.svg");
        result.Findings.ShouldNotContain(x => x.Rule == "config/unknownRule");
    }

    private static FileRead Read(RunResult result, string path) => result.Files.Single(x => x.Path == path);

    private static RunResult Run(IReadOnlyDictionary<string, string> files, params IDriftExtension[] extensions) => Run(files, "", extensions);

    private static RunResult Run(IReadOnlyDictionary<string, string> files, string exceptions, params IDriftExtension[] extensions)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-claims-").FullName;
        try
        {
            foreach (var (path, text) in files)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(root, path))!);
                File.WriteAllText(Path.Combine(root, path), text);
            }

            var designers = FixtureModel.Build(JsonNode.Parse(Scope.Replace("EXCEPTIONS", exceptions))!, out var references);
            return DriftEngine.Run(new DriftInput(designers, root, [Pack.Stock("intent.core")!], extensions) { References = references });
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    /// <summary>A framework whose code claims the given files with the given evidence, and records what it was given to read.</summary>
    private sealed class Framework(string name, int priority, string syntax, params (string Path, ClaimEvidence Evidence)[] claims) : IDriftExtension, IClaimingLanguage
    {
        public List<string> Read { get; } = [];

        public string Id => name;

        public IEnumerable<ILanguage> Languages => [this];

        public string Name => name;

        public string Title => name;

        public int Priority => priority;

        public bool Reads(string path) => false;

        public bool MayClaim(string path) => path.EndsWith(".html", StringComparison.Ordinal);

        public bool WrittenIn(string text) => text.Contains(syntax, StringComparison.Ordinal);

        public IEnumerable<FileClaim> Claim(string root, IReadOnlyList<string> files, CodeGraph graph) =>
            claims.Where(x => files.Contains(x.Path)).Select(x => new FileClaim(x.Path, x.Evidence, $"{name} evidence"));

        void ILanguage.Read(string root, IReadOnlyList<string> files, CodeGraph graph, CancellationToken cancellationToken)
        {
            Read.AddRange(files);
            foreach (var file in files)
            {
                graph.Files.Add(new FileNode { Path = file, Language = name });
            }
        }
    }
}
