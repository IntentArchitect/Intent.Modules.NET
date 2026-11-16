using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Utils;

namespace Intent.Modules.Common.DriftEngine.Reporting
{
    /// <summary>
    /// The engine's run inside the Software Factory. It reads the designers the stock packs check and the Codebase Structure's
    /// Drift Scope, runs the engine over the application's output folder, and adds each finding to its Area of the drift
    /// report (Intent.Common.DriftReporting logs them and writes the report). It only reads code.
    /// </summary>
    internal static class DriftEngineReport
    {
        private const string ScopeArea = "Drift Scope (DriftEngine)";

        /// <summary>
        /// The designers the engine reads, by the name packs give them. The ids are each designer module's own
        /// (<c>ApiMetadataDesignerExtensions</c>), copied so the engine needs none of those modules installed. A designer
        /// here that no pack or extension reads is reported as unchecked (R7), so the SDK's lack of a list of an
        /// application's designers doesn't hide one.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string> DesignerIds = new Dictionary<string, string>
        {
            ["Codebase Structure"] = "0701433c-36c0-4569-b1f4-9204986b587d",
            ["Domain"] = "6ab29b31-27af-4f56-a67c-986d82097d63",
            ["Services"] = "81104ae6-2bc5-4bae-b05a-f987b0372d81",
            ["User Interface"] = "f492faed-0665-4513-9853-5a230721786f"
        };

        // Every stock pack and extension runs; one whose designer isn't installed makes no claims. Choosing them on the Root
        // Folder's Drift Check stereotype (scope.md) replaces these lists when that stereotype is modelled.
        private static readonly string[] StockPacks = { "intent.domain", "intent.services", "intent.ui" };

