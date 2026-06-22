using System.Collections.Generic;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    internal interface IVsSolutionSyncTarget
    {
        string SolutionModelId { get; }
        IFileMetadata GetMetadata();
        string ApplySolutionItems(string currentContent, IReadOnlyList<SolutionItemAction> actions, string diskContent = null);
    }

    internal sealed class SolutionItemAction
    {
        public required string EventIdentifier { get; init; }
        public required string PhysicalPath { get; init; }
        public required IReadOnlyCollection<SolutionFolderModel> FolderPath { get; init; }
        public string RelativeOutputPathPrefix { get; init; }
        public bool HasMaterializedFolder { get; init; }
    }
}
