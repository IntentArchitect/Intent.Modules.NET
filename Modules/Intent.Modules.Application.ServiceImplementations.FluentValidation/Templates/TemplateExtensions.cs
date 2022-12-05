using System.Collections.Generic;
using Intent.Modules.Application.ServiceImplementations.FluentValidation.Templates.ValidationProvider;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.FluentValidation.Templates
{
    public static class TemplateExtensions
    {
        public static string GetValidationProviderName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ValidationProviderTemplate.TemplateId);
        }

    }
}