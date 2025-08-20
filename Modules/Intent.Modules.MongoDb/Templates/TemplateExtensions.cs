using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Templates.MongoDbDocument;
using Intent.Modules.MongoDb.Templates.MongoDbDocumentInterface;
using Intent.Modules.MongoDb.Templates.MongoDbDocumentOfTInterface;
using Intent.Modules.MongoDb.Templates.MongoDbMultiTenantConnectionFactory;
using Intent.Modules.MongoDb.Templates.MongoDbPagedList;
using Intent.Modules.MongoDb.Templates.MongoDbRepository;
using Intent.Modules.MongoDb.Templates.MongoDbRepositoryBase;
using Intent.Modules.MongoDb.Templates.MongoDbRepositoryInterface;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWork;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWorkInterface;
using Intent.Modules.MongoDb.Templates.MongoDbValueObjectDocument;
using Intent.Modules.MongoDb.Templates.MongoDbValueObjectDocumentInterface;
using Intent.Modules.MongoDb.Templates.ReflectionHelper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMongoDbDocumentName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(MongoDbDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetMongoDbDocumentName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(MongoDbDocumentTemplate.TemplateId, model);
        }

        public static string GetMongoDbDocumentInterfaceName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(MongoDbDocumentInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetMongoDbDocumentInterfaceName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(MongoDbDocumentInterfaceTemplate.TemplateId, model);
        }

        public static string GetMongoDbDocumentOfTInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbDocumentOfTInterfaceTemplate.TemplateId);
        }

        public static string GetMongoDbMultiTenantConnectionFactoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbMultiTenantConnectionFactoryTemplate.TemplateId);
        }

        public static string GetMongoDbPagedListName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbPagedListTemplate.TemplateId);
        }

        public static string GetMongoDbRepositoryName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(MongoDbRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetMongoDbRepositoryName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(MongoDbRepositoryTemplate.TemplateId, model);
        }

        public static string GetMongoDbRepositoryBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbRepositoryBaseTemplate.TemplateId);
        }

        public static string GetMongoDbRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetMongoDbUnitOfWorkName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbUnitOfWorkTemplate.TemplateId);
        }

        public static string GetMongoDbUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbUnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetMongoDbValueObjectDocumentName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(MongoDbValueObjectDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetMongoDbValueObjectDocumentName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(MongoDbValueObjectDocumentTemplate.TemplateId, model);
        }

        public static string GetMongoDbValueObjectDocumentInterfaceName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(MongoDbValueObjectDocumentInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetMongoDbValueObjectDocumentInterfaceName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(MongoDbValueObjectDocumentInterfaceTemplate.TemplateId, model);
        }

        public static string GetReflectionHelperName(this IIntentTemplate template)
        {
            return template.GetTypeName(ReflectionHelperTemplate.TemplateId);
        }

    }
}