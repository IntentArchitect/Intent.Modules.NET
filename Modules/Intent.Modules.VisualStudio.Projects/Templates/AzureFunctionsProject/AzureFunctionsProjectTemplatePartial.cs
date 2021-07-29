using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.AzureFunctionsProject
{
    partial class AzureFunctionsProjectTemplate : VisualStudioProjectTemplateBase
    {
        private readonly AzureFunctionsProjectModel _model;

        public const string TemplateId = "Intent.VisualStudio.Projects.AzureFunctionsProject";

        public AzureFunctionsProjectTemplate(IOutputTarget outputTarget, AzureFunctionsProjectModel model)
            : base(TemplateId, outputTarget, model)
        {
            _model = model;
        }

        private string GetAzureFunctionsVersion()
        {
            return _model.GetAzureFunctionsProjectSettings().AzureFunctionsVersion().Value;
        }
    }
}