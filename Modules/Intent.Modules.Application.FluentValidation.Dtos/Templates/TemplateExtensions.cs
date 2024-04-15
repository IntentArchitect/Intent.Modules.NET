using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.FluentValidation.Dtos.Templates.DTOValidator;
using Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidationService;
using Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidationServiceInterface;
using Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidatorProvider;
using Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidatorProviderInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDTOValidatorName<T>(this IIntentTemplate<T> template) where T : DTOModel
        {
            return template.GetTypeName(DTOValidatorTemplate.TemplateId, template.Model);
        }

        public static string GetDTOValidatorName(this IIntentTemplate template, DTOModel model)
        {
            return template.GetTypeName(DTOValidatorTemplate.TemplateId, model);
        }

        public static string GetValidationServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(ValidationServiceTemplate.TemplateId);
        }

        public static string GetValidationServiceInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(ValidationServiceInterfaceTemplate.TemplateId);
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