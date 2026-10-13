using System.Xml.Linq;
using Intent.Modules.VisualStudio.Projects.Templates;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.TargetFrameworks
{
    public class TargetFrameworksSyncTests
    {
        // dotnet/eShop's src/ClientApp/ClientApp.csproj (MAUI): multi-target plus a conditional line
        // in the same PropertyGroup that appends the Windows target.
        private const string EShopClientApp = """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0</TargetFrameworks>
                <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>
                <OutputType Condition="'$(TargetFramework)' != 'net10.0'">Exe</OutputType>
                <RootNamespace>eShop.ClientApp</RootNamespace>
                <UseMaui>true</UseMaui>
              </PropertyGroup>

            </Project>
            """;

        // dotnet/eShop's src/HybridApp/HybridApp.csproj, indented with 4 spaces.
        private const string EShopHybridApp = """
            <Project Sdk="Microsoft.NET.Sdk.Razor">

                <PropertyGroup>
                    <TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
                    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>
                    <OutputType>Exe</OutputType>
                    <UseMaui>true</UseMaui>
                </PropertyGroup>

            </Project>
            """;

        [Fact]
        public void WhenTheProjectHasConditionalTargetFrameworks_EShopClientApp_ShouldBeUntouched()
        {
            var doc = XDocument.Parse(EShopClientApp);

            var result = TargetFrameworksSync.Sync(doc, ["net10.0"]);

            result.ShouldBe(TargetFrameworksSyncResult.Unchanged);
            doc.ToString().ShouldBe(XDocument.Parse(EShopClientApp).ToString());
        }

        [Fact]
        public void WhenTheProjectHasConditionalTargetFrameworks_EShopHybridApp_ShouldBeUntouched()
        {
            var doc = XDocument.Parse(EShopHybridApp);

            var result = TargetFrameworksSync.Sync(doc, ["net10.0"]);

            result.ShouldBe(TargetFrameworksSyncResult.Unchanged);
            doc.ToString().ShouldBe(XDocument.Parse(EShopHybridApp).ToString());
        }

        [Fact]
        public void WhenTheOnlyElementIsConditional_ShouldBeUntouched()
        {
            var doc = Project("""<TargetFramework Condition="'$(Configuration)' == 'Debug'">net8.0</TargetFramework>""");

            TargetFrameworksSync.Sync(doc, ["net10.0"]).ShouldBe(TargetFrameworksSyncResult.Unchanged);
            Value(doc).ShouldBe("net8.0");
        }

        [Fact]
        public void WhenThePropertyGroupIsConditional_ShouldBeUntouched()
        {
            var doc = XDocument.Parse("""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup Condition="'$(Configuration)' == 'Debug'">
                    <TargetFramework>net8.0</TargetFramework>
                  </PropertyGroup>
                </Project>
                """);

            TargetFrameworksSync.Sync(doc, ["net10.0"]).ShouldBe(TargetFrameworksSyncResult.Unchanged);
            Value(doc).ShouldBe("net8.0");
        }

        [Fact]
        public void WhenTheValueReferencesAProperty_ShouldBeUntouched()
        {
            var doc = Project("<TargetFrameworks>$(DefaultTargetFrameworks);net8.0</TargetFrameworks>");

            TargetFrameworksSync.Sync(doc, ["net10.0"]).ShouldBe(TargetFrameworksSyncResult.Unchanged);
            Value(doc).ShouldBe("$(DefaultTargetFrameworks);net8.0");
        }

        [Fact]
        public void WhenPlatformTargetsReduceToTheModelledFramework_ShouldBeUntouched()
        {
            var doc = Project("<TargetFrameworks>net10.0-android;net10.0-ios;net10.0</TargetFrameworks>");

            TargetFrameworksSync.Sync(doc, ["net10.0"]).ShouldBe(TargetFrameworksSyncResult.Unchanged);
            Value(doc).ShouldBe("net10.0-android;net10.0-ios;net10.0");
        }

        [Fact]
        public void WhenPlatformTargetsDifferFromTheModel_ShouldBeUntouchedAndReportIt()
        {
            var doc = Project("<TargetFrameworks>net10.0-android;net10.0-ios</TargetFrameworks>");

            TargetFrameworksSync.Sync(doc, ["net11.0"]).ShouldBe(TargetFrameworksSyncResult.PlatformTargetsPreserved);
            Value(doc).ShouldBe("net10.0-android;net10.0-ios");
        }

        [Fact]
        public void WhenTheSetIsEqualButReordered_ShouldBeUntouched()
        {
            var doc = Project("<TargetFrameworks>net8.0;net6.0</TargetFrameworks>");

            TargetFrameworksSync.Sync(doc, ["net6.0", "net8.0"]).ShouldBe(TargetFrameworksSyncResult.Unchanged);
            Value(doc).ShouldBe("net8.0;net6.0");
        }

        [Fact]
        public void WhenAPlainFrameworkChanges_ShouldRewriteIt()
        {
            var doc = Project("<TargetFramework>net8.0</TargetFramework>");

            TargetFrameworksSync.Sync(doc, ["net10.0"]).ShouldBe(TargetFrameworksSyncResult.Rewritten);
            Value(doc).ShouldBe("net10.0");
            doc.Root!.Element("PropertyGroup")!.Element("TargetFramework").ShouldNotBeNull();
        }

        [Fact]
        public void WhenASingleFrameworkBecomesSeveral_ShouldRewriteAsTargetFrameworks()
        {
            var doc = Project("<TargetFramework>net8.0</TargetFramework>");

            TargetFrameworksSync.Sync(doc, ["net9.0", "net8.0"]).ShouldBe(TargetFrameworksSyncResult.Rewritten);
            doc.Root!.Element("PropertyGroup")!.Element("TargetFrameworks")!.Value.ShouldBe("net8.0;net9.0");
            doc.Root!.Element("PropertyGroup")!.Element("TargetFramework").ShouldBeNull();
        }

        [Fact]
        public void WhenThereIsNoTargetFrameworkElement_ShouldBeUntouched()
        {
            var doc = Project("<OutputType>Exe</OutputType>");

            TargetFrameworksSync.Sync(doc, ["net10.0"]).ShouldBe(TargetFrameworksSyncResult.Unchanged);
        }

        private static XDocument Project(string propertyGroupContent) => XDocument.Parse($"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                {propertyGroupContent}
              </PropertyGroup>
            </Project>
            """);

        private static string Value(XDocument doc) =>
            (doc.Root!.Element("PropertyGroup")!.Element("TargetFramework") ??
             doc.Root!.Element("PropertyGroup")!.Element("TargetFrameworks"))!.Value;
    }
}
