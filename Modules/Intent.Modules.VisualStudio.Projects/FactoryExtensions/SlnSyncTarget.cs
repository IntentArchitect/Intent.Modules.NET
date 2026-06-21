using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Templates;
using Microsoft.DotNet.Cli.Sln.Internal;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    internal sealed class SlnSyncTarget : IVsSolutionSyncTarget
    {
        private readonly Func<IFileMetadata> _getMetadata;

        public SlnSyncTarget(string solutionModelId, Func<IFileMetadata> getMetadata)
        {
            SolutionModelId = solutionModelId;
            _getMetadata = getMetadata;
        }

        public string SolutionModelId { get; }
        public IFileMetadata GetMetadata() => _getMetadata();

        public string ApplySolutionItems(string currentContent, IReadOnlyList<SolutionItemAction> actions, string diskContent = null)
        {
            var filePath = GetMetadata().GetFilePath();
            var slnFile = SlnFile.Read(filePath, currentContent);
            var original = slnFile.Generate();

            if (diskContent != null)
                PreserveDiskItems(slnFile, filePath, diskContent, actions);

            foreach (var action in actions)
            {
                switch (action.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAddedEvent:
                        if (SolutionItemExists(slnFile, filePath, action.PhysicalPath))
                            break;

                        if (action.FolderPath.Count == 0)
                        {
                            slnFile.AddSolutionItem(
                                parentProject: null,
                                solutionItemPhysicalPath: action.PhysicalPath,
                                relativeOutputPathPrefix: action.RelativeOutputPathPrefix,
                                hasMaterializedFolder: action.HasMaterializedFolder);
                            break;
                        }

                        var solutionFolderProject = action.FolderPath.Aggregate(
                            seed: default(SlnProject),
                            func: (current, folder) => current?.GetOrCreateFolder(folder.Id, folder.Name)
                                                     ?? slnFile.GetOrCreateFolder(folder.Id, folder.Name));

                        slnFile.AddSolutionItem(
                            parentProject: solutionFolderProject,
                            solutionItemPhysicalPath: action.PhysicalPath,
                            relativeOutputPathPrefix: action.RelativeOutputPathPrefix,
                            hasMaterializedFolder: action.HasMaterializedFolder);
                        break;

                    case SoftwareFactoryEvents.FileRemovedEvent:
                        slnFile.RemoveSolutionItem(action.PhysicalPath);
                        break;
                    default:
                        break;
                }
            }

            var updated = slnFile.Generate();
            return original == updated ? null : updated;
        }

        private void PreserveDiskItems(SlnFile slnFile, string slnFilePath, string diskContent, IReadOnlyList<SolutionItemAction> actions)
        {
            var slnDir = Path.GetDirectoryName(slnFilePath) ?? string.Empty;

            var removeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var action in actions)
            {
                if (action.EventIdentifier == SoftwareFactoryEvents.FileRemovedEvent)
                    removeSet.Add(Path.GetRelativePath(slnDir, action.PhysicalPath).Replace('/', '\\'));
            }

            var diskSlnFile = SlnFile.Read(slnFilePath, diskContent);
            foreach (var relativePath in GetSolutionItemRelativePaths(diskSlnFile))
            {
                if (removeSet.Contains(relativePath))
                    continue;
                var absolutePath = Path.Combine(slnDir, relativePath);
                if (SolutionItemExists(slnFile, slnFilePath, absolutePath))
                    continue;
                slnFile.AddSolutionItem(parentProject: null, solutionItemPhysicalPath: absolutePath,
                    relativeOutputPathPrefix: null, hasMaterializedFolder: false);
            }
        }

        private static IEnumerable<string> GetSolutionItemRelativePaths(SlnFile slnFile)
        {
            return slnFile.Projects
                .SelectMany(p => p.Sections)
                .Where(s => s.Id == "SolutionItems")
                .SelectMany(s => s.Properties.Keys);
        }

        private static bool SolutionItemExists(SlnFile slnFile, string slnFilePath, string itemPhysicalPath)
        {
            var slnDir = Path.GetDirectoryName(slnFilePath) ?? string.Empty;
            var relativePath = Path.GetRelativePath(slnDir, itemPhysicalPath).Replace('/', '\\');
            return slnFile.Projects
                .Any(p => p.Sections
                    .Any(s => s.Id == "SolutionItems"
                           && s.Properties.ContainsKey(relativePath)));
        }
    }
}
