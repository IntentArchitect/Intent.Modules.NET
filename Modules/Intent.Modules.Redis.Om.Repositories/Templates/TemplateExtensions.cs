using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.IndexCreationService;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmConfiguration;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocument;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocumentInterface;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocumentOfTInterface;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocumentTypeExtensionMethods;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmPagedList;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmRepository;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmRepositoryBase;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmRepositoryInterface;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmUnitOfWork;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmUnitOfWorkInterface;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmValueObjectDocument;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmValueObjectDocumentInterface;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.ReflectionHelper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates
{
    public static class TemplateExtensions
    {
        public static string GetIndexCreationServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(IndexCreationServiceTemplate.TemplateId);
        }
        public static string GetRedisOmConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedisOmConfigurationTemplate.TemplateId);
        }
        public static string GetRedisOmDocumentName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(RedisOmDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetRedisOmDocumentName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(RedisOmDocumentTemplate.TemplateId, model);
        }

        public static string GetRedisOmDocumentInterfaceName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(RedisOmDocumentInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetRedisOmDocumentInterfaceName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(RedisOmDocumentInterfaceTemplate.TemplateId, model);
        }

        public static string GetRedisOmDocumentOfTInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedisOmDocumentOfTInterfaceTemplate.TemplateId);
        }

        public static string GetRedisOmDocumentTypeExtensionMethodsName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedisOmDocumentTypeExtensionMethodsTemplate.TemplateId);
        }

        public static string GetRedisOmPagedListName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedisOmPagedListTemplate.TemplateId);
        }

        public static string GetRedisOmRepositoryName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(RedisOmRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetRedisOmRepositoryName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(RedisOmRepositoryTemplate.TemplateId, model);
        }

        public static string GetRedisOmRepositoryBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedisOmRepositoryBaseTemplate.TemplateId);
        }

        public static string GetRedisOmRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedisOmRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetRedisOmUnitOfWorkName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedisOmUnitOfWorkTemplate.TemplateId);
        }

        public static string GetRedisOmUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedisOmUnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetRedisOmValueObjectDocumentName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(RedisOmValueObjectDocumentTemplate.TemplateId, template.Model);
        }

        public static string GetRedisOmValueObjectDocumentName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(RedisOmValueObjectDocumentTemplate.TemplateId, model);
        }

        public static string GetRedisOmValueObjectDocumentInterfaceName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(RedisOmValueObjectDocumentInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetRedisOmValueObjectDocumentInterfaceName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(RedisOmValueObjectDocumentInterfaceTemplate.TemplateId, model);
        }

        public static string GetReflectionHelperName(this IIntentTemplate template)
        {
            return template.GetTypeName(ReflectionHelperTemplate.TemplateId);
        }

    }
}