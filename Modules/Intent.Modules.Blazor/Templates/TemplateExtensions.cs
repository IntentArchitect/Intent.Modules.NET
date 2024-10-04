using System.Collections.Generic;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.ClientImportsRazor;
using Intent.Modules.Blazor.Templates.Templates.Client.DependencyInjection;
using Intent.Modules.Blazor.Templates.Templates.Client.ModelDefinition;
using Intent.Modules.Blazor.Templates.Templates.Client.Program;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutCodeBehind;
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

        public static string GetDependencyInjectionName(this IIntentTemplate template)
        {
            return template.GetTypeName(DependencyInjectionTemplate.TemplateId);
        }

        public static string GetModelDefinitionTemplateName<T>(this IIntentTemplate<T> template) where T : ModelDefinitionModel
        {
            return template.GetTypeName(ModelDefinitionTemplate.TemplateId, template.Model);
        }

        public static string GetModelDefinitionTemplateName(this IIntentTemplate template, ModelDefinitionModel model)
        {
            return template.GetTypeName(ModelDefinitionTemplate.TemplateId, model);
        }

        public static string GetProgramTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ProgramTemplate.TemplateId);
        }

        public static string GetRazorComponentCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(RazorComponentCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetRazorComponentCodeBehindTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(RazorComponentCodeBehindTemplate.TemplateId, model);
        }

        public static string GetRazorLayoutCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutModel
        {
            return template.GetTypeName(RazorLayoutCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutCodeBehindTemplateName(this IIntentTemplate template, LayoutModel model)
        {
            return template.GetTypeName(RazorLayoutCodeBehindTemplate.TemplateId, model);
        }

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