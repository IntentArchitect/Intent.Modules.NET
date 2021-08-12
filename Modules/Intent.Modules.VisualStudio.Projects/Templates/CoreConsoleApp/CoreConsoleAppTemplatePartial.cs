using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreConsoleApp
{
    partial class CoreConsoleAppTemplate : VisualStudioProjectTemplateBase
    {
        public const string TemplateId = "Intent.VisualStudio.Projects.CoreConsoleApp";

        public CoreConsoleAppTemplate(IOutputTarget outputTarget, ConsoleAppNETCoreModel model)
            : base(TemplateId, outputTarget, model)
        {
        }
    }
}