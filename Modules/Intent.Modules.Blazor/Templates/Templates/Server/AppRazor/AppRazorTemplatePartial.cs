using System;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Blazor.Settings.Blazor;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Server.AppRazor
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AppRazorTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Server.AppRazorTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="AppRazorTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AppRazorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, "App")
                .Configure(file =>
                {
                    file.AddHtmlElement("html", html =>
                    {
                        var enableThemeToggle = ExecutionContext.Settings.GetBlazor().EnableThemeToggle();

                        html.AddAttribute("lang", "en");
                        html.AddAttribute("class", "ux-drawer-open");
                        if (enableThemeToggle)
                        {
                            html.AddAttribute("data-theme", "@_theme");
                            html.AddAttribute("data-theme-storage", "cookie");
                        }
                        html.AddEmptyLine();
                        html.AddCodeBlock($"@Intent.Merge()");
                        html.AddEmptyLine();
                        html.AddHtmlElement("head", head =>
                        {
                            head.AddHtmlElement("meta", t => t.AddAttribute("charset", "utf-8"));
                            head.AddHtmlElement("meta", t => t
                                .AddAttribute("name", "viewport")
                                .AddAttribute("content", "width=device-width, initial-scale=1.0"));
                            head.AddHtmlElement("base", t => t.AddAttribute("href", "/"));
                            head.AddHtmlElement("link", t => t
                                .AddAttribute("rel", "stylesheet")
                                .AddAttribute("href", "ux-tokens.css"));
                            head.AddHtmlElement("link", t => t
                                .AddAttribute("rel", "stylesheet")
                                .AddAttribute("href", "ux-base.css"));
                            head.AddHtmlElement("link", t => t
                                .AddAttribute("rel", "stylesheet")
                                .AddAttribute("href", "ux-components.css"));
                            // app.css only exists when a content group actually shipped it - see
                            // TemplateHelper.ShipsAppCss. Emitting the link unconditionally 404'd on
                            // every page load of every application the content groups do not cover.
                            if (TemplateHelper.ShipsAppCss(ExecutionContext))
                            {
                                head.AddHtmlElement("link", t => t
                                    .AddAttribute("rel", "stylesheet")
                                    .AddAttribute("href", "app.css"));
                            }
                            head.AddHtmlElement("link", t => t
                                .AddAttribute("rel", "stylesheet")
                                .AddAttribute("href", $"{outputTarget.GetProject().Name}.styles.css"));

                            head.AddHtmlElement("HeadOutlet", t => t.AddAttribute("@rendermode", "GetRenderModeForPage()"));
                        });

                        html.AddEmptyLine();

                        html.AddHtmlElement("body", body =>
                        {
                            body.AddHtmlElement("Routes", t => t.AddAttribute("@rendermode", "GetRenderModeForPage()"));
                            body.AddHtmlElement("script", t => t.AddAttribute("src", "_framework/blazor.web.js"));
                            if (enableThemeToggle)
                            {
                                body.AddHtmlElement("script", t => t.AddAttribute("src", "theme-storage.js"));
                            }
                            body.AddHtmlElement("script", t => t.AddAttribute("src", "nav-drawer.js"));
                            body.AddHtmlElement("script", t => t.AddAttribute("src", "user-menu.js"));
                            if (enableThemeToggle)
                            {
                                body.AddHtmlElement("script", t => t.WithText("themeStorage.init();"));
                            }
                        });

                        html.AddEmptyLine();

                    });
                    file.AddCodeBlock(code =>
                    {
                        code.AddProperty("HttpContext", "HttpContext", property =>
                        {
                            property.WithInitialValue("default!");
                            property.AddAttribute("[CascadingParameter]");
                        });

                        code.AddMethod("IComponentRenderMode?", "GetRenderModeForPage", method =>
                        {
                            // Honoured in every render mode. A prerendered page runs on the server, so
                            // anything it calls is issued server-side rather than from the browser.
                            if (!ExecutionContext.Settings.GetBlazor().ServerPrerendering())
                            {
                                method.AddStatement($"return new {GetRenderModeConfiguration(ExecutionContext.Settings.GetBlazor()?.RenderMode()?.AsEnum())}RenderMode(prerender: false);");
                            }
                            else
                            {
                                method.AddStatement($"return {GetRenderModeConfiguration(ExecutionContext.Settings.GetBlazor()?.RenderMode()?.AsEnum())};");
                            }
                        });

                        if (ExecutionContext.Settings.GetBlazor().EnableThemeToggle())
                        {
                            code.AddProperty("string", "_theme", prop =>
                            {
                                prop.Private();
                                prop.WithoutSetter();
                                prop.Getter.WithExpressionImplementation("HttpContext.Request.Cookies.TryGetValue(\"theme\", out var t) && t == \"light\" ? \"light\" : \"dark\"");
                            });
                        }
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
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $"""
                <!DOCTYPE html>
                {RazorFile.ToString().Trim()}
                """;

        }

        private static string GetRenderModeConfiguration(RenderModeOptionsEnum? renderMode) => renderMode switch
        {
            RenderModeOptionsEnum.InteractiveAuto => "InteractiveAuto",
            RenderModeOptionsEnum.InteractiveServer => "InteractiveServer",
            RenderModeOptionsEnum.InteractiveWebAssembly => "InteractiveWebAssembly",
            _ => "InteractiveWebAssembly"
        };
    }
}
