using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
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
                    var layoutNamespace = $"{rootNamespace}Components.Layout";
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

                    file.AddCodeBlock(code =>
                    {
                        code.AddProperty("HttpContext?", "HttpContext", httpContext =>
                        {
                            httpContext.Private();
                            httpContext.AddAttribute("CascadingParameter");
                        });

                        // Loading placeholder shown by the content guard while HttpContext is null
                        // (i.e. during the OnParametersSet forced reload). See AddContentGuard.
                        code.AddProperty("RenderFragment", "Loading", loading =>
                        {
                            loading.Private();
                            loading.WithoutSetter().Getter.WithExpressionImplementation(
                                "builder => builder.AddMarkupContent(0, \"<p>Loading...</p>\")");
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

        private static void AddMudBlazorAccountShell(IRazorFile file)
        {
            file.AddHtmlElement("MudThemeProvider", themeProvider => themeProvider.AddAttribute("IsDarkMode", "@IsDarkTheme"));
            file.AddHtmlElement("MudPopoverProvider");
            file.AddHtmlElement("MudDialogProvider");
            file.AddHtmlElement("MudSnackbarProvider");
            file.AddEmptyLine();

            file.AddHtmlElement("ThemeToggle", themeToggle => themeToggle.AddAttribute("Class", "account-theme-toggle"));
            file.AddEmptyLine();

            file.AddHtmlElement("MudLayout", mudLayout => mudLayout
                .AddHtmlElement("MudMainContent", mainContent =>
                {
                    mainContent.AddAttribute("Class", "account-layout-main pa-4");
                    mainContent.AddHtmlElement("div", contentDiv =>
                    {
                        contentDiv.AddClass("account-layout-content");
                        AddContentGuard(contentDiv);
                    });
                }));
            file.AddEmptyLine();

            file.AddHtmlElement("style", style => style.WithText(@"
    .account-theme-toggle {
        position: fixed;
        top: 0.75rem;
        right: 0.75rem;
        z-index: 1100;
        width: 2.5rem;
        height: 2.5rem;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        border: 1px solid var(--border);
        border-radius: 999px;
        color: var(--text);
        background: var(--surface-2);
        box-shadow: var(--ux-shadow-sm);
        cursor: pointer;
    }

    .account-theme-toggle:hover {
        background: color-mix(in srgb, var(--surface-2) 86%, var(--hover-overlay));
    }

    .account-layout-main {
        min-height: 100vh;
    }

    .account-layout-content {
        width: 100%;
        max-width: 1120px;
        margin: 0 auto;
        padding-top: 1.5rem;
    }

    @@media (max-width: 600px) {
        .account-layout-content {
            padding-top: 2.5rem;
        }
    }
"));
        }

        private static void AddStandardAccountShell(IRazorFile file)
        {
            file.AddHtmlElement("ThemeToggle", themeToggle => themeToggle.AddAttribute("Class", "account-theme-toggle"));
            file.AddEmptyLine();

            file.AddHtmlElement("main", main =>
            {
                main.AddClass("account-layout-main");
                main.AddHtmlElement("div", contentDiv =>
                {
                    contentDiv.AddClass("account-layout-content");
                    AddContentGuard(contentDiv);
                });
            });
            file.AddEmptyLine();

            file.AddHtmlElement("style", style => style.WithText(@"
    .account-theme-toggle {
        position: fixed;
        top: 0.75rem;
        right: 0.75rem;
        z-index: 1100;
        display: inline-flex;
    }

    .account-theme-toggle .theme-toggle-button {
        width: 2.5rem;
        height: 2.5rem;
        border: 1px solid var(--border);
        border-radius: 999px;
        color: var(--text);
        background: var(--surface-2);
        box-shadow: var(--ux-shadow-sm);
    }

    .account-theme-toggle .theme-toggle-button:hover {
        background: color-mix(in srgb, var(--surface-2) 86%, var(--hover-overlay));
    }

    .account-layout-main {
        min-height: 100vh;
        padding: 1rem;
    }

    .account-layout-content {
        width: 100%;
        max-width: 1120px;
        margin: 0 auto;
        padding-top: 1.5rem;
    }

    @@media (max-width: 600px) {
        .account-layout-content {
            padding-top: 2.5rem;
        }
    }
"));
        }

        // Renders the HttpContext guard inside the content area.
        //
        // What this SHOULD be (and what the hand-written prototype uses) is an @if/else block:
        //
        //     @if (HttpContext is null)
        //     {
        //         <p>Loading...</p>
        //     }
        //     else
        //     {
        //         @Body
        //     }
        //
        // However the Intent Architect Razor merger (Intent.Code.Weaving.Razor.RazorTreeMerger,
        // v5.1.x) throws "Unexpected type: MarkupEphemeralTextLiteralSyntax" when re-merging an @if
        // control-flow block nested inside markup, so a template that emits one cannot be regenerated.
        // See razor-merger-if-block-bug.md (submitted to the maintainers). Until that is fixed we use a
        // merger-safe single razor expression that selects between the Loading fragment and @Body — it
        // is behaviourally identical and merges cleanly. Revert to the @if/else above once the merger
        // handles nested @if blocks.
        private static void AddContentGuard(IHtmlElement contentDiv)
        {
            contentDiv.WithText("@(HttpContext is null ? Loading : Body)");
        }
    }
}
