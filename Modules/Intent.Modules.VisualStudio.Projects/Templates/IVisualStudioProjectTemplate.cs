using System.Collections.Generic;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    public interface IVisualStudioProjectTemplate
    {
        string ProjectId { get; }
        string Name { get; }
        string FilePath { get; }
        string LoadContent();
        void UpdateContent(string content, ISoftwareFactoryEventDispatcher sfEventDispatcher);
        IEnumerable<INugetPackageInfo> RequestedNugetPackages();
        IEnumerable<string> GetTargetFrameworks();
    }
}