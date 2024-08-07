using System;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

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
                        html.AddAttribute("lang", "en");
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
                                .AddAttribute("href", "bootstrap/bootstrap.min.css"));
                            head.AddHtmlElement("link", t => t
                                .AddAttribute("rel", "stylesheet")
                                .AddAttribute("href", "app.css"));
                            head.AddHtmlElement("link", t => t
                                .AddAttribute("rel", "stylesheet")
                                .AddAttribute("href", $"{outputTarget.GetProject().Name}.styles.css"));
                            head.AddHtmlElement("link", t => t
                                .AddAttribute("rel", "icon")
                                .AddAttribute("type", "image/png")
                                .AddAttribute("href", $"favicon.png"));
                            head.AddHtmlElement("HeadOutlet", t => t.AddAttribute("@rendermode", "GetRenderModeForPage()"));
                        });

                        html.AddEmptyLine();

                        html.AddHtmlElement("body", body =>
                        {
                            body.AddHtmlElement("Routes", t => t.AddAttribute("@rendermode", "GetRenderModeForPage()"));
                            body.AddHtmlElement("script", t => t.AddAttribute("src", "_framework/blazor.web.js"));
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
                            method.AddStatement("return InteractiveAuto;");
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
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $"""
                    <!DOCTYPE html>
                    {RazorFile.ToString().Trim()}
                    """;
        }
    }
}