using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
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
                var groupedVersions = new HashSet<IApiVersionModel>();

                UpdateControllerActionMethods(@class, groupedVersions);
                UpdateControllerClass(file, @class, groupedVersions);
            });
        }
    }

    private static void UpdateControllerActionMethods(CSharpClass @class,
        HashSet<IApiVersionModel> groupedVersions)
    {
        foreach (var method in @class.Methods)
        {
            var methodModel = method.GetMetadata<IControllerOperationModel>("model");
            if (methodModel.ApplicableVersions is null)
            {
                continue;
            }

            foreach (var version in methodModel.ApplicableVersions)
            {
                groupedVersions.Add(version);
                method.AddAttribute($@"[MapToApiVersion(""{version.Version.Replace("v", "", StringComparison.OrdinalIgnoreCase)}"")]");
            }

            foreach (var attribute in method.Attributes.Where(p => p.Name.Contains("{version}")))
            {
                attribute.Name = attribute.Name.Replace("{version}", "v{version:apiVersion}");
            }
        }
    }

    private static void UpdateControllerClass(CSharpFile file, CSharpClass @class,
        HashSet<IApiVersionModel> groupedVersions)
    {
        foreach (var attribute in @class.Attributes.Where(p => p.Name.Contains("{version}")))
        {
            attribute.Name = attribute.Name.Replace("{version}", "v{version:apiVersion}");
        }

        var classModel = @class.GetMetadata<IControllerModel>("model");

        foreach (var version in classModel.ApplicableVersions)
        {
            groupedVersions.Add(version);
        }

        foreach (var version in groupedVersions)
        {
            file.AddUsing("Asp.Versioning");
            @class.AddAttribute($@"[ApiVersion(""{version.Version.Replace("v", "", StringComparison.OrdinalIgnoreCase)}""{(version.IsDeprecated ? ", Deprecated = true" : string.Empty)})]");
        }
    }
}