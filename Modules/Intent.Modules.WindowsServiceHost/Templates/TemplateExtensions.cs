using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.WindowsServiceHost.Templates.Program;
using Intent.Modules.WindowsServiceHost.Templates.WindowsBackgroundService;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.WindowsServiceHost.Templates
{
    public static class TemplateExtensions
    {
        public static string GetProgramName(this IIntentTemplate template)
        {
            return template.GetTypeName(ProgramTemplate.TemplateId);
        }

        public static string GetWindowsBackgroundServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(WindowsBackgroundServiceTemplate.TemplateId);
        }

    }
}