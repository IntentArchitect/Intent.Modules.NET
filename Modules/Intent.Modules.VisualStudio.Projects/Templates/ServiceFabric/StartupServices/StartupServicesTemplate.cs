using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.StartupServices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class StartupServicesTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return
                """
                <?xml version="1.0" encoding="utf-8"?>
                <StartupServicesManifest xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                         xmlns="http://schemas.microsoft.com/2011/01/fabric">
                  <Parameters>
                  </Parameters>
                  <Services>
                    <!-- The section below creates instances of service types, when an instance of this 
                         application type is created. You can also create one or more instances of service type using the 
                         ServiceFabric PowerShell module.
                
                         The attribute ServiceTypeName below must match the name defined in the imported ServiceManifest.xml file. -->
                  </Services>
                </StartupServicesManifest>
                """;
        }
    }
}