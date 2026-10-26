using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.Sync;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Modules.VisualStudio.Projects.Templates.ServiceWorker.ServiceWorkerProgram;
using Intent.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.ServiceWorker
{
    public class ServiceWorkerProgramTemplateTests
    {
        [Fact]
        public void ProjectNamespace_WithNothingUnderIt_IsNotDeclared()
        {
            // dotnet/eShop: the OrderProcessor project's code lives under eShop.OrderProcessor.*, so
            // "using OrderProcessor;" fails with CS0246.
            ServiceWorkerProgramTemplate.IsNamespaceDeclared(
                "OrderProcessor",
                ["eShop.OrderProcessor.Services", "eShop.OrderProcessor.Extensions"]).ShouldBeFalse();
        }

        [Fact]
        public void ProjectNamespace_WithATypeInIt_IsDeclared()
        {
            ServiceWorkerProgramTemplate.IsNamespaceDeclared(
                "WindowsServiceHost.Tests",
                ["WindowsServiceHost.Tests"]).ShouldBeTrue();
        }

        [Fact]
        public void ProjectNamespace_WithANestedNamespace_IsDeclared()
        {
            ServiceWorkerProgramTemplate.IsNamespaceDeclared(
                "WebAndWorker.QuartzWorker",
                ["WebAndWorker.QuartzWorker.Configuration"]).ShouldBeTrue();
        }

        [Fact]
        public void ANamespaceSharingOnlyAPrefix_DoesNotDeclareIt()
        {
            ServiceWorkerProgramTemplate.IsNamespaceDeclared(
                "OrderProcessor",
                ["OrderProcessorTools", null]).ShouldBeFalse();
        }

        [Fact]
        public void NoGeneratedTypes_DeclaresNothing()
        {
            ServiceWorkerProgramTemplate.IsNamespaceDeclared("OrderProcessor", []).ShouldBeFalse();
        }

        [Fact]
        public void GenerateProgramFileOn_AlwaysRuns()
        {
            ServiceWorkerProgramTemplate.ShouldRunFor(isHandsOff: false, ExistingProgramShapeKind.None).ShouldBeTrue();
            ServiceWorkerProgramTemplate.ShouldRunFor(isHandsOff: false, ExistingProgramShapeKind.Recognised).ShouldBeTrue();
            ServiceWorkerProgramTemplate.ShouldRunFor(isHandsOff: false, ExistingProgramShapeKind.Unrecognised).ShouldBeTrue();
        }

        [Fact]
        public void HandsOff_WithNoExistingFile_DoesNotCreateOne()
        {
            ServiceWorkerProgramTemplate.ShouldRunFor(isHandsOff: true, ExistingProgramShapeKind.None).ShouldBeFalse();
        }

        [Fact]
        public void HandsOff_WithAnExistingFile_StillRuns()
        {
            // Still "generated" (OnceOff, so unchanged), otherwise the file would be proposed for deletion
            ServiceWorkerProgramTemplate.ShouldRunFor(isHandsOff: true, ExistingProgramShapeKind.Recognised).ShouldBeTrue();
            ServiceWorkerProgramTemplate.ShouldRunFor(isHandsOff: true, ExistingProgramShapeKind.Unrecognised).ShouldBeTrue();
        }

        [Fact]
        public void GenerateProgramFile_SetToFalse_IsHandsOff()
        {
            ServiceWorkerProgramTemplate.IsHandsOff(new NetSettingsStereotype(("Generate Program File", "false"))).ShouldBeTrue();
        }

        [Fact]
        public void GenerateProgramFile_SetToTrue_IsNotHandsOff()
        {
            ServiceWorkerProgramTemplate.IsHandsOff(new NetSettingsStereotype(("Generate Program File", "true"))).ShouldBeFalse();
        }

        [Fact]
        public void GenerateProgramFile_Missing_IsNotHandsOff()
        {
            // A project saved before the property existed: Worker projects always had Program.cs generated
            ServiceWorkerProgramTemplate.IsHandsOff(new NetSettingsStereotype(("SDK", "Microsoft.NET.Sdk.Worker"))).ShouldBeFalse();
        }

        [Fact]
        public void NetSettings_Missing_IsNotHandsOff()
        {
            ServiceWorkerProgramTemplate.IsHandsOff(null).ShouldBeFalse();
        }

        [Fact]
        public void HandsOff_FileConfig_IsNotRoslynWeaver()
        {
            // RoslynWeaver adds Intent.RoslynWeaver.Attributes to any project with a RoslynWeaver template
            var config = ProgramFileConfig();
            config.CodeGenType.ShouldBe(CodeGenType.RoslynWeaver);

            var result = ServiceWorkerProgramTemplate.GetFileConfig(config, isHandsOff: true);

            result.CodeGenType.ShouldBe(Intent.Modules.Common.CodeGenType.Basic);
            result.FileName.ShouldBe("Program");
            result.FileExtension.ShouldBe("cs");
            result.LocationInProject.ShouldBe("Workers");
            result.OverwriteBehaviour.ShouldBe(OverwriteBehaviour.OnceOff);
            // Other modules resolve the Program template's type name from these
            result.CustomMetadata["ClassName"].ShouldBe("Program");
            result.CustomMetadata["Namespace"].ShouldBe("OrderProcessor");
            // A brownfield .csproj item for Program.cs is neither added nor removed
            result.CustomMetadata[FileAddedData.LeaveProjectItemsUnchangedKey].ShouldBe("true");
        }

        [Fact]
        public void HandsOffTemplate_ContributesNoDependencies()
        {
            var handsOff = HandsOffTemplate(isHandsOff: true);

            new[] { handsOff }.ExceptHandsOff().ShouldBeEmpty();
        }

        [Fact]
        public void TemplateThatCanBeHandsOff_ButIsNot_ContributesDependencies()
        {
            var generated = HandsOffTemplate(isHandsOff: false);
            var ordinary = Substitute.For<ITemplate>();

            new[] { generated, ordinary }.ExceptHandsOff().ShouldBe(new[] { generated, ordinary });
        }

        [Fact]
        public void HandsOffTemplate_IsExcluded_AlongsideOrdinaryTemplates()
        {
            var before = Substitute.For<ITemplate>();
            var handsOff = HandsOffTemplate(isHandsOff: true);
            var after = Substitute.For<ITemplate>();

            new[] { before, handsOff, after }.ExceptHandsOff().ShouldBe(new[] { before, after });
        }

        [Fact]
        public void ProgramTemplate_IsSkippedByTheCollectorsWhenHandsOff()
        {
            typeof(IHandsOffTemplate).IsAssignableFrom(typeof(ServiceWorkerProgramTemplate)).ShouldBeTrue();
        }

        [Fact]
        public void NuGetCollector_SkipsWhatIsAttachedToAHandsOffTemplate()
        {
            // e.g. Intent.WindowsServiceHost attaches Microsoft.Extensions.Hosting.WindowsServices to the Program template
            var templates = new[]
            {
                TemplateRequestingPackage("Microsoft.Extensions.Hosting.WindowsServices", isHandsOff: true),
                TemplateRequestingPackage("Microsoft.Extensions.DependencyInjection", isHandsOff: false)
            };
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.TemplateInstances.Returns(templates);

            NugetDependencyResolutionFactoryExtensions.GetAllTemplateNuGetInstalls(outputTarget)
                .Select(x => x.Package.Name)
                .ShouldBe(new[] { "Microsoft.Extensions.DependencyInjection" });
        }

        [Fact]
        public void FrameworkCollector_SkipsWhatIsAttachedToAHandsOffTemplate()
        {
            var templates = new[]
            {
                TemplateRequestingFramework("Microsoft.AspNetCore.App", isHandsOff: true),
                TemplateRequestingFramework("Microsoft.WindowsDesktop.App", isHandsOff: false)
            };
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.TemplateInstances.Returns(templates);

            FrameworkDependencyResolutionFactoryExtensions.GetAllTemplateFrameworkDependencies(outputTarget)
                .ShouldBe(new[] { "Microsoft.WindowsDesktop.App" });
        }

        private static ITemplate HandsOffTemplate(bool isHandsOff)
        {
            var template = Substitute.For<ITemplate, IHandsOffTemplate>();
            ((IHandsOffTemplate)template).IsHandsOff.Returns(isHandsOff);
            return template;
        }

        private static ITemplate TemplateRequestingPackage(string packageName, bool isHandsOff)
        {
            var package = Substitute.For<INugetPackageInfo>();
            package.Name.Returns(packageName);

            var template = Substitute.For<ITemplate, IHasNugetDependencies, IHandsOffTemplate>();
            ((IHasNugetDependencies)template).GetNugetDependencies().Returns(new[] { package });
            ((IHandsOffTemplate)template).IsHandsOff.Returns(isHandsOff);
            return template;
        }

        private static ITemplate TemplateRequestingFramework(string framework, bool isHandsOff)
        {
            var template = Substitute.For<ITemplate, IHasFrameworkDependencies, IHandsOffTemplate>();
            ((IHasFrameworkDependencies)template).GetFrameworkDependencies().Returns(new[] { framework });
            ((IHandsOffTemplate)template).IsHandsOff.Returns(isHandsOff);
            return template;
        }

        [Fact]
        public void GenerateProgramFileOn_FileConfig_IsUnchanged()
        {
            var config = ProgramFileConfig();

            ServiceWorkerProgramTemplate.GetFileConfig(config, isHandsOff: false).ShouldBeSameAs(config);
        }

        private static CSharpFileConfig ProgramFileConfig()
        {
            return new CSharpFile("OrderProcessor", "Workers")
                .AddClass("Program")
                .WithOverwriteBehaviour(OverwriteBehaviour.OnceOff)
                .GetConfig();
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

        private class NetSettingsStereotype : IStereotype
        {
            public NetSettingsStereotype(params (string Key, string Value)[] properties)
            {
                Properties = properties.Select(x => new StereotypeProperty(x.Key, x.Value)).ToArray();
            }

            public IStereotypeProperty GetProperty(string nameOrId)
            {
                return Properties.SingleOrDefault(x => x.Key == nameOrId);
            }

            public bool TryGetProperty(string nameOrId, out IStereotypeProperty property)
            {
                property = GetProperty(nameOrId);
                return property != null;
            }

            public string DefinitionId => "a490a23f-5397-40a1-a3cb-6da7e0b467c0";
            public IStereotypeDefinition Definition { get; }
            public string Name => ".NET Settings";
            public IEnumerable<IStereotypeProperty> Properties { get; }
        }
    }
}
