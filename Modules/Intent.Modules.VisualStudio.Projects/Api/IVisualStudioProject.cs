using System.Collections.Generic;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public interface IVisualStudioSolutionProject : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IOutputTargetConfig ToOutputTargetConfig();
        SolutionFolderModel ParentFolder { get; }
        string FileExtension { get; }
        public IElement InternalElement { get; }
        string ProjectTypeId { get; }
        string RelativeLocation { get; }
        VisualStudioSolutionModel Solution { get; }
        IList<OutputAnchorModel> OutputAnchors { get; }
        IList<TemplateOutputModel> TemplateOutputs { get; }
        IList<FolderModel> Folders { get; }
    }

    public interface IVisualStudioProject : IVisualStudioSolutionProject
    {
        string Type { get; }

        IEnumerable<string> TargetFrameworkVersion();
        string LanguageVersion { get; }
        bool NullableEnabled { get; }
    }
}