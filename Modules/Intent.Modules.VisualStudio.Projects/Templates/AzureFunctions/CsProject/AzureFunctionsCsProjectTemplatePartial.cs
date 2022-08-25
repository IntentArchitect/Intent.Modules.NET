using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.AzureFunctions.CsProject
{
    public partial class AzureFunctionsCsProjectTemplate : VisualStudioProjectTemplateBase<AzureFunctionsProjectModel>
    {
        public const string TemplateId = "Intent.VisualStudio.Projects.AzureFunctions.CsProject";

        public AzureFunctionsCsProjectTemplate(IOutputTarget outputTarget, AzureFunctionsProjectModel model)
            : base(TemplateId, outputTarget, model)
        {
        }
    }
}