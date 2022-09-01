using System.Collections.Generic;
using Intent.Modules.CloudStorageClient.Templates.AzureBlobStorageImplementation;
using Intent.Modules.CloudStorageClient.Templates.CloudStorageInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.CloudStorageClient.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureBlobStorageImplementationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AzureBlobStorageImplementationTemplate.TemplateId);
        }

        public static string GetCloudStorageInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CloudStorageInterfaceTemplate.TemplateId);
        }

    }
}