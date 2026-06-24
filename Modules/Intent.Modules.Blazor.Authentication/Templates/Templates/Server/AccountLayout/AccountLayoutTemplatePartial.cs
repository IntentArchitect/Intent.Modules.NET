using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AccountLayoutCodeBehind;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AccountLayout
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AccountLayoutTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.AccountLayoutTemplate";
        private const string MudBlazorModuleId = "Intent.Blazor.Components.MudBlazor";

        /// <summary>
        /// Creates a new instance of <see cref="AccountLayoutTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccountLayoutTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"AccountLayout")
                .Configure(file =>
                {
                    // AccountLayout is the minimal pre-login shell: no app nav, just the theme toggle.
                    // It deliberately does NOT nest MainLayout (ManageLayout does that for the post-login
                    // Manage pages). Account pages are static SSR, so the toggle is circuit-free and the
                    // Mud theme is derived from the cookie via HttpContext rather than a JS/theme service.
                    var rootNamespace = outputTarget.GetNamespace().Replace("Components.Account.Shared", "");
                    // AppBrand/ThemeToggle live in the layout namespace — the .Client project for the
                    // Wasm/Auto render modes, the server project for InteractiveServer. (The dynamic
                    // lookup below wins when it resolves; this is the correct fallback for Wasm, where
                    // it can't find the client-project layout from the server template's context.)
                    var layoutNamespace = outputTarget.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer()
                        ? $"{rootNamespace}Components.Layout"
                        : $"{rootNamespace}Client.Components.Layout";
                    if (TryGetTemplate<RazorLayoutTemplate>("Intent.Blazor.Templates.Client.RazorLayoutTemplate", out var layoutTemplate))
                    {
                        layoutNamespace = layoutTemplate.Namespace;
                    }

                    var mudBlazorInstalled = outputTarget.ExecutionContext.InstalledModules.Any(module => module.ModuleId == MudBlazorModuleId);

                    file.AddInheritsDirective("LayoutComponentBase");
                    // The ThemeToggle component lives in the layout namespace; import it so <ThemeToggle /> resolves.
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement($"@using {layoutNamespace}"), file));
                    file.AddInjectDirective("Microsoft.AspNetCore.Components.NavigationManager", "NavigationManager");
                    file.AddEmptyLine();

                    if (mudBlazorInstalled)
                    {
                        AddMudBlazorAccountShell(file);
                    }
                    else
                    {
                        AddStandardAccountShell(file);
                    }

                    // Route the @code members into the sibling AccountLayout.razor.cs code-behind (via
                    // GetCodeBehind), so usings are managed there. Falls back to an inline @code block if
                    // the code-behind template isn't present.
                    var code = GetCodeBehind();
                    code.AddProperty($"{code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext")}?", "HttpContext", httpContext =>
                    {
                        httpContext.Private();
                        httpContext.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute"));
                    });

                    if (mudBlazorInstalled)
                    {
                        // Derive the Mud dark/light mode from the theme cookie on the server so the
                        // (circuit-free) static-SSR account pages render the correct palette first paint.
                        code.AddProperty("bool", "IsDarkTheme", isDarkTheme =>
                        {
                            isDarkTheme.Private();
                            isDarkTheme.WithoutSetter().Getter.WithExpressionImplementation(
                                @"!(HttpContext?.Request.Cookies.TryGetValue(""theme"", out var theme) == true && theme == ""light"")");
                        });
                    }

                    code.AddMethod("void", "OnParametersSet", onParametersSet =>
                    {
                        onParametersSet.Protected().Override();

                        onParametersSet.AddIfStatement("HttpContext is null", @if =>
                        {
                            @if.AddStatement("NavigationManager.Refresh(forceReload: true);");
                        });
                    });
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public IRazorFile RazorFile { get; }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            return RazorFile.GetConfig();
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public override string TransformText() => RazorFile.ToString();

        // Code-behind plumbing: routes the @code members into the sibling AccountLayoutCodeBehindTemplate
        // (.razor.cs) when present, falling back to an inline @code block otherwise. Mirrors the account
        // pages and Intent.Modules.Blazor's RazorComponentTemplateBase without requiring a model.
        private IBuildsCSharpMembers _codeBehind;

        public ICSharpFileBuilderTemplate CodeBehindTemplate { get; private set; }

        public override ICSharpCodeContext RootCodeContext => GetCodeBehind();

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            CodeBehindTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(AccountLayoutCodeBehindTemplate.TemplateId);
        }

        private IBuildsCSharpMembers GetCodeBehind()
        {
            if (_codeBehind != null)
            {
                return _codeBehind;
            }

            if (CodeBehindTemplate != null)
            {
                _codeBehind = CodeBehindTemplate.CSharpFile.Classes.First();
            }
            else
            {
                RazorFile.AddCodeBlock(x => _codeBehind = x);
            }

            return _codeBehind;
        }

        private static void AddMudBlazorAccountShell(IRazorFile file)
        {
            file.AddHtmlElement("MudThemeProvider", themeProvider => themeProvider.AddAttribute("IsDarkMode", "@IsDarkTheme"));
            file.AddHtmlElement("MudPopoverProvider");
            file.AddHtmlElement("MudDialogProvider");
            file.AddHtmlElement("MudSnackbarProvider");
            file.AddEmptyLine();

            file.AddHtmlElement("AppBrand", appBrand => appBrand.AddAttribute("Class", "account-brand"));
            file.AddHtmlElement("ThemeToggle", themeToggle => themeToggle.AddAttribute("Class", "account-theme-toggle"));
            file.AddEmptyLine();

            file.AddHtmlElement("MudLayout", mudLayout => mudLayout
                .AddHtmlElement("MudMainContent", mainContent =>
                {
                    mainContent.AddAttribute("Class", "pa-4");
                    mainContent.AddHtmlElement("div", contentDiv =>
                    {
                        contentDiv.AddClass("account-layout-content");
                        AddContentGuard(file, contentDiv);
                    });
                }));
        }

        private static void AddStandardAccountShell(IRazorFile file)
        {
            file.AddHtmlElement("AppBrand", appBrand => appBrand.AddAttribute("Class", "account-brand"));
            file.AddHtmlElement("ThemeToggle", themeToggle => themeToggle.AddAttribute("Class", "account-theme-toggle"));
            file.AddEmptyLine();

            file.AddHtmlElement("main", main =>
            {
                main.AddClass("account-layout-main");
                main.AddHtmlElement("div", contentDiv =>
                {
                    contentDiv.AddClass("account-layout-content");
                    AddContentGuard(file, contentDiv);
                });
            });
        }

        // Shows a loading placeholder while HttpContext is null (during the OnParametersSet forced reload),
        // otherwise renders the page, as an @if/else control-flow block inside the content area.
        private static void AddContentGuard(IRazorFile file, IHtmlElement contentDiv)
        {
            // A RazorCodeDirective renders its own braces and indents its child nodes, so each branch's
            // markup is added as children of the @if / else directive rather than emitting { } ourselves.
            var ifDirective = IRazorCodeDirective.Create(new CSharpStatement("@if (HttpContext is null)"), file);
            ifDirective.AddChildNode(new HtmlElement("p", file).WithText("Loading..."));
            contentDiv.AddChildNode(ifDirective);

            var elseDirective = IRazorCodeDirective.Create(new CSharpStatement("else"), file);
            elseDirective.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement("@Body"), file));
            contentDiv.AddChildNode(elseDirective);
        }
    }
}
