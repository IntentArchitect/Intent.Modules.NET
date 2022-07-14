using System.Collections.Generic;
using Intent.Modules.AzureFunctions.FluentValidation.Templates.DTOValidator;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDTOValidatorName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.Api.DTOModel
        {
            return template.GetTypeName(DTOValidatorTemplate.TemplateId, template.Model);
        }

        public static string GetDTOValidatorName(this IntentTemplateBase template, Intent.Modelers.Services.Api.DTOModel model)
        {
            return template.GetTypeName(DTOValidatorTemplate.TemplateId, model);
        }

    }
}