        /// <param name="context">The drift report's context.</param>
        /// <param name="extensions">The extensions installed modules registered; they run after the stock ones, so theirs win.</param>
        /// <param name="node">Intent Architect's own Node.js, which runs TypeScript's parser.</param>
        public static void Report(IDriftContext context, IReadOnlyList<IDriftExtension> extensions, INodeRunner node)
        {
            var started = Stopwatch.GetTimestamp();
            var application = context.Application;
            var config = application.GetApplicationConfig();
            var output = Path.GetFullPath(Path.Combine(config.DirectoryPath, config.OutputLocation));
            var designers = new Dictionary<string, IReadOnlyList<IPackage>>();
            var references = new Dictionary<string, IReadOnlyList<IPackage>>();
            var all = StockExtensions.With(node).Concat(extensions).ToList();

            // A module's own designer is read under the name its extension gives it. Packages another application owns are
            // only references: read for what they say, never claimed on.
            var designerIds = DesignerIds.Concat(all.SelectMany(x => x.Designers)).GroupBy(x => x.Key).Select(x => x.Last());
            foreach (var (name, id) in designerIds)
            {
                var loaded = application.MetadataManager.GetDesigner(config.Id, id)?.Packages.ToList() ?? new List<IPackage>();
                var packages = loaded.Where(x => x.ApplicationId == config.Id).ToList();
                if (packages.Count > 0)
                {
                    designers[name] = packages;
                }

                var referenced = loaded.Where(x => x.ApplicationId != config.Id).ToList();
                if (referenced.Count > 0)
                {
                    references[name] = referenced;
                }
            }

            var model = Stopwatch.GetElapsedTime(started);
            RunResult result;
            try
            {
                result = DriftEngine.Run(new DriftInput(designers, output, StockPacks.Select(x => Pack.Stock(x)).ToList(), all) { References = references });
            }
            catch (Exception e)
            {
                context.Area(ScopeArea).Log(new DriftFinding(DriftCategory.Code, DriftSeverity.Error,
                    $"The drift engine failed, so nothing could be verified: {e.GetType().Name}: {e.Message}", DriftAnchor.Text(ScopeArea)));
                return;
            }

            // Until a Drift Scope says what code to read the check is off. It only warns, and has no Area (which would show as
            // checked): Intent applies none of a failing run's changes.
            if (result.Findings.FirstOrDefault(x => x.Rule == "scope/missing") is { } missing)
            {
                Logging.Log.Warning(missing.Message);
                return;
            }

            // What was checked, so a run with no drift is told apart from one that checked nothing.
            Logging.Log.Info($"Drift engine: checked {result.Claims.Count} claims against {result.Files.Count} files: " +
                $"{result.Claims.Count(x => x.Outcome == "verified")} verified, {result.Claims.Count(x => x.Outcome == "contradicted")} contradicted, " +
                $"{result.Claims.Count(x => x.Outcome == "cantVerify")} can't verify.");
            var timings = $"Drift engine timings: model {model.TotalMilliseconds:0} ms, " +
                string.Join(", ", result.Timings.Take(result.Timings.Count(x => !x.What.StartsWith("kind ")) + 8)
                    .Select(x => $"{x.What} {x.Milliseconds:0} ms{(x.Count is { } count ? $" ({count})" : "")}"));
            Logging.Log.Info(timings);

            // The report holds only drift: an Area is created by its first finding and declares no categories, so nothing
            // shows as "Clean".
            var areas = new Dictionary<string, IDriftArea>();
            var elements = designers.Values
                .SelectMany(x => x)
                .SelectMany(x => Descendants(x.ChildElements))
                .SelectMany(x => new[] { x }.Concat(DriftEngine.Ends(x)))
                .GroupBy(x => x.Id)
                .ToDictionary(x => x.Key, x => x.First());
            foreach (var finding in result.Findings)
            {
                if (finding.Severity == "info")
                {
                    Logging.Log.Info(finding.Message);
                    continue;
                }

                if (!areas.TryGetValue(finding.Area, out var area))
                {
                    area = areas[finding.Area] = context.Area(finding.Area);
                }

                var claim = result.Claims.FirstOrDefault(x => x.Element == finding.Subject.Element && finding.Rule == $"claim/{x.Kind}");
                var at = claim?.Reading?.At ?? claim?.Reading?.Candidates?.FirstOrDefault();
                var against = (finding.Against ?? Array.Empty<Anchor>()).Select(x => Anchor(x, elements, output))
                    .Append(at == null ? null : DriftAnchor.File(Full(output, at.File), at.Line))
                    .Where(x => x != null)
                    .ToArray();
                area.Log(new DriftFinding(
                    finding.Category switch
                    {
                        "ModelCode" => DriftCategory.ModelCode,
                        "CodeCode" => DriftCategory.CodeCode,
                        _ => DriftCategory.Code
                    },
                    finding.Severity == "warning" ? DriftSeverity.Warning : DriftSeverity.Error,
                    finding.Message,
                    Anchor(finding.Subject, elements, output) ?? DriftAnchor.Text(finding.Area),
                    against)
                {
                    Rule = finding.Rule
                });
            }
        }

        /// <summary>A finding's subject: its element (a package isn't one, so a Root Folder is named), file or text.</summary>
        private static DriftAnchor Anchor(Anchor anchor, IReadOnlyDictionary<string, IElement> elements, string output)
        {
            if (anchor.Element != null)
            {
                return elements.TryGetValue(anchor.Element, out var element) ? DriftAnchor.Element(element) : DriftAnchor.Text("Codebase Structure Root Folder");
            }

            if (anchor.File != null)
            {
                return DriftAnchor.File(Full(output, anchor.File), anchor.Line);
            }

            return anchor.Text == null ? null : DriftAnchor.Text(anchor.Text);
        }

        private static string Full(string output, string file) => Path.GetFullPath(Path.Combine(output, file)).Replace('\\', '/');

        private static IEnumerable<IElement> Descendants(IEnumerable<IElement> elements) =>
            elements.SelectMany(x => new[] { x }.Concat(Descendants(x.ChildElements)));
    }
}
