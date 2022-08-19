using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.ValueObjects.Templates.ValueObject;
using Intent.Modules.Entities.ValueObjects.Templates.ValueObjectBase;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.ValueObjects.Templates
{
    public static class TemplateExtensions
    {
        public static string GetValueObjectName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.ValueObjects.Api.ValueObjectModel
        {
            return template.GetTypeName(ValueObjectTemplate.TemplateId, template.Model);
        }

        public static string GetValueObjectName(this IntentTemplateBase template, Intent.Modelers.Domain.ValueObjects.Api.ValueObjectModel model)
        {
            return template.GetTypeName(ValueObjectTemplate.TemplateId, model);
        }

        public static string GetValueObjectBaseName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ValueObjectBaseTemplate.TemplateId);
        }

    }
}