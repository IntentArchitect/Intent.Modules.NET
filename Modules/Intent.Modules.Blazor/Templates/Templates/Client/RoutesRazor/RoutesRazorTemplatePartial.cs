using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RoutesRazor
{
    [IntentManaged(Mode.Merge)]
    public partial class RoutesRazorTemplate : CSharpTemplateBase<object>, IRazorFileTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Client.RoutesRazorTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RoutesRazorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = new RazorFile(this).Configure(file =>
            {
                file.AddHtmlElement("Router", router =>
                {
                    router.AddAttribute("AppAssembly", $"typeof({this.GetProgramTemplateName()}).Assembly");
                    router.AddHtmlElement("Found", found =>
                    {
                        found.AddAttribute("Context", "routeData");
                        found.AddHtmlElement("RouteView", html => html.AddAttribute("RouteData", "routeData").AddAttribute("DefaultLayout", $"typeof(Layout.MainLayout)"));
                        found.AddHtmlElement("FocusOnNavigate", html => html.AddAttribute("RouteData", "routeData").AddAttribute("Selector", $"h1"));
                    });
                });
            });
        }

        public RazorFile RazorFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Routes",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}",
                fileExtension: "razor");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return RazorFile.Build().ToString();
        }

    }
}