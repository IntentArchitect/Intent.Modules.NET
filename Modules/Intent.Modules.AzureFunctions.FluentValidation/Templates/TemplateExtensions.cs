using System.Collections.Generic;
using Intent.Modules.AzureFunctions.FluentValidation.Templates.ValidationService;
using Intent.Modules.AzureFunctions.FluentValidation.Templates.ValidationServiceInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.Templates
{
    public static class TemplateExtensions
    {
        public static string GetValidationServiceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ValidationServiceTemplate.TemplateId);
        }

        public static string GetValidationServiceInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ValidationServiceInterfaceTemplate.TemplateId);
        }

    }
}