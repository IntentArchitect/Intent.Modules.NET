using System.Collections.Generic;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Blazor.Components.Core.Templates.RazorLayout;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.Core.Templates
{
    public static class TemplateExtensions
    {
        public static string GetRazorComponentTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(RazorComponentTemplate.TemplateId, template.Model);
        }

        public static string GetRazorComponentTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(RazorComponentTemplate.TemplateId, model);
        }

        public static string GetRazorLayoutTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutModel
        {
            return template.GetTypeName(RazorLayoutTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutTemplateName(this IIntentTemplate template, LayoutModel model)
        {
            return template.GetTypeName(RazorLayoutTemplate.TemplateId, model);
        }

    }
}