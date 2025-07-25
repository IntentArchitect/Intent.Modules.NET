using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Templates.Templates.Server.AppRazor;
using Intent.Modules.Blazor.Templates.Templates.Server.ServerImportsRazor;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeRenderMode : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Authentication.ChangeRenderMode";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var app = application.FindTemplateInstance<IRazorFileTemplate>(AppRazorTemplate.TemplateId)?.RazorFile;

            if (app == null)
            {
                Logging.Log.Warning("Unable to change rendermode. App.Razor file could not be found.");
                return;
            }

            app.OnBuild(file =>
            {
                var razorCodeBlock = file.ChildNodes.FirstOrDefault(n => n is IRazorCodeBlock);

                if (razorCodeBlock is not null)
                {
                    var codeBlock = razorCodeBlock as ICSharpClass;
                    var renderModeMethod = codeBlock.FindMethod("GetRenderModeForPage");

                    if (renderModeMethod is not null)
                    {
                        renderModeMethod.Statements.Clear();
                        renderModeMethod.AddIfStatement("HttpContext.Request.Path.StartsWithSegments(\"/Account\")", cif =>
                        {
                            cif.AddReturn(new CSharpStatement("null"));
                        });

                        switch (app.Template.ExecutionContext.GetSettings().GetGroup("489a67db-31b2-4d51-96d7-52637c3795be").GetSetting("3e3d24f8-ad29-44d6-b7e5-e76a5af2a7fa").Value)
                        {
                            case "interactive-web-assembly":
                                renderModeMethod.AddStatements("return InteractiveWebAssembly;");
                                break;
                            case "interactive-auto":
                                renderModeMethod.AddStatements("return InteractiveAuto;");
                                break;
                            case "interactive-server":
                                renderModeMethod.AddStatements("return InteractiveServer;");
                                break;
                            default:
                                break;
                        }
                    }
                }

                var imports = application.FindTemplateInstance<IRazorFileTemplate>(ServerImportsRazorTemplate.TemplateId);
                imports?.RazorFile.AddUsing("Microsoft.AspNetCore.Components.Authorization");
            });
        }
    }
}