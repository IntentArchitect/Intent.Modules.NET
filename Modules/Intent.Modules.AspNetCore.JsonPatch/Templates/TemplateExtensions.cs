using System.Collections.Generic;
using Intent.Modules.AspNetCore.JsonPatch.Templates.Templates.JsonMergePatchExecutor;
using Intent.Modules.AspNetCore.JsonPatch.Templates.Templates.PatchExecutorInterface;
using Intent.Modules.AspNetCore.JsonPatch.Templates.Templates.PatchIgnoreAttribute;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.JsonPatch.Templates
{
    public static class TemplateExtensions
    {
        public static string GetJsonMergePatchExecutorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JsonMergePatchExecutorTemplate.TemplateId);
        }

        public static string GetPatchExecutorInterfaceTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(PatchExecutorInterfaceTemplate.TemplateId);
        }

        public static string GetPatchIgnoreAttributeTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(PatchIgnoreAttributeTemplate.TemplateId);
        }

    }
}