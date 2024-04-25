using System.Collections.Generic;
using Intent.Modules.Azure.BlobStorage.Templates.AzureBlobStorageImplementation;
using Intent.Modules.Azure.BlobStorage.Templates.BlobStorageExtensions;
using Intent.Modules.Azure.BlobStorage.Templates.BlobStorageInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Azure.BlobStorage.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureBlobStorageImplementationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureBlobStorageImplementationTemplate.TemplateId);
        }

        public static string GetBlobStorageExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(BlobStorageExtensionsTemplate.TemplateId);
        }

        public static string GetBlobStorageInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(BlobStorageInterfaceTemplate.TemplateId);
        }

    }
}