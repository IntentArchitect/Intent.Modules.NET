using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.CollectionExtensions;
using Intent.Modules.Entities.Templates.CollectionWrapper;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Entities.Templates.UpdateHelper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCollectionExtensionsName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CollectionExtensionsTemplate.TemplateId);
        }
        public static string GetCollectionWrapperName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CollectionWrapperTemplate.TemplateId);
        }
        public static string GetDomainEntityName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(DomainEntityTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEntityName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(DomainEntityTemplate.TemplateId, model);
        }

        public static string GetDomainEntityInterfaceName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(DomainEntityInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEntityInterfaceName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(DomainEntityInterfaceTemplate.TemplateId, model);
        }

        public static string GetDomainEntityStateName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(DomainEntityStateTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEntityStateName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(DomainEntityStateTemplate.TemplateId, model);
        }

        public static string GetDomainEnumName<T>(this IntentTemplateBase<T> template) where T : Intent.Modules.Common.Types.Api.EnumModel
        {
            return template.GetTypeName(DomainEnumTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEnumName(this IntentTemplateBase template, Intent.Modules.Common.Types.Api.EnumModel model)
        {
            return template.GetTypeName(DomainEnumTemplate.TemplateId, model);
        }

        public static string GetUpdateHelperName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(UpdateHelperTemplate.TemplateId);
        }

    }
}