using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
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
        var templates = application.FindTemplateInstances<ControllerTemplate>(TemplateDependency.OnTemplate(ControllerTemplate.TemplateId));
        foreach (var template in templates)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var groupedVersions = new HashSet<string>();
                foreach (var method in @class.Methods)
                {
                    var methodModel = method.GetMetadata<IControllerOperationModel>("model");
                    var versions = methodModel.InternalElement.GetApiVersion().ApplicableVersions()
                        .Select(x => x.AsVersionModel())
                        .Select(x => x.Name)
                        .ToList();
                    foreach (var version in versions)
                    {
                        groupedVersions.Add(version);
                        method.AddAttribute($@"[MapToApiVersion(""{version}"")]");
                    }
                }

                foreach (var version in groupedVersions)
                {
                    @class.AddAttribute($@"[ApiVersion(""{version}"")]");
                }
            });
        }
    }
}