using System.Xml.Linq;
using Intent.Modules.VisualStudio.Projects.Templates;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.Sdk
{
    public class SdkAttributeSyncTests
    {
        public static TheoryData<string> ModelledSdks => new()
        {
            "Microsoft.NET.Sdk",
            "Microsoft.NET.Sdk.Web",
            "Microsoft.NET.Sdk.BlazorWebAssembly",
            "Microsoft.NET.Sdk.Razor",
            "Microsoft.NET.Sdk.Worker",
            "Microsoft.NET.Sdk.WindowsDesktop",
            "Aspire.AppHost.Sdk",
            "MSTest.Sdk"
        };

        [Theory]
        [MemberData(nameof(ModelledSdks))]
        public void WhenTheExistingSdkCannotBeModelled_ItIsPreservedWhateverTheModelSays(string modelledSdk)
        {
            foreach (var unknownSdk in new[] { "Microsoft.Build.NoTargets/3.7.0", "Uno.Sdk", "Microsoft.NET.Sdk.Wrong" })
            {
                var doc = Project(unknownSdk);

                SdkAttributeSync.Sync(doc, modelledSdk, aspireSdkVersion: "13.0", msTestSdkVersion: "4.0.2").ShouldBeFalse();
                Sdk(doc).ShouldBe(unknownSdk);
            }
        }

        [Fact]
        public void WhenModelledAsMSTestWithNoVersion_AnExistingMSTestSdkIsKept()
        {
            var doc = Project("MSTest.Sdk");

            SdkAttributeSync.Sync(doc, "MSTest.Sdk", aspireSdkVersion: null, msTestSdkVersion: null).ShouldBeFalse();
            Sdk(doc).ShouldBe("MSTest.Sdk");
        }

        [Fact]
        public void WhenModelledAsMSTestWithNoVersion_AnExistingVersionIsKept()
        {
            var doc = Project("MSTest.Sdk/3.6.4");

            SdkAttributeSync.Sync(doc, "MSTest.Sdk", aspireSdkVersion: null, msTestSdkVersion: "").ShouldBeFalse();
            Sdk(doc).ShouldBe("MSTest.Sdk/3.6.4");
        }

        [Fact]
        public void WhenModelledAsMSTestWithAVersion_TheVersionIsWritten()
        {
            var doc = Project("MSTest.Sdk/3.6.4");

            SdkAttributeSync.Sync(doc, "MSTest.Sdk", aspireSdkVersion: null, msTestSdkVersion: "4.0.2").ShouldBeTrue();
            Sdk(doc).ShouldBe("MSTest.Sdk/4.0.2");
        }

        [Fact]
        public void WhenModelledAsMSTestWithAVersion_AndItAlreadyMatches_NothingChanges()
        {
            var doc = Project("MSTest.Sdk/4.0.2");

            SdkAttributeSync.Sync(doc, "MSTest.Sdk", aspireSdkVersion: null, msTestSdkVersion: "4.0.2").ShouldBeFalse();
        }

        [Fact]
        public void WhenAPlainProjectIsModelledAsMSTest_ItIsRewrittenWithoutAVersion()
        {
            var doc = Project("Microsoft.NET.Sdk");

            SdkAttributeSync.Sync(doc, "MSTest.Sdk", aspireSdkVersion: null, msTestSdkVersion: null).ShouldBeTrue();
            Sdk(doc).ShouldBe("MSTest.Sdk");
        }

        [Fact]
        public void WhenAnMSTestProjectIsModelledAsThePlainSdk_TheModelWins()
        {
            var doc = Project("MSTest.Sdk");

            SdkAttributeSync.Sync(doc, "Microsoft.NET.Sdk", aspireSdkVersion: null, msTestSdkVersion: null).ShouldBeTrue();
            Sdk(doc).ShouldBe("Microsoft.NET.Sdk");
        }

        [Fact]
        public void Aspire_IsWrittenWithItsVersion()
        {
            var doc = Project("Microsoft.NET.Sdk");

            SdkAttributeSync.Sync(doc, "Aspire.AppHost.Sdk", aspireSdkVersion: "13.0", msTestSdkVersion: null).ShouldBeTrue();
            Sdk(doc).ShouldBe("Aspire.AppHost.Sdk/13.0");
        }

        [Fact]
        public void Aspire_AChangedVersionIsRewritten()
        {
            var doc = Project("Aspire.AppHost.Sdk/9.5.0");

            SdkAttributeSync.Sync(doc, "Aspire.AppHost.Sdk", aspireSdkVersion: "13.0", msTestSdkVersion: null).ShouldBeTrue();
            Sdk(doc).ShouldBe("Aspire.AppHost.Sdk/13.0");
        }

        [Fact]
        public void WhenTheSdkMatches_NothingChanges()
        {
            var doc = Project("Microsoft.NET.Sdk.Web");

            SdkAttributeSync.Sync(doc, "Microsoft.NET.Sdk.Web", aspireSdkVersion: null, msTestSdkVersion: null).ShouldBeFalse();
        }

        [Fact]
        public void WhenTheProjectHasNoSdkAttribute_NothingChanges()
        {
            var doc = XDocument.Parse("""<Project><Sdk Name="Microsoft.NET.Sdk" /></Project>""");

            SdkAttributeSync.Sync(doc, "Microsoft.NET.Sdk.Web", aspireSdkVersion: null, msTestSdkVersion: null).ShouldBeFalse();
            doc.Root!.Attribute("Sdk").ShouldBeNull();
        }

        [Theory]
        [InlineData("MSTest.Sdk/4.0.2", "MSTest.Sdk")]
        [InlineData("Aspire.AppHost.Sdk/13.0", "Aspire.AppHost.Sdk")]
        [InlineData("Microsoft.NET.Sdk", "Microsoft.NET.Sdk")]
        [InlineData(null, null)]
        public void GetSdkName_StripsTheVersion(string value, string expected)
        {
            SdkAttributeSync.GetSdkName(value).ShouldBe(expected);
        }

        private static XDocument Project(string sdk) => XDocument.Parse($"""
            <Project Sdk="{sdk}">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);

        private static string Sdk(XDocument doc) => doc.Root!.Attribute("Sdk")!.Value;
    }
}
