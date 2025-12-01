using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.ApplicationParametersLocal5Node
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApplicationParametersLocal5NodeTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return
                $"""
                <?xml version="1.0" encoding="utf-8"?>
                <Application Name="fabric:/{ExecutionContext.GetApplicationConfig().Name}" xmlns="http://schemas.microsoft.com/2011/01/fabric" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                  <Parameters>
                  </Parameters>
                </Application>
                """;
        }
    }
}