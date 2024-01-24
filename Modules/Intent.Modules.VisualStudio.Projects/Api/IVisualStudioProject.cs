using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;

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
        IList<RoleModel> Roles { get; }
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

    public static class VisualStudioProjectExtensions
    {
        public static IList<IOutputTargetRole> GetRoles(this IVisualStudioProject project)
        {
            return project.Roles.Select(x => new ProjectOutput(x.Name, x.Folder?.Name)).Cast<IOutputTargetRole>()
                .Concat(project.Folders.SelectMany(project.GetRolesInFolder))
                .ToList();
        }

        private static IEnumerable<IOutputTargetRole> GetRolesInFolder(this IVisualStudioProject project, FolderModel folder)
        {
            var roles = folder.Roles.Select(x => new ProjectOutput(x.Name, x.Folder?.Name)).ToList<IOutputTargetRole>();
            return roles;
        }
    }
}