using System.Collections.Generic;
using Intent.Modules.Blazor.Templates.Templates.Client.ClientImportsRazor;
using Intent.Modules.Blazor.Templates.Templates.Client.Program;
using Intent.Modules.Blazor.Templates.Templates.Client.RoutesRazor;
using Intent.Modules.Blazor.Templates.Templates.Server.AppRazor;
using Intent.Modules.Blazor.Templates.Templates.Server.ServerImportsRazor;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates
{
    public static class TemplateExtensions
    {
        public static string GetClientImportsRazorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ClientImportsRazorTemplate.TemplateId);
        }

        public static string GetProgramTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ProgramTemplate.TemplateId);
        }

        public static string GetRoutesRazorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(RoutesRazorTemplate.TemplateId);
        }

        public static string GetAppRazorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(AppRazorTemplate.TemplateId);
        }

        public static string GetServerImportsRazorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ServerImportsRazorTemplate.TemplateId);
        }

    }
}