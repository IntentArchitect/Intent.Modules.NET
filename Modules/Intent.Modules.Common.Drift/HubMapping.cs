using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftReporting.Api;

namespace Intent.Modules.Common.Drift
{
    /// <summary>
    /// A designer's drift check as findings in its Area of the drift report. Every drift line is a Model / Code error on the
    /// innermost model element of its row that the designer's <see cref="DriftDesigner.ReadElements"/> resolves (else the
    /// row's name, or its file), against the code's file and line. What is wrong with the recipe file is a Code finding on
    /// it: an error when the recipe can't be read, a warning for a kind of fact with no recipe or a stale exception. The
    /// check's counts and explanations are notes.
    /// </summary>
    internal static class HubMapping
    {
        public static void Map(DriftReport report, IDriftArea area, IReadOnlyDictionary<string, IElement> elements, string codeRoot, string recipeFile)
        {
            foreach (var finding in report.Findings)
            {
                area.Log(ToFinding(finding, elements, codeRoot, recipeFile));
            }

            foreach (var kind in report.NotChecked)
            {
                area.Log(new DriftFinding(DriftCategory.Code, DriftSeverity.Warning,
                    $"{kind}: `{report.RecipePath}` has no recipe for it, so it isn't checked. Load the configure-drift-check skill to add one.",
                    DriftAnchor.File(recipeFile)));
            }

            foreach (var exception in report.UnusedExceptions)
            {
                area.Log(new DriftFinding(DriftCategory.Code, DriftSeverity.Warning,
                    $"The exception {exception} matches nothing: remove it from `{report.RecipePath}`.",
                    DriftAnchor.File(recipeFile)));
            }
        }

        internal static DriftFinding ToFinding(Finding finding, IReadOnlyDictionary<string, IElement> elements, string codeRoot, string recipeFile)
        {
            var row = finding.Row;
            var message = row?.Drift ?? finding.Line;
            var element = Innermost(row?.ElementIds, elements);
            var name = row?.Element == null ? null : DriftAnchor.Text($"`{row.Element}`");
            var file = FileAnchor(row?.Path ?? finding.File, row?.Line ?? 0, row?.FileExists ?? true, codeRoot);
            switch (finding.Section)
            {
                case DriftSection.Contradicted:
                    return ModelCode(message, element ?? name ?? file ?? DriftAnchor.File(recipeFile), element != null || name != null ? file : null);
                case DriftSection.Missing:
                    var expected = string.IsNullOrEmpty(row?.Path ?? finding.File) ? null : DriftAnchor.Text($"`{(row?.Path ?? finding.File).Replace('\\', '/')}`");
                    return ModelCode(message, element ?? name ?? expected ?? DriftAnchor.File(recipeFile), element != null || name != null ? expected : null);
                case DriftSection.Undescribed:
                    return ModelCode(message, element ?? file ?? DriftAnchor.File(recipeFile), element != null ? file : null);
                default:
                    // A fact that can't be verified names its element; the recipe file being wrong, or the check failing, doesn't.
                    if (row?.Element != null || row?.ElementIds.Count > 0)
                    {
                        var where = file ?? DriftAnchor.File(recipeFile);
                        return (element ?? name) is { } subject ? ModelCode(message, subject, where) : ModelCode(message, where, null);
                    }

                    return new DriftFinding(DriftCategory.Code, DriftSeverity.Error, message, DriftAnchor.File(recipeFile));
            }
        }

        private static DriftFinding ModelCode(string message, DriftAnchor subject, DriftAnchor against) =>
            new(DriftCategory.ModelCode, DriftSeverity.Error, message, subject, against == null ? Array.Empty<DriftAnchor>() : new[] { against });

        /// <summary>The innermost of a row's elements (they are outermost first: a class, then its attribute) the model has.</summary>
        private static DriftAnchor Innermost(IReadOnlyList<string> ids, IReadOnlyDictionary<string, IElement> elements)
        {
            var element = (ids ?? Array.Empty<string>()).Reverse().Select(x => elements.GetValueOrDefault(x)).FirstOrDefault(x => x != null);
            return element == null ? null : DriftAnchor.Element(element);
        }

        /// <summary>
        /// The row's code file, absolute, with its line. A file the model expects but the code doesn't have yet (one this run's
        /// pending changes add) is named, not linked.
        /// </summary>
        private static DriftAnchor FileAnchor(string path, int line, bool exists, string codeRoot)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (!exists)
            {
                return DriftAnchor.Text($"`{path.Replace('\\', '/')}`");
            }

            var full = Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(codeRoot, path));
            return DriftAnchor.File(full.Replace('\\', '/'), line > 0 ? line : null);
        }
    }
}
