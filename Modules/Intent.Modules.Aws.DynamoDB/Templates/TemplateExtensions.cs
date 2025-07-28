using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBDocument;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBDocumentOfTInterface;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBRepository;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBRepositoryBase;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBRepositoryInterface;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBTableInitializer;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBUnitOfWork;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBUnitOfWorkInterface;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBValueObjectDocument;
using Intent.Modules.Aws.DynamoDB.Templates.EnumAsStringConverter;
using Intent.Modules.Aws.DynamoDB.Templates.ReflectionHelper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDynamoDBDocumentName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DynamoDBDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetDynamoDBDocumentName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DynamoDBDocumentTemplate.TemplateId, model);
        }

        public static string GetDynamoDBDocumentOfTInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DynamoDBDocumentOfTInterfaceTemplate.TemplateId);
        }

        public static string GetDynamoDBRepositoryName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DynamoDBRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetDynamoDBRepositoryName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DynamoDBRepositoryTemplate.TemplateId, model);
        }

        public static string GetDynamoDBRepositoryBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(DynamoDBRepositoryBaseTemplate.TemplateId);
        }

        public static string GetDynamoDBRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DynamoDBRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetDynamoDBTableInitializerName(this IIntentTemplate template)
        {
            return template.GetTypeName(DynamoDBTableInitializerTemplate.TemplateId);
        }

        public static string GetDynamoDBUnitOfWorkName(this IIntentTemplate template)
        {
            return template.GetTypeName(DynamoDBUnitOfWorkTemplate.TemplateId);
        }

        public static string GetDynamoDBUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DynamoDBUnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetDynamoDBValueObjectDocumentName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(DynamoDBValueObjectDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetDynamoDBValueObjectDocumentName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(DynamoDBValueObjectDocumentTemplate.TemplateId, model);
        }

        public static string GetEnumAsStringConverterName(this IIntentTemplate template)
        {
            return template.GetTypeName(EnumAsStringConverterTemplate.TemplateId);
        }

        public static string GetReflectionHelperName(this IIntentTemplate template)
        {
            return template.GetTypeName(ReflectionHelperTemplate.TemplateId);
        }

    }
}