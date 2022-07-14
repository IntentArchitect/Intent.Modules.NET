using System.Collections.Generic;
using Intent.Modules.Application.FluentValidation.Templates.ValidationBehaviour;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Templates
{
    public static class TemplateExtensions
    {
        public static string GetValidationBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ValidationBehaviourTemplate.TemplateId);
        }

    }
}