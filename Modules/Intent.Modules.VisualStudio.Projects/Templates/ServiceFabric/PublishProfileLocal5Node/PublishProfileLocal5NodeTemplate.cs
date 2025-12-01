using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.PublishProfileLocal5Node
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PublishProfileLocal5NodeTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return
                """
                <?xml version="1.0" encoding="utf-8"?>
                <PublishProfile xmlns="http://schemas.microsoft.com/2015/05/fabrictools">
                  <!-- ClusterConnectionParameters allows you to specify the PowerShell parameters to use when connecting to the Service Fabric cluster.
                       Valid parameters are any that are accepted by the Connect-ServiceFabricCluster cmdlet.
                
                       For a local cluster, you would typically not use any parameters.
                         For example: <ClusterConnectionParameters />
                  -->
                  <ClusterConnectionParameters />
                  <ApplicationParameterFile Path="..\ApplicationParameters\Local.5Node.xml" />
                  <StartupServiceParameterFile Path="..\StartupServiceParameters\Local.5Node.xml" />
                </PublishProfile>
                """;
        }
    }
}