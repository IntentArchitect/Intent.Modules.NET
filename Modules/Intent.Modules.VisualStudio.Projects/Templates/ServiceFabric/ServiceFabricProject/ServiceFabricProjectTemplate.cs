using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.ServiceFabricProject
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceFabricProjectTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var nugetPackageVersion = NuGetPackages.MicrosoftVisualStudioAzureFabricMSBuild;

            return
                $$"""
                <?xml version="1.0" encoding="utf-8"?>
                <Project ToolsVersion="14.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                  <Import Project="..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.{{nugetPackageVersion}}\build\Microsoft.VisualStudio.Azure.Fabric.Application.props" Condition="Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.{{nugetPackageVersion}}\build\Microsoft.VisualStudio.Azure.Fabric.Application.props')" />
                  <PropertyGroup Label="Globals">
                    <ProjectGuid>a6224586-24a3-4fc9-95cc-1845d093271c</ProjectGuid>
                    <ProjectVersion>2.6</ProjectVersion>
                    <MinToolsVersion>16.10</MinToolsVersion>
                    <SupportedMSBuildNuGetPackageVersion>1.7.6</SupportedMSBuildNuGetPackageVersion>
                    <TargetFrameworkVersion>{{GetTargetFrameworks().First()}}</TargetFrameworkVersion>
                  </PropertyGroup>
                  <ItemGroup Label="ProjectConfigurations">
                    <ProjectConfiguration Include="Debug|x64">
                      <Configuration>Debug</Configuration>
                      <Platform>x64</Platform>
                    </ProjectConfiguration>
                    <ProjectConfiguration Include="Release|x64">
                      <Configuration>Release</Configuration>
                      <Platform>x64</Platform>
                    </ProjectConfiguration>
                  </ItemGroup>
                  <ItemGroup>
                    <None Include="ApplicationPackageRoot\ApplicationManifest.xml" />
                    <None Include="ApplicationParameters\Cloud.xml" />
                    <None Include="ApplicationParameters\Local.1Node.xml" />
                    <None Include="ApplicationParameters\Local.5Node.xml" />
                    <None Include="PublishProfiles\Cloud.xml" />
                    <None Include="PublishProfiles\Local.1Node.xml" />
                    <None Include="PublishProfiles\Local.5Node.xml" />
                    <None Include="Scripts\Deploy-FabricApplication.ps1" />
                    <None Include="StartupServiceParameters\Cloud.xml" />
                    <None Include="StartupServiceParameters\Local.1Node.xml" />
                    <None Include="StartupServiceParameters\Local.5Node.xml" />
                    <None Include="StartupServices.xml" />
                  </ItemGroup>
                  <ItemGroup>
                    <Content Include="packages.config" />
                  </ItemGroup>
                  <Import Project="$(MSBuildToolsPath)\Microsoft.Common.targets" />
                  <PropertyGroup>
                    <ApplicationProjectTargetsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\Service Fabric Tools\Microsoft.VisualStudio.Azure.Fabric.ApplicationProject.targets</ApplicationProjectTargetsPath>
                  </PropertyGroup>
                  <Import Project="$(ApplicationProjectTargetsPath)" Condition="Exists('$(ApplicationProjectTargetsPath)')" />
                  <Import Project="..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.{{nugetPackageVersion}}\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets" Condition="Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.{{nugetPackageVersion}}\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets')" />
                  <Target Name="ValidateMSBuildFiles" BeforeTargets="PrepareForBuild">
                    <Error Condition="!Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.{{nugetPackageVersion}}\build\Microsoft.VisualStudio.Azure.Fabric.Application.props')" Text="Unable to find the '..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.{{nugetPackageVersion}}\build\Microsoft.VisualStudio.Azure.Fabric.Application.props' file. Please restore the 'Microsoft.VisualStudio.Azure.Fabric.MSBuild' Nuget package." />
                    <Error Condition="!Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.{{nugetPackageVersion}}\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets')" Text="Unable to find the '..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.{{nugetPackageVersion}}\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets' file. Please restore the 'Microsoft.VisualStudio.Azure.Fabric.MSBuild' Nuget package." />
                  </Target>
                </Project>
                """;
        }
    }
}