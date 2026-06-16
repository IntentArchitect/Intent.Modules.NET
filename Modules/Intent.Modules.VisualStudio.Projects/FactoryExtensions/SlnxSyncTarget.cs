using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Templates;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using IntentSolutionFolderModel = Intent.Modules.VisualStudio.Projects.Api.SolutionFolderModel;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    internal sealed class SlnxSyncTarget : IVsSolutionSyncTarget
    {
        private readonly Func<IFileMetadata> _getMetadata;

        public SlnxSyncTarget(string solutionModelId, Func<IFileMetadata> getMetadata)
        {
            SolutionModelId = solutionModelId;
            _getMetadata = getMetadata;
        }

        public string SolutionModelId { get; }
        public IFileMetadata GetMetadata() => _getMetadata();

        public string ApplySolutionItems(string currentContent, IReadOnlyList<SolutionItemAction> actions, string diskContent = null)
        {
            var filePath = GetMetadata().GetFilePath();
            var solutionDir = Path.GetDirectoryName(filePath) ?? string.Empty;

            var existingFiles = GetExistingFiles(currentContent);

            // Build remove set first so disk-item preservation can respect explicit removes
            var removeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var action in actions)
            {
                if (action.EventIdentifier == SoftwareFactoryEvents.FileRemovedEvent)
                    removeSet.Add(Path.GetRelativePath(solutionDir, action.PhysicalPath).Replace('\\', '/'));
            }

            // Disk items to preserve are injected via XDocument to avoid SolutionModel.AddFolder
            // constraints — it rejects certain folder path strings on XML-backed models.
            var diskAddItems = new List<(string RelativePath, string FolderPath)>();
            if (diskContent != null)
            {
                foreach (var (path, folderPath) in GetExistingFilesWithFolders(diskContent))
                {
                    if (!existingFiles.Contains(path) && !removeSet.Contains(path))
                    {
                        diskAddItems.Add((path, folderPath));
                        existingFiles.Add(path);
                    }
                }
            }

            // Intent-driven adds use SolutionModel so folder hierarchy is created correctly.
            var intentAddItems = new List<(string RelativePath, string FolderPath)>();
            var removeRelativePaths = new List<string>();
            foreach (var action in actions)
            {
                var relativePath = Path.GetRelativePath(solutionDir, action.PhysicalPath).Replace('\\', '/');

                switch (action.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAddedEvent:
                        if (!existingFiles.Contains(relativePath))
                        {
                            intentAddItems.Add((relativePath, BuildFolderPath(action.FolderPath)));
                            existingFiles.Add(relativePath);
                        }
                        break;

                    case SoftwareFactoryEvents.FileRemovedEvent:
                        if (existingFiles.Contains(relativePath))
                        {
                            removeRelativePaths.Add(relativePath);
                            existingFiles.Remove(relativePath);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (intentAddItems.Count == 0 && diskAddItems.Count == 0 && removeRelativePaths.Count == 0)
                return null;

            var resultContent = currentContent;

            // Phase 1 — Intent-driven adds via SolutionModel (handles folder creation).
            if (intentAddItems.Count > 0)
            {
                var slnxModel = ParseSlnx(resultContent);
                foreach (var (relativePath, folderPath) in intentAddItems)
                {
                    var folder = slnxModel.SolutionFolders
                        .FirstOrDefault(f => string.Equals(f.Path, folderPath, StringComparison.OrdinalIgnoreCase))
                        ?? slnxModel.AddFolder(folderPath);
                    folder.AddFile(relativePath);
                }
                resultContent = Serialize(slnxModel);
            }

            // Phase 2 — Disk item preservation via XDocument (avoids SolutionModel.AddFolder constraints).
            if (diskAddItems.Count > 0)
                resultContent = InjectFilesIntoContent(resultContent, diskAddItems);

            // Phase 3 — Explicit removes via XDocument (SolutionModel.RemoveFile is a no-op on XML-backed models).
            if (removeRelativePaths.Count > 0)
                resultContent = RemoveFilesFromContent(resultContent, removeRelativePaths);

            return resultContent;
        }

        private static string BuildFolderPath(IReadOnlyCollection<IntentSolutionFolderModel> folderPath)
        {
            return "/" + string.Join("/", folderPath.Select(f => f.Name)) + "/";
        }

        private static IReadOnlyList<(string RelativePath, string FolderPath)> GetExistingFilesWithFolders(string content)
        {
            var result = new List<(string, string)>();
            if (string.IsNullOrWhiteSpace(content))
                return result;
            try
            {
                var doc = XDocument.Parse(content);
                foreach (var folderEl in doc.Descendants("Folder"))
                {
                    var folderPath = folderEl.Attribute("Path")?.Value ?? "/";
                    foreach (var fileEl in folderEl.Elements("File"))
                    {
                        var path = fileEl.Attribute("Path")?.Value;
                        if (path != null)
                            result.Add((path, folderPath));
                    }
                }
            }
            catch
            {
                // Ignore parse errors; treat as empty
            }
            return result;
        }

        private static HashSet<string> GetExistingFiles(string content)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(content))
                return result;
            try
            {
                var doc = XDocument.Parse(content);
                foreach (var fileEl in doc.Descendants("File"))
                {
                    var path = fileEl.Attribute("Path")?.Value;
                    if (path != null)
                        result.Add(path);
                }
            }
            catch
            {
                // Ignore parse errors; treat as empty
            }
            return result;
        }

        private static string InjectFilesIntoContent(string content, IReadOnlyList<(string RelativePath, string FolderPath)> items)
        {
            var doc = XDocument.Parse(content);
            var root = doc.Root;
            foreach (var (relativePath, folderPath) in items)
            {
                var folderEl = root
                    .Elements("Folder")
                    .FirstOrDefault(f => string.Equals(f.Attribute("Path")?.Value, folderPath, StringComparison.OrdinalIgnoreCase));
                if (folderEl == null)
                {
                    folderEl = new XElement("Folder", new XAttribute("Path", folderPath));
                    root.Add(folderEl);
                }
                folderEl.Add(new XElement("File", new XAttribute("Path", relativePath)));
            }
            using var stream = new MemoryStream();
            doc.Save(stream);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static string RemoveFilesFromContent(string content, IReadOnlyList<string> relativePaths)
        {
            var doc = XDocument.Parse(content);
            foreach (var relativePath in relativePaths)
            {
                var toRemove = doc.Descendants("File")
                    .Where(f => string.Equals(f.Attribute("Path")?.Value, relativePath, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                foreach (var el in toRemove)
                    el.Remove();
            }
            using var stream = new MemoryStream();
            doc.Save(stream);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static SolutionModel ParseSlnx(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new SolutionModel();
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return SolutionSerializers.SlnXml.OpenAsync(stream, CancellationToken.None).GetAwaiter().GetResult();
        }

        private static string Serialize(SolutionModel model)
        {
            using var stream = new MemoryStream();
            SolutionSerializers.SlnXml.SaveAsync(stream, model, CancellationToken.None).GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
