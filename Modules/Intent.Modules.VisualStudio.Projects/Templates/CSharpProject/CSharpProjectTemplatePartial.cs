using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CSharpProject
{
    partial class CSharpProjectTemplate : VisualStudioProjectTemplateBase<CSharpProjectNETModel>
    {
        public const string TemplateId = "Intent.VisualStudio.Projects.CSharpProject";

        public CSharpProjectTemplate(IOutputTarget outputTarget, CSharpProjectNETModel model)
            : base(TemplateId, outputTarget, model)
        {
        }
    }
}
