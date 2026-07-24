using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class LayoutComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public LayoutComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var layoutModel = new LayoutModel(component);
        var enableThemeToggle = _componentTemplate.ExecutionContext.Settings.GetBlazor().EnableThemeToggle();

        if (enableThemeToggle)
        {
            var themeProvider = new HtmlElement("MudThemeProvider", _componentTemplate.RazorFile);
            themeProvider.AddAttribute("IsDarkMode", "@_themeService.IsDark");
            parentNode.AddChildNode(themeProvider);
        }

        var popoverProvider = new HtmlElement("MudPopoverProvider", _componentTemplate.RazorFile);
        //popoverProvider.AddAttribute("@rendermode", "InteractiveServer"); // throws exception. Check with Dom
        parentNode.AddChildNode(popoverProvider);
        parentNode.AddChildNode(new HtmlElement("MudDialogProvider", _componentTemplate.RazorFile));
        parentNode.AddChildNode(new HtmlElement("MudSnackbarProvider", _componentTemplate.RazorFile));
        parentNode.AddChildNode(new EmptyLine(_componentTemplate.RazorFile));

        var layoutHtml = new HtmlElement("MudLayout", _componentTemplate.RazorFile);
        parentNode.AddChildNode(layoutHtml);
        var code = _componentTemplate.GetCodeBehind();

        if (layoutModel.Header != null)
        {
            layoutHtml.AddHtmlElement($"{layoutModel.Name}Header", header =>
            {
                if (layoutModel.Sider != null)
                {
                    header.AddAttribute("OnDrawerToggle", "DrawerToggle");

                    code.AddField("bool", "_drawerOpen", field => field.WithAssignment(new CSharpStatement("true")));
                    code.AddMethod("void", "DrawerToggle", method =>
                    {
                        method.AddStatement("_drawerOpen = !_drawerOpen;");
                    });
                }
                if (enableThemeToggle)
                {
                    header.AddAttribute("OnThemeToggle", "ToggleTheme");
                }

                if (enableThemeToggle)
                {
                    ConfigureThemeSelection(header, code);
                }
            });
        }
        if (layoutModel.Sider != null)
        {
            layoutHtml.AddHtmlElement($"{layoutModel.Name}Sider", sider =>
            {
                if (layoutModel.Header != null)
                {
                    sider.AddAttribute("@bind-DrawerOpen", "_drawerOpen");
                }
            });
        }
        layoutHtml.AddHtmlElement("MudMainContent", layoutContent =>
        {
            layoutContent.AddAttribute("Class", "mt-16 pa-4");
            layoutContent.WithText("@Body");
        });

        if (layoutModel.Footer != null)
        {
            layoutHtml.AddHtmlElement($"{layoutModel.Name}Footer", layoutContent =>
            {
            });
        }

        return [layoutHtml];

    }

    private void AddUserMenuSlot(IHtmlElement appBar)
    {
        // Always render <AppUserMenu/>. The Authentication module ships the real one (Profile / My Account /
        // antiforgery-POST Logout) into Components/Account/Shared — its namespace reaches this layout via the
        // root Components/_Imports.razor (contributed by Auth's ChangeRenderMode factory extension). When Auth
        // isn't installed, the component module ships a no-op scaffold AppUserMenu in its place (see the
        // AppUserMenu static-content registration), so the reference always resolves.
        appBar.AddHtmlElement("AppUserMenu");
    }

    private static void ConfigureThemeSelection(IHtmlElement appBar, IBuildsCSharpMembers code)
    {
        var themeTemplate = code.File.Template.OutputTarget.FindTemplateInstance("Intent.Blazor.Templates.Common.ThemeServiceTemplate");
        // Really should never be null
        if (themeTemplate != null)
        {
            ((IntentTemplateBase)code.Template).AddTemplateDependency(themeTemplate.Id);

            // Ensure the correct namespace is imported for ThemeService.
            var themeServiceType = code.Template.UseType(code.Template.GetTypeName("Intent.Blazor.Templates.Common.ThemeServiceTemplate"));

            // add the services to support the switching
            code.AddProperty(themeServiceType, "_themeService", ts =>
            {
                ts.WithInitialValue("default!");
                ts.AddAttribute(code.File.Template.UseType("Microsoft.AspNetCore.Components.Inject"));
            });
            code.AddProperty(code.Template.UseType("Microsoft.JSInterop.IJSRuntime"), "JS", ts =>
            {
                ts.WithInitialValue("default!");
                ts.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.Inject"));
            });

            code.AddMethod(code.File.Template.UseType("System.Threading.Tasks.Task"), "OnAfterRenderAsync", rm =>
            {
                rm.Protected().Override().Async();
                rm.AddParameter("bool", "firstRender");

                rm.AddIfStatement("firstRender", @if =>
                {
                    @if.AddStatement("_themeService.OnChange += StateHasChanged;");

                    @if.AddAssignmentStatement("var saved",
                        new CSharpStatement(@"await JS.InvokeAsync<string>(""themeStorage.get"");"));

                    @if.AddIfStatement(@"saved == ""dark""", innerIf =>
                    {
                        innerIf.AddInvocationStatement("_themeService.SetDark", inv => inv.AddArgument("true"));
                    });
                    @if.AddElseIfStatement("saved == \"light\"", elseIf =>
                    {
                        elseIf.AddInvocationStatement("_themeService.SetDark", inv => inv.AddArgument("false"));
                    });

                    @if.AddIfStatement("!string.IsNullOrEmpty(saved)", stateIf =>
                    {
                        stateIf.AddInvocationStatement("StateHasChanged");
                    });
                });
            });

            code.AddMethod(code.File.Template.UseType("System.Threading.Tasks.Task"), "ToggleTheme", method =>
            {
                method.Async();
                method.AddInvocationStatement("_themeService.Toggle");
                method.AddInvocationStatement("await JS.InvokeVoidAsync", invoc =>
                {
                    invoc.AddArgument(@"""themeStorage.set""");
                    invoc.AddArgument(@"_themeService.IsDark ? ""dark"" : ""light""");
                });
            });

            code.AddMethod("void", "Dispose", method =>
            {
                method.AddStatement("_themeService.OnChange -= StateHasChanged;");
            });
        }
    }
}
