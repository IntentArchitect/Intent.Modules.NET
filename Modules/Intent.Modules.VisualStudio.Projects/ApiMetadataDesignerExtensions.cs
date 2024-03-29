using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public static IDesigner VisualStudio(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.VisualStudio(application.Id);
        }

        public static IDesigner VisualStudio(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, "Visual Studio");
        }

    }
}