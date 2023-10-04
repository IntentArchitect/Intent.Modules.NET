using System.Collections.Generic;
using Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates.DtoValidator;
using Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates.ValidatorProvider;
using Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates.ValidatorProviderInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDtoValidatorName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Services.Api.DTOModel
        {
            return template.GetTypeName(DtoValidatorTemplate.TemplateId, template.Model);
        }

        public static string GetDtoValidatorName(this IIntentTemplate template, Intent.Modelers.Services.Api.DTOModel model)
        {
            return template.GetTypeName(DtoValidatorTemplate.TemplateId, model);
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