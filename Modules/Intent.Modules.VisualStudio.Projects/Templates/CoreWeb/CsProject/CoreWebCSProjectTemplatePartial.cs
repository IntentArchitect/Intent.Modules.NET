using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject
{
    partial class CoreWebCSProjectTemplate : VisualStudioProjectTemplateBase<ASPNETCoreWebApplicationModel>
    {
        public const string TemplateId = "Intent.VisualStudio.Projects.CoreWeb.CSProject";

        public CoreWebCSProjectTemplate(IOutputTarget project, ASPNETCoreWebApplicationModel model)
            : base(TemplateId, project, model)
        {
        }
    }
}
