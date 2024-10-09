using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Google.CloudStorage.Templates.CloudStorageInterface;
using Intent.Modules.Google.CloudStorage.Templates.GoogleCloudStorageConfiguration;
using Intent.Modules.Google.CloudStorage.Templates.GoogleCloudStorageImplementation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Google.CloudStorage.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCloudStorageInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CloudStorageInterfaceTemplate.TemplateId);
        }

        public static string GetGoogleCloudStorageConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(GoogleCloudStorageConfigurationTemplate.TemplateId);
        }

        public static string GetGoogleCloudStorageImplementationName(this IIntentTemplate template)
        {
            return template.GetTypeName(GoogleCloudStorageImplementationTemplate.TemplateId);
        }

    }
}