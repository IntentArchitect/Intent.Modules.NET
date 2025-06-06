using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MetadataLoaderExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.IaC.Terraform.MetadataLoaderExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterMetadataLoad(IApplication application)
        {
            Intent.Modules.Integration.IaC.Shared.AzureEventGrid.IntegrationManager.Initialize(application);
            Intent.Modules.Integration.IaC.Shared.AzureServiceBus.IntegrationManager.Initialize(application);
        }
    }
}