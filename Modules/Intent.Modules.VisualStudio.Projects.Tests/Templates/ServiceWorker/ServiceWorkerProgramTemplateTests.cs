using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Templates.ServiceWorker.ServiceWorkerProgram;
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
