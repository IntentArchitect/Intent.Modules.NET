using System.Collections.Generic;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.FluentValidation.Templates.ModelDefinitionValidator;
using Intent.Modules.Blazor.FluentValidation.Templates.ValidatorProvider;
using Intent.Modules.Blazor.FluentValidation.Templates.ValidatorProviderInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.FluentValidation.Templates
{
    public static class TemplateExtensions
    {
        public static string GetModelDefinitionValidatorName<T>(this IIntentTemplate<T> template) where T : ModelDefinitionModel
        {
            return template.GetTypeName(ModelDefinitionValidatorTemplate.TemplateId, template.Model);
        }

        public static string GetModelDefinitionValidatorName(this IIntentTemplate template, ModelDefinitionModel model)
        {
            return template.GetTypeName(ModelDefinitionValidatorTemplate.TemplateId, model);
        }
        public static string GetValidatorProviderName(this IIntentTemplate template)
        {
            return template.GetTypeName(ValidatorProviderTemplate.TemplateId);
        }

        public static string GetValidatorProviderInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(ValidatorProviderInterfaceTemplate.TemplateId);
        }

    }
}