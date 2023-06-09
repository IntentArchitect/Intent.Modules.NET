using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Versioning.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class ControllerInstaller : FactoryExtensionBase
{
    public override string Id => "Intent.AspNetCore.Versioning.ControllerInstaller";

    [IntentManaged(Mode.Ignore)]
    public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Distribution.WebApi.Controller));
        foreach (var template in templates)
        {
            var @class = template.CSharpFile.Classes.First();
            foreach (var method in @class.Methods)
            {
                
            }
        }
    }
}