using System.Collections.Generic;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.ValueObjects.Templates.ValueObject;
using Intent.Modules.ValueObjects.Templates.ValueObjectBase;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ValueObjects.Templates
{
    public static class TemplateExtensions
    {
        public static string GetValueObjectName<T>(this IIntentTemplate<T> template) where T : ValueObjectModel
        {
            return template.GetTypeName(ValueObjectTemplate.TemplateId, template.Model);
        }

        public static string GetValueObjectName(this IIntentTemplate template, ValueObjectModel model)
        {
            return template.GetTypeName(ValueObjectTemplate.TemplateId, model);
        }

        public static string GetValueObjectBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(ValueObjectBaseTemplate.TemplateId);
        }

    }
}