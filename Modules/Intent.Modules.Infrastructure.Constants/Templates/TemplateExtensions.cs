using Intent.Modules.Common.Templates;
using Intent.Modules.Infrastructure.Constants.Templates.Constants;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Infrastructure.Constants.Templates
{
    public static class TemplateExtensions
    {
        public static string GetConstantsTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ConstantsTemplate.TemplateId);
        }
    }
}
