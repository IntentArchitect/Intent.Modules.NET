using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RoutesRazor
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore, Comments = Mode.Fully)]
    public class RoutesRazorTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RoutesRazorTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RoutesRazorTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RoutesRazorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, "Routes").Configure(file =>
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
    }
}