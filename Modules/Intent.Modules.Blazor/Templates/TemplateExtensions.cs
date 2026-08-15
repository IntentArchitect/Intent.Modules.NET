using System.Collections.Generic;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.ClientImportsRazor;
using Intent.Modules.Blazor.Templates.Templates.Client.DependencyInjection;
using Intent.Modules.Blazor.Templates.Templates.Client.Dialog;
using Intent.Modules.Blazor.Templates.Templates.Client.DialogCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.ModelDefinition;
using Intent.Modules.Blazor.Templates.Templates.Client.Page;
using Intent.Modules.Blazor.Templates.Templates.Client.PageCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.Program;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutFooter;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutFooterCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutHeader;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutHeaderCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutProfile;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutSider;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutSiderCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RoutesRazor;
using Intent.Modules.Blazor.Templates.Templates.Common.ThemeService;
using Intent.Modules.Blazor.Templates.Templates.Server.AppRazor;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerComponent;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerComponentCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerDialog;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerDialogCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerPage;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerPageCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Server.ScopedExecutor;
using Intent.Modules.Blazor.Templates.Templates.Server.ScopedExecutorInterface;
using Intent.Modules.Blazor.Templates.Templates.Server.ScopedMediator;
using Intent.Modules.Blazor.Templates.Templates.Server.ScopedMediatorInterface;
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

        public static string GetDialogTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(DialogTemplate.TemplateId, template.Model);
        }

        public static string GetDialogTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(DialogTemplate.TemplateId, model);
        }

        public static string GetPageTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(PageTemplate.TemplateId, template.Model);
        }

        public static string GetPageTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(PageTemplate.TemplateId, model);
        }

        public static string GetDependencyInjectionName(this IIntentTemplate template)
        {
            return template.GetTypeName(DependencyInjectionTemplate.TemplateId);
        }

        public static string GetDialogCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(DialogCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetDialogCodeBehindTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(DialogCodeBehindTemplate.TemplateId, model);
        }

        public static string GetModelDefinitionTemplateName<T>(this IIntentTemplate<T> template) where T : ModelDefinitionModel
        {
            return template.GetTypeName(ModelDefinitionTemplate.TemplateId, template.Model);
        }

        public static string GetModelDefinitionTemplateName(this IIntentTemplate template, ModelDefinitionModel model)
        {
            return template.GetTypeName(ModelDefinitionTemplate.TemplateId, model);
        }

        public static string GetPageCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(PageCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetPageCodeBehindTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(PageCodeBehindTemplate.TemplateId, model);
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

        public static string GetRazorLayoutFooterTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutFooterModel
        {
            return template.GetTypeName(RazorLayoutFooterTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutFooterTemplateName(this IIntentTemplate template, LayoutFooterModel model)
        {
            return template.GetTypeName(RazorLayoutFooterTemplate.TemplateId, model);
        }

        public static string GetRazorLayoutFooterCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutFooterModel
        {
            return template.GetTypeName(RazorLayoutFooterCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutFooterCodeBehindTemplateName(this IIntentTemplate template, LayoutFooterModel model)
        {
            return template.GetTypeName(RazorLayoutFooterCodeBehindTemplate.TemplateId, model);
        }

        public static string GetRazorLayoutHeaderTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutHeaderModel
        {
            return template.GetTypeName(RazorLayoutHeaderTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutHeaderTemplateName(this IIntentTemplate template, LayoutHeaderModel model)
        {
            return template.GetTypeName(RazorLayoutHeaderTemplate.TemplateId, model);
        }

        public static string GetRazorLayoutProfileTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutProfileMenuModel
        {
            return template.GetTypeName(RazorLayoutProfileTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutProfileTemplateName(this IIntentTemplate template, LayoutProfileMenuModel model)
        {
            return template.GetTypeName(RazorLayoutProfileTemplate.TemplateId, model);
        }

        public static string GetRazorLayoutHeaderCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutHeaderModel
        {
            return template.GetTypeName(RazorLayoutHeaderCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutHeaderCodeBehindTemplateName(this IIntentTemplate template, LayoutHeaderModel model)
        {
            return template.GetTypeName(RazorLayoutHeaderCodeBehindTemplate.TemplateId, model);
        }

        public static string GetRazorLayoutSiderTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutSiderModel
        {
            return template.GetTypeName(RazorLayoutSiderTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutSiderTemplateName(this IIntentTemplate template, LayoutSiderModel model)
        {
            return template.GetTypeName(RazorLayoutSiderTemplate.TemplateId, model);
        }

        public static string GetRazorLayoutSiderCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : LayoutSiderModel
        {
            return template.GetTypeName(RazorLayoutSiderCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetRazorLayoutSiderCodeBehindTemplateName(this IIntentTemplate template, LayoutSiderModel model)
        {
            return template.GetTypeName(RazorLayoutSiderCodeBehindTemplate.TemplateId, model);
        }

        public static string GetScopedExecutorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ScopedExecutorTemplate.TemplateId);
        }

        public static string GetScopedExecutorInterfaceTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ScopedExecutorInterfaceTemplate.TemplateId);
        }

        public static string GetScopedMediatorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ScopedMediatorTemplate.TemplateId);
        }

        public static string GetScopedMediatorInterfaceTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ScopedMediatorInterfaceTemplate.TemplateId);
        }

        public static string GetThemeServiceTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ThemeServiceTemplate.TemplateId);
        }

        public static string GetRazorServerComponentCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(RazorServerComponentCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetRazorServerComponentCodeBehindTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(RazorServerComponentCodeBehindTemplate.TemplateId, model);
        }

        public static string GetRazorServerDialogCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(RazorServerDialogCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetRazorServerDialogCodeBehindTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(RazorServerDialogCodeBehindTemplate.TemplateId, model);
        }

        public static string GetRazorServerPageCodeBehindTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(RazorServerPageCodeBehindTemplate.TemplateId, template.Model);
        }

        public static string GetRazorServerPageCodeBehindTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(RazorServerPageCodeBehindTemplate.TemplateId, model);
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

        public static string GetRazorServerComponentTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(RazorServerComponentTemplate.TemplateId, template.Model);
        }

        public static string GetRazorServerComponentTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(RazorServerComponentTemplate.TemplateId, model);
        }

        public static string GetRazorServerDialogTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(RazorServerDialogTemplate.TemplateId, template.Model);
        }

        public static string GetRazorServerDialogTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(RazorServerDialogTemplate.TemplateId, model);
        }

        public static string GetRazorServerPageTemplateName<T>(this IIntentTemplate<T> template) where T : ComponentModel
        {
            return template.GetTypeName(RazorServerPageTemplate.TemplateId, template.Model);
        }

        public static string GetRazorServerPageTemplateName(this IIntentTemplate template, ComponentModel model)
        {
            return template.GetTypeName(RazorServerPageTemplate.TemplateId, model);
        }

        public static string GetServerImportsRazorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ServerImportsRazorTemplate.TemplateId);
        }

    }
}