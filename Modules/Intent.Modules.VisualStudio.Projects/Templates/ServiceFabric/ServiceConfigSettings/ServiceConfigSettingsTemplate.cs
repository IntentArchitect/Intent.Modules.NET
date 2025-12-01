using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.VisualStudio.Projects.Api.Extensions;
using Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.ServiceConfigSettings
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceConfigSettingsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            const string webContent =
                """
                  <!-- Add your custom configuration sections and parameters here -->
                  <!--
                  <Section Name="MyConfigSection">
                    <Parameter Name="MyParameter" Value="Value1" />
                  </Section>
                  -->
                """;

            var sb = new StringBuilder();
            sb.AppendLine("""<?xml version="1.0" encoding="utf-8"?>""");
            sb.AppendLine(
                $"""
                <ServiceManifest Name="{Model.Name}Pkg"
                                 Version="1.0.0"
                                 xmlns="http://schemas.microsoft.com/2011/01/fabric"
                                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                """);

            switch (Model.GetServiceFabric().Type)
            {
                case ServiceFabricType.Actor:
                    sb.AppendLine("  <!-- The content will be generated during build -->");
                    break;
                case ServiceFabricType.Stateful:
                    if (IsAspNetProject)
                    {
                        sb.AppendLine(webContent);
                        break;
                    }

                    sb.AppendLine(
                        """
                          <!-- This is used by the StateManager's replicator. -->
                          <Section Name="ReplicatorConfig">
                            <Parameter Name="ReplicatorEndpoint" Value="ReplicatorEndpoint" />
                          </Section>
                          <!-- This is used for securing StateManager's replication traffic. -->
                          <Section Name="ReplicatorSecurityConfig" />
                          
                          <!-- Add your custom configuration sections and parameters here. -->
                          <!--
                          <Section Name="MyConfigSection">
                            <Parameter Name="MyParameter" Value="Value1" />
                          </Section>
                          -->
                        """);

                    break;
                case ServiceFabricType.Stateless:
                    if (IsAspNetProject)
                    {
                        sb.AppendLine(webContent);
                        break;
                    }

                    sb.AppendLine(
                        """
                          <!-- Add your custom configuration sections and parameters here -->
                          <!--
                          <Section Name="MyConfigSection">
                            <Parameter Name="MyParameter" Value="Value1" />
                          </Section>
                          -->
                        """);

                    break;
                default:
                    throw new InvalidOperationException($"Unknown type: {Model.GetServiceFabric().Type}");
            }

            sb.AppendLine("</ServiceManifest>");

            return sb.ToString();
        }
    }
}