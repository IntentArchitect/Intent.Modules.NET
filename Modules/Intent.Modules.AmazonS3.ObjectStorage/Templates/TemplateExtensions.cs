using System.Collections.Generic;
using Intent.Modules.AmazonS3.ObjectStorage.Templates.AmazonS3ObjectStorageConfiguration;
using Intent.Modules.AmazonS3.ObjectStorage.Templates.AmazonS3ObjectStorageImplementation;
using Intent.Modules.AmazonS3.ObjectStorage.Templates.ObjectStorageExtensions;
using Intent.Modules.AmazonS3.ObjectStorage.Templates.ObjectStorageInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AmazonS3.ObjectStorage.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAmazonS3ObjectStorageConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AmazonS3ObjectStorageConfigurationTemplate.TemplateId);
        }
        public static string GetAmazonS3ObjectStorageImplementationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AmazonS3ObjectStorageImplementationTemplate.TemplateId);
        }

        public static string GetObjectStorageExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(ObjectStorageExtensionsTemplate.TemplateId);
        }

        public static string GetObjectStorageInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(ObjectStorageInterfaceTemplate.TemplateId);
        }

    }
}