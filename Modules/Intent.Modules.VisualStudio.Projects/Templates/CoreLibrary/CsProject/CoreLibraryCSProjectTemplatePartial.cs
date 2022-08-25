using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreLibrary.CsProject
{
    partial class CoreLibraryCSProjectTemplate : VisualStudioProjectTemplateBase<ClassLibraryNETCoreModel>
    {
        public const string TemplateId = "Intent.VisualStudio.Projects.CoreLibrary.CSProject";

        public CoreLibraryCSProjectTemplate(IOutputTarget outputTarget, ClassLibraryNETCoreModel model)
            : base(TemplateId, outputTarget, model)
        {
        }
    }
}
