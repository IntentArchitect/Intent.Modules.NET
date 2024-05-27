using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout;
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
    public partial class RoutesRazorTemplate : RazorTemplateBase<object>, IRazorFileTemplate
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
                        found.AddHtmlElement("RouteView", html =>
                        {
                            html.AddAttribute("RouteData", "routeData");
                            var defaultLayoutModel = ExecutionContext.MetadataManager.UserInterface(ExecutionContext.GetApplicationConfig().Id)
                                .GetElementsOfType(LayoutModel.SpecializationTypeId)
                                .Select(x => new LayoutModel(x))
                                .FirstOrDefault(x => x.Name is "MainLayout" or "DefaultLayout");
                            if (defaultLayoutModel != null)
                            {
                                html.AddAttribute("DefaultLayout", $"typeof({NormalizeNamespace(GetTemplate<IClassProvider>(RazorLayoutTemplate.TemplateId, defaultLayoutModel).FullTypeName())})");
                            }
                        });
                        found.AddHtmlElement("FocusOnNavigate", html => html.AddAttribute("RouteData", "routeData").AddAttribute("Selector", $"h1"));
                    });
                });
            });
        }

        public RazorFile RazorFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            return new RazorFileConfig(
                className: $"Routes",
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath()
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return RazorFile.Build().ToString();
        }

    }
}