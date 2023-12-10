using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Entities.Templates.CollectionExtensions;
using Intent.Modules.Entities.Templates.CollectionWrapper;
using Intent.Modules.Entities.Templates.DataContract;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Entities.Templates.NotFoundException;
using Intent.Modules.Entities.Templates.UpdateHelper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCollectionExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(CollectionExtensionsTemplate.TemplateId);
        }
        public static string GetCollectionWrapperName(this IIntentTemplate template)
        {
            return template.GetTypeName(CollectionWrapperTemplate.TemplateId);
        }

        public static string GetDataContractName<T>(this IIntentTemplate<T> template) where T : DataContractModel
        {
            return template.GetTypeName(DataContractTemplate.TemplateId, template.Model);
        }

        public static string GetDataContractName(this IIntentTemplate template, DataContractModel model)
        {
            return template.GetTypeName(DataContractTemplate.TemplateId, model);
        }

        public static string GetDomainEntityName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DomainEntityTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEntityName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DomainEntityTemplate.TemplateId, model);
        }

        public static string GetDomainEntityInterfaceName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DomainEntityInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEntityInterfaceName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DomainEntityInterfaceTemplate.TemplateId, model);
        }

        public static string GetDomainEntityStateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DomainEntityStateTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEntityStateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DomainEntityStateTemplate.TemplateId, model);
        }

        public static string GetDomainEnumName<T>(this IIntentTemplate<T> template) where T : EnumModel
        {
            return template.GetTypeName(DomainEnumTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEnumName(this IIntentTemplate template, EnumModel model)
        {
            return template.GetTypeName(DomainEnumTemplate.TemplateId, model);
        }

        public static string GetNotFoundExceptionName(this IIntentTemplate template)
        {
            return template.GetTypeName(NotFoundExceptionTemplate.TemplateId);
        }

        public static string GetUpdateHelperName(this IIntentTemplate template)
        {
            return template.GetTypeName(UpdateHelperTemplate.TemplateId);
        }

    }
}