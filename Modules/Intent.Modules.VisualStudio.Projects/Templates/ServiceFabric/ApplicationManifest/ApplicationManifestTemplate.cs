using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.ApplicationManifest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApplicationManifestTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return
                $"""
                <?xml version="1.0" encoding="utf-8"?>
                <ApplicationManifest ApplicationTypeName="{ExecutionContext.GetApplicationConfig().Name}Type"
                                     ApplicationTypeVersion="1.0.0"
                                     xmlns="http://schemas.microsoft.com/2011/01/fabric"
                                     xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                     xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                  <Parameters>
                  </Parameters>
                  <!-- Import the ServiceManifest from the ServicePackage. The ServiceManifestName and ServiceManifestVersion 
                       should match the Name and Version attributes of the ServiceManifest element defined in the 
                       ServiceManifest.xml file. -->
                </ApplicationManifest>
                """;
        }
    }
}