using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocument;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocumentInterface;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocumentTypeExtensionMethods;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepository;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryInterface;
using Intent.Modules.CosmosDB.Templates.CosmosDBUnitOfWork;
using Intent.Modules.CosmosDB.Templates.CosmosDBUnitOfWorkBehaviour;
using Intent.Modules.CosmosDB.Templates.CosmosDBUnitOfWorkInterface;
using Intent.Modules.CosmosDB.Templates.CosmosPagedList;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCosmosDBDocumentName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(CosmosDBDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetCosmosDBDocumentName(this IIntentTemplate template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(CosmosDBDocumentTemplate.TemplateId, model);
        }

        public static string GetCosmosDBDocumentInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBDocumentInterfaceTemplate.TemplateId);
        }

        public static string GetCosmosDBDocumentTypeExtensionMethodsName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBDocumentTypeExtensionMethodsTemplate.TemplateId);
        }
        public static string GetCosmosDBRepositoryName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(CosmosDBRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetCosmosDBRepositoryName(this IIntentTemplate template, Intent.Modelers.Domain.Api.ClassModel model)
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

        public static string GetCosmosDBUnitOfWorkBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBUnitOfWorkBehaviourTemplate.TemplateId);
        }

        public static string GetCosmosDBUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosDBUnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetCosmosPagedListName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosPagedListTemplate.TemplateId);
        }

    }
}