using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.PackagesConfig
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PackagesConfigTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return
                $"""
                <?xml version="1.0" encoding="utf-8"?>
                <packages>
                  <package id="Microsoft.VisualStudio.Azure.Fabric.MSBuild" version="{NuGetPackages.MicrosoftVisualStudioAzureFabricMSBuild}" targetFramework="net48" />
                </packages>
                """;
        }
    }
}