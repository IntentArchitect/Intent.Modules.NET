using System.Collections.Generic;
using Intent.Modules.Blazor.Wasm.Templates.Templates.Program;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Wasm.Templates
{
    public static class TemplateExtensions
    {
        public static string GetProgramTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ProgramTemplate.TemplateId);
        }

    }
}