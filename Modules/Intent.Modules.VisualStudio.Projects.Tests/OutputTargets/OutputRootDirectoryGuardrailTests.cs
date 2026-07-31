using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.OutputTargets
{
    /// <summary>
    /// Guards against a new absolute-placement consumer bypassing the Root Folder shift by reading
    /// <c>Application.OutputRootDirectory</c> directly instead of going through
    /// <c>OutputLocationOptions</c>. Every legitimate remaining usage outside the helper itself is
    /// listed explicitly in <see cref="AllowedUsages"/> - a new one anywhere else fails this test and
    /// forces a deliberate decision (add it here with a reason, or use <c>.RootDirectory</c> instead).
    /// </summary>
    public class OutputRootDirectoryGuardrailTests
    {
        private const string HelperFile = "OutputTargets/OutputLocationOptions.cs";

        // (file, line) -> why it's allowed to reference OutputRootDirectory directly.
        private static readonly Dictionary<(string File, int Line), string> AllowedUsages = new()
        {
            [("Templates/DirectoryPackagesProps/DirectoryPackagesPropsTemplatePartial.cs", 64)] =
                "constructor null-fallback: builds an unshifted OutputLocationOptions when the registration didn't supply one",
            [("Templates/GitIgnore/GitIgnoreTemplatePartial.cs", 40)] =
                "constructor null-fallback: builds an unshifted OutputLocationOptions when the registration didn't supply one",
            [("Templates/VisualStudioSolution/VisualStudioSolutionSlnxTemplate.cs", 43)] =
                "constructor null-fallback: builds an unshifted OutputLocationOptions when the registration didn't supply one",
            [("Templates/VisualStudioSolution/VisualStudioSolutionSlnxTemplate.cs", 56)] =
                "relative-math base: the anchor both the project's and the .sln's shift offsets are already expressed relative to - not an absolute placement",
            [("Templates/VisualStudioSolution/VisualStudioSolutionTemplate.cs", 35)] =
                "constructor null-fallback: builds an unshifted OutputLocationOptions when the registration didn't supply one",
            [("Templates/VisualStudioSolution/VisualStudioSolutionTemplate.cs", 68)] =
                "relative-math base: the anchor both the project's and the .sln's shift offsets are already expressed relative to - not an absolute placement",
        };

        [Fact]
        public void OutputRootDirectory_ShouldOnlyBeUsedInTheHelperOrExplicitlyAllowedSpots()
        {
            var moduleRoot = GetModuleRoot();
            var unexpected = new List<string>();

            foreach (var file in Directory.GetFiles(moduleRoot, "*.cs", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(moduleRoot, file).Replace('\\', '/');
                if (relativePath == HelperFile)
                {
                    continue;
                }

                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var trimmed = lines[i].Trim();
                    if (!trimmed.Contains("OutputRootDirectory") || trimmed.StartsWith("//") || trimmed.StartsWith("///") || trimmed.StartsWith("*"))
                    {
                        continue;
                    }

                    var lineNumber = i + 1;
                    if (!AllowedUsages.ContainsKey((relativePath, lineNumber)))
                    {
                        unexpected.Add($"{relativePath}:{lineNumber}: {trimmed}");
                    }
                }
            }

            unexpected.ShouldBeEmpty();
        }

        private static string GetModuleRoot([CallerFilePath] string testFilePath = "")
        {
            var testsProjectOutputTargetsDir = Path.GetDirectoryName(testFilePath)!;
            return Path.GetFullPath(Path.Combine(testsProjectOutputTargetsDir, "..", "..", "Intent.Modules.VisualStudio.Projects"));
        }
    }
}
