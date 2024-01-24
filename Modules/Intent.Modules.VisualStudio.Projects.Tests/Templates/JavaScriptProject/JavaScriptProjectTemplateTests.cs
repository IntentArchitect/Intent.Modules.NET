using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Templates.JavaScriptProject;
using Shouldly;
using Xunit;
using static Intent.Modules.VisualStudio.Projects.Api.JavaScriptProjectModelStereotypeExtensions;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.JavaScriptProject;

public class JavaScriptProjectTemplateTests
{
    [Fact]
    public void ItShouldWork()
    {
        // Arrange
        var stereotype = new JavaScriptSettingsStereotype(new Dictionary<string, string>
        {
            ["Should run npm install"] = "true",
            ["Should run build script"] = "false",
            ["Build command"] = "npm run build",
            ["Startup command"] = "npm run start",
            ["Test command"] = "npm run test",
            ["Clean command"] = "clean",
            ["Publish command"] = "publish",
        });

        // Act
        var result = JavaScriptProjectTemplate.Generate(null, new JavaScriptSettings(stereotype));

        // Assert
        result.ReplaceLineEndings().ShouldBe("""
                        <Project Sdk="Microsoft.VisualStudio.JavaScript.Sdk/0.5.128-alpha">
                          <PropertyGroup>
                            <ShouldRunNpmInstall>true</ShouldRunNpmInstall>
                            <ShouldRunBuildScript>true</ShouldRunBuildScript>
                            <BuildCommand>npm run build</BuildCommand>
                            <StartupCommand>npm run start</StartupCommand>
                            <TestCommand>npm run test</TestCommand>
                            <CleanCommand>clean</CleanCommand>
                            <PublishCommand>publish</PublishCommand>
                          </PropertyGroup>
                        </Project>
                        """.ReplaceLineEndings());
    }

    private class StereotypeProperty : IStereotypeProperty
    {
        public StereotypeProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string PropertyDefinitionId { get; }
        public string Key { get; }
        public string Value { get; }
        public StereotypePropertyControlType ControlType { get; } 
        public StereotypePropertyOptionsSource OptionsSource { get; }
    }

    private class JavaScriptSettingsStereotype : IStereotype
    {
        public JavaScriptSettingsStereotype(Dictionary<string, string> properties)
        {
            Properties = properties.Select(x => new StereotypeProperty(x.Key, x.Value));
        }

        public IStereotypeProperty GetProperty(string nameOrId)
        {
            throw new NotImplementedException();
        }

        public bool TryGetProperty(string nameOrId, out IStereotypeProperty property)
        {
            property = Properties.SingleOrDefault(x => x.Key == nameOrId);
            return property != default;
        }

        public string DefinitionId => "141b4305-433b-4d5a-97ed-7796eabbe2aa";
        public string Name => "JavaScript Settings";
        public IEnumerable<IStereotypeProperty> Properties { get; }
    }
}