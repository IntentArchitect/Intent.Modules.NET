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
                        var cif = new CSharpIfStatement("HttpContext.Request.Path.StartsWithSegments(\"/Account\")");
                        cif.AddReturn(new CSharpStatement("null"));
                        renderModeMethod.InsertStatement(0, cif);
                    }
                }

                var imports = application.FindTemplateInstance<IRazorFileTemplate>(ServerImportsRazorTemplate.TemplateId);
                imports?.RazorFile.AddUsing("Microsoft.AspNetCore.Components.Authorization");
            });
        }
    }
}