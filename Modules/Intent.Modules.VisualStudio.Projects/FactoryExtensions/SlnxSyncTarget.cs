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

        public string ApplySolutionItems(string currentContent, IReadOnlyList<SolutionItemAction> actions)
        {
            var filePath = GetMetadata().GetFilePath();
            var solutionDir = Path.GetDirectoryName(filePath) ?? string.Empty;

            var existingFiles = GetExistingFiles(currentContent);
            var addItems = new List<(string RelativePath, string FolderPath)>();
            var removeRelativePaths = new List<string>();

            foreach (var action in actions)
            {
                var relativePath = Path.GetRelativePath(solutionDir, action.PhysicalPath).Replace('\\', '/');

                switch (action.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAddedEvent:
                        if (!existingFiles.Contains(relativePath))
                        {
                            addItems.Add((relativePath, BuildFolderPath(action.FolderPath)));
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

            if (addItems.Count == 0 && removeRelativePaths.Count == 0)
                return null;

            var resultContent = currentContent;

            // Adds: use SolutionModel. OpenAsync gives an XML-backed model where the
            // SolutionFolders collection IS populated, but each folder's Files list is
            // NOT (it stays null). We only call AddFile for paths not already in the XML,
            // so SaveAsync writes them from the in-memory list without producing duplicates.
            if (addItems.Count > 0)
            {
                var slnxModel = ParseSlnx(currentContent);
                foreach (var (relativePath, folderPath) in addItems)
                {
                    var folder = slnxModel.SolutionFolders
                        .FirstOrDefault(f => string.Equals(f.Path, folderPath, StringComparison.OrdinalIgnoreCase))
                        ?? slnxModel.AddFolder(folderPath);
                    folder.AddFile(relativePath);
                }
                resultContent = Serialize(slnxModel);
            }

            // Removes: manipulate the XML document directly. SolutionModel.RemoveFile does
            // not work on XML-backed models because the Files collection is null and the
            // underlying XML backing is not modified by RemoveFile.
            if (removeRelativePaths.Count > 0)
            {
                resultContent = RemoveFilesFromContent(resultContent, removeRelativePaths);
            }

            return resultContent;
        }

        private static string BuildFolderPath(IReadOnlyCollection<IntentSolutionFolderModel> folderPath)
        {
            return "/" + string.Join("/", folderPath.Select(f => f.Name)) + "/";
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
