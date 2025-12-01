using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.StartupServiceParametersLocal5Node
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class StartupServiceParametersLocal5NodeTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return
                """
                <?xml version="1.0" encoding="utf-8"?>
                <StartupServices xmlns="http://schemas.microsoft.com/2011/01/fabric" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                  <Parameters>
                  </Parameters>
                </StartupServices>
                """;
        }
    }
}