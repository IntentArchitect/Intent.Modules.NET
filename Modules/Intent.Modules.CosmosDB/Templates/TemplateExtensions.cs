using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocument;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocumentInterface;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocumentOfTInterface;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocumentTypeExtensionMethods;
using Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenancyConfiguration;
using Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenantClientProvider;
using Intent.Modules.CosmosDB.Templates.CosmosDBMultitenantContainerProvider;
using Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenantMiddleware;
using Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenantOptionsMonitor;
using Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenantRepositoryOptions;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepository;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryInterface;
using Intent.Modules.CosmosDB.Templates.CosmosDBUnitOfWork;
using Intent.Modules.CosmosDB.Templates.CosmosDBUnitOfWorkInterface;
using Intent.Modules.CosmosDB.Templates.CosmosDBValueObjectDocument;
using Intent.Modules.CosmosDB.Templates.CosmosDBValueObjectDocumentInterface;
using Intent.Modules.CosmosDB.Templates.CosmosPagedList;
using Intent.Modules.CosmosDB.Templates.EnumJsonConverter;
using Intent.Modules.CosmosDB.Templates.ReflectionHelper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCosmosDBDocumentName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(CosmosDBDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetCosmosDBDocumentName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(CosmosDBDocumentTemplate.TemplateId, model);
        }

        public static string GetCosmosDBDocumentInterfaceName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(CosmosDBDocumentInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetCosmosDBDocumentInterfaceName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(CosmosDBDocumentInterfaceTemplate.TemplateId, model);
        }

        public static string GetCosmosDBDocumentOfTInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBDocumentOfTInterfaceTemplate.TemplateId);
        }

        public static string GetCosmosDBDocumentTypeExtensionMethodsName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBDocumentTypeExtensionMethodsTemplate.TemplateId);
        }

        public static string GetCosmosDBMultiTenancyConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBMultiTenancyConfigurationTemplate.TemplateId);
        }

        public static string GetCosmosDBMultiTenantClientProviderName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBMultiTenantClientProviderTemplate.TemplateId);
        }

        public static string GetCosmosDBMultitenantContainerProviderName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBMultitenantContainerProviderTemplate.TemplateId);
        }

        public static string GetCosmosDBMultiTenantMiddlewareName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBMultiTenantMiddlewareTemplate.TemplateId);
        }

        public static string GetCosmosDBMultiTenantOptionsMonitorName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBMultiTenantOptionsMonitorTemplate.TemplateId);
        }

        public static string GetCosmosDBMultiTenantRepositoryOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBMultiTenantRepositoryOptionsTemplate.TemplateId);
        }

        public static string GetCosmosDBRepositoryName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(CosmosDBRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetCosmosDBRepositoryName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(CosmosDBRepositoryTemplate.TemplateId, model);
        }

        public static string GetCosmosDBRepositoryBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBRepositoryBaseTemplate.TemplateId);
        }

        public static string GetCosmosDBRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetCosmosDBUnitOfWorkName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBUnitOfWorkTemplate.TemplateId);
        }

        public static string GetCosmosDBUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBUnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetCosmosDBValueObjectDocumentName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(CosmosDBValueObjectDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetCosmosDBValueObjectDocumentName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(CosmosDBValueObjectDocumentTemplate.TemplateId, model);
        }

        public static string GetCosmosDBValueObjectDocumentInterfaceName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(CosmosDBValueObjectDocumentInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetCosmosDBValueObjectDocumentInterfaceName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(CosmosDBValueObjectDocumentInterfaceTemplate.TemplateId, model);
        }

        public static string GetCosmosPagedListName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosPagedListTemplate.TemplateId);
        }

        public static string GetEnumJsonConverterName(this IIntentTemplate template)
        {
            return template.GetTypeName(EnumJsonConverterTemplate.TemplateId);
        }

        public static string GetReflectionHelperName(this IIntentTemplate template)
        {
            return template.GetTypeName(ReflectionHelperTemplate.TemplateId);
        }

    }
}