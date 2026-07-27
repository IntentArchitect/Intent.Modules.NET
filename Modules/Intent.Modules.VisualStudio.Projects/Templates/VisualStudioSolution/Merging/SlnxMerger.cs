#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Intent.Engine;
using Intent.Exceptions;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution.Merging
{
    internal record SlnxMergeResult(string Content, HasDestructiveChanges HasDestructiveChanges);

    /// <summary>
    /// Reconciles the template's freshly-generated ("Ours") .slnx content with the real file on
    /// disk ("Existing"), using the template's own previous raw output ("Base") to tell renames
    /// and moves Intent wants to make apart from manual edits the user made directly to the file.
    /// Base and Ours carry an Id (the Intent element's own persistent Id) that lets the same
    /// project/folder be recognised across a rename - but that Id is purely private bookkeeping
    /// between merge runs: it is never read from or written to Existing, so the file a user opens
    /// never contains it.
    /// </summary>
    internal static class SlnxMerger
    {
        public static SlnxMergeResult Merge(string generated, string? existing, string? previousOutput)
        {
            var generatedModel = Parse(generated);

            if (existing == null)
            {
                return new SlnxMergeResult(Serialize(Rebuild(generatedModel)), HasDestructiveChanges.False);
            }

            var existingModel = ParseExisting(existing);
            var previousOutputModel = previousOutput == null ? null : TryParse(previousOutput);

            Reconcile(generatedModel, existingModel, previousOutputModel);

            // Rebuild fresh rather than serializing existingModel directly: this guarantees no Id
            // attribute ever reaches the file a user opens, regardless of whether one was already
            // present in Existing from some other source (e.g. a stray leftover from a different
            // module version) that this reconciliation pass never had reason to touch.
            return new SlnxMergeResult(Serialize(Rebuild(existingModel)), HasDestructiveChanges.False);
        }

        private static void Reconcile(SolutionModel generatedModel, SolutionModel existingModel, SolutionModel? previousOutputModel)
        {
            var baseFoldersById = previousOutputModel != null ? FoldersById(previousOutputModel) : new Dictionary<Guid, SolutionFolderModel>();
            var folderMapping = new Dictionary<Guid, SolutionFolderModel>();

            foreach (var depthGroup in generatedModel.SolutionFolders.GroupBy(FolderDepth).OrderBy(g => g.Key))
            {
                var claimed = new HashSet<SolutionFolderModel>();
                var resolutions = new List<(SolutionFolderModel Generated, SolutionFolderModel? Target, SolutionFolderModel? ResolvedParent)>();

                // Resolve identity for every folder at this depth against a frozen snapshot of
                // Existing before any of this depth's mutations happen - this stops two folders
                // whose old/new paths happen to collide in the same run (e.g. a literal swap) from
                // cross-contaminating each other's resolution.
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

                // SolutionPersistence validates path uniqueness eagerly on assignment (not only at
                // save time), so applying two renames that form a cycle (e.g. a literal swap of two
                // folders' names) in sequence would throw on the first assignment, even though the
                // final state is perfectly valid. Quarantine every folder being mutated to a
                // guaranteed-unique placeholder name first, so no intermediate assignment can ever
                // collide with another folder still awaiting its own rename.
                foreach (var (_, target, _) in resolutions)
                {
                    if (target != null)
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

            // Same eager-uniqueness-validation concern as folders above - quarantine every project
            // being mutated to a unique placeholder path before applying final paths, so a cycle
            // (e.g. two projects swapping paths in the same run) can never hit a transient collision.
            foreach (var (_, target) in projectResolutions)
            {
                if (target != null)
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

        /// <summary>
        /// Prefers a candidate resolved via the Base Id correlation (a rename/move Intent wants to
        /// make); falls back to a plain path match against Existing (legacy files with no Base Id,
        /// or manually-added entries). A candidate already claimed by an earlier resolution in this
        /// same pass is skipped so two generated entries never collide onto the same Existing entry.
        /// </summary>
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

        /// <summary>
        /// Builds a brand new, Id-free <see cref="SolutionModel"/> from <paramref name="source"/> -
        /// used only for the first-ever generation of a file, where there is no Existing content to
        /// reconcile against and <paramref name="source"/> (the template's raw output) still carries
        /// its private Ids.
        /// </summary>
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
            catch (SolutionException ex)
            {
                // Most commonly a manually-introduced duplicate <Project>/<Folder> entry (the same
                // path listed under two folders), which Microsoft.VisualStudio.SolutionPersistence
                // refuses to parse. There is no way to recover the file's manual customisations
                // automatically here - fix the duplicate by hand, or delete the file to let the
                // Software Factory regenerate it from scratch (this discards any manual additions).
                throw new FriendlyException(
                    $"Could not read the existing Visual Studio Solution file: {ex.Message} " +
                    "This is usually caused by a manually-edited entry that duplicates a Project or Folder path. " +
                    "Fix the duplicate entry in the file directly, or delete the file to have it regenerated from the Intent model (this will discard any manual customisations to the file).");
            }
        }

        private static SolutionModel? TryParse(string content)
        {
            try
            {
                return Parse(content);
            }
            catch (SolutionException)
            {
                // The cached previous template output is corrupt/unparsable - treat as if there is
                // no usable history to correlate against rather than failing the whole run over
                // stale internal state that is only ever an optimisation, never load-bearing.
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
