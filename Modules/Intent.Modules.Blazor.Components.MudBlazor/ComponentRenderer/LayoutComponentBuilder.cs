using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client;
using Intent.Modules.Blazor.Templates.Templates.Common.ThemeService;
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
        var template = parentNode.File.Template;

        // MudThemeProvider is always rendered so Mud components stay correctly themed even when the
        // toggle is disabled. When the toggle is enabled, IsDarkMode is bound to the IsDarkMode property
        // below (backed by ThemeService); when disabled, it's fixed (no ThemeService involved).
        var themeProvider = new HtmlElement("MudThemeProvider", _componentTemplate.RazorFile);
        themeProvider.AddAttribute("IsDarkMode", enableThemeToggle ? "@IsDarkMode" : "true");
        parentNode.AddChildNode(themeProvider);

        var popoverProvider = new HtmlElement("MudPopoverProvider", _componentTemplate.RazorFile);
        //popoverProvider.AddAttribute("@rendermode", "InteractiveServer"); // throws exception. Check with Dom
        parentNode.AddChildNode(popoverProvider);
        parentNode.AddChildNode(new HtmlElement("MudDialogProvider", _componentTemplate.RazorFile));
        parentNode.AddChildNode(new HtmlElement("MudSnackbarProvider", _componentTemplate.RazorFile));
        parentNode.AddChildNode(new EmptyLine(_componentTemplate.RazorFile));

        var layoutHtml = new HtmlElement("MudLayout", _componentTemplate.RazorFile);
        parentNode.AddChildNode(layoutHtml);
        var code = _componentTemplate.GetCodeBehind();

        if (enableThemeToggle)
        {
            // Only wire up ThemeService when the toggle is enabled: ThemeServiceTemplate.CanRunTemplate()
            // returns false when disabled, so GetTypeName(ThemeServiceTemplate.TemplateId) would fail to
            // resolve a template instance that was never registered.
            code.AddProperty(code.File.Template.GetTypeName(ThemeServiceTemplate.TemplateId), "_themeService", prop =>
            {
                prop.WithInitialValue("default!");
                prop.AddAttribute(code.File.Template.UseType("Microsoft.AspNetCore.Components.Inject"));
                prop.Public();
            });

            code.AddProperty(code.File.Template.UseType("Microsoft.JSInterop.IJSRuntime"), "JS", prop =>
            {
                prop.WithInitialValue("default!");
                prop.AddAttribute(code.File.Template.UseType("Microsoft.AspNetCore.Components.Inject"));
                prop.Public();
            });

            code.AddProperty(code.File.Template.UseType("Microsoft.AspNetCore.Http.HttpContext?"), "HttpContext", prop =>
            {
                prop.AddAttribute(code.File.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameter"));
                prop.Public();
            });

            code.AddProperty("bool", "IsDarkMode", prop =>
            {
                prop.WithoutSetter();
                prop.Getter.WithExpressionImplementation("HttpContext is not null ? !(HttpContext.Request.Cookies.TryGetValue(\"theme\", out var theme) && theme == \"light\") : _themeService.IsDark");
                prop.Private().ReadOnly();
            });

            code.AddMethod("Task", "OnAfterRenderAsync", method =>
            {
                method.Override().Async().Protected();
                method.AddParameter("bool", "firstRender");

                method.AddIfStatement("firstRender && RendererInfo.IsInteractive", @if =>
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

            code.AddMethod("void", "Dispose", method =>
            {
                method.AddStatement("_themeService.OnChange -= StateHasChanged;");
            });
        }

        if (layoutModel.Header != null)
        {
            layoutHtml.AddHtmlElement($"{layoutModel.Name}Header", header =>
            {
            });
        }
        if (layoutModel.Sider != null)
        {
            layoutHtml.AddHtmlElement($"{layoutModel.Name}Sider", sider =>
            {
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
}
