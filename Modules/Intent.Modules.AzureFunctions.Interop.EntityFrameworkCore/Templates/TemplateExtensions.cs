using System.Collections.Generic;
using Intent.Modules.AzureFunctions.Interop.EntityFrameworkCore.Templates.DbInitializationService;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.EntityFrameworkCore.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDbInitializationServiceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DbInitializationServiceTemplate.TemplateId);
        }

    }
}