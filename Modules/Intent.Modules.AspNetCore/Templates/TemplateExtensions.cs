using System.Collections.Generic;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Templates
{
    public static class TemplateExtensions
    {
        public static string GetProgramName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ProgramTemplate.TemplateId);
        }

        public static string GetStartupName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(StartupTemplate.TemplateId);
        }

    }
}