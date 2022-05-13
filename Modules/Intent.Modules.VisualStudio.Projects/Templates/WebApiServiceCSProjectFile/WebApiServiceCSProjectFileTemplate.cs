using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Templates;
using Microsoft.Build.Construction;

namespace Intent.Modules.VisualStudio.Projects.Templates.WebApiServiceCSProjectFile
{
    public class WebApiServiceCSProjectFileTemplate : VisualStudioProjectTemplateBase, IHasNugetDependencies, /*IProjectTemplate,*/ IHasDecorators<IWebApiServiceCSProjectDecorator>
    {
        public const string Identifier = "Intent.VisualStudio.Projects.WebApiServiceCSProjectFile";
        private readonly string _sslPort = "";

        public WebApiServiceCSProjectFileTemplate(IOutputTarget project, IVisualStudioProject model)
            : base(Identifier, project, model)
        {
        }

        public override string TransformText()
        {
            var doc = TryGetExistingFileContent(out var content)
                ? XDocument.Parse(content, LoadOptions.PreserveWhitespace)
                : XDocument.Parse(CreateTemplate());

            foreach (var xmlDecorator in GetDecorators())
            {
                xmlDecorator.Install(doc, Project);
            }

            var text = doc.ToStringUTF8();
            return text;
        }

        public string CreateTemplate()
        {
            var root = ProjectRootElement.Create();
            root.ToolsVersion = "12.0";
            root.DefaultTargets = "Build";

            root.AddImport(@"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props").Condition = "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')";

            var group = root.AddPropertyGroup();
            group.AddProperty("Configuration", "Debug").Condition = " '$(Configuration)' == '' ";
            group.AddProperty("Platform", "AnyCPU").Condition = " '$(Platform)' == '' ";
            group.AddProperty("ProductVersion", "");
            group.AddProperty("SchemaVersion", "2.0");
            group.AddProperty("ProjectGuid", Model.ToString());
            group.AddProperty("ProjectTypeGuids", "{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}");
            group.AddProperty("OutputType", "Library");
            group.AddProperty("AppDesignerFolder", "Properties");
            group.AddProperty("RootNamespace", Model.Name);
            group.AddProperty("AssemblyName", Model.Name);
            group.AddProperty("TargetFrameworkVersion", GetTargetFrameworkVersion());
            group.AddProperty("WcfConfigValidationEnabled", "True");
            group.AddProperty("AutoGenerateBindingRedirects", "true");
            group.AddProperty("UseIISExpress", "True");
            group.AddProperty("IISExpressSSLPort", _sslPort);
            group.AddProperty("IISExpressAnonymousAuthentication", "");
            group.AddProperty("IISExpressWindowsAuthentication", "");
            group.AddProperty("IISExpressUseClassicPipelineMode", "");
            group.AddProperty("UseGlobalApplicationHostFile", "");
            group.AddProperty("NuGetPackageImportStamp", "");
            group.AddProperty("TargetFrameworkProfile", "");

            group = root.AddPropertyGroup();
            group.Condition = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
            group.AddProperty("DebugSymbols", "true");
            group.AddProperty("DebugType", "full");
            group.AddProperty("Optimize", "false");
            group.AddProperty("OutputPath", @"bin\");
            group.AddProperty("DefineConstants", "DEBUG;TRACE");
            group.AddProperty("ErrorReport", "prompt");
            group.AddProperty("WarningLevel", "4");

            group = root.AddPropertyGroup();
            group.Condition = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ";
            group.AddProperty("DebugType", "pdbonly");
            group.AddProperty("Optimize", "true");
            group.AddProperty("OutputPath", @"bin\");
            group.AddProperty("DefineConstants", "TRACE");
            group.AddProperty("ErrorReport", "prompt");
            group.AddProperty("WarningLevel", "4");

            var itemGroup = root.AddItemGroup();

            foreach (var reference in Project.References())
            {
                AddReference(itemGroup, reference);
            }

            foreach (var dependency in Project.Dependencies())
            {
                AddItem(itemGroup, "ProjectReference", string.Format("..\\{0}\\{0}.csproj", dependency.Name),
                    new[]
                    {
                        new KeyValuePair<string, string>("Project", $"{{{dependency.Id}}}"),
                        new KeyValuePair<string, string>("Name", $"{dependency.Name}"),
                    });
            }

            root.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
            root.AddImport(@"$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets").Condition = "'$(VSToolsPath)' != ''";
            root.AddImport(@"$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v10.0\WebApplications\Microsoft.WebApplication.targets").Condition = "false";

            group = root.AddPropertyGroup();
            group.AddProperty("VisualStudioVersion", "10.0").Condition = "'$(VisualStudioVersion)' == ''";
            group.AddProperty("VSToolsPath", @"$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)").Condition = "'$(VSToolsPath)' == ''";

            if (!string.IsNullOrEmpty(_sslPort))
            {
                var extension = root.CreateProjectExtensionsElement();
                extension.Content =
                    $@"    <VisualStudio>
      <FlavorProperties GUID=""{{349c5851-65df-11da-9384-00065b846f21}}"">
        <WebProjectProperties>
          <UseIIS>True</UseIIS>
          <AutoAssignPort>True</AutoAssignPort>
          <DevelopmentServerPort>0</DevelopmentServerPort>
          <DevelopmentServerVPath>/</DevelopmentServerVPath>
          <IISUrl>https://localhost:{_sslPort}/</IISUrl>
          <NTLMAuthentication>False</NTLMAuthentication>
          <UseCustomServer>False</UseCustomServer>
          <CustomServerUrl>
          </CustomServerUrl>
          <SaveServerSettingsInUserFile>False</SaveServerSettingsInUserFile>
        </WebProjectProperties>
      </FlavorProperties>
    </VisualStudio>
";
                root.AppendChild(extension);
            }

            return root.RawXml.Replace("utf-16", "utf-8");
        }

        private string GetTargetFrameworkVersion()
        {
            return Model.TargetFrameworkVersion().SingleOrDefault() ?? "4.7.2";
        }

        private static void AddItem(ProjectItemGroupElement itemGroup, string groupName, string item, IEnumerable<KeyValuePair<string, string>> metadata)
        {
            itemGroup.AddItem(groupName, item, metadata);
        }

        private void AddReference(ProjectItemGroupElement itemGroup, IAssemblyReference reference)
        {
            var metadata = new List<KeyValuePair<string, string>>();
            if (reference.HasHintPath())
            {
                metadata.Add(new KeyValuePair<string, string>("HintPath", reference.HintPath));
            }
            AddItem(itemGroup, "Reference", reference.Library, metadata);
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            if (FileMetadata.CustomMetadata.TryGetValue("Ignore NuGet Dependencies", out var ignoreNuGet) && ignoreNuGet == "true")
            {
                return Array.Empty<INugetPackageInfo>();
            }
            return new[]
            {
                NugetPackages.MicrosoftAspNetWebApi,
                NugetPackages.MicrosoftAspNetWebApiClient,
                NugetPackages.MicrosoftAspNetWebApiCore,
                NugetPackages.MicrosoftAspNetWebApiWebHost,
                NugetPackages.NewtonsoftJson,
            };
        }

        private readonly ICollection<IWebApiServiceCSProjectDecorator> _decorators = new List<IWebApiServiceCSProjectDecorator>();
        public IEnumerable<IWebApiServiceCSProjectDecorator> GetDecorators()
        {
            return _decorators;
        }

        public void AddDecorator(IWebApiServiceCSProjectDecorator decorator)
        {
            _decorators.Add(decorator);
        }
    }
}