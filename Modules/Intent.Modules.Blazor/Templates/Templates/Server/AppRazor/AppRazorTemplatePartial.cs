using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Server.AppRazor
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AppRazorTemplate : CSharpTemplateBase<object>, IRazorFileTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Server.AppRazorTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AppRazorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = new RazorFile(this)
                .Configure(file =>
                {
                    file.AddHtmlElement("head", head =>
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
                        head.AddHtmlElement("HeadOutlet");
                    });

                    file.AddEmptyLine();

                    file.AddHtmlElement("body", body =>
                    {
                        body.AddHtmlElement("Routes");
                        body.AddHtmlElement("script", t => t.AddAttribute("src", "_framework/blazor.web.js"));
                    });
                });
        }


        public RazorFile RazorFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"App",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}",
                fileExtension: "razor");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $@"<!DOCTYPE html>
<html lang=""en"">

{RazorFile.Build().ToString().Trim()}

</html>";
        }
    }
}