#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution.Merging
{
    /// <summary>
    /// Merges the template's generated .slnx ("Ours") with the file on disk ("Existing"), using the
    /// template's previous raw output ("Base") to correlate renames/moves against manual edits.
    /// Preservation is the default; content is removed only when a rename/move/removal requires it.
    /// </summary>
    internal static class SlnxMerger
    {
        public static string Merge(string generated, string? existing, string? previousOutput)
        {
            var generatedModel = Parse(generated);

            if (existing == null)
            {
                return Serialize(Rebuild(generatedModel));
            }

            var existingModel = ParseExisting(existing);
            var previousOutputModel = previousOutput == null ? null : TryParse(previousOutput);

            var projectsByOriginalPath = existingModel.SolutionProjects
                .Select(p => (Project: p, OriginalPath: p.FilePath))
                .ToList();

            Reconcile(generatedModel, existingModel, previousOutputModel);

            // SolutionPersistence's own DOM patching preserves everything outside a Project element
            // (Configurations, comments, whitespace). A Project element gets no such guarantee: its
            // attributes are regenerated from the typed model on every save regardless of whether it
            // was touched, dropping Type/DisplayName and eliding an identity BuildType rule as
            // redundant. Splicing the original raw XML back in below closes both gaps.
            var merged = Serialize(existingModel);
            merged = RestoreOriginalConfigurations(existing, merged);

            var projectPaths = projectsByOriginalPath
                .Select(x => (x.OriginalPath, FinalPath: x.Project.FilePath))
                .ToList();

            return RestoreOriginalProjectElements(existing, merged, projectPaths);
        }

        private static string RestoreOriginalConfigurations(string existingRaw, string merged)
        {
            var originalSpan = FindConfigurationsSpan(existingRaw);
            if (originalSpan == null)
                return merged;

            var originalText = existingRaw[originalSpan.Value.Start..originalSpan.Value.End];
            var mergedSpan = FindConfigurationsSpan(merged);
            if (mergedSpan != null)
                return merged[..mergedSpan.Value.Start] + originalText + merged[mergedSpan.Value.End..];

            var solutionTagEnd = FindTagEnd(merged, merged.IndexOf("<Solution", StringComparison.Ordinal));
            return merged[..(solutionTagEnd + 1)] + "\n  " + originalText + merged[(solutionTagEnd + 1)..];
        }

        private static (int Start, int End)? FindConfigurationsSpan(string xml)
        {
            var tagStart = xml.IndexOf("<Configurations", StringComparison.Ordinal);
            if (tagStart < 0)
                return null;

            var tagEnd = FindTagEnd(xml, tagStart);
            var openTag = xml[tagStart..(tagEnd + 1)];
            if (openTag.TrimEnd().EndsWith("/>", StringComparison.Ordinal))
                return (tagStart, tagEnd + 1);

            const string closeTag = "</Configurations>";
            var closeIndex = xml.IndexOf(closeTag, tagEnd + 1, StringComparison.Ordinal);
            return closeIndex < 0 ? null : (tagStart, closeIndex + closeTag.Length);
        }

        private static string RestoreOriginalProjectElements(string existingRaw, string merged, IReadOnlyList<(string OriginalPath, string FinalPath)> projectPaths)
        {
            foreach (var (originalPath, finalPath) in projectPaths)
            {
                var originalSpan = FindProjectElementSpan(existingRaw, originalPath);
                if (originalSpan == null)
                    continue;

                var originalElementText = existingRaw[originalSpan.Value.Start..originalSpan.Value.End];
                if (!PathsEqual(originalPath, finalPath))
                    originalElementText = WithReplacedPathAttribute(originalElementText, finalPath);

                var mergedSpan = FindProjectElementSpan(merged, finalPath);
                if (mergedSpan == null)
                    continue;

                merged = merged[..mergedSpan.Value.Start] + originalElementText + merged[mergedSpan.Value.End..];
            }

            return merged;
        }

        private static string WithReplacedPathAttribute(string projectElementText, string newPath)
        {
            var openTagEnd = FindTagEnd(projectElementText, 0);
            var openTag = projectElementText[..(openTagEnd + 1)];
            var rest = projectElementText[(openTagEnd + 1)..];

            var pathAttributeStart = openTag.IndexOf("Path=\"", StringComparison.Ordinal);
            if (pathAttributeStart < 0)
                return projectElementText;

            var valueStart = pathAttributeStart + "Path=\"".Length;
            var valueEnd = openTag.IndexOf('"', valueStart);
            var newOpenTag = openTag[..valueStart] + newPath + openTag[valueEnd..];

            return newOpenTag + rest;
        }

        private static (int Start, int End)? FindProjectElementSpan(string xml, string path)
        {
            var searchStart = 0;
            while (true)
            {
                var tagStart = xml.IndexOf("<Project", searchStart, StringComparison.Ordinal);
                if (tagStart < 0)
                    return null;

                var tagEnd = FindTagEnd(xml, tagStart);
                if (tagEnd < 0)
                    return null;

                var openTag = xml[tagStart..(tagEnd + 1)];
                if (!PathsEqual(ExtractAttribute(openTag, "Path"), path))
                {
                    searchStart = tagEnd + 1;
                    continue;
                }

                if (openTag.TrimEnd().EndsWith("/>", StringComparison.Ordinal))
                    return (tagStart, tagEnd + 1);

                const string closeTag = "</Project>";
                var closeIndex = xml.IndexOf(closeTag, tagEnd + 1, StringComparison.Ordinal);
                return closeIndex < 0 ? null : (tagStart, closeIndex + closeTag.Length);
            }
        }

        private static int FindTagEnd(string xml, int tagStart)
        {
            var inQuotes = false;
            var quoteChar = '"';
            for (var i = tagStart; i < xml.Length; i++)
            {
                var c = xml[i];
                if (inQuotes)
                {
                    if (c == quoteChar)
                        inQuotes = false;
                }
                else if (c is '"' or '\'')
                {
                    inQuotes = true;
                    quoteChar = c;
                }
                else if (c == '>')
                {
                    return i;
                }
            }

            return -1;
        }

        private static string? ExtractAttribute(string openTag, string attributeName)
        {
            var marker = $"{attributeName}=\"";
            var start = openTag.IndexOf(marker, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += marker.Length;
            var end = openTag.IndexOf('"', start);
            return end < 0 ? null : openTag[start..end];
        }

        private static void Reconcile(SolutionModel generatedModel, SolutionModel existingModel, SolutionModel? previousOutputModel)
        {
            var baseFoldersById = previousOutputModel != null ? FoldersById(previousOutputModel) : new Dictionary<Guid, SolutionFolderModel>();
            var folderMapping = new Dictionary<Guid, SolutionFolderModel>();

            foreach (var depthGroup in generatedModel.SolutionFolders.GroupBy(FolderDepth).OrderBy(g => g.Key))
            {
                var claimed = new HashSet<SolutionFolderModel>();
                var resolutions = new List<(SolutionFolderModel Generated, SolutionFolderModel? Target, SolutionFolderModel? ResolvedParent)>();

                foreach (var generatedFolder in depthGroup)
                {
                    var resolvedParent = generatedFolder.Parent is { } gp ? folderMapping[gp.Id] : null;
                    var target = ResolveTarget(
                        candidate: baseFoldersById.TryGetValue(generatedFolder.Id, out var baseFolder)
                        ? FindFolderByPath(existingModel, baseFolder.Path)
                        : null,
                        fallback: () => FindFolderByPath(existingModel, ChildPath(resolvedParent, generatedFolder.Name)),
                        claimed: claimed);

                    resolutions.Add((generatedFolder, target, resolvedParent));
                }

                // Quarantine only folders actually being renamed, to a unique placeholder name first,
                // so a cyclic rename (e.g. two folders swapping names) can't hit a transient collision.
                foreach (var (generatedFolder, target, _) in resolutions)
                {
                    if (target != null && target.Name != generatedFolder.Name)
                        target.Name = QuarantineName();
                }

                foreach (var (generatedFolder, target, resolvedParent) in resolutions)
                {
                    var resolved = target ?? existingModel.AddFolder(ChildPath(resolvedParent, generatedFolder.Name));
                    if (target != null)
                        ApplyFolderPlacement(resolved, generatedFolder.Name, resolvedParent);

                    folderMapping[generatedFolder.Id] = resolved;
                }
            }

            var baseProjectsById = previousOutputModel != null ? ProjectsById(previousOutputModel) : new Dictionary<Guid, SolutionProjectModel>();
            var projectClaimed = new HashSet<SolutionProjectModel>();
            var projectResolutions = new List<(SolutionProjectModel Generated, SolutionProjectModel? Target)>();

            foreach (var generatedProject in generatedModel.SolutionProjects)
            {
                var target = ResolveTarget(
                    candidate: baseProjectsById.TryGetValue(generatedProject.Id, out var baseProject)
                    ? FindProjectByPath(existingModel, baseProject.FilePath)
                    : null,
                    fallback: () => FindProjectByPath(existingModel, generatedProject.FilePath),
                    claimed: projectClaimed);

                projectResolutions.Add((generatedProject, target));
            }

            // Same rename-cycle concern as folders above, and same reason to skip an unchanged path.
            foreach (var (generatedProject, target) in projectResolutions)
            {
                if (target != null && !PathsEqual(target.FilePath, generatedProject.FilePath))
                    target.FilePath = QuarantinePath();
            }

            foreach (var (generatedProject, target) in projectResolutions)
            {
                var resolvedParent = generatedProject.Parent is { } gp ? folderMapping[gp.Id] : null;

                if (target != null)
                {
                    ApplyProjectPlacement(target, generatedProject.FilePath, resolvedParent);
                }
                else
                {
                    existingModel.AddProject(generatedProject.FilePath, null, resolvedParent);
                }
            }
        }

        private static string QuarantineName() => $"__slnx_merge_quarantine_{Guid.NewGuid():N}";

        private static string QuarantinePath() => $"__slnx_merge_quarantine_{Guid.NewGuid():N}/__slnx_merge_quarantine_{Guid.NewGuid():N}.tmp";

        private static TNode? ResolveTarget<TNode>(TNode? candidate, Func<TNode?> fallback, HashSet<TNode> claimed)
            where TNode : class
        {
            var target = candidate != null && !claimed.Contains(candidate) ? candidate : null;
            target ??= fallback() is { } exact && !claimed.Contains(exact) ? exact : null;

            if (target != null)
                claimed.Add(target);

            return target;
        }

        private static void ApplyFolderPlacement(SolutionFolderModel folder, string name, SolutionFolderModel? parent)
        {
            if (folder.Name != name)
                folder.Name = name;

            if (folder.Parent != parent)
                folder.MoveToFolder(parent);
        }

        private static void ApplyProjectPlacement(SolutionProjectModel project, string path, SolutionFolderModel? parent)
        {
            if (!PathsEqual(project.FilePath, path))
                project.FilePath = path;

            if (project.Parent != parent)
                project.MoveToFolder(parent);
        }

        // Used only for first-ever generation, where there is no Existing to reconcile against and
        // the template's raw output still carries its private Ids.
        private static SolutionModel Rebuild(SolutionModel source)
        {
            var result = new SolutionModel();
            var folderMap = new Dictionary<SolutionFolderModel, SolutionFolderModel>();

            foreach (var folder in source.SolutionFolders.OrderBy(FolderDepth))
            {
                var parent = folder.Parent != null ? folderMap[folder.Parent] : null;
                var newFolder = result.AddFolder(ChildPath(parent, folder.Name));
                foreach (var file in folder.Files ?? Array.Empty<string>())
                    newFolder.AddFile(file);

                folderMap[folder] = newFolder;
            }

            foreach (var project in source.SolutionProjects)
            {
                var parent = project.Parent != null ? folderMap[project.Parent] : null;
                result.AddProject(project.FilePath, null, parent);
            }

            return result;
        }

        private static int FolderDepth(SolutionFolderModel folder)
        {
            var depth = 0;
            for (var current = folder.Parent; current != null; current = current.Parent)
                depth++;

            return depth;
        }

        private static Dictionary<Guid, SolutionFolderModel> FoldersById(SolutionModel model) =>
            model.SolutionFolders
                .Where(f => !f.IsDefaultId)
                .GroupBy(f => f.Id)
                .ToDictionary(g => g.Key, g => g.First());

        private static Dictionary<Guid, SolutionProjectModel> ProjectsById(SolutionModel model) =>
            model.SolutionProjects
                .Where(p => !p.IsDefaultId)
                .GroupBy(p => p.Id)
                .ToDictionary(g => g.Key, g => g.First());

        private static SolutionFolderModel? FindFolderByPath(SolutionModel model, string path) =>
            model.SolutionFolders.FirstOrDefault(f => PathsEqual(f.Path, path));

        private static SolutionProjectModel? FindProjectByPath(SolutionModel model, string path) =>
            model.SolutionProjects.FirstOrDefault(p => PathsEqual(p.FilePath, path));

        private static string ChildPath(SolutionFolderModel? parent, string name) =>
            parent == null ? $"/{name}/" : $"{parent.Path}{name}/";

        private static SolutionModel Parse(string content)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return SolutionSerializers.SlnXml.OpenAsync(stream, CancellationToken.None).GetAwaiter().GetResult();
        }

        private static SolutionModel ParseExisting(string content)
        {
            try
            {
                return Parse(content);
            }
            catch (XmlException ex)
            {
                throw new Exception(WithFileContent(
                    $"Could not read the existing Visual Studio Solution file: it is not valid XML ({ex.Message}) " +
                    "Fix the XML directly, or delete the file to have it regenerated from the Intent model (this will discard any manual customisations to the file).",
                    content));
            }
            catch (SolutionException ex) when (IsDuplicateEntryError(ex))
            {
                throw new Exception(WithFileContent(
                    $"Could not read the existing Visual Studio Solution file: {ex.Message} " +
                    "This is usually caused by a manually-edited entry that duplicates a Project or Folder path. " +
                    "Fix the duplicate entry in the file directly, or delete the file to have it regenerated from the Intent model (this will discard any manual customisations to the file).",
                    content));
            }
            catch (SolutionException ex)
            {
                throw new Exception(WithFileContent(
                    $"Could not read the existing Visual Studio Solution file: {ex.Message} " +
                    "Fix the underlying issue directly, or delete the file to have it regenerated from the Intent model (this will discard any manual customisations to the file).",
                    content));
            }
            catch (Exception ex)
            {
                throw new Exception(WithFileContent(
                    $"Could not read the existing Visual Studio Solution file: {ex.Message} " +
                    "Delete the file to have it regenerated from the Intent model (this will discard any manual customisations to the file).",
                    content));
            }
        }

        // SolutionPersistence doesn't consistently set ErrorType for a duplicate Project/Folder path
        // (can come back Undefined), so the message text is checked too.
        private static bool IsDuplicateEntryError(SolutionException ex) =>
            ex.ErrorType is SolutionErrorType.DuplicateItemRef
            or SolutionErrorType.DuplicateName
            or SolutionErrorType.DuplicateProjectName
            or SolutionErrorType.DuplicateProjectPath
            or SolutionErrorType.DuplicateExtension
            or SolutionErrorType.DuplicateDefaultProjectType
            or SolutionErrorType.DuplicateProjectTypeId
            || ex.Message.Contains("Duplicate item", StringComparison.OrdinalIgnoreCase);

        private static string WithFileContent(string message, string content)
        {
            var normalized = content.Replace("\r\n", "\n");
            return $"{message}\n\nExisting file content:\n\n{normalized}";
        }

        private static SolutionModel? TryParse(string content)
        {
            try
            {
                return Parse(content);
            }
            catch (Exception ex) when (ex is XmlException or SolutionException)
            {
                // Corrupt/unparsable cached previous output (including a .sln-to-.slnx format switch,
                // which hands back the old classic .sln text here) - treat as no usable history.
                return null;
            }
        }

        private static string Serialize(SolutionModel model)
        {
            using var stream = new MemoryStream();
            SolutionSerializers.SlnXml.SaveAsync(stream, model, CancellationToken.None).GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static bool PathsEqual(string? a, string? b)
        {
            if (a == null || b == null)
                return a == b;

            return string.Equals(a.Replace('\\', '/'), b.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase);
        }
    }
}
