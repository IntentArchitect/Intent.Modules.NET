using System.Collections.Generic;
using Intent.Modules.Blazor.Components.MudBlazor.Templates.FluentValidationProviderExtensions;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates
{
    public static class TemplateExtensions
    {
        public static string GetFluentValidationProviderExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(FluentValidationProviderExtensionsTemplate.TemplateId);
        }

    }
}