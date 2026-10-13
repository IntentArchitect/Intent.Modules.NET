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
    }
}
