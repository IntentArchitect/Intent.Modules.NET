using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.VisualStudio.Projects.Api.Extensions;
using Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.ServiceManifest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceManifestTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                $"""
                <?xml version="1.0" encoding="utf-8"?>
                <ServiceManifest Name="{Model.Name}Pkg"
                                 Version="1.0.0"
                                 xmlns="http://schemas.microsoft.com/2011/01/fabric"
                                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                """);

            switch (Model.GetServiceFabric().Type)
            {
                case ServiceFabricType.Actor:
                    OutputTarget.Emit(new ManifestImportRequiredEvent(serviceManifestName: $"{Model.Name}Pkg"));

                    sb.AppendLine("  <!-- The content will be generated during build -->");
                    break;
                case ServiceFabricType.Stateful:
                    OutputTarget.Emit<ServiceRegistrationRequiredBase>(new StatefulServiceRegistrationRequired(
                        name: Model.Name,
                        serviceTypeName: $"{Model.Name}Type",
                        targetReplicaSetSize: $"[{Model.Name}_TargetReplicaSetSize]",
                        minReplicaSetSize: $"[{Model.Name}_MinReplicaSetSize]",
                        partitionCount: $"[{Model.Name}_PartitionCount]")
                    {
                        PartitionCountParameterDefaultValue = 1,
                        PartitionCountParameterEnvironmentValues =
                        {
                            ["Cloud"] = 1,
                            ["Local.1Node.xml"] = 1,
                            ["Local.5Node.xml"] = 1
                        },
                        MinReplicaSetSizeParameterDefaultValue = 3,
                        MinReplicaSetSizeParameterEnvironmentValues =
                        {
                            ["Cloud"] = 3,
                            ["Local.1Node.xml"] = 1,
                            ["Local.5Node.xml"] = 3
                        },
                        TargetReplicaSetSizeParameterDefaultValue = 3,
                        TargetReplicaSizeParameterEnvironmentValues =
                        {
                            ["Cloud"] = 3,
                            ["Local.1Node.xml"] = 1,
                            ["Local.5Node.xml"] = 3
                        }
                    });

                    if (IsAspNetProject)
                    {
                        OutputTarget.Emit(new ManifestImportRequiredEvent(serviceManifestName: $"{Model.Name}Pkg")
                        {
                            ConfigOverrides = [],
                            EnvironmentOverrides =
                            [
                                new EnvironmentVariable("ASPNETCORE_ENVIRONMENT", $"[{Model.Name}_ASPNETCORE_ENVIRONMENT]", "")
                            ]
                        });

                        sb.AppendLine(
                            $"""
                              <ServiceTypes>
                                <!-- This is the name of your ServiceType. 
                                     This name must match the string used in RegisterServiceType call in Program.cs. -->
                                <StatefulServiceType ServiceTypeName="{Model.Name}Type"  HasPersistedState="true" />
                              </ServiceTypes>
                              
                              <!-- Code package is your service executable. -->
                              <CodePackage Name="Code" Version="1.0.0">
                                <EntryPoint>
                                  <ExeHost>
                                    <Program>{Model.Name}.exe</Program>
                                    <WorkingFolder>CodePackage</WorkingFolder>
                                  </ExeHost>
                                </EntryPoint>
                                <EnvironmentVariables>
                                  <EnvironmentVariable Name="ASPNETCORE_ENVIRONMENT" Value=""/>
                                </EnvironmentVariables>
                              </CodePackage>
                              
                              <!-- Config package is the contents of the Config directory under PackageRoot that contains an 
                                   independently-updateable and versioned set of custom configuration settings for your service. -->
                              <ConfigPackage Name="Config" Version="1.0.0" />
                            
                            """);

                        break;
                    }

                    OutputTarget.Emit(new ManifestImportRequiredEvent(serviceManifestName: $"{Model.Name}Pkg")
                    {
                        ConfigOverrides = []
                    });

                    sb.AppendLine(
                        $"""
                          <ServiceTypes>
                            <!-- This is the name of your ServiceType. 
                                 This name must match the string used in RegisterServiceType call in Program.cs. -->
                            <StatefulServiceType ServiceTypeName="{Model.Name}Type" HasPersistedState="true" />
                          </ServiceTypes>
                          
                          <!-- Code package is your service executable. -->
                          <CodePackage Name="Code" Version="1.0.0">
                            <EntryPoint>
                              <ExeHost>
                                <Program>{Model.Name}.exe</Program>
                              </ExeHost>
                            </EntryPoint>
                          </CodePackage>
                          
                          <!-- Config package is the contents of the Config directory under PackageRoot that contains an 
                               independently-updateable and versioned set of custom configuration settings for your service. -->
                          <ConfigPackage Name="Config" Version="1.0.0" />
                          
                          <Resources>
                            <Endpoints>
                              <!-- This endpoint is used by the communication listener to obtain the port on which to 
                                   listen. Please note that if your service is partitioned, this port is shared with 
                                   replicas of different partitions that are placed in your code. -->
                              <Endpoint Name="ServiceEndpoint" />
                          
                              <!-- This endpoint is used by the replicator for replicating the state of your service.
                                   This endpoint is configured through a ReplicatorSettings config section in the Settings.xml
                                   file under the ConfigPackage. -->
                              <Endpoint Name="ReplicatorEndpoint" />
                            </Endpoints>
                          </Resources>
                        """);

                    break;
                case ServiceFabricType.Stateless:
                    OutputTarget.Emit<ServiceRegistrationRequiredBase>(new StatelessServiceRegistrationRequired(
                        name: Model.Name,
                        serviceTypeName: $"{Model.Name}Type",
                        instanceCount: $"[{Model.Name}_InstanceCount]")
                    {
                        InstanceCountParameterDefaultValue = -1,
                        InstanceCountParameterEnvironmentValues =
                        {
                            ["Cloud"] = -1,
                            ["Local.1Node.xml"] = 1,
                            ["Local.5Node.xml"] = 1,
                        }
                    });

                    if (IsAspNetProject)
                    {
                        OutputTarget.Emit(new ManifestImportRequiredEvent(serviceManifestName: $"{Model.Name}Pkg")
                        {
                            ConfigOverrides = [],
                            EnvironmentOverrides =
                            [
                                new EnvironmentVariable("ASPNETCORE_ENVIRONMENT", $"[{Model.Name}_ASPNETCORE_ENVIRONMENT]", "")
                            ]
                        });

                        sb.AppendLine(
                           $"""
                              <ServiceTypes>
                                <!-- This is the name of your ServiceType. 
                                     This name must match the string used in RegisterServiceType call in Program.cs. -->
                                <StatelessServiceType ServiceTypeName="{Model.Name}Type" />
                              </ServiceTypes>
                              
                              <!-- Code package is your service executable. -->
                              <CodePackage Name="Code" Version="1.0.0">
                                <EntryPoint>
                                  <ExeHost>
                                    <Program>{Model.Name}.exe</Program>
                                    <WorkingFolder>CodePackage</WorkingFolder>
                                  </ExeHost>
                                </EntryPoint>
                                <EnvironmentVariables>
                                  <EnvironmentVariable Name="ASPNETCORE_ENVIRONMENT" Value=""/>
                                </EnvironmentVariables>
                              </CodePackage>
                              
                              <!-- Config package is the contents of the Config directory under PackageRoot that contains an 
                                   independently-updateable and versioned set of custom configuration settings for your service. -->
                              <ConfigPackage Name="Config" Version="1.0.0" />
                              
                              <Resources>
                                <Endpoints>
                                  <!-- This endpoint is used by the communication listener to obtain the port on which to 
                                       listen. Please note that if your service is partitioned, this port is shared with 
                                       replicas of different partitions that are placed in your code. -->
                                  <Endpoint Protocol="http" Name="ServiceEndpoint" Type="Input" Port="8777" />
                                </Endpoints>
                              </Resources>
                            """);

                        break;
                    }

                    OutputTarget.Emit(new ManifestImportRequiredEvent(serviceManifestName: $"{Model.Name}Pkg")
                    {
                        ConfigOverrides = []
                    });

                    sb.AppendLine(
                        $"""
                        <ServiceTypes>
                          <!-- This is the name of your ServiceType. 
                               This name must match the string used in the RegisterServiceAsync call in Program.cs. -->
                          <StatelessServiceType ServiceTypeName="{Model.Name}Type" />
                        </ServiceTypes>
                        
                        <!-- Code package is your service executable. -->
                        <CodePackage Name="Code" Version="1.0.0">
                          <EntryPoint>
                            <ExeHost>
                              <Program>{Model.Name}.exe</Program>
                            </ExeHost>
                          </EntryPoint>
                        </CodePackage>
                        
                        <!-- Config package is the contents of the Config directory under PackageRoot that contains an 
                             independently-updateable and versioned set of custom configuration settings for your service. -->
                        <ConfigPackage Name="Config" Version="1.0.0" />
                        
                        <Resources>
                          <Endpoints>
                            <!-- This endpoint is used by the communication listener to obtain the port on which to 
                                 listen. Please note that if your service is partitioned, this port is shared with 
                                 replicas of different partitions that are placed in your code. -->
                            <Endpoint Name="ServiceEndpoint" />
                          </Endpoints>
                        </Resources>
                        """);

                    break;
                default:
                    throw new InvalidOperationException($"Unknown type: {Model.GetServiceFabric().Type}");
            }

            sb.Append("</ServiceManifest>");
            return sb.ToString();
        }
    }
